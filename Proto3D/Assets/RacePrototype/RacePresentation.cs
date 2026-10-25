using System;
using System.Collections.Generic;
using UnityEngine;

// Original procedural presentation: no external assets and no ownership of race time.
public sealed class RacePresentation : MonoBehaviour
{
    public bool MenuOpen = true;
    public int SelectedTrack { get; private set; }
    public int SelectedVehicle { get; private set; }
    public int SelectedCharacter { get; private set; }
    public event Action<int, int> StartRequested;

    public bool Muted { get; private set; }
    public bool CocoaUnlocked { get; private set; }
    public float VehicleSpeedMultiplier { get { return SelectedVehicle == 1 ? .95f : SelectedVehicle == 2 ? 1.06f : 1f; } }
    public float VehicleHandlingMultiplier { get { return SelectedVehicle == 1 ? 1.12f : SelectedVehicle == 2 ? .94f : 1f; } }
    public string TrackName { get { return SelectedTrack == 1 ? "COCOA CLOUD" : "SUGAR BLOOM"; } }
    public string VehicleName { get { return VehicleNames[Mathf.Clamp(SelectedVehicle, 0, 2)]; } }

    static readonly string[] CharacterNames = { "Lia Fresa", "Milo Menta", "Coco Robot" };
    static readonly string[] VehicleNames = { "Fresa Sprint", "Menta Drift", "Cocoa Comet" };
    static readonly string[] VehicleDescriptions = {
        "Equilibrado y facil de conducir.\nVelocidad 100%  /  Giro 100%",
        "Agil en curvas y amable al conducir.\nVelocidad 95%  /  Giro 112%",
        "Mas velocidad y curvas exigentes.\nVelocidad 106%  /  Giro 94%"
    };
    static readonly Color[] VehicleColors = {
        new Color(1f, .34f, .58f), new Color(.3f, .85f, .7f), new Color(.43f, .22f, .16f)
    };
    const string Prefix = "Proto3D.Arcade.";
    AudioSource musicSource, fxSource;
    AudioClip musicClip;
    readonly Dictionary<string, AudioClip> cues = new Dictionary<string, AudioClip>();
    readonly List<Material> generatedMaterials = new List<Material>();
    readonly List<Material> previewMaterials = new List<Material>();
    List<Material> activeMaterials;

    void Awake()
    {
        CocoaUnlocked = PlayerPrefs.GetInt(Prefix + "CocoaUnlocked", 0) == 1;
        SelectedVehicle = Mathf.Clamp(PlayerPrefs.GetInt(Prefix + "Vehicle", 0), 0, CocoaUnlocked ? 2 : 1);
        SelectedTrack = Mathf.Clamp(PlayerPrefs.GetInt(Prefix + "Track", 0), 0, 1);
        SelectedCharacter = Mathf.Clamp(PlayerPrefs.GetInt(Prefix + "Character", 0), 0, 2);
        Muted = PlayerPrefs.GetInt(Prefix + "Muted", 0) == 1;
        BuildAudio();
        if(!GetComponent<RaceMenuView>())gameObject.AddComponent<RaceMenuView>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) SetMuted(!Muted);
        if (!MenuOpen) return;
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) BeginRace();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) ChooseVehicle((SelectedVehicle + (CocoaUnlocked ? 2 : 1)) % (CocoaUnlocked ? 3 : 2));
        if (Input.GetKeyDown(KeyCode.RightArrow)) ChooseVehicle((SelectedVehicle + 1) % (CocoaUnlocked ? 3 : 2));
        if (Input.GetKeyDown(KeyCode.Tab)) { SelectedTrack = 1 - SelectedTrack; PlayCue("click"); }
    }

    public void OpenMenu()
    {
        MenuOpen = true;
        PlayCue("click");
    }

    // Allows the race manager to begin the selected race for its F2 demonstration.
    public void StartSelection() { BeginRace(); }

    public void RecordResult(int position)
    {
        if (position == 1 && !CocoaUnlocked)
        {
            CocoaUnlocked = true;
            PlayerPrefs.SetInt(Prefix + "CocoaUnlocked", 1);
            PlayerPrefs.Save();
        }
        PlayCue("finish");
    }

    public void SetMuted(bool muted)
    {
        Muted = muted;
        if (musicSource) musicSource.mute = muted;
        if (fxSource) fxSource.mute = muted;
        PlayerPrefs.SetInt(Prefix + "Muted", muted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SelectVehicle(int index){ChooseVehicle(index);}
    public void SelectCharacter(int index){SelectedCharacter=Mathf.Clamp(index,0,2);PlayCue("click");}
    public void SelectTrack(int index){SelectedTrack=Mathf.Clamp(index,0,1);PlayCue("click");}
    void ChooseVehicle(int index)
    {
        if (index == 2 && !CocoaUnlocked) return;
        SelectedVehicle = Mathf.Clamp(index, 0, 2);
        PlayCue("click");
    }

    void BeginRace()
    {
        if (!MenuOpen || StartRequested == null) return;
        PlayerPrefs.SetInt(Prefix + "Vehicle", SelectedVehicle);
        PlayerPrefs.SetInt(Prefix + "Character", SelectedCharacter);
        PlayerPrefs.SetInt(Prefix + "Track", SelectedTrack);
        PlayerPrefs.Save();
        MenuOpen = false;
        PlayCue("click");
        StartRequested(SelectedTrack, SelectedVehicle);
    }

    // Visual trim attaches to the existing kart without changing its physics or source materials.
    public void ApplyVehicleVisual(GameObject kart)
    {
        if (!kart) return;
        Transform old = kart.transform.Find("Proto3D Vehicle Trim");
        if (old) { old.gameObject.SetActive(false); Destroy(old.gameObject); }
        var bucket=kart.name.StartsWith("MenuKartPreview")?previewMaterials:generatedMaterials;
        foreach(Material material in bucket)if(material)Destroy(material);bucket.Clear();activeMaterials=bucket;
        GameObject trim = new GameObject("Proto3D Vehicle Trim");
        trim.transform.SetParent(kart.transform, false);
        Color color = VehicleColors[Mathf.Clamp(SelectedVehicle, 0, 2)];
        Material accent = MakeMaterial(color);
        Material cream = MakeMaterial(new Color(1f, .92f, .76f));
        var handmade=kart.transform.Find("Candy kart - handmade");if(handmade)foreach(var renderer in handmade.GetComponentsInChildren<Renderer>())if(renderer.name=="Strawberry icing"||renderer.name=="Seat back")renderer.sharedMaterial=accent;
        BuildCharacter(kart.transform,trim.transform,cream);
        Ornament(trim.transform, "Candy spoiler", PrimitiveType.Cube, new Vector3(0, 1.05f, -1.6f), new Vector3(1.65f, .15f, .34f), accent);
        Ornament(trim.transform, "Left trim", PrimitiveType.Cube, new Vector3(-.67f, .72f, .15f), new Vector3(.13f, .12f, 2.3f), accent);
        Ornament(trim.transform, "Right trim", PrimitiveType.Cube, new Vector3(.67f, .72f, .15f), new Vector3(.13f, .12f, 2.3f), accent);
        if (SelectedVehicle == 1)
        {
            Ornament(trim.transform, "Mint drop left", PrimitiveType.Sphere, new Vector3(-.56f, 1.05f, -1.55f), Vector3.one * .27f, cream);
            Ornament(trim.transform, "Mint drop right", PrimitiveType.Sphere, new Vector3(.56f, 1.05f, -1.55f), Vector3.one * .27f, cream);
        }
        else if (SelectedVehicle == 2)
        {
            for (int i = 0; i < 3; i++)
                Ornament(trim.transform, "Chocolate square " + i, PrimitiveType.Cube, new Vector3((i - 1) * .43f, .85f, 1f), new Vector3(.33f, .13f, .42f), accent);
        }
        else
            Ornament(trim.transform, "Strawberry candy", PrimitiveType.Sphere, new Vector3(0, .87f, 1.05f), new Vector3(.42f, .18f, .5f), accent);
        activeMaterials=null;
    }

    void BuildCharacter(Transform kart,Transform parent,Material cream)
    {
        Transform handmade=kart.Find("Candy kart - handmade");
        if(handmade)foreach(string part in new[]{"Driver","Helmet","Visor","Helmet candy"}){var old=handmade.Find(part);if(old){var renderer=old.GetComponent<Renderer>();if(renderer)renderer.enabled=false;}}
        Material suit=MakeMaterial(SelectedCharacter==0?new Color(1,.3f,.55f):SelectedCharacter==1?new Color(.2f,.8f,.65f):new Color(.45f,.24f,.13f));
        Material face=MakeMaterial(new Color(1,.79f,.61f)),eye=MakeMaterial(new Color(.09f,.055f,.1f));
        Ornament(parent,"Pilot suit",PrimitiveType.Capsule,new Vector3(0,1.65f,-.35f),new Vector3(.6f,.4f,.55f),suit);
        Ornament(parent,"Pilot face",SelectedCharacter==2?PrimitiveType.Cube:PrimitiveType.Sphere,new Vector3(0,2.15f,-.23f),Vector3.one*.66f,SelectedCharacter==2?cream:face);
        for(int side=-1;side<=1;side+=2){Ornament(parent,"Pilot eye",PrimitiveType.Sphere,new Vector3(side*.14f,2.2f,.09f),new Vector3(.09f,.11f,.06f),eye);Ornament(parent,"Pilot arm",PrimitiveType.Capsule,new Vector3(side*.34f,1.72f,.05f),new Vector3(.16f,.25f,.16f),suit);}
        if(SelectedCharacter==0){Ornament(parent,"Strawberry helmet",PrimitiveType.Sphere,new Vector3(0,2.43f,-.24f),new Vector3(.75f,.3f,.7f),suit);Ornament(parent,"Strawberry stem",PrimitiveType.Cube,new Vector3(0,2.65f,-.24f),new Vector3(.13f,.22f,.14f),MakeMaterial(new Color(.2f,.65f,.28f)));}
        else if(SelectedCharacter==1){for(int side=-1;side<=1;side+=2)Ornament(parent,"Mint ears",PrimitiveType.Capsule,new Vector3(side*.23f,2.6f,-.23f),new Vector3(.19f,.27f,.2f),suit);}
        else{Ornament(parent,"Robot antenna",PrimitiveType.Cylinder,new Vector3(0,2.59f,-.23f),new Vector3(.05f,.15f,.05f),suit);Ornament(parent,"Robot candy antenna",PrimitiveType.Sphere,new Vector3(0,2.77f,-.23f),Vector3.one*.16f,suit);}
    }

    Material MakeMaterial(Color color)
    {
        Shader shader = Shader.Find("Standard");
        if (!shader) shader = Shader.Find("Sprites/Default");
        Material material = new Material(shader);
        material.color = color;
        (activeMaterials??generatedMaterials).Add(material);
        return material;
    }

    static void Ornament(Transform parent, string label, PrimitiveType type, Vector3 position, Vector3 scale, Material material)
    {
        GameObject part = GameObject.CreatePrimitive(type);
        part.name = label;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = position;
        part.transform.localScale = scale;
        Collider collider = part.GetComponent<Collider>();
        if (collider) { collider.enabled = false; Destroy(collider); }
        part.GetComponent<Renderer>().sharedMaterial = material;
    }

    void BuildAudio()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        fxSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        fxSource.playOnAwake = false;
        musicSource.spatialBlend = fxSource.spatialBlend = 0;
        musicSource.volume = .15f;
        fxSource.volume = .24f;
        musicSource.loop = true;
        musicSource.mute = fxSource.mute = Muted;
        musicClip = ComposeMusic();
        musicSource.clip = musicClip;
        musicSource.Play();
        cues.Add("click", Tone("Candy click", .1f, 520, 780));
        cues.Add("pickup", Tone("Candy pickup", .28f, 440, 880));
        cues.Add("boost", Tone("Sugar boost", .65f, 180, 780));
        cues.Add("hit", Tone("Chocolate splash", .28f, 220, 65));
        cues.Add("shield", Tone("Marshmallow shield", .42f, 660, 1100));
        cues.Add("checkpoint", Tone("Checkpoint bell", .16f, 880, 880));
        cues.Add("finish", Tone("Finish sparkle", 1.2f, 440, 880));
    }

    public void PlayCue(string cue)
    {
        AudioClip clip;
        if (!Muted && fxSource && !string.IsNullOrEmpty(cue) && cues.TryGetValue(cue, out clip)) fxSource.PlayOneShot(clip);
    }

    // Pentatonic phrase composed for this prototype, synthesized locally without samples.
    static AudioClip ComposeMusic()
    {
        const int rate = 22050;
        const float beat = .4f;
        const int steps = 32;
        int[] notes = { 72, 76, 79, 76, 74, 72, 67, -1, 69, 72, 76, 79, 81, 79, 76, -1, 74, 77, 79, 77, 74, 72, 69, -1, 67, 69, 72, 76, 74, 72, 67, -1 };
        int[] bass = { 48, 45, 53, 43 };
        float[] data = new float[Mathf.RoundToInt(rate * beat * steps)];
        for (int i = 0; i < data.Length; i++)
        {
            float t = i / (float)rate;
            int step = Mathf.Min(steps - 1, (int)(t / beat));
            float noteTime = t - step * beat;
            float envelope = Mathf.Clamp01(noteTime / .015f) * Mathf.Clamp01((beat * .87f - noteTime) / .12f);
            float melody = notes[step] < 0 ? 0 : Mathf.Sin(2 * Mathf.PI * Midi(notes[step]) * noteTime) * envelope * .24f;
            float bassTime = t % (beat * 8);
            float bassEnvelope = Mathf.Clamp01(bassTime / .04f) * Mathf.Clamp01((beat * 7.8f - bassTime) / .3f);
            float low = Mathf.Sin(2 * Mathf.PI * Midi(bass[step / 8]) * bassTime) * bassEnvelope * .12f;
            float bell = Mathf.Sin(2 * Mathf.PI * 1320 * noteTime) * Mathf.Exp(-noteTime * 45) * Mathf.Clamp01(noteTime / .004f) * .018f;
            data[i] = melody + low + bell;
        }
        AudioClip clip = AudioClip.Create("Proto3D original candy loop", data.Length, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }

    static float Midi(int note) { return 440f * Mathf.Pow(2, (note - 69) / 12f); }

    static AudioClip Tone(string name, float duration, float from, float to)
    {
        const int rate = 22050;
        float[] data = new float[Mathf.RoundToInt(duration * rate)];
        float phase = 0;
        for (int i = 0; i < data.Length; i++)
        {
            float t = i / (float)rate;
            float u = t / duration;
            phase += 2 * Mathf.PI * Mathf.Lerp(from, to, u) / rate;
            float envelope = Mathf.Clamp01(t / .012f) * Mathf.Clamp01((duration - t) / .09f) * (1 - .5f * u);
            data[i] = (Mathf.Sin(phase) * .48f + Mathf.Sin(phase * 2) * .08f) * envelope;
        }
        AudioClip clip = AudioClip.Create(name, data.Length, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }

    void OnDestroy()
    {
        if (musicClip) Destroy(musicClip);
        foreach (AudioClip clip in cues.Values) if (clip) Destroy(clip);
        foreach (Material material in generatedMaterials) if (material) Destroy(material);
        foreach(Material material in previewMaterials)if(material)Destroy(material);
        cues.Clear();
    }
}
