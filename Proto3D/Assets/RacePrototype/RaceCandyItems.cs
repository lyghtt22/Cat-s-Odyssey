using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

// All candy assets are original runtime primitives; no redistributable third-party assets.
public class RaceCandyItems : MonoBehaviour
{
    RacePrototypeGame game;
    CarController[] racers;
    Rigidbody[] bodies;
    Vector3[] lastPositions;
    int[] cpuHeld;
    float[] cpuShield, hazardReady, shortcutReady, cpuUseAt;
    Transform[] shieldVisuals;
    CandySlow[] slows;
    readonly List<CandyProjectile> projectiles = new List<CandyProjectile>();
    readonly List<Transform> boxes = new List<Transform>();
    readonly List<float> boxReady = new List<float>();
    readonly List<Transform> traps = new List<Transform>();
    readonly List<Transform> moving = new List<Transform>();
    readonly List<Vector3> movingOrigins = new List<Vector3>();
    readonly List<Vector3> movingSides = new List<Vector3>();
    readonly List<Material> materials = new List<Material>();
    Transform trackRoot;
    Material pink, cream, chocolate, blue, gold, warning;
    Vector3 shortcutStart, shortcutEnd;
    bool initialized, haveShortcut;
    int variant, held;
    float shieldUntil, messageUntil;
    string message = "";
    string proximity = "";
    float nextWarning;
    public int HeldId { get { return held; } }
    public float ShieldSeconds { get { return Mathf.Max(0, shieldUntil-Time.time); } }
    public string Alert { get { return proximity.Length>0 ? proximity : Time.time<messageUntil ? message : ""; } }
    public bool HasAlert { get { return Alert.Length>0; } }
    public static string ItemHelp(int id) { return id==1 ? "Acelera durante 2,5 segundos" : id==2 ? "Bloquea un golpe durante 8 s" : id==3 ? "Lanza chocolate hacia delante" : id==4 ? "Frena rivales a menos de 19 m" : "Cruza una caja rosa con signo ?"; }

    public string HeldName { get { return ItemName(held); } }
    public string StatusMessage { get { return Time.time < messageUntil ? message : "E: usar objeto / cajas rosas en la pista"; } }
    public bool ShieldActive { get { return Time.time < shieldUntil; } }
    public int ActiveProjectileCount { get { return projectiles.Count; } }

    void Start()
    {
        game = GetComponent<RacePrototypeGame>();
        pink = MakeMaterial(new Color(1f, .45f, .72f));
        cream = MakeMaterial(new Color(1f, .94f, .80f));
        chocolate = MakeMaterial(new Color(.30f, .12f, .055f));
        blue = MakeMaterial(new Color(.22f, .83f, .96f));
        gold = MakeMaterial(new Color(1f, .77f, .16f));
        warning = MakeMaterial(new Color(.74f, .19f, .48f));
        EnsureRacers();
        BuildTrack();
        if(!GetComponent<RacePowerHud>())gameObject.AddComponent<RacePowerHud>();
    }

    void EnsureRacers()
    {
        if (!game) return;
        CarController[] current = game.Racers;
        if (current == null || current.Length == 0) return;
        if (racers == current && initialized) return;
        if (shieldVisuals != null) foreach (var oldAura in shieldVisuals) if (oldAura) Destroy(oldAura.gameObject);
        if (slows != null) foreach (var oldSlow in slows) if (oldSlow) { oldSlow.enabled = false; Destroy(oldSlow); }
        racers = current;
        int count = racers.Length;
        bodies = new Rigidbody[count]; lastPositions = new Vector3[count];
        cpuHeld = new int[count]; cpuShield = new float[count]; hazardReady = new float[count];
        shortcutReady = new float[count]; cpuUseAt = new float[count];
        shieldVisuals = new Transform[count]; slows = new CandySlow[count];
        for (int i = 0; i < count; i++)
        {
            bodies[i] = racers[i].GetComponent<Rigidbody>();
            lastPositions[i] = racers[i] ? racers[i].transform.position : Vector3.zero;
            var aura = Primitive("Escudo de azucar", PrimitiveType.Sphere, racers[i].transform, blue);
            aura.localPosition = new Vector3(0, .3f, 0);
            aura.localScale = new Vector3(3.2f, .12f, 5.5f);
            aura.gameObject.SetActive(false);
            shieldVisuals[i] = aura;
        }
        initialized = true;
    }

    public void ConfigureTrack(int trackVariant)
    {
        variant = trackVariant;
        if (game && pink) BuildTrack();
        ResetItems();
    }

    Material MakeMaterial(Color color)
    {
        var shader = Shader.Find("Standard");
        if (!shader) shader = Shader.Find("Sprites/Default");
        var material = new Material(shader);
        material.color = color;
        materials.Add(material);
        return material;
    }

    Transform Primitive(string title, PrimitiveType type, Transform parent, Material material)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = title;
        go.transform.SetParent(parent, false);
        // Gameplay contacts use swept distances. Decorative primitives cannot stop cars.
        var collider = go.GetComponent<Collider>();
        if (collider) { collider.enabled = false; Destroy(collider); }
        go.GetComponent<Renderer>().sharedMaterial = material;
        return go.transform;
    }

    Vector3 Point(float fraction)
    {
        Vector3[] route = game.route;
        float total = 0;
        for (int i = 0; i < route.Length; i++) total += Vector3.Distance(route[i], route[(i + 1) % route.Length]);
        float remaining = Mathf.Repeat(fraction, 1) * total;
        for (int i = 0; i < route.Length; i++)
        {
            Vector3 a = route[i], b = route[(i + 1) % route.Length];
            float length = Vector3.Distance(a, b);
            if (remaining <= length) return Vector3.Lerp(a, b, length > .01f ? remaining / length : 0);
            remaining -= length;
        }
        return route[0];
    }

    Vector3 Tangent(float fraction)
    {
        Vector3 forward = Point(fraction + .007f) - Point(fraction - .007f);
        forward.y = 0;
        return forward.sqrMagnitude > .01f ? forward.normalized : Vector3.forward;
    }

    void SnapToRoad(Transform part,Vector3 fallback,float rise,bool align)
    {
        Vector3 requested=part.position-Vector3.up*rise,point,normal;
        if(!game.TryRoadSurface(requested,out point,out normal)){
            for(int k=15;k>=0;k--)if(game.TryRoadSurface(Vector3.Lerp(fallback,requested,k/16f),out point,out normal))break;
        }
        part.position=point+normal*rise;
        if(align)part.rotation=Quaternion.FromToRotation(Vector3.up,normal)*part.rotation;
    }
    void BuildTrack()
    {
        if (game.route == null || game.route.Length < 3) return;
        if (trackRoot) { trackRoot.gameObject.SetActive(false); Destroy(trackRoot.gameObject); }
        boxes.Clear(); boxReady.Clear(); traps.Clear(); moving.Clear(); movingOrigins.Clear(); movingSides.Clear();
        trackRoot = new GameObject("Proto3D - objetos y desafios originales").transform;
        trackRoot.SetParent(transform, false);
        for (int station = 0; station < 3; station++)
        {
            float fraction = .13f + station * .29f;
            Vector3 right = Vector3.Cross(Vector3.up, Tangent(fraction));
            for (int lane = 0; lane < 3; lane++)
            {
                var box = Primitive("Caja sorpresa de azucar", PrimitiveType.Cube, trackRoot, pink);
                box.position = Point(fraction) + right * (lane - 1) * 3.1f + Vector3.up * 1.35f;
                box.localScale = Vector3.one * 1.65f;
                SnapToRoad(box,Point(fraction),1.35f,false);
                boxes.Add(box); boxReady.Add(0);
                var mark = Primitive("Lazo de caja", PrimitiveType.Cube, box, cream);
                mark.localScale = new Vector3(.22f, 1.05f, 1.05f);
                for(int face=0;face<4;face++){var question=new GameObject("Signo de poder sorpresa").AddComponent<TextMesh>();question.transform.SetParent(box,false);question.transform.localRotation=Quaternion.Euler(0,face*90,0);question.transform.localPosition=question.transform.localRotation*Vector3.back*.54f;question.anchor=TextAnchor.MiddleCenter;question.alignment=TextAlignment.Center;question.fontSize=64;question.characterSize=.25f;question.fontStyle=FontStyle.Bold;question.color=new Color(1,.98f,.85f);question.text="?";}
            }
        }
        for (int i = 0; i < 2; i++)
        {
            float f = .27f + i * .39f;
            Vector3 side = Vector3.Cross(Vector3.up, Tangent(f));
            var puddle = Primitive("Charco de malvavisco - ralentiza", PrimitiveType.Cylinder, trackRoot, i == 0 ? cream : chocolate);
            puddle.position = Point(f) + side * (i == 0 ? -2.8f : 2.8f) + Vector3.up * .055f;
            puddle.localScale = new Vector3(6, .055f, 7);
            SnapToRoad(puddle,Point(f),.055f,true);
            traps.Add(puddle);
            Sign("MALVAVISCO\nDESPACIO", Point(f - .012f) + side * 8, -Tangent(f), warning);
        }
        for (int i = 0; i < 2; i++)
        {
            float f = .38f + i * .40f;
            Vector3 origin = Point(f), side = Vector3.Cross(Vector3.up, Tangent(f));
            var marshmallow = Primitive("Malvavisco movil", PrimitiveType.Capsule, trackRoot, cream);
            marshmallow.position = origin + Vector3.up * 1.15f;
            marshmallow.localScale = new Vector3(2.6f, 1.2f, 2.6f);
            moving.Add(marshmallow); movingOrigins.Add(origin); movingSides.Add(side);
            var stripe = Primitive("Aviso obstaculo movil", PrimitiveType.Cube, trackRoot, warning);
            stripe.position = Point(f - .009f) + Vector3.up * .035f;
            stripe.rotation = Quaternion.LookRotation(Tangent(f)); stripe.localScale = new Vector3(12, .04f, .7f);
            Sign("DULCE MOVIL", Point(f - .016f) + side * 8, -Tangent(f), warning);
        }
        BuildShortcutLane();
    }

    void Sign(string caption, Vector3 position, Vector3 facing, Material color)
    {
        var pole = Primitive("Senal de pista", PrimitiveType.Cylinder, trackRoot, color);
        pole.position = position + Vector3.up * 1.5f; pole.localScale = new Vector3(.15f, 1.5f, .15f);
        var sign = new GameObject(caption).AddComponent<TextMesh>();
        sign.transform.SetParent(trackRoot, false);
        sign.transform.position = position + Vector3.up * 3.2f;
        sign.transform.rotation = Quaternion.LookRotation(facing);
        sign.anchor = TextAnchor.MiddleCenter; sign.alignment = TextAlignment.Center;
        sign.fontSize = 36; sign.characterSize = .09f; sign.color = new Color(.30f, .10f, .25f);
        sign.text = caption;
    }

    void BuildShortcutLane()
    {
        // A marked fast inner lane stays within the existing track/checkpoint corridor.
        // It saves time by boosting; it never teleports or skips a control gate.
        int best = -1, bestEnd = -1; float bestLength = 25;
        // Combine short waypoint segments, because original route points are only 18 m apart.
        for (int i = 0; i < game.route.Length; i++)
        {
            for (int span = 2; span <= Mathf.Min(7, game.route.Length / 4); span++)
            {
                int end = (i + span) % game.route.Length;
                Vector3 start = game.route[i], finish = game.route[end];
                float length = Vector3.Distance(start, finish);
                if (length <= bestLength || length > 95) continue;
                bool withinRoad = true;
                for (int step = 1; step < span; step++)
                    if (HorizontalSegmentDistance(game.route[(i + step) % game.route.Length], start, finish) > 2.2f) { withinRoad = false; break; }
                if (withinRoad) { best = i; bestEnd = end; bestLength = length; }
            }
        }
        haveShortcut = best >= 0;
        if (!haveShortcut) return;
        Vector3 a = game.route[best], b = game.route[bestEnd];
        Vector3 forward = (b - a).normalized, center = Vector3.zero;
        foreach (var p in game.route) center += p;
        center /= game.route.Length;
        Vector3 side = Vector3.Cross(Vector3.up, forward);
        if (Vector3.Dot(side, center - (a + b) * .5f) < 0) side = -side;
        shortcutStart = Vector3.Lerp(a, b, .18f) + side * 4.3f;
        shortcutEnd = Vector3.Lerp(a, b, .82f) + side * 4.3f;
        var lane = Primitive("Carril secreto de azucar - turbo sin saltar checkpoints", PrimitiveType.Cube, trackRoot, gold);
        lane.position = (shortcutStart + shortcutEnd) * .5f + Vector3.up * .045f;
        lane.rotation = Quaternion.LookRotation(forward);
        lane.localScale = new Vector3(2.6f, .035f, Vector3.Distance(shortcutStart, shortcutEnd));
        for (int i = 0; i < 4; i++)
        {
            var arrow = Primitive("Flecha carril secreto", PrimitiveType.Cube, trackRoot, cream);
            arrow.position = Vector3.Lerp(shortcutStart, shortcutEnd, (i + .5f) / 4) + Vector3.up * .075f;
            arrow.rotation = Quaternion.LookRotation(forward) * Quaternion.Euler(0, 45, 0);
            arrow.localScale = new Vector3(.75f, .035f, .75f);
        }
        Sign("CARRIL SECRETO\n+ TURBO", shortcutStart + side * 3, -forward, gold);
    }

    public void ResetItems()
    {
        held = 0; shieldUntil = 0; messageUntil = 0; proximity="";nextWarning=0;
        for (int i = projectiles.Count - 1; i >= 0; i--)
            if (projectiles[i]) { projectiles[i].gameObject.SetActive(false); Destroy(projectiles[i].gameObject); }
        projectiles.Clear();
        for (int i = 0; i < boxReady.Count; i++) { boxReady[i] = 0; if (boxes[i]) boxes[i].gameObject.SetActive(true); }
        if (!initialized) return;
        for (int i = 0; i < racers.Length; i++)
        {
            cpuHeld[i] = 0; cpuShield[i] = 0; hazardReady[i] = 0; shortcutReady[i] = 0; cpuUseAt[i] = 0;
            if (shieldVisuals[i]) shieldVisuals[i].gameObject.SetActive(false);
            if (slows[i]) { slows[i].enabled = false; Destroy(slows[i]); slows[i] = null; }
            lastPositions[i] = racers[i] ? racers[i].transform.position : Vector3.zero;
        }
    }

    void Update()
    {
        if (!game) return;
        EnsureRacers();
        if (!initialized) return;
        for (int i = 0; i < racers.Length; i++)
            if (shieldVisuals[i]) shieldVisuals[i].gameObject.SetActive(i == 0 ? ShieldActive : Time.time < cpuShield[i]);
        if (!game.IsRacing || Time.timeScale == 0) return;
        if(Time.time>=nextWarning){nextWarning=Time.time+.12f;UpdateWarning();}
        for (int i = 0; i < boxes.Count; i++)
        {
            if (!boxes[i]) continue;
            boxes[i].gameObject.SetActive(Time.time >= boxReady[i]);
            if (boxes[i].gameObject.activeSelf) boxes[i].Rotate(0, 50 * Time.deltaTime, 20 * Time.deltaTime, Space.Self);
        }
        if (Input.GetKeyDown(KeyCode.E)) UseItem();
        for (int i = 1; i < racers.Length; i++)
            if (cpuHeld[i] > 0 && Time.time >= cpuUseAt[i]) UseRacerItem(i);
    }

    void FixedUpdate()
    {
        if (!initialized || !game) return;
        if (!game.IsRacing)
        {
            for (int c = 0; c < racers.Length; c++) if (racers[c]) lastPositions[c] = racers[c].transform.position;
            return;
        }
        for (int m = 0; m < moving.Count; m++)
            {moving[m].position = movingOrigins[m] + movingSides[m] * Mathf.Sin(Time.time * (1.1f + variant * .13f) + m * 2.3f) * 4.7f + Vector3.up * 1.15f;SnapToRoad(moving[m],movingOrigins[m],1.15f,false);}
        for (int c = 0; c < racers.Length; c++)
        {
            if (!racers[c]) continue;
            Vector3 now = racers[c].transform.position;
            int currentItem = c == 0 ? held : cpuHeld[c];
            if (currentItem == 0)
            {
                for (int b = 0; b < boxes.Count; b++)
                {
                    if (Time.time < boxReady[b] || !boxes[b] || !Contact(lastPositions[c], now, boxes[b].position, 2.8f, 3.3f)) continue;
                    int item = Random.Range(1, 5);
                    if (c == 0) { held = item; Notify("¡PODER CONSEGUIDO! " + ItemName(item) + "\nPulsa E: " + ItemHelp(item)); }
                    else { cpuHeld[c] = item; cpuUseAt[c] = Time.time + Random.Range(1.1f, 3.2f); }
                    boxReady[b] = Time.time + 9f; boxes[b].gameObject.SetActive(false); Cue(0); break;
                }
            }
            if (Time.time >= hazardReady[c])
            {
                bool touched = false;
                foreach (var puddle in traps) if (Contact(lastPositions[c], now, puddle.position, 3.3f, 3f)) { touched = true; break; }
                if (!touched) foreach (var obstacle in moving) if (Contact(lastPositions[c], now, obstacle.position, 2.8f, 3.3f)) { touched = true; break; }
                if (touched) { hazardReady[c] = Time.time + 3.2f; ApplySlow(racers[c], 1.7f); }
            }
            if (haveShortcut && Time.time >= shortcutReady[c] && HorizontalSegmentDistance(now, shortcutStart, shortcutEnd) < 2.4f && Mathf.Abs(now.y - shortcutStart.y) < 3)
            {
                shortcutReady[c] = Time.time + 9; game.ActivateBoost(c, 1.6f);
                if (c == 0) Notify("CARRIL SECRETO: TURBO DE AZUCAR");
                Cue(1);
            }
            lastPositions[c] = now;
        }
    }

    void UseItem() { if (game && game.IsRacing && initialized) UseRacerItem(0); }
    public void UseItem(CarController car) { int index = IndexOf(car); if (index >= 0 && game.IsRacing) UseRacerItem(index); }

    void UseRacerItem(int index)
    {
        int item = index == 0 ? held : cpuHeld[index];
        if (item == 0) { if(index==0)Notify("SIN PODER\nCruza una caja rosa con signo ? para conseguir uno");return; }
        if (index == 0) held = 0; else cpuHeld[index] = 0;
        if (item == 1)
        {
            game.ActivateBoost(index, 2.5f); Cue(1);
            if (index == 0) Notify("TURBO DE AZUCAR / 2,5 s");
        }
        else if (item == 2)
        {
            if (index == 0) { shieldUntil = Time.time + 8; Notify("ESCUDO DE CARAMELO / protege un golpe"); }
            else cpuShield[index] = Time.time + 8;
            Cue(2);
        }
        else if (item == 3)
        {
            // Explicit cap prevents unbounded primitives even during repeated automated checks.
            if (projectiles.Count >= 12) { var first = projectiles[0]; projectiles.RemoveAt(0); if (first) Destroy(first.gameObject); }
            var projectile = Primitive("Bola de chocolate", PrimitiveType.Sphere, null, chocolate);
            projectile.position = racers[index].transform.position + racers[index].transform.forward * 3.6f + Vector3.up * .7f;
            projectile.localScale = Vector3.one * 1.1f;
            var shot = projectile.gameObject.AddComponent<CandyProjectile>();
            shot.Initialize(this, racers[index], racers[index].transform.forward, Mathf.Max(48, bodies[index] ? bodies[index].velocity.magnitude + 22 : 48));
            projectiles.Add(shot); Cue(3);
            if (index == 0) Notify("CHOCOLATE LANZADO");
        }
        else if (item == 4)
        {
            // A marshmallow burst slows nearby opponents, with the same shield rules as chocolate.
            for (int i = 0; i < racers.Length; i++)
                if (i != index && (racers[i].transform.position - racers[index].transform.position).sqrMagnitude < 19 * 19) ApplySlow(racers[i], 2.2f);
            Cue(4); if (index == 0) Notify("NUBE DE MALVAVISCO / radio 19 m");
        }
    }

    public bool Absorb() { return game && TryShield(game.player); }
    public bool TryShield(CarController car)
    {
        int index = IndexOf(car); if (index < 0) return false;
        bool active = index == 0 ? Time.time < shieldUntil : Time.time < cpuShield[index];
        if (!active) return false;
        if (index == 0) { shieldUntil = 0; Notify("ESCUDO: GOLPE BLOQUEADO"); }
        else cpuShield[index] = 0;
        if (shieldVisuals[index]) shieldVisuals[index].gameObject.SetActive(false);
        Cue(2); return true;
    }

    public void ApplySlow(CarController car, float seconds)
    {
        int index = IndexOf(car); if (index < 0 || TryShield(car)) return;
        if (!slows[index]) slows[index] = car.gameObject.AddComponent<CandySlow>();
        slows[index].remaining = Mathf.Max(slows[index].remaining, Mathf.Clamp(seconds, .1f, 5));
        slows[index].race = game;
        Cue(4); if (index == 0) Notify("¡TE HAN FRENADO!\nVelocidad reducida durante "+seconds.ToString("0.0")+" s");
    }

    int IndexOf(CarController car)
    {
        if (racers == null || !car) return -1;
        for (int i = 0; i < racers.Length; i++) if (racers[i] == car) return i;
        return -1;
    }

    public bool SweepChocolate(CarController owner, Vector3 from, Vector3 to)
    {
        if (!initialized) return false;
        int target = -1; float nearest = float.MaxValue;
        for (int i = 0; i < racers.Length; i++)
        {
            if (!racers[i] || racers[i] == owner) continue;
            Vector3 center = racers[i].transform.position + Vector3.up * .55f;
            if (!Contact(from, to, center, 2.5f, 2.5f)) continue;
            float distance = (center - from).sqrMagnitude;
            if (distance < nearest) { nearest = distance; target = i; }
        }
        if (target < 0) return false;
        ApplySlow(racers[target], 2.5f); return true;
    }

    public bool IsActiveRace { get { return game && game.IsRacing; } }
    public void ReleaseProjectile(CandyProjectile projectile) { projectiles.Remove(projectile); }
    public static float HorizontalSegmentDistance(Vector3 point, Vector3 a, Vector3 b)
    {
        point.y = a.y = b.y = 0;
        Vector3 segment = b - a;
        float t = segment.sqrMagnitude > .0001f ? Mathf.Clamp01(Vector3.Dot(point - a, segment) / segment.sqrMagnitude) : 0;
        return Vector3.Distance(point, a + segment * t);
    }
    public static bool Contact(Vector3 from, Vector3 to, Vector3 center, float radius, float height)
    {
        return Mathf.Abs((from.y + to.y) * .5f - center.y) < height && HorizontalSegmentDistance(center, from, to) < radius;
    }
    void UpdateWarning(){
        proximity="";if(!game.player)return;var player=game.player.transform;
        foreach(var shot in projectiles){if(!shot||shot.owner==game.player)continue;Vector3 delta=player.position-shot.transform.position;delta.y=0;float along=Vector3.Dot(delta,shot.direction);if(along>0&&along<48&&(delta-shot.direction*along).sqrMagnitude<20){proximity="¡CHOCOLATE EN CAMINO!\nEsquiva o activa tu escudo con E";return;}}
        foreach(var obstacle in moving)if(Ahead(player,obstacle.position,30)){proximity="¡MALVAVISCO MOVIL CERCA!\nBusca un hueco para pasar";return;}
        foreach(var trap in traps)if(Ahead(player,trap.position,27)){proximity="¡TRAMPA CERCA!\nEsquiva el charco para no perder velocidad";return;}
    }
    static bool Ahead(Transform car,Vector3 point,float distance){Vector3 delta=point-car.position;if(Mathf.Abs(delta.y)>4)return false;delta.y=0;Vector3 forward=Vector3.ProjectOnPlane(car.forward,Vector3.up).normalized;float along=Vector3.Dot(delta,forward);return along>3&&along<distance&&(delta-forward*along).sqrMagnitude<30;}
    string ItemName(int item) { return item == 1 ? "TURBO DE FRESA" : item == 2 ? "ESCUDO DE CARAMELO" : item == 3 ? "BOLA DE CHOCOLATE" : item == 4 ? "NUBE DE MALVAVISCO" : "SIN PODER"; }
    void Notify(string text) { message = text; messageUntil = Time.time + 4; }
    void Cue(int cue) { gameObject.SendMessage("CandyCue", cue, SendMessageOptions.DontRequireReceiver); }

    void OnDestroy()
    {
        ResetItems();
        if (trackRoot) Destroy(trackRoot.gameObject);
        if (shieldVisuals != null) foreach (var aura in shieldVisuals) if (aura) Destroy(aura.gameObject);
        foreach (var material in materials) if (material) Destroy(material);
    }
}

public class CandyProjectile : MonoBehaviour
{
    public Vector3 direction;
    public CarController owner;
    RaceCandyItems items;
    float speed = 52, life;
    bool released;
    public void Initialize(RaceCandyItems source, CarController sourceCar, Vector3 forward, float velocity)
    {
        items = source; owner = sourceCar; direction = forward.normalized; speed = Mathf.Clamp(velocity, 35, 75);
    }
    void FixedUpdate()
    {
        if (!items || !items.IsActiveRace) { Destroy(gameObject); return; }
        Vector3 from = transform.position, to = from + direction * speed * Time.fixedDeltaTime;
        // Segment sweep avoids tunneling past cars between physics ticks.
        if (items.SweepChocolate(owner, from, to)) { Destroy(gameObject); return; }
        transform.position = to; transform.Rotate(100 * Time.fixedDeltaTime, 0, 0);
        life += Time.fixedDeltaTime;
        if (life > 4) Destroy(gameObject);
    }
    void OnDestroy() { if (!released && items) { released = true; items.ReleaseProjectile(this); } }
}

public class CandySlow : MonoBehaviour
{
    public float remaining;
    public RacePrototypeGame race;
    Rigidbody body;
    void Awake() { body = GetComponent<Rigidbody>(); }
    void FixedUpdate()
    {
        if (race && !race.IsRacing) return;
        if (body)
        {
            Vector3 horizontal = body.velocity; horizontal.y = 0;
            if (horizontal.magnitude > 9)
            {
                horizontal = Vector3.MoveTowards(horizontal, horizontal.normalized * 9, 28 * Time.fixedDeltaTime);
                body.velocity = new Vector3(horizontal.x, body.velocity.y, horizontal.z);
            }
        }
        remaining -= Time.fixedDeltaTime;
        if (remaining <= 0) Destroy(this);
    }
}
