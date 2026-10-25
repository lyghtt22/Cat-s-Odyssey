using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityStandardAssets.Vehicles.Car;
 public static class SugarBloomFinalize {

 static void Run(){
  if(GameObject.Find("Sugar Bloom - Persistent Setup"))return;
  if(EditorApplication.isPlayingOrWillChangePlaymode){if(EditorApplication.isPlaying)EditorApplication.isPlaying=false;EditorApplication.delayCall+=Run;return;}
  var scene=EditorSceneManager.GetActiveScene();if(scene.path!="Assets/Scenes/RacePrototype.unity"||!GameObject.Find("Sugar Bloom - Ready"))return;
  var game=Object.FindObjectOfType<RacePrototypeGame>();
  var skid=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/RacePrototype/CandyArt/CandySkid.prefab");
  var cocoa=AssetDatabase.LoadAssetAtPath<Material>("Assets/RacePrototype/CandyArt/Dark chocolate.mat");var mint=AssetDatabase.LoadAssetAtPath<Material>("Assets/RacePrototype/CandyArt/Mint icing.mat");var wafer=AssetDatabase.LoadAssetAtPath<Material>("Assets/RacePrototype/CandyArt/Biscuit.mat");var pink=AssetDatabase.LoadAssetAtPath<Material>("Assets/RacePrototype/CandyArt/Strawberry frosting.mat");
  foreach(var car in new[]{game.player,game.rival}){
   foreach(var effect in car.GetComponentsInChildren<WheelEffects>()){effect.SkidTrailPrefab=skid.transform;Record(effect);}
   var candy=car.transform.Find("Candy kart - handmade");foreach(var r in car.GetComponentsInChildren<Renderer>()){r.enabled=candy&&r.transform.IsChildOf(candy);Record(r);}
   var c=car.GetComponent<CarUserControl>();if(c){c.enabled=false;Record(c);}
  }
  game.player.transform.SetPositionAndRotation(new Vector3(-15,.65f,-29),Quaternion.Euler(0,90,0));game.rival.transform.SetPositionAndRotation(new Vector3(-9,.65f,-34),Quaternion.Euler(0,90,0));
  foreach(var root in scene.GetRootGameObjects()){
   if(root.name.StartsWith("oval"))foreach(var r in root.GetComponentsInChildren<Renderer>(true)){
    string n=r.name.ToLower();
    if(n.Contains("fence")||n.Contains("wall")||n.Contains("tree")||n.Contains("garage")||n.Contains("lamp")||n=="plane001"||n=="0grass"||n=="cylinder001")r.enabled=false;
    Material m=null;if(n.Contains("tarmac")||n.Contains("pitlane"))m=cocoa;else if(n.Contains("grass"))m=mint;else if(n.Contains("gravel")||n.Contains("sand")||n.Contains("kerb"))m=wafer;else if(n.Contains("concrete"))m=pink;
    if(m){var arr=r.sharedMaterials;for(int i=0;i<arr.Length;i++)arr[i]=m;r.sharedMaterials=arr;}Record(r);
   }
   Record(root);foreach(var c in root.GetComponentsInChildren<Component>(true))if(c)Record(c);
  }
  new GameObject("Sugar Bloom - Persistent Setup");EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);EditorSceneManager.OpenScene(scene.path);Debug.Log("SUGAR BLOOM: prefab changes recorded, scene saved and reopened");
 }
 static void Record(Object o){if(PrefabUtility.IsPartOfPrefabInstance(o))PrefabUtility.RecordPrefabInstancePropertyModifications(o);}
}
