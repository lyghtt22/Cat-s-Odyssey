using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Vehicles.Car;
using System;

public class RacePrototypeGame : MonoBehaviour {
 public CarController player, rival;
 public Camera chase;
 public Vector3[] route;
 public Vector3[] gates;
 public int laps=3;
 float countdown=3f, elapsed, boostUntil, boostReady, noticeUntil;
 string notice="";
 int[] next={1,1}, completed={0,0};
 bool finished, demo;
 Vector3[] previous=new Vector3[2];
 float[] stuck=new float[2];
 Rigidbody[] bodies;
 CarController[] cars;
 TrailRenderer[] trails;
 GUIStyle label, big, small;
 void Start(){
  cars=new[]{player,rival}; bodies=new[]{player.GetComponent<Rigidbody>(),rival.GetComponent<Rigidbody>()};
  trails=new TrailRenderer[2];
  for(int i=0;i<2;i++){
   previous[i]=cars[i].transform.position;
   var go=new GameObject("Turbo trail"); go.transform.SetParent(cars[i].transform,false); go.transform.localPosition=new Vector3(0,.55f,-2);
   var t=go.AddComponent<TrailRenderer>(); t.time=.45f;t.startWidth=.8f;t.endWidth=0;t.material=new Material(Shader.Find("Sprites/Default"));t.startColor=new Color(1f,.22f,.6f);t.endColor=new Color(1,.4f,.8f,0);t.emitting=false;trails[i]=t;
  }
 }
 void Update(){
  if(Input.GetKeyDown(KeyCode.F2)){demo=!demo;Message(demo?"DEMOSTRACION AUTOMATICA":"CONTROL MANUAL");}
  if(Input.GetKeyDown(KeyCode.R)) Recover(0);
  if(Input.GetKeyDown(KeyCode.Return)&&finished)Restart();
  if(countdown>0){countdown-=Time.deltaTime;return;}
  if(!finished)elapsed+=Time.deltaTime;
  if(countdown>-1)countdown-=Time.deltaTime;
 }
 void FixedUpdate(){
  if(cars==null)return;
  for(int i=0;i<2;i++){
   var car=cars[i];var rb=bodies[i];
   if(countdown>0||completed[i]>=laps){if(i==1&&countdown<=0&&car.transform.position.x<30){foreach(var w in car.GetComponentsInChildren<WheelCollider>())w.brakeTorque=0;car.Move(0,.35f,0,0);continue;}car.Move(0,0,0,1);rb.velocity=Vector3.zero;continue;}
   foreach(var wheel in car.GetComponentsInChildren<WheelCollider>())wheel.brakeTorque=0; float steer, throttle;
   if(i==1||demo){
    int nearest=Nearest(car.transform.position);float distance=0;int target=nearest;
    while(distance<Mathf.Clamp(rb.velocity.magnitude*1.1f,14,35)){int n=(target+1)%route.Length;distance+=Vector3.Distance(route[target],route[n]);target=n;}
    Vector3 local=car.transform.InverseTransformPoint(route[target]);float angle=Mathf.Atan2(local.x,local.z)*Mathf.Rad2Deg;
    steer=Mathf.Clamp(angle/27f,-1,1);
    if(i==0)steer=Mathf.Clamp(Mathf.Atan2(5f*local.x,local.x*local.x+local.z*local.z)*Mathf.Rad2Deg/30f,-1,1); float desired=23;desired-=Mathf.Clamp(Mathf.Abs(angle)*.18f,0,10);
    throttle=rb.velocity.magnitude<desired?1f:-.25f;
   }else{steer=Input.GetAxis("Horizontal");throttle=Input.GetAxis("Vertical");}
   car.Move(steer,Mathf.Max(0,throttle),Mathf.Min(0,throttle),i==0&&!demo&&Input.GetKey(KeyCode.Space)?1:0);
   if(i==0){
    Vector3 p=car.transform.position;
    bool pad=p.x>35&&p.x<49&&Mathf.Abs(p.z+29)<8;
    if((pad||(!demo&&Input.GetKey(KeyCode.LeftShift)))&&Time.time>boostReady){boostUntil=Time.time+2.5f;boostReady=Time.time+7;Message("TURBO +++");Debug.Log("RACE BOOST speed="+rb.velocity.magnitude*3.6f);}
    bool boost=Time.time<boostUntil;trails[0].emitting=boost;
    if(boost&&rb.velocity.magnitude<(demo?28:39))rb.AddForce(car.transform.forward*(demo?4:12),ForceMode.Acceleration);
   }
   Vector3 now=car.transform.position; Vector3 g=gates[next[i]]; Vector3 dir=(route[(GateRouteIndex(next[i])+1)%route.Length]-g).normalized;
   float a=Vector3.Dot(previous[i]-g,dir),b=Vector3.Dot(now-g,dir);
   if(a<=0&&b>0){Vector3 crossing=Vector3.Lerp(previous[i],now,Mathf.Clamp01(-a/(b-a)));Vector3 off=crossing-g;off.y=0;off-=dir*Vector3.Dot(off,dir);
    if(off.magnitude<13&&Mathf.Abs(crossing.y-g.y)<8){
     if(next[i]==0){completed[i]++;if(i==0){if(completed[i]>=laps){finished=true;Message("META - CARRERA COMPLETADA");}else Message("VUELTA "+(completed[i]+1)+" / "+laps);}}
     Debug.Log("RACE CHECKPOINT car="+i+" gate="+next[i]+" laps="+completed[i]+" time="+elapsed); next[i]=(next[i]+1)%gates.Length;
    }
   }
   previous[i]=now;
   if(rb.velocity.magnitude<1)stuck[i]+=Time.fixedDeltaTime;else stuck[i]=0;
   if(now.y<-12||((i==1||demo)&&stuck[i]>5)||Vector3.Dot(car.transform.up,Vector3.up)<.15f)Recover(i);
  }
 }
 int GateRouteIndex(int gate){return Mathf.RoundToInt((float)gate*route.Length/gates.Length)%route.Length;}
 int Nearest(Vector3 p){int best=0;float d=float.MaxValue;for(int j=0;j<route.Length;j++){float n=(p-route[j]).sqrMagnitude;if(n<d){d=n;best=j;}}return best;}
 void Restart(){countdown=3f;elapsed=0;finished=false;boostUntil=0;boostReady=0;for(int i=0;i<2;i++){completed[i]=0;next[i]=1;bodies[i].position=new Vector3(i==0?-15:-9,.65f,i==0?-29:-34);bodies[i].rotation=Quaternion.Euler(0,90,0);bodies[i].velocity=Vector3.zero;bodies[i].angularVelocity=Vector3.zero;previous[i]=bodies[i].position;}}
 void Recover(int i){int prev=(next[i]+gates.Length-1)%gates.Length;int k=GateRouteIndex(prev);var dir=(route[(k+1)%route.Length]-route[k]).normalized;bodies[i].position=route[k]+Vector3.up*.8f; bodies[i].rotation=Quaternion.LookRotation(dir);bodies[i].velocity=Vector3.zero;bodies[i].angularVelocity=Vector3.zero;previous[i]=bodies[i].position;stuck[i]=0;if(i==0)Message("AUTO RECUPERADO");}
 void Message(string s){notice=s;noticeUntil=Time.time+2.5f;}
 void LateUpdate(){if(!player||!chase)return;var t=player.transform;var desired=t.position-t.forward*6.8f+Vector3.up*3.6f;chase.transform.position=Vector3.Lerp(chase.transform.position,desired,1-Mathf.Exp(-7*Time.deltaTime));chase.transform.LookAt(t.position+t.forward*6+Vector3.up);chase.fieldOfView=Mathf.Lerp(chase.fieldOfView,Time.time<boostUntil?75:62,Time.deltaTime*3);}
 void OnGUI(){
  if(bodies==null)return;
  GUI.matrix=Matrix4x4.TRS(Vector3.zero,Quaternion.identity,new Vector3(Screen.width/1280f,Screen.height/720f,1));
  if(label==null||round==null){
   label=new GUIStyle(GUI.skin.label){fontSize=23,fontStyle=FontStyle.Bold};label.normal.textColor=new Color(.32f,.13f,.22f);
   big=new GUIStyle(label){fontSize=84,alignment=TextAnchor.MiddleCenter};big.normal.textColor=new Color(1,.96f,.88f);
   small=new GUIStyle(label){fontSize=15,fontStyle=FontStyle.Normal};small.normal.textColor=new Color(.42f,.27f,.33f);
   round=new Texture2D(64,64,TextureFormat.RGBA32,false);for(int x=0;x<64;x++)for(int y=0;y<64;y++){float dx=Mathf.Max(12-x,x-51),dy=Mathf.Max(12-y,y-51);round.SetPixel(x,y,new Color(1,1,1,Mathf.Sqrt(Mathf.Max(0,dx)*Mathf.Max(0,dx)+Mathf.Max(0,dy)*Mathf.Max(0,dy))<12?1:0));}round.Apply();
  }
  Panel(new Rect(26,26,294,124),new Color(1,.95f,.88f,.96f));
  GUI.Label(new Rect(46,37,260,35),"SUGAR BLOOM",label);
  GUI.Label(new Rect(47,69,260,24),"C A N D Y   G R A N D   P R I X",small);
  GUI.Label(new Rect(47,101,260,36),"VUELTA  "+Mathf.Min(completed[0]+1,laps)+" / "+laps,label);
  Panel(new Rect(1010,26,244,124),new Color(1,.95f,.88f,.96f));
  GUI.Label(new Rect(1030,36,205,36),Mathf.RoundToInt(bodies[0].velocity.magnitude*3.6f)+" km/h",label);
  GUI.Label(new Rect(1030,75,205,30),TimeSpan.FromSeconds(elapsed).ToString(@"mm\:ss\.ff"),label);
  for(int i=0;i<7;i++)Panel(new Rect(1032+i*27,119,18,7),i<((next[0]+7)%8)?new Color(1,.4f,.65f):new Color(.84f,.76f,.74f));
  if(demo){Panel(new Rect(454,28,372,36),new Color(1,.95f,.88f,.9f));GUI.Label(new Rect(473,34,350,26),"DEMO  /  piloto automatico",small);}
  string center=countdown>0?Mathf.CeilToInt(countdown).ToString():countdown>-1?"¡YA!":finished?"¡META!":"";
  if(center!=""){Panel(new Rect(420,236,440,150),new Color(.75f,.2f,.42f,.8f));GUI.Label(new Rect(420,236,440,150),center,big);}
  if(Time.time<noticeUntil&&!finished){Panel(new Rect(435,161,410,42),new Color(1,.95f,.88f,.95f));GUI.Label(new Rect(452,166,400,35),notice,label);}
  Panel(new Rect(27,656,827,39),new Color(1,.95f,.88f,.93f));GUI.Label(new Rect(42,664,810,27),"WASD conducir   ·   ESPACIO frenar   ·   SHIFT turbo   ·   R recuperar   ·   F2 demo",small);
  Panel(new Rect(972,607,282,89),new Color(1,.95f,.88f,.96f));
  GUI.Label(new Rect(992,618,250,30),Time.time<boostUntil?"¡SUGAR BOOST!":Time.time<boostReady?"RECARGANDO AZUCAR":"SUGAR BOOST  /  SHIFT",small);
  float fill=Time.time<boostUntil?(boostUntil-Time.time)/2.5f:Mathf.Clamp01(1-(boostReady-Time.time)/7);
  Panel(new Rect(992,660,242,12),new Color(.87f,.8f,.79f));Panel(new Rect(992,660,Mathf.Max(4,242*fill),12),new Color(1,.39f,.64f));
  if(finished){Panel(new Rect(390,405,500,110),new Color(1,.95f,.88f,.97f));GUI.Label(new Rect(420,419,480,38),"Tiempo final: "+TimeSpan.FromSeconds(elapsed).ToString(@"mm\:ss\.ff"),label);GUI.Label(new Rect(420,463,480,35),"ENTER  /  volver a correr",label);}
 }
 Texture2D round;
 void Panel(Rect rect,Color c){GUI.color=c;GUI.DrawTexture(rect,round);GUI.color=Color.white;}
}




