using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Text;
[InitializeOnLoad] public static class RaceEdgeVerification {
 static readonly BindingFlags flags=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance;
 static string request=Path.GetFullPath(Path.Combine(Application.dataPath,"../Temp/RaceEdgeVerification.request"));
 static string output;static RacePrototypeGame game;static int phase=-1,track;static float started;static StringBuilder log;static bool bad;
 static RaceEdgeVerification(){EditorApplication.update+=Tick;}
 static object Get(string name){return typeof(RacePrototypeGame).GetField(name,flags).GetValue(game);}
 static void Set(string name,object value){typeof(RacePrototypeGame).GetField(name,flags).SetValue(game,value);}
 static void Call(string name){typeof(RacePrototypeGame).GetMethod(name,flags,null,Type.EmptyTypes,null).Invoke(game,null);}
 static void Select(int t){track=t;game.Presentation.OpenMenu();typeof(RacePresentation).GetProperty("SelectedTrack").SetValue(game.Presentation,t,null);game.Presentation.StartSelection();Set("demo",false);Set("countdown",-2f);}
 static void Pose(Vector3 p,Quaternion q){var args=new object[]{0,p,q,Vector3.zero,Quaternion.identity};typeof(RacePrototypeGame).GetMethod("SupportRoad",flags).Invoke(game,args);var rb=game.player.GetComponent<Rigidbody>();rb.position=(Vector3)args[3];rb.rotation=(Quaternion)args[4];((float[])Get("speeds"))[0]=0;Physics.SyncTransforms();Set("previous",CopyPrevious(rb.position));}
 static Vector3[] CopyPrevious(Vector3 player){var values=(Vector3[])Get("previous");values[0]=player;return values;}
 static void Survey(){int samples=0;float minimum=100;for(int k=0;k<game.route.Length;k+=2){Vector3 forward=game.route[(k+1)%game.route.Length]-game.route[k];forward.y=0;Vector3 side=Vector3.Cross(Vector3.up,forward.normalized);foreach(float lane in new[]{-11f,-6f,0f,6f,11f})foreach(float yaw in new[]{-60f,0f,60f}){Pose(game.route[k]+side*lane,Quaternion.LookRotation(forward)*Quaternion.Euler(0,yaw,0));float clearance;if(!game.RoadContactValid(0,out clearance)){bad=true;log.AppendLine("FAIL surface track="+track+" point="+k+" lane="+lane+" yaw="+yaw+" clearance="+clearance);}minimum=Mathf.Min(minimum,clearance);samples++;}}log.AppendLine("Survey track="+track+" poses="+samples+" minimum tire clearance="+minimum);}
 static void Tick(){try{
  if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;
  if(File.Exists(request)){output=File.ReadAllText(request).Trim();File.Delete(request);Directory.CreateDirectory(output);phase=0;log=new StringBuilder();bad=false;EditorApplication.isPaused=false;if(!EditorApplication.isPlaying)EditorApplication.isPlaying=true;}
  if(phase<0||!EditorApplication.isPlaying)return;
  if(!game)game=UnityEngine.Object.FindObjectOfType<RacePrototypeGame>();if(!game||game.Racers==null||!game.Presentation)return;
  if(phase==0){Select(0);Survey();Pose(game.route[18],Quaternion.LookRotation(game.route[19]-game.route[18]));Set("verificationInput",true);Set("verificationAxes",new Vector2(1,1));((float[])Get("speeds"))[0]=20;started=Time.time;phase=1;}
  float gap;if(!game.RoadContactValid(0,out gap)){bad=true;log.AppendLine("FAIL driving track="+track+" phase="+phase+" gap="+gap);}
  float since=Time.time-started;
  if(phase==1&&since>5){Set("verificationAxes",new Vector2(-1,1));Set("cameraMode",1);started=Time.time;phase=2;ScreenCapture.CaptureScreenshot(Path.Combine(output,"borde-derecho-"+track+".png"));}
  else if(phase==2&&since>5){Set("verificationAxes",new Vector2(1,-1));Set("cameraMode",2);started=Time.time;phase=3;ScreenCapture.CaptureScreenshot(Path.Combine(output,"borde-izquierdo-"+track+".png"));}
  else if(phase==3&&since>5){Set("verificationInput",false);Set("verificationAxes",Vector2.zero);if(track==0){Select(1);Survey();Pose(game.route[18],Quaternion.LookRotation(game.route[19]-game.route[18]));Set("verificationInput",true);Set("verificationAxes",new Vector2(1,1));((float[])Get("speeds"))[0]=20;started=Time.time;phase=1;}else{log.AppendLine(bad?"FAIL edge checks":"PASS physical road contact across both tracks and manual steering/reverse");File.WriteAllText(Path.Combine(output,"edge-checks.txt"),log.ToString());Call("OpenMenu");phase=-1;}}
 }catch(Exception e){if(output!=null)File.WriteAllText(Path.Combine(output,"edge-errors.txt"),e.ToString());phase=-1;}}
}
