using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Text;
// Explicit opt-in Play verification; no scene authoring runs on import.
[InitializeOnLoad] public static class RaceVerification {
 static readonly BindingFlags flags=BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public;
 static readonly string root=Path.GetFullPath(Path.Combine(Application.dataPath,".."));
 static readonly string request=Path.Combine(root,"Temp/RaceVerification.request");
 static bool contactCaptured;static double next,finishAt;static int initialized=-1,shotStage;static string errors="";
 static RaceVerification(){EditorApplication.update+=Tick;Application.logMessageReceived+=Log;}
 static void Log(string message,string trace,LogType type){if(SessionState.GetBool("RaceVerification.active",false)&&(type==LogType.Error||type==LogType.Exception||type==LogType.Assert))errors+=message+"\n"+trace+"\n";}
 static object Get(object obj,string name){return obj.GetType().GetField(name,flags).GetValue(obj);}
 static void Set(object obj,string name,object value){obj.GetType().GetField(name,flags).SetValue(obj,value);}
 static object Call(object obj,string name){return obj.GetType().GetMethod(name,flags,null,Type.EmptyTypes,null).Invoke(obj,null);}
 static string Output{get{return SessionState.GetString("RaceVerification.output",Path.Combine(root,"TestResults"));}}
 static void Tick(){
  try{
   if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;
   if(File.Exists(request)){
    string[] command;try{string input=File.ReadAllText(request).Trim();if(input.Length==0)return;command=input.Split('|');}catch(IOException){return;}File.Delete(request);
    if(command[0]=="stop"){SessionState.SetBool("RaceVerification.active",false);EditorApplication.isPlaying=false;return;}
    if(command[0]=="start"||command[0]=="preview"){SessionState.SetBool("RaceVerification.preview",command[0]=="preview");
     SessionState.SetBool("RaceVerification.active",true);SessionState.SetInt("RaceVerification.track",command.Length>1?int.Parse(command[1]):0);
     if(command.Length>2)SessionState.SetString("RaceVerification.output",command[2]);
     initialized=-1;shotStage=0;finishAt=0;errors="";EditorApplication.isPaused=false;if(!EditorApplication.isPlaying)EditorApplication.isPlaying=true;
    }
   }
   if(!SessionState.GetBool("RaceVerification.active",false)||!EditorApplication.isPlaying)return;
   var game=UnityEngine.Object.FindObjectOfType<RacePrototypeGame>();if(!game||game.Racers==null||!game.Presentation||!game.GetComponent<RaceCandyItems>())return;
   Directory.CreateDirectory(Output);int track=SessionState.GetInt("RaceVerification.track",0);
   if(initialized!=track){game.Presentation.OpenMenu();if(SessionState.GetBool("RaceVerification.preview",false)){typeof(RacePresentation).GetProperty("SelectedVehicle").SetValue(game.Presentation,1,null);typeof(RacePresentation).GetProperty("SelectedCharacter").SetValue(game.Presentation,1,null);}typeof(RacePresentation).GetProperty("SelectedTrack").SetValue(game.Presentation,track,null);game.Presentation.StartSelection();Set(game,"demo",true);initialized=track;contactCaptured=false;next=0;shotStage=0;finishAt=0;errors="";}
   float elapsed=(float)Get(game,"elapsed");bool finished=(bool)Get(game,"finished");
   if(EditorApplication.timeSinceStartup>=next){next=EditorApplication.timeSinceStartup+5;File.AppendAllText(Path.Combine(Output,"runtime-track-"+track+".txt"),DateTime.UtcNow.ToString("o")+" elapsed="+elapsed+" completed="+string.Join(",",(int[])Get(game,"completed"))+" next="+string.Join(",",(int[])Get(game,"next"))+" finished="+finished+" pos="+game.player.transform.position+Environment.NewLine);}
   if(shotStage<3&&!finished){if(elapsed>8+shotStage*10)Set(game,"cameraMode",shotStage);if(elapsed>10+shotStage*10){ScreenCapture.CaptureScreenshot(Path.Combine(Output,"track-"+track+"-camera-"+shotStage+".png"));shotStage++;}}
   if(SessionState.GetBool("RaceVerification.preview",false)&&elapsed>30){Set(game,"cameraMode",0);if(elapsed>32&&!contactCaptured){contactCaptured=true;ScreenCapture.CaptureScreenshot(Path.Combine(Output,"contacto-ruedas.png"));}}
   if(SessionState.GetBool("RaceVerification.preview",false)&&elapsed>33){Set(game,"cameraMode",0);float gap=100;foreach(var renderer in game.player.GetComponentsInChildren<Renderer>())if(renderer.enabled&&renderer.name=="Chocolate cookie tire")gap=Mathf.Min(gap,renderer.bounds.min.y-game.player.transform.position.y+((float[])Get(game,"rideOffsets"))[0]);File.WriteAllText(Path.Combine(Output,"wheel-contact.txt"),"Wheel visual clearance="+gap+"\nmaxGroundError="+Get(game,"maxGroundError")+"\nminimumUpright="+Get(game,"minimumUpright"));SessionState.SetBool("RaceVerification.active",false);Call(game,"OpenMenu");ScreenCapture.CaptureScreenshot(Path.Combine(Output,"menu-final.png"));return;}
   if(finished&&finishAt==0){finishAt=EditorApplication.timeSinceStartup;ScreenCapture.CaptureScreenshot(Path.Combine(Output,"track-"+track+"-finish.png"));
    string result="track="+track+"\nfinalTime="+Get(game,"finalTime")+"\nposition="+Get(game,"finalPosition")+"\nlaps="+string.Join(",",(int[])Get(game,"completed"))+"\nframes="+Get(game,"observedFrames")+"\nframeSeconds="+Get(game,"observedFrameTime")+"\nworstFrame="+Get(game,"worstFrame")+"\nmaxGroundError="+Get(game,"maxGroundError")+"\nminimumUpright="+Get(game,"minimumUpright")+"\nminimumCameraClearance="+Get(game,"minimumCameraClearance")+"\nerrors="+errors;
    File.WriteAllText(Path.Combine(Output,"test-track-"+track+".txt"),result);
   }
   if(finishAt>0&&EditorApplication.timeSinceStartup-finishAt>3){RunSmoke(game,track);if(track==0){SessionState.SetInt("RaceVerification.track",1);initialized=-1;}else{SessionState.SetBool("RaceVerification.active",false);Call(game,"OpenMenu");ScreenCapture.CaptureScreenshot(Path.Combine(Output,"menu-final.png"));}}
   if(elapsed>520&&!finished){File.WriteAllText(Path.Combine(Output,"timeout-track-"+track+".txt"),"Race did not finish. "+errors);SessionState.SetBool("RaceVerification.active",false);}
  }catch(Exception ex){Directory.CreateDirectory(Output);File.AppendAllText(Path.Combine(Output,"verification-errors.txt"),ex+Environment.NewLine);SessionState.SetBool("RaceVerification.active",false);}
 }
 static void RunSmoke(RacePrototypeGame game,int track){
  var log=new StringBuilder();Action<string,bool> check=(name,ok)=>{log.AppendLine((ok?"PASS ":"FAIL ")+name);if(!ok)throw new Exception(name);};
  try{
   check("Karts remain grounded",(float)Get(game,"maxGroundError")<.03f&&(float)Get(game,"minimumUpright")>.85f);
   check("Camera stays above physical asphalt",(float)Get(game,"minimumCameraClearance")>2.15f);
   check("Three completed laps",((int[])Get(game,"completed"))[0]==3);
   check("Forward gate",RacePrototypeGame.CrossedGate(Vector3.back,Vector3.forward,Vector3.zero,Vector3.forward));
   check("Reverse rejected",!RacePrototypeGame.CrossedGate(Vector3.forward,Vector3.back,Vector3.zero,Vector3.forward));
   check("Far gate rejected",!RacePrototypeGame.CrossedGate(new Vector3(20,0,-1),new Vector3(20,0,1),Vector3.zero,Vector3.forward));
   Call(game,"Restart");check("Restart cleared laps",Array.TrueForAll((int[])Get(game,"completed"),x=>x==0));check("Countdown restored",(float)Get(game,"countdown")>0&&!(bool)Get(game,"finished"));
   Set(game,"countdown",-2f);var items=game.GetComponent<RaceCandyItems>();
   Set(items,"held",1);Call(items,"UseItem");check("Box boost",(float)Get(game,"boostUntil")>Time.time);
   Set(items,"held",2);Call(items,"UseItem");check("Shield single hit",items.Absorb()&&!items.Absorb());
   Set(items,"held",3);Call(items,"UseItem");check("Chocolate projectile",items.ActiveProjectileCount>0);
   items.ApplySlow(game.rival,2);items.ApplySlow(game.rival,2);check("Slow does not stack",Array.FindAll(game.rival.GetComponents<CandySlow>(),x=>x.enabled).Length==1);
   Call(game,"TogglePause");check("Pause freezes time",Time.timeScale==0);Call(game,"TogglePause");check("Resume restores time",Time.timeScale==1);
   for(int c=0;c<3;c++){Set(game,"cameraMode",c);Call(game,"LateUpdate");check("Camera "+c+" finite",!float.IsNaN(game.chase.transform.position.x)&&!float.IsNaN(game.chase.transform.rotation.x));}
   items.ResetItems();check("Projectiles cleared",items.ActiveProjectileCount==0);
   ScreenCapture.CaptureScreenshot(Path.Combine(Output,"track-"+track+"-restart.png"));
  }catch(Exception e){log.AppendLine(e.ToString());}File.WriteAllText(Path.Combine(Output,"smoke-track-"+track+".txt"),log.ToString());
 }
}
