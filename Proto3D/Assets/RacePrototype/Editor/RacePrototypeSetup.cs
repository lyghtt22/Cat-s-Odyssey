using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityStandardAssets.Vehicles.Car;
using System.Collections.Generic;
using System.IO;
[InitializeOnLoad] public static class RacePrototypeSetup {
 static RacePrototypeSetup(){EditorApplication.delayCall+=BuildOnce;}
 [MenuItem("Race Prototype/Preparar avance")]
 static void BuildOnce(){
  if(EditorApplication.isPlayingOrWillChangePlaymode)return;
  var scene=EditorSceneManager.GetActiveScene();
  if(scene.path!="Assets/Scenes/RacePrototype.unity"||GameObject.Find("RacePrototype - Race Manager"))return;
  string output=@"C:\Users\lyght\Documents\Codex\2026-10-18\referenced-chatgpt-conversation-this-is-an-3\outputs";
  Directory.CreateDirectory(output);
  File.Copy(scene.path,Path.Combine(output,"RacePrototype-before.unity"),false);
  var player=GameObject.Find("Car").GetComponent<CarController>();
  foreach(var root in scene.GetRootGameObjects())if(root.name.Contains("CameraRig")||root.name=="LimitFloor")root.SetActive(false);
  Directory.CreateDirectory("Assets/RacePrototype/Materials"); Configure(player);
  player.transform.position=new Vector3(-15,.65f,-29);player.transform.rotation=Quaternion.Euler(0,90,0);
  var rival=Object.Instantiate(player.gameObject).GetComponent<CarController>();rival.name="Rival IA - Crimson";rival.transform.position=new Vector3(-9,.65f,-34);
  foreach(var r in rival.GetComponentsInChildren<Renderer>())if(r.name.ToLower().Contains("body")){foreach(var m in r.materials){if(m.HasProperty("_Color"))m.color=new Color(.8f,.04f,.09f);AssetDatabase.CreateAsset(m,"Assets/RacePrototype/Materials/Rival"+(materialCount++)+".mat");}}
  var manager=new GameObject("RacePrototype - Race Manager").AddComponent<RacePrototypeGame>();manager.player=player;manager.rival=rival;
  var points=new List<Vector3>();
  for(int i=0;i<8;i++)points.Add(new Vector3(145f*i/8,0,-29));
  for(int i=0;i<24;i++){float a=Mathf.PI*i/24;points.Add(new Vector3(145+132.5f*Mathf.Sin(a),0,-161.5f+132.5f*Mathf.Cos(a)));}
  for(int i=0;i<16;i++)points.Add(new Vector3(145-290f*i/16,0,-294));
  for(int i=0;i<24;i++){float a=Mathf.PI+Mathf.PI*i/24;points.Add(new Vector3(-145+132.5f*Mathf.Sin(a),0,-161.5f+132.5f*Mathf.Cos(a)));}
  for(int i=0;i<8;i++)points.Add(new Vector3(-145+145f*i/8,0,-29));
  for(int i=0;i<points.Count;i++){RaycastHit hit;if(Physics.Raycast(points[i]+Vector3.up*15,Vector3.down,out hit,30))points[i]=hit.point;}
  manager.route=points.ToArray();manager.gates=new Vector3[8];
  var routeRoot=new GameObject("Waypoints - Oval").transform;routeRoot.SetParent(manager.transform);
  for(int i=0;i<points.Count;i++){var p=new GameObject("WP "+i.ToString("00")).transform;p.SetParent(routeRoot);p.position=points[i];}
  for(int i=0;i<8;i++){
   int k=i*points.Count/8;manager.gates[i]=points[k];Vector3 dir=(points[(k+1)%points.Count]-points[k]).normalized;
   var gate=new GameObject(i==0?"Salida - Meta":"Checkpoint "+i);gate.transform.SetParent(manager.transform);gate.transform.position=points[k];gate.transform.rotation=Quaternion.LookRotation(dir);
   for(int side=-1;side<=1;side+=2){var post=Primitive("Marker",gate.transform,new Vector3(side*9,2,0),new Vector3(.35f,4,.35f),i==0?Color.white:Color.cyan);}
  }
  var matBlack=Material(new Color(.035f,.04f,.05f));var matWhite=Material(Color.white);
  for(int x=0;x<2;x++)for(int z=0;z<12;z++){var t=Primitive("Finish checker",manager.transform,new Vector3(x*.8f, .05f,-38+z*1.5f),new Vector3(.8f,.05f,1.5f),Color.white);t.GetComponent<Renderer>().sharedMaterial=(x+z)%2==0?matBlack:matWhite;}
  var pad=new GameObject("TURBO - Cyan launch pad").transform;pad.SetParent(manager.transform);pad.position=new Vector3(42,.08f,-29);
  Primitive("Boost surface",pad,Vector3.zero,new Vector3(14,.08f,14),new Color(0,.55f,.7f));
  for(int i=0;i<4;i++){var arrow=Primitive("Boost stripe",pad,new Vector3(-4+i*2.5f,.07f,0),new Vector3(.6f,.08f,11),Color.cyan);arrow.transform.localRotation=Quaternion.Euler(0,25,0);}
  var text=new GameObject("TURBO sign");text.transform.SetParent(pad);text.transform.localPosition=new Vector3(0,3,-8);text.transform.rotation=Quaternion.Euler(0,180,0);var tm=text.AddComponent<TextMesh>();tm.text="TURBO >>>";tm.fontSize=60;tm.characterSize=.14f;tm.anchor=TextAnchor.MiddleCenter;tm.color=Color.cyan;
  var camera=new GameObject("Race Chase Camera").AddComponent<Camera>();camera.tag="MainCamera";camera.gameObject.AddComponent<AudioListener>();camera.nearClipPlane=.15f;camera.farClipPlane=1500;camera.transform.position=player.transform.position-player.transform.forward*8.5f+Vector3.up*4.5f;camera.transform.LookAt(player.transform.position+player.transform.forward*8);manager.chase=camera;
  RenderSettings.ambientLight=new Color(.65f,.68f,.75f);RenderSettings.ambientIntensity=1.3f;
  EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
  File.WriteAllText(Path.Combine(output,"RacePrototype-setup.txt"),"RacePrototype saved. Player, AI, 80 route points, 8 ordered gates, 3 laps, countdown, HUD, boost, recovery. Original 3P untouched.\n");
  Debug.Log("RACE PROTOTYPE: setup complete and saved");
 }
 static int materialCount; static Material Material(Color c){var m=new Material(Shader.Find("Standard"));m.color=c;if(c==Color.cyan){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",c*2);}AssetDatabase.CreateAsset(m,"Assets/RacePrototype/Materials/Track"+(materialCount++)+".mat");return m;}
 static GameObject Primitive(string name,Transform parent,Vector3 p,Vector3 scale,Color c){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=p;g.transform.localScale=scale;g.GetComponent<Collider>().enabled=false;g.GetComponent<Renderer>().sharedMaterial=Material(c);return g;}
 static void Configure(CarController car){var control=car.GetComponent<CarUserControl>();if(control)control.enabled=false;var ai=car.GetComponent<CarAIControl>();if(ai)ai.enabled=false;
  var so=new SerializedObject(car);so.FindProperty("m_Topspeed").floatValue=160;so.FindProperty("m_SpeedType").enumValueIndex=1;so.FindProperty("m_MaximumSteerAngle").floatValue=30;so.FindProperty("m_FullTorqueOverAllWheels").floatValue=3000;so.FindProperty("m_TractionControl").floatValue=.1f;so.FindProperty("m_SteerHelper").floatValue=.8f;so.FindProperty("m_CentreOfMassOffset").vector3Value=new Vector3(0,-.4f,0);so.ApplyModifiedPropertiesWithoutUndo();var rb=car.GetComponent<Rigidbody>();rb.interpolation=RigidbodyInterpolation.Interpolate;rb.collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;
 }
}

