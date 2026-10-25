using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Reflection;
[InitializeOnLoad] public static class RaceMenuVerification {
 static string request=Path.GetFullPath(Path.Combine(Application.dataPath,"../Temp/RaceMenuVerification.request"));
 static BindingFlags flags=BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public;
 static double due;static StringBuilder log;
 static RaceMenuVerification(){EditorApplication.update+=Tick;}
 static void Call(RacePrototypeGame game,string name){typeof(RacePrototypeGame).GetMethod(name,flags,null,Type.EmptyTypes,null).Invoke(game,null);}
 static void Tick(){try{
  if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;
  if(File.Exists(request)){SessionState.SetString("RaceMenuVerification.output",File.ReadAllText(request).Trim());File.Delete(request);SessionState.SetInt("RaceMenuVerification.stage",0);SessionState.SetBool("RaceMenuVerification.active",true);EditorApplication.isPaused=false;if(!EditorApplication.isPlaying)EditorApplication.isPlaying=true;}
  if(!SessionState.GetBool("RaceMenuVerification.active",false)||!EditorApplication.isPlaying)return;
  var game=UnityEngine.Object.FindObjectOfType<RacePrototypeGame>();if(!game||!game.Presentation||!game.GetComponent<RaceMenuView>())return;
  var view=game.GetComponent<RaceMenuView>();var preview=(RenderTexture)typeof(RaceMenuView).GetField("preview",flags).GetValue(view);if(!preview||!preview.IsCreated())return;
  string output=SessionState.GetString("RaceMenuVerification.output","");Directory.CreateDirectory(output);int stage=SessionState.GetInt("RaceMenuVerification.stage",0);
  if(stage==0){log=new StringBuilder();Call(game,"OpenMenu");game.Presentation.SelectVehicle(0);game.Presentation.SelectCharacter(0);game.Presentation.SelectTrack(0);due=EditorApplication.timeSinceStartup+2;SessionState.SetInt("RaceMenuVerification.stage",1);return;}
  if(EditorApplication.timeSinceStartup<due)return;
  Action<string,bool> check=(name,ok)=>{log.AppendLine((ok?"PASS ":"FAIL ")+name);if(!ok)throw new Exception(name);};
  if(stage<=3){check("Menu and preview character "+(stage-1),game.Presentation.MenuOpen&&preview.IsCreated()&&typeof(RaceMenuView).GetField("previewKart",flags).GetValue(view)!=null);ScreenCapture.CaptureScreenshot(Path.Combine(output,"menu-piloto-"+(stage-1)+".png"));if(stage<3){game.Presentation.SelectCharacter(stage);game.Presentation.SelectVehicle(stage==2&&!game.Presentation.CocoaUnlocked?0:stage);game.Presentation.SelectTrack(stage%2);}else{check("Background artwork loaded",typeof(RaceMenuView).GetField("backdrop",flags).GetValue(view)!=null);game.Presentation.StartSelection();}SessionState.SetInt("RaceMenuVerification.stage",stage+1);due=EditorApplication.timeSinceStartup+2;}
  else if(stage==4){check("Start selected race closes menu",!game.Presentation.MenuOpen&&Time.timeScale==1);Call(game,"TogglePause");check("Pause still works",Time.timeScale==0);Call(game,"OpenMenu");game.Presentation.SelectVehicle(0);game.Presentation.SelectCharacter(0);game.Presentation.SelectTrack(0);SessionState.SetInt("RaceMenuVerification.stage",5);due=EditorApplication.timeSinceStartup+2;}
  else{check("Return to menu",game.Presentation.MenuOpen&&Time.timeScale==0);ScreenCapture.CaptureScreenshot(Path.Combine(output,"menu-final.png"));File.WriteAllText(Path.Combine(output,"menu-checks.txt"),log.ToString());SessionState.SetBool("RaceMenuVerification.active",false);}
 }catch(Exception e){string output=SessionState.GetString("RaceMenuVerification.output","");if(output!=""){Directory.CreateDirectory(output);File.WriteAllText(Path.Combine(output,"menu-errors.txt"),e.ToString());}SessionState.SetBool("RaceMenuVerification.active",false);}}
}
