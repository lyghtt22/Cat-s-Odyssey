using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Collections.Generic;
 public static class PinkRushStyle {
 static Color rose=new Color(1,.32f,.62f), blush=new Color(1,.79f,.88f), cream=new Color(1,.94f,.86f), lavender=new Color(.66f,.5f,.72f);
 static int id;static Dictionary<Material,Material> styled=new Dictionary<Material,Material>();

 [MenuItem("Race Prototype/Aplicar estilo rosita")]
 static void Apply(){
  if(EditorApplication.isPlayingOrWillChangePlaymode)return;
  var scene=EditorSceneManager.GetActiveScene();if(scene.path!="Assets/Scenes/RacePrototype.unity"||!GameObject.Find("RacePrototype - Race Manager")||GameObject.Find("Pink Rush - Art Direction"))return;
  Directory.CreateDirectory("Assets/RacePrototype/PinkMaterials");
  foreach(var root in scene.GetRootGameObjects())foreach(var r in root.GetComponentsInChildren<Renderer>(true)){
   if(!root.activeSelf)continue;
   var mats=r.sharedMaterials;
   for(int j=0;j<mats.Length;j++){
    if(!mats[j])continue;string n=(r.name+" "+mats[j].name).ToLower();Color c=Color.white;bool change=false,flat=false;
    if(root.name=="Car"&&r.name.ToLower().Contains("body")){c=rose;change=true;flat=true;}
    else if(n.Contains("tarmac_oval")){c=new Color(.37f,.25f,.38f);change=true;flat=true;}
    else if(n.Contains("grass")){c=new Color(.59f,.75f,.67f);change=true;flat=true;}
    else if(n.Contains("rock")){c=lavender;change=true;flat=true;}
    else if(n.Contains("wall_")&&!n.Contains("fence")){c=blush;change=true;flat=true;}
    else if(n.Contains("boost")){c=n.Contains("surface")?rose:cream;change=true;flat=true;}
    else if(n.Contains("marker")){c=rose;change=true;flat=true;}
    if(change){var m=new Material(mats[j]);m.name="Pink "+r.name;if(m.HasProperty("_Color"))m.color=c;if(flat){m.mainTexture=null;if(m.HasProperty("_BumpMap"))m.SetTexture("_BumpMap",null);}if(m.HasProperty("_Glossiness"))m.SetFloat("_Glossiness",root.name=="Car"?.6f:.12f);if(m.IsKeywordEnabled("_EMISSION"))m.SetColor("_EmissionColor",c*.5f);AssetDatabase.CreateAsset(m,"Assets/RacePrototype/PinkMaterials/Pink"+(id++)+".mat");mats[j]=m;}
   }r.sharedMaterials=mats;
  }
  var art=new GameObject("Pink Rush - Art Direction").transform;
  var sky=new Material(Shader.Find("Skybox/Procedural"));sky.SetColor("_SkyTint",new Color(.7f,.45f,.64f));sky.SetColor("_GroundColor",blush);sky.SetFloat("_AtmosphereThickness",.6f);sky.SetFloat("_Exposure",1.3f);AssetDatabase.CreateAsset(sky,"Assets/RacePrototype/PinkMaterials/PinkSky.mat");RenderSettings.skybox=sky;
  RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;RenderSettings.ambientLight=new Color(.78f,.69f,.77f);RenderSettings.ambientIntensity=1.1f;RenderSettings.fog=true;RenderSettings.fogMode=FogMode.Linear;RenderSettings.fogColor=new Color(.93f,.79f,.87f);RenderSettings.fogStartDistance=220;RenderSettings.fogEndDistance=900;
  foreach(var l in Object.FindObjectsOfType<Light>())if(l.type==LightType.Directional){l.color=new Color(1,.88f,.9f);l.intensity=1.15f;l.shadows=LightShadows.Soft;}
  for(int i=0;i<16;i++){
   float x=-110+i*15;var deco=new GameObject("Candy blossom "+i).transform;deco.SetParent(art);deco.position=new Vector3(x,0,-10);
   Shape("Stem",PrimitiveType.Cylinder,deco,new Vector3(0,3,0),new Vector3(.35f,3,.35f),cream);
   Shape("Blossom",PrimitiveType.Sphere,deco,new Vector3(0,7,0),new Vector3(5,5,5),i%2==0?rose:blush);
   Shape("Leaf",PrimitiveType.Sphere,deco,new Vector3(1.1f,4,0),new Vector3(2,.6f,1),new Color(.4f,.72f,.61f));
  }
  var arch=new GameObject("Pink Rush - Finish arch").transform;arch.SetParent(art);arch.position=new Vector3(1,0,-29);
  Shape("Left post",PrimitiveType.Cube,arch,new Vector3(0,4,-11),new Vector3(1.4f,8,1.4f),rose);
  Shape("Right post",PrimitiveType.Cube,arch,new Vector3(0,4,11),new Vector3(1.4f,8,1.4f),rose);
  Shape("Header",PrimitiveType.Cube,arch,new Vector3(0,8,0),new Vector3(1.3f,2,23),blush);
  var sign=new GameObject("PINK RUSH sign");sign.transform.SetParent(arch,false);sign.transform.localPosition=new Vector3(-.75f,8,0);sign.transform.rotation=Quaternion.Euler(0,90,0);var tm=sign.AddComponent<TextMesh>();tm.text="P I N K   R U S H";tm.fontSize=80;tm.characterSize=.15f;tm.anchor=TextAnchor.MiddleCenter;tm.color=new Color(.42f,.09f,.3f);
  var turbo=GameObject.Find("TURBO sign");if(turbo){turbo.GetComponent<TextMesh>().color=rose;}
  EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("PINK RUSH: pastel art direction saved");
 }
 static void Shape(string name,PrimitiveType type,Transform parent,Vector3 p,Vector3 scale,Color c){var g=GameObject.CreatePrimitive(type);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=p;g.transform.localScale=scale;g.GetComponent<Collider>().enabled=false;var m=new Material(Shader.Find("Standard"));m.color=c;m.SetFloat("_Glossiness",.25f);AssetDatabase.CreateAsset(m,"Assets/RacePrototype/PinkMaterials/Decor"+(id++)+".mat");g.GetComponent<Renderer>().sharedMaterial=m;}
}
