using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

// Explicit, short Play test: never runs on import or scene opening.
[InitializeOnLoad] public static class RacePowerVerification {
 static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
 static string request=Path.GetFullPath(Path.Combine(Application.dataPath,"../Temp/RacePowerVerification.request")),output;
 static int stage=-1;static double due;static StringBuilder log;static RacePrototypeGame game;static RaceCandyItems items;
 static RacePowerVerification(){if(SessionState.GetBool("RacePowerVerification.active",false)){stage=0;output=SessionState.GetString("RacePowerVerification.output","");log=new StringBuilder();}EditorApplication.update+=Tick;}
 static object Get(object obj,string field){return obj.GetType().GetField(field,F).GetValue(obj);}
 static void Set(object obj,string field,object value){obj.GetType().GetField(field,F).SetValue(obj,value);}
 static void Call(object obj,string method){obj.GetType().GetMethod(method,F,null,Type.EmptyTypes,null).Invoke(obj,null);}
 static void Check(string name,bool ok){log.AppendLine((ok?"PASS ":"FAIL ")+name);if(!ok)throw new Exception(name);}
 static void Shot(string name){ScreenCapture.CaptureScreenshot(Path.Combine(output,name+".png"));}
 static void Tick(){try{
  if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;
  if(File.Exists(request)){output=File.ReadAllText(request).Trim();File.Delete(request);Directory.CreateDirectory(output);SessionState.SetString("RacePowerVerification.output",output);SessionState.SetBool("RacePowerVerification.active",true);SessionState.SetBool("RaceVerification.active",false);SessionState.SetBool("RaceMenuVerification.active",false);stage=0;log=new StringBuilder();EditorApplication.isPaused=false;if(!EditorApplication.isPlaying)EditorApplication.isPlaying=true;}
  if(stage<0||!EditorApplication.isPlaying)return;
  if(!game)game=UnityEngine.Object.FindObjectOfType<RacePrototypeGame>();if(!game||game.Racers==null||!game.Presentation)return;
  if(!items)items=game.GetComponent<RaceCandyItems>();if(!items||game.GetComponent<RacePowerHud>()==null)return;
  if(EditorApplication.timeSinceStartup<due)return;due=EditorApplication.timeSinceStartup+1.2;
  if(stage==0){Call(game,"OpenMenu");game.Presentation.SelectTrack(0);game.Presentation.StartSelection();Set(game,"countdown",-2f);Time.timeScale=0;Check("Four racers preserved",game.Racers.Length==4);stage++;return;}
  if(stage>=1&&stage<=8){int id=(stage+1)/2;
   if(stage%2==1){items.ResetItems();Set(items,"held",id);typeof(RaceCandyItems).GetMethod("Notify",F).Invoke(items,new object[]{"¡PODER CONSEGUIDO! "+items.HeldName+"\nPulsa E: "+RaceCandyItems.ItemHelp(id)});Check("Power "+id+" named and explained",items.HeldId==id&&items.HasAlert&&RaceCandyItems.ItemHelp(id).Length>15);Shot("poder-"+id);}
   else{Call(items,"UseItem");Check("Power "+id+" consumed by E action",items.HeldId==0);if(id==1)Check("Turbo active",(float)Get(game,"boostUntil")>Time.time);if(id==2)Check("Shield blocks one hit",items.ShieldActive&&items.Absorb()&&!items.Absorb());if(id==3)Check("Chocolate projectile created",items.ActiveProjectileCount>0);if(id==4)Check("Marshmallow used",items.Alert.Contains("MALVAVISCO"));}
   stage++;return;
  }
  if(stage==9){items.ResetItems();Call(items,"UseItem");Check("Empty E explains pickup",items.Alert.Contains("SIN PODER"));Shot("sin-poder");stage++;return;}
  if(stage==10){items.ResetItems();var traps=(List<Transform>)Get(items,"traps");traps[0].position=game.player.transform.position+Vector3.ProjectOnPlane(game.player.transform.forward,Vector3.up).normalized*18;Call(items,"UpdateWarning");Check("Trap warning before contact",items.Alert.Contains("TRAMPA CERCA"));Shot("aviso-trampa");stage++;return;}
  if(stage==11){items.ResetItems();var obstacles=(List<Transform>)Get(items,"moving");obstacles[0].position=game.player.transform.position+Vector3.ProjectOnPlane(game.player.transform.forward,Vector3.up).normalized*18;Call(items,"UpdateWarning");Check("Moving obstacle warning",items.Alert.Contains("MOVIL CERCA"));Shot("aviso-obstaculo");stage++;return;}
  if(stage==12){items.ResetItems();var obj=new GameObject("Verification incoming chocolate");Vector3 forward=Vector3.ProjectOnPlane(game.player.transform.forward,Vector3.up).normalized;obj.transform.position=game.player.transform.position-forward*20;var shot=obj.AddComponent<CandyProjectile>();shot.Initialize(items,game.rival,forward,48);((List<CandyProjectile>)Get(items,"projectiles")).Add(shot);Call(items,"UpdateWarning");Check("Incoming CPU projectile warning",items.Alert.Contains("CHOCOLATE EN CAMINO"));Shot("aviso-ataque");stage++;return;}
  if(stage==13){items.ResetItems();items.ApplySlow(game.player,2.5f);Check("Hit warning shows slowdown",items.Alert.Contains("TE HAN FRENADO"));Shot("aviso-impacto");stage++;return;}
  if(stage==14){Set(items,"messageUntil",Time.time-1);Set(items,"proximity","");Check("Brief alert expires",!items.HasAlert);Check("Boxes display question marks",UnityEngine.Object.FindObjectsOfType<TextMesh>().Length>=36);items.ResetItems();Call(game,"OpenMenu");File.WriteAllText(Path.Combine(output,"power-checks.txt"),log.ToString());stage=15;return;}
  SessionState.SetBool("RacePowerVerification.active",false);EditorApplication.isPlaying=false;stage=-1;
 }catch(Exception e){if(output!=null){Directory.CreateDirectory(output);File.WriteAllText(Path.Combine(output,"power-errors.txt"),log+"\n"+e);}Time.timeScale=1;SessionState.SetBool("RacePowerVerification.active",false);stage=-1;}}
}
