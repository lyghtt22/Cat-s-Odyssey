using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
[InitializeOnLoad] public static class PinkRushVerification {
 static string root=@"C:\Users\lyght\Documents\Codex\2026-10-18\referenced-chatgpt-conversation-this-is-an-3\outputs";
 static bool preview; static double ended;static float maxSpeed;static bool boost;static int maxCheckpoint;
 static BindingFlags flags=BindingFlags.Instance|BindingFlags.NonPublic;
 static PinkRushVerification(){EditorApplication.update+=Tick;}
 static void Tick(){
  if(!File.Exists(Path.Combine(root,"verify-request.txt")))return;
  if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;
  if(!EditorApplication.isPlaying){EditorApplication.isPlaying=true;return;}
  var g=Object.FindObjectOfType<RacePrototypeGame>();if(!g||!g.player)return;
  if(!SessionState.GetBool("SugarBloomVerificationV2",false)){
   if(g.player.GetComponent<Rigidbody>()==null)return;
   try{typeof(RacePrototypeGame).GetMethod("Restart",flags).Invoke(g,null);}catch{return;}
   typeof(RacePrototypeGame).GetField("demo",flags).SetValue(g,true);SessionState.SetBool("SugarBloomVerificationV2",true);Debug.Log("PINK RUSH verification started");
  }
  if(!preview && (float)typeof(RacePrototypeGame).GetField("countdown",flags).GetValue(g)<2){ScreenCapture.CaptureScreenshot(Path.Combine(root,"SugarBloom-preview.png"));preview=true;} if(ended>0){if(EditorApplication.timeSinceStartup-ended>3){typeof(RacePrototypeGame).GetMethod("Restart",flags).Invoke(g,null);File.WriteAllText(Path.Combine(root,"SugarBloom-restart.txt"),"Restart checked: countdown="+typeof(RacePrototypeGame).GetField("countdown",flags).GetValue(g)+", laps="+((int[])typeof(RacePrototypeGame).GetField("completed",flags).GetValue(g))[0]);File.Delete(Path.Combine(root,"verify-request.txt"));SessionState.SetBool("SugarBloomVerificationV2",false);EditorApplication.isPlaying=false;}return;}
  maxSpeed=Mathf.Max(maxSpeed,g.player.GetComponent<Rigidbody>().velocity.magnitude*3.6f);
  float until=(float)typeof(RacePrototypeGame).GetField("boostUntil",flags).GetValue(g);if(until>Time.time)boost=true;
  int[] cp=(int[])typeof(RacePrototypeGame).GetField("next",flags).GetValue(g);maxCheckpoint=Mathf.Max(maxCheckpoint,cp[0]);
  int[] laps=(int[])typeof(RacePrototypeGame).GetField("completed",flags).GetValue(g);
  bool finish=(bool)typeof(RacePrototypeGame).GetField("finished",flags).GetValue(g);
  float elapsed=(float)typeof(RacePrototypeGame).GetField("elapsed",flags).GetValue(g);
  if(finish||elapsed>600){
   var report=new Report{finished=finish,playerLaps=laps[0],rivalLaps=laps[1],maxKmh=maxSpeed,boostActivated=boost,maxCheckpoint=maxCheckpoint,timeSeconds=elapsed};
   File.WriteAllText(Path.Combine(root,"SugarBloom-verification.json"),JsonUtility.ToJson(report,true));
   ScreenCapture.CaptureScreenshot(Path.Combine(root,"SugarBloom-finish.png"));ended=EditorApplication.timeSinceStartup;
   Debug.Log("PINK RUSH verification result: "+JsonUtility.ToJson(report));
  }
 }
 [System.Serializable] class Report{public bool finished;public int playerLaps,rivalLaps;public float maxKmh,timeSeconds;public bool boostActivated;public int maxCheckpoint;}
}


