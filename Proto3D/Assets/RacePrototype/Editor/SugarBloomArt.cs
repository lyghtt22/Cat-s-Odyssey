using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityStandardAssets.Vehicles.Car;
using System.IO;
using System.Collections.Generic;
[InitializeOnLoad] public static class SugarBloomArt {
 static Color pink=new Color(1,.43f,.67f),mint=new Color(.42f,.85f,.74f),vanilla=new Color(1,.9f,.72f),chocolate=new Color(.29f,.12f,.1f),berry=new Color(.65f,.18f,.4f);
 static Material frosting,cream,cocoa,peppermint,wafer,raspberry,glass;
 static Transform art; static int num;
 static SugarBloomArt(){EditorApplication.delayCall+=Apply;}
 static void Apply(){ if(GameObject.Find("Sugar Bloom - Ready"))return;
  if(EditorApplication.isCompiling||EditorApplication.isUpdating){EditorApplication.delayCall+=Apply;return;}
  if(EditorApplication.isPlayingOrWillChangePlaymode){if(EditorApplication.isPlaying)EditorApplication.isPlaying=false;EditorApplication.delayCall+=Apply;return;}
  var scene=EditorSceneManager.GetActiveScene();if(scene.path!="Assets/Scenes/RacePrototype.unity"||GameObject.Find("Sugar Bloom - Ready"))return;
  var game=Object.FindObjectOfType<RacePrototypeGame>();if(!game)return;
  var partial=GameObject.Find("Sugar Bloom - Candy World");if(partial)Object.DestroyImmediate(partial); Directory.CreateDirectory("Assets/RacePrototype/CandyArt");
  try{
   frosting=Mat("Strawberry frosting",pink,.5f);cream=Mat("Vanilla cream",vanilla,.25f);cocoa=Mat("Dark chocolate",chocolate,.3f);peppermint=Mat("Mint icing",mint,.45f);wafer=Mat("Biscuit",new Color(.77f,.51f,.3f),.15f);raspberry=Mat("Berry sugar",berry,.4f);glass=Mat("Dark candy glass",new Color(.12f,.1f,.17f),.7f);
   foreach(var root in scene.GetRootGameObjects()){
    if(root.name=="Pink Rush - Art Direction"){root.SetActive(false);continue;}
    if(!root.name.StartsWith("oval"))continue;
    foreach(var r in root.GetComponentsInChildren<Renderer>(true)){
     string n=r.name.ToLower();
     if(n.Contains("fence")||n.Contains("wall")||n.Contains("tree")||n.Contains("garage")||n.Contains("lamp")||n=="plane001"||n=="0grass"||n=="cylinder001"){r.enabled=false;continue;}
     if(n.StartsWith("line_seg_")){int k;if(int.TryParse(n.Substring(9),out k)&&k>4){r.enabled=false;continue;}}
     Material m=null;
     if(n.Contains("tarmac")||n.Contains("pitlane"))m=cocoa;
     else if(n.Contains("grass"))m=peppermint;
     else if(n.Contains("gravel")||n.Contains("sand")||n.Contains("kerb"))m=wafer;
     else if(n.Contains("concrete"))m=frosting;
     if(m){var a=r.sharedMaterials;for(int i=0;i<a.Length;i++)a[i]=m;r.sharedMaterials=a;}
    }
   }
   art=new GameObject("Sugar Bloom - Candy World").transform;
   Shape("Fondant landscape",PrimitiveType.Cube,art,new Vector3(0,-5.5f,-160),new Vector3(1600,2,1400),cream);
   RenderSettings.skybox=null;RenderSettings.fog=true;RenderSettings.fogMode=FogMode.Linear;RenderSettings.fogColor=new Color(.79f,.9f,.92f);RenderSettings.fogStartDistance=200;RenderSettings.fogEndDistance=700;
   game.chase.clearFlags=CameraClearFlags.SolidColor;game.chase.backgroundColor=new Color(.79f,.9f,.92f);
   RenderSettings.ambientLight=new Color(.83f,.8f,.81f);
   foreach(var l in Object.FindObjectsOfType<Light>())if(l.type==LightType.Directional){l.intensity=1.05f;l.color=new Color(1,.94f,.86f);l.transform.rotation=Quaternion.Euler(45,-35,0);}
   Random.InitState(42);
   // Rounded candy barriers follow the real race route, including its banking.
   for(int i=0;i<game.route.Length;i++){
    Vector3 p=game.route[i],q=game.route[(i+1)%game.route.Length];Vector3 d=(q-p).normalized;Vector3 side=Vector3.Cross(Vector3.up,d).normalized;
    for(int s=-1;s<=1;s+=2){
     Vector3 b=(p+q)*.5f+side*s*10.1f;var rail=Shape("Marshmallow rail",PrimitiveType.Capsule,art,b+Vector3.up*.7f,new Vector3(1.2f,Vector3.Distance(p,q)*.48f,1.2f),i%3==0?frosting:cream);rail.transform.rotation=Quaternion.FromToRotation(Vector3.up,d);
     if(i%4==0)Lollipop(art,p+side*s*20,6+i%3,i%8==0?peppermint:frosting);
    }
    if(i%3==0){Vector3 pos=p+side*(i%2==0?1:-1)*Random.Range(38,90);pos.y=-1;Cupcake(art,pos,Random.Range(6f,12f),i%2==0?frosting:peppermint);}
   }
   // Candy hills, soft clouds and sprinkles replace the industrial skyline.
   for(int i=0;i<24;i++){
    float a=i*Mathf.PI*2/24;Vector3 pos=new Vector3(Mathf.Cos(a)*430,-5,-160+Mathf.Sin(a)*320);
    float h=Random.Range(35,70);Shape("Gumdrop mountain",PrimitiveType.Sphere,art,pos,new Vector3(Random.Range(70,120),h*2,Random.Range(70,110)),i%3==0?peppermint:i%3==1?frosting:cream);
   }
   for(int i=0;i<16;i++){Vector3 pos=new Vector3(Random.Range(-450,450),Random.Range(65,105),Random.Range(-500,100));for(int j=0;j<3;j++)Shape("Cotton candy cloud",PrimitiveType.Sphere,art,pos+new Vector3(j*11,Mathf.Sin(j)*4,0),new Vector3(25,13,16),cream);}
   BuildKart(game.player.transform,false);BuildKart(game.rival.transform,true);
   var trailGo=new GameObject("Candy skid trail");var tr=trailGo.AddComponent<TrailRenderer>();tr.time=4;tr.startWidth=.16f;tr.endWidth=.08f;tr.sharedMaterial=cocoa;tr.startColor=new Color(.2f,.08f,.1f,.35f);tr.endColor=new Color(.2f,.08f,.1f,0);trGoSave(trailGo,game);
   // Chocolate starting arch, frosting trim and candy lettering.
   var old=GameObject.Find("Pink Rush - Finish arch");if(old)old.SetActive(false);
   var arch=new GameObject("Sugar Bloom - Cookie finish arch").transform;arch.SetParent(art);arch.position=new Vector3(1,0,-29);
   for(int s=-1;s<=1;s+=2){Shape("Wafer column",PrimitiveType.Cube,arch,new Vector3(0,4,s*11),new Vector3(2,8,2),wafer);for(int j=0;j<6;j++){var stripe=Shape("Icing stripe",PrimitiveType.Cube,arch,new Vector3(-1.05f,1+j*1.2f,s*11),new Vector3(.12f,.4f,2.1f),frosting);stripe.transform.localRotation=Quaternion.Euler(25,0,0);}Shape("Marshmallow cap",PrimitiveType.Sphere,arch,new Vector3(0,8,s*11),Vector3.one*3,cream);}
   Shape("Cookie header",PrimitiveType.Cube,arch,new Vector3(0,8,0),new Vector3(2,2,21),cocoa);
   Shape("Frosted header",PrimitiveType.Capsule,arch,new Vector3(-1.1f,8,0),new Vector3(1.6f,10,1.6f),frosting).transform.localRotation=Quaternion.Euler(90,0,0);
   Text("S U G A R   B L O O M",arch,new Vector3(-2,8,0),.12f,vanilla);
   var pad=GameObject.Find("TURBO - Cyan launch pad");if(pad){foreach(var r in pad.GetComponentsInChildren<Renderer>()){if(r.GetComponent<TextMesh>()){r.GetComponent<TextMesh>().text="SUGAR BOOST";r.GetComponent<TextMesh>().color=pink;}else r.sharedMaterial=r.name.Contains("surface")?peppermint:frosting;}}
   new GameObject("Sugar Bloom - Ready").transform.SetParent(art);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
  }finally{AssetDatabase.SaveAssets();}
  Debug.Log("SUGAR BLOOM: candy world and candy karts saved");
 }
 static Material Mat(string name,Color c,float gloss){var m=new Material(Shader.Find("Standard"));m.name=name;m.color=c;m.SetFloat("_Glossiness",gloss);AssetDatabase.CreateAsset(m,"Assets/RacePrototype/CandyArt/"+name+".mat");return m;}
 static GameObject Shape(string name,PrimitiveType type,Transform parent,Vector3 p,Vector3 scale,Material m){var g=GameObject.CreatePrimitive(type);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=p;g.transform.localScale=scale;g.GetComponent<Collider>().enabled=false;g.GetComponent<Renderer>().sharedMaterial=m;return g;}
 static void Text(string text,Transform parent,Vector3 pos,float size,Color color){var g=new GameObject(text);g.transform.SetParent(parent,false);g.transform.localPosition=pos;g.transform.localRotation=Quaternion.Euler(0,90,0);var t=g.AddComponent<TextMesh>();t.text=text;t.characterSize=size;t.fontSize=80;t.anchor=TextAnchor.MiddleCenter;t.color=color;}
 static void Lollipop(Transform parent,Vector3 p,float height,Material icing){
  var t=new GameObject("Swirl lollipop").transform;t.SetParent(parent);t.position=p;
  Shape("Cookie stick",PrimitiveType.Cylinder,t,new Vector3(0,height*.5f,0),new Vector3(.4f,height*.5f,.4f),cream);
  var disc=Shape("Candy disc",PrimitiveType.Cylinder,t,new Vector3(0,height,0),new Vector3(4,.35f,4),icing);disc.transform.localRotation=Quaternion.Euler(90,0,0);
  for(int j=0;j<36;j++){float a=j*.37f,r=.08f+j*.048f;Shape("Cream swirl",PrimitiveType.Sphere,t,new Vector3(Mathf.Cos(a)*r,height+Mathf.Sin(a)*r,-.38f),new Vector3(.32f,.32f,.15f),cream);}
 }
 static void Cupcake(Transform parent,Vector3 p,float size,Material icing){
  var t=new GameObject("Giant cupcake").transform;t.SetParent(parent);t.position=p;t.localScale=Vector3.one*size;
  Shape("Biscuit cup",PrimitiveType.Cylinder,t,new Vector3(0,.5f,0),new Vector3(1.6f,.5f,1.6f),wafer);
  for(int j=0;j<12;j++){float a=j*Mathf.PI*2/12;Shape("Cup groove",PrimitiveType.Capsule,t,new Vector3(Mathf.Sin(a)*.77f,.5f,Mathf.Cos(a)*.77f),new Vector3(.12f,.47f,.12f),cream);}
  Shape("Frosting",PrimitiveType.Sphere,t,new Vector3(0,1.1f,0),new Vector3(1.9f,1.1f,1.9f),icing);
  Shape("Whipped cream",PrimitiveType.Sphere,t,new Vector3(0,1.6f,0),new Vector3(1.2f,.8f,1.2f),cream);
  Shape("Cherry",PrimitiveType.Sphere,t,new Vector3(0,2.05f,0),Vector3.one*.42f,raspberry);
  for(int j=0;j<12;j++){float a=j*2.4f;Shape("Sugar sprinkle",PrimitiveType.Capsule,t,new Vector3(Mathf.Sin(a)*.65f,1.35f+(j%3)*.1f,Mathf.Cos(a)*.65f),new Vector3(.08f,.17f,.08f),j%2==0?peppermint:raspberry).transform.localRotation=Random.rotation;}
 }
 static void BuildKart(Transform car,bool rival){
  var prior=car.Find("Candy kart - handmade");if(prior)Object.DestroyImmediate(prior.gameObject); foreach(var r in car.GetComponentsInChildren<Renderer>())r.enabled=false;
  var t=new GameObject("Candy kart - handmade").transform;t.SetParent(car,false);
  Material color=rival?peppermint:frosting;
  Shape("Cookie chassis",PrimitiveType.Cube,t,new Vector3(0,.65f,0),new Vector3(1.8f,.4f,3.4f),wafer);
  Shape("Chocolate shell",PrimitiveType.Sphere,t,new Vector3(0,.75f,.05f),new Vector3(2,.85f,3.7f),cocoa);
  Shape("Strawberry icing",PrimitiveType.Sphere,t,new Vector3(0,.96f,.05f),new Vector3(1.98f,.65f,3.5f),color);
  Shape("Mint hood",PrimitiveType.Sphere,t,new Vector3(0,1.12f,1.05f),new Vector3(1.45f,.3f,1.15f),rival?frosting:peppermint);
  Shape("Chocolate seat",PrimitiveType.Sphere,t,new Vector3(0,1.24f,-.35f),new Vector3(1.05f,.35f,1.2f),cocoa);
  Shape("Seat back",PrimitiveType.Sphere,t,new Vector3(0,1.52f,-.8f),new Vector3(1,.9f,.3f),color);
  // Tiny original candy driver, with a mint racing helmet.
  Shape("Driver",PrimitiveType.Capsule,t,new Vector3(0,1.65f,-.35f),new Vector3(.55f,.42f,.5f),peppermint);
  Shape("Helmet",PrimitiveType.Sphere,t,new Vector3(0,2.13f,-.27f),Vector3.one*.66f,color);
  Shape("Visor",PrimitiveType.Sphere,t,new Vector3(0,2.13f,.02f),new Vector3(.5f,.24f,.13f),glass);
  Shape("Helmet candy",PrimitiveType.Sphere,t,new Vector3(.2f,2.43f,-.27f),Vector3.one*.2f,cream);
  for(int side=-1;side<=1;side+=2)for(int end=-1;end<=1;end+=2){
   Vector3 p=new Vector3(side*.99f,.5f,end*1.17f);
   var wheel=Shape("Chocolate cookie tire",PrimitiveType.Cylinder,t,p,new Vector3(.85f,.22f,.85f),cocoa);wheel.transform.localRotation=Quaternion.Euler(0,0,90);
   var cap=Shape("Peppermint wheel cap",PrimitiveType.Cylinder,t,p+Vector3.right*side*.25f,new Vector3(.65f,.05f,.65f),cream);cap.transform.localRotation=Quaternion.Euler(0,0,90);
   for(int j=0;j<6;j++){float a=j*Mathf.PI/3;Shape("Wheel candy",PrimitiveType.Sphere,t,p+new Vector3(side*.32f,Mathf.Sin(a)*.2f,Mathf.Cos(a)*.2f),new Vector3(.08f,.15f,.15f),color);}
  }
  for(int i=0;i<20;i++){float a=i*2.4f;var sprinkle=Shape("Kart sprinkle",PrimitiveType.Capsule,t,new Vector3(Mathf.Sin(a)*.8f,1.12f,Mathf.Cos(a)*1.45f),new Vector3(.07f,.12f,.07f),i%3==0?cream:i%3==1?peppermint:raspberry);sprinkle.transform.localRotation=Quaternion.Euler(0,i*20,65);}
  for(int s=-1;s<=1;s+=2)Shape("Sugar headlamp",PrimitiveType.Sphere,t,new Vector3(s*.6f,.95f,1.65f),Vector3.one*.3f,cream);
  Shape("Wafer spoiler",PrimitiveType.Cube,t,new Vector3(0,1.45f,-1.6f),new Vector3(2.1f,.18f,.48f),cream);
 }
 static void trGoSave(GameObject go,RacePrototypeGame game){var prefab=PrefabUtility.SaveAsPrefabAsset(go,"Assets/RacePrototype/CandyArt/CandySkid.prefab");Object.DestroyImmediate(go);foreach(var car in new[]{game.player,game.rival})foreach(var effect in car.GetComponentsInChildren<WheelEffects>())effect.SkidTrailPrefab=prefab.transform;}
}


