using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using System;

public class RacePrototypeGame : MonoBehaviour {
 public CarController player, rival;
 public Camera chase;
 public Vector3[] route, gates;
 public int laps=3;
 [Header("Controles arcade")]
 [Range(.2f,1f)] public float steeringSensitivity=.65f;
 [Min(.1f)] public float steeringResponse=2.2f, steeringReturn=3.5f;
 [Range(.1f,1f)] public float acceleration=.65f;
 public float maximumSpeedKmh=100,boostSpeedKmh=125,reversePower=.5f;
 public bool showMinimap=true;
 public Color playerMapColor=new Color(0,.72f,.85f),rivalMapColor=new Color(.9f,.12f,.35f);
 public bool IsRacing {get{return enabled&&cars!=null&&!finished&&!paused&&!InMenu&&countdown<=0;}}
 public CarController[] Racers {get{return cars;}}
 public RacePresentation Presentation {get{return presentation;}}
 bool InMenu {get{return presentation&&presentation.MenuOpen;}}
 int observedFrames;float observedFrameTime,worstFrame,maxGroundError,minimumUpright=1,minimumCameraClearance=float.MaxValue;
 float keyboardSteer,finalTime,countdown=3,elapsed,boostUntil,boostReady,noticeUntil;
 string notice="";
 int[] next,completed; bool finished,demo,paused;
 int initializedFrame,cameraMode,finalPosition;
 float[] finishTimes,boosts,stuck,speeds,rideOffsets; Vector3[] arcadeVelocity;
 Vector3[] previous,spawnPositions,originalRoute,originalGates;
 Quaternion[] spawnRotations;
 Rigidbody[] bodies;CarController[] cars;TrailRenderer[] trails;WheelCollider[][] wheels;
 RaceCandyItems items;RacePresentation presentation;RaceFantasyTrack fantasy;
 GUIStyle label,big,small;Texture2D round;
  Collider[] roadSurfaces;
 Vector3[][] wheelContacts;
 bool verificationInput=false;Vector2 verificationAxes=Vector2.zero;
 void CacheRoadSurfaces(){var list=new System.Collections.Generic.List<Collider>();foreach(var collider in FindObjectsOfType<Collider>(true)){string n=collider.name.ToLowerInvariant();if(collider is MeshCollider&&(n=="1tarmac_oval"||n=="carretera continua"||n.StartsWith("puente real - atajo")))list.Add(collider);}roadSurfaces=list.ToArray();}
 public bool TryRoadSurface(Vector3 position,out Vector3 point,out Vector3 normal){
  point=position;normal=Vector3.up;bool found=false;float highest=float.MinValue;
  if(roadSurfaces==null)return false;
  Ray ray=new Ray(new Vector3(position.x,ClosestRoad(position).y+25,position.z),Vector3.down);
  foreach(var surface in roadSurfaces)if(surface&&surface.enabled&&surface.gameObject.activeInHierarchy){RaycastHit hit;if(surface.Raycast(ray,out hit,50)&&hit.normal.y>.6f&&hit.point.y>highest){point=hit.point;normal=hit.normal;highest=point.y;found=true;}}
  return found;
 }
 bool FootprintFits(Vector3 center,Quaternion heading){
  Vector3 forward=Vector3.ProjectOnPlane(heading*Vector3.forward,Vector3.up).normalized,right=Vector3.Cross(Vector3.up,forward),p,n;
  if(!TryRoadSurface(center,out p,out n))return false;
  for(int side=-1;side<=1;side+=2)for(int end=-1;end<=1;end+=2)if(!TryRoadSurface(center+right*side*1.35f+forward*end*1.75f,out p,out n))return false;
  return true;
 }
 bool SupportRoad(int index,Vector3 requested,Quaternion heading,out Vector3 supported,out Quaternion aligned){
  Vector3 center=ClosestRoad(requested),offset=requested-center;offset.y=0;offset=Vector3.ClampMagnitude(offset,11.5f);
  Vector3 candidate=center+offset;bool edge=!FootprintFits(candidate,heading);
  if(edge){float low=0,high=1;for(int k=0;k<7;k++){float mid=(low+high)*.5f;if(FootprintFits(center+offset*mid,heading))low=mid;else high=mid;}candidate=center+offset*low;}
  Vector3 surface,normal;
  if(!TryRoadSurface(candidate,out surface,out normal)){surface=center;normal=Vector3.up;edge=true;}
  Vector3 forward=Vector3.ProjectOnPlane(heading*Vector3.forward,normal);if(forward.sqrMagnitude<.01f)forward=Vector3.ProjectOnPlane(Vector3.forward,normal);
  aligned=Quaternion.LookRotation(forward.normalized,normal);supported=surface+normal*rideOffsets[index];
  // Fit every visual tire to the physical asphalt, including banked edges and crests.
  float lift=0;
  foreach(var contact in wheelContacts[index]){Vector3 tire=supported+aligned*contact,p,n;if(TryRoadSurface(tire,out p,out n))lift=Mathf.Max(lift,p.y+.02f-tire.y);}
  supported.y+=lift;
  return edge||offset.sqrMagnitude>132;
 }
 public bool RoadContactValid(int index,out float smallestClearance){
  smallestClearance=float.MaxValue;if(index<0||index>=cars.Length)return false;
  foreach(var contact in wheelContacts[index]){Vector3 tire=cars[index].transform.TransformPoint(contact),p,n;if(!TryRoadSurface(tire,out p,out n))return false;smallestClearance=Mathf.Min(smallestClearance,tire.y-p.y);}
  return smallestClearance>=-.025f&&smallestClearance<.3f;
 }

 void Start(){
  initializedFrame=Time.frameCount;
  if(!player||!rival||!chase||route==null||route.Length<8||gates==null||gates.Length<2||laps<1){Debug.LogError("RACE CONFIG ERROR: revisar configuracion de carrera");enabled=false;return;}
  originalRoute=(Vector3[])route.Clone();originalGates=(Vector3[])gates.Clone();
  var extraA=Instantiate(rival.gameObject).GetComponent<CarController>();
  var extraB=Instantiate(rival.gameObject).GetComponent<CarController>();
  extraA.name="CPU - Vanilla";extraB.name="CPU - Cocoa";
  cars=new[]{player,rival,extraA,extraB};int count=cars.Length;
  bodies=new Rigidbody[count];next=new int[count];completed=new int[count];previous=new Vector3[count];stuck=new float[count];
  wheelContacts=new Vector3[count][];rideOffsets=new float[count];speeds=new float[count];arcadeVelocity=new Vector3[count];trails=new TrailRenderer[count];finishTimes=new float[count];boosts=new float[count];spawnPositions=new Vector3[count];spawnRotations=new Quaternion[count];wheels=new WheelCollider[count][];
  for(int i=0;i<count;i++){
   var user=cars[i].GetComponent<CarUserControl>();if(user)user.enabled=false;
   var ai=cars[i].GetComponent<CarAIControl>();if(ai)ai.enabled=false;
   bodies[i]=cars[i].GetComponent<Rigidbody>();if(!bodies[i]){Debug.LogError("RACE CONFIG ERROR: falta Rigidbody");enabled=false;return;}
   bodies[i].isKinematic=true;cars[i].enabled=false; foreach(var component in cars[i].GetComponentsInChildren<MonoBehaviour>())if(component.GetType().Name=="CarAudio"||component.GetType().Name=="WheelEffects")component.enabled=false;
   float lowestWheel=float.MaxValue;foreach(var renderer in cars[i].GetComponentsInChildren<Renderer>())if(renderer.enabled&&renderer.name=="Chocolate cookie tire")lowestWheel=Mathf.Min(lowestWheel,renderer.bounds.min.y);rideOffsets[i]=lowestWheel<float.MaxValue?bodies[i].position.y-lowestWheel+.015f:.1f;
   var contacts=new System.Collections.Generic.List<Vector3>();foreach(var renderer in cars[i].GetComponentsInChildren<Renderer>())if(renderer.enabled&&renderer.name=="Chocolate cookie tire")contacts.Add(cars[i].transform.InverseTransformPoint(new Vector3(renderer.bounds.center.x,renderer.bounds.min.y,renderer.bounds.center.z)));wheelContacts[i]=contacts.ToArray();
   bodies[i].maxAngularVelocity=3;
   wheels[i]=cars[i].GetComponentsInChildren<WheelCollider>();foreach(var wheel in wheels[i])wheel.enabled=false;
   var go=new GameObject("Sugar turbo trail");go.transform.SetParent(cars[i].transform,false);go.transform.localPosition=new Vector3(0,.55f,-2);
   var t=go.AddComponent<TrailRenderer>();t.time=.45f;t.startWidth=.8f;t.endWidth=0;t.material=new Material(Shader.Find("Sprites/Default"));t.startColor=new Color(1,.22f,.6f);t.endColor=new Color(1,.4f,.8f,0);t.emitting=false;trails[i]=t;
  }
  fantasy=gameObject.AddComponent<RaceFantasyTrack>();fantasy.Build();fantasy.SetVisible(false);CacheRoadSurfaces();
  items=gameObject.AddComponent<RaceCandyItems>();
  presentation=gameObject.AddComponent<RacePresentation>();presentation.StartRequested+=SelectRace;
  SetGrid();ResetRace();Time.timeScale=0;
 }
 void SetGrid(){
  Vector3 forward=route[1]-route[0];forward.y=0;forward.Normalize();Vector3 right=Vector3.Cross(Vector3.up,forward);
  for(int i=0;i<cars.Length;i++){spawnPositions[i]=route[0]-forward*(14+(i/2)*9)+right*((i%2==0?-1:1)*3)+Vector3.up*rideOffsets[i];spawnRotations[i]=Quaternion.LookRotation(forward);Vector3 support;Quaternion heading;SupportRoad(i,spawnPositions[i],spawnRotations[i],out support,out heading);spawnPositions[i]=support;spawnRotations[i]=heading;}
 }
 void SelectRace(int track,int vehicle){
  route=track==0?(Vector3[])originalRoute.Clone():fantasy.Route;gates=track==0?(Vector3[])originalGates.Clone():fantasy.Gates;
  fantasy.SetVisible(track!=0);SetGrid();presentation.ApplyVehicleVisual(player.gameObject);ResetRace();items.ConfigureTrack(track);
  if(chase){chase.transform.position=player.transform.position-player.transform.forward*7+Vector3.up*4;chase.transform.LookAt(player.transform.position+player.transform.forward*5);}
  Time.timeScale=1;Debug.Log("RACE START version=surface-v2 track="+track+" vehicle="+vehicle+" participants="+cars.Length);
 }
 public void ActivateBoost(int carIndex,float seconds){if(cars==null||carIndex<0||carIndex>=cars.Length||completed[carIndex]>=laps)return;boosts[carIndex]=Mathf.Max(boosts[carIndex],Time.time+Mathf.Clamp(seconds,.1f,5));if(carIndex==0){boostUntil=boosts[0];Message("¡TURBO DE AZUCAR!");}if(presentation)presentation.PlayCue("boost");}
 void CandyBoost(){ActivateBoost(0,2.5f);}
 void CandyCue(int cue){if(presentation)presentation.PlayCue(cue==0?"pickup":cue==1?"boost":cue==2?"shield":cue==3?"hit":"hit");}
 void OnDisable(){Time.timeScale=1;}
 void OnDestroy(){if(presentation)presentation.StartRequested-=SelectRace;if(trails!=null)foreach(var t in trails)if(t&&t.sharedMaterial)Destroy(t.sharedMaterial);if(round)Destroy(round);}
 void Update(){
  if(cars==null)return;
  if(InMenu){if(Input.GetKeyDown(KeyCode.F2)){demo=true;presentation.StartSelection();}else if(demo)presentation.StartSelection();return;}
  if(Input.GetKeyDown(KeyCode.C))cameraMode=(cameraMode+1)%3;
  if(Input.GetKeyDown(KeyCode.Escape)&&!finished)TogglePause();
  if(paused)return;
  if(IsRacing){observedFrames++;observedFrameTime+=Time.unscaledDeltaTime;worstFrame=Mathf.Max(worstFrame,Time.unscaledDeltaTime);}
  if(Input.GetKeyDown(KeyCode.F2)){demo=!demo;Message(demo?"DEMOSTRACION AUTOMATICA":"CONTROL MANUAL");}
  if(Input.GetKeyDown(KeyCode.R)&&!finished)Recover(0);
  if(Input.GetKeyDown(KeyCode.Return)&&finished)Restart();
  if(countdown>0){countdown-=Time.deltaTime;return;}
  bool all=true;for(int i=0;i<completed.Length;i++)if(completed[i]<laps)all=false;
  if(!all)elapsed+=Time.deltaTime;
  if(countdown>-1)countdown-=Time.deltaTime;
 }
 void TogglePause(){paused=!paused;Time.timeScale=paused?0:1;}
 void OpenMenu(){paused=false;demo=false;presentation.OpenMenu();Time.timeScale=0;}
 void FixedUpdate(){
  if(cars==null||paused||InMenu||Time.frameCount<=initializedFrame+1)return;
  for(int i=0;i<cars.Length;i++){
   if(countdown>0||completed[i]>=laps){speeds[i]=0;arcadeVelocity[i]=Vector3.zero;trails[i].emitting=false;continue;}
   var car=cars[i];var rb=bodies[i];float clearance;bool grounded=RoadContactValid(i,out clearance);maxGroundError=Mathf.Max(maxGroundError,grounded?Mathf.Max(0,-clearance):1);minimumUpright=Mathf.Min(minimumUpright,Vector3.Dot(car.transform.up,Vector3.up));float speed=Mathf.Abs(speeds[i]);Quaternion rotation=rb.rotation;
   bool automatic=i>0||demo;
   if(automatic){
    int target=Nearest(rb.position);float distance=0;int attempts=0;
    while(distance<Mathf.Clamp(speed*1.05f,12,30)&&attempts++<route.Length){int n=(target+1)%route.Length;distance+=Vector3.Distance(route[target],route[n]);target=n;}
    Vector3 tangent=(route[(target+1)%route.Length]-route[target]).normalized;float lane=i==0?0:(i-2)*2.4f;
    if(i>0){
     Vector3 following=car.transform.InverseTransformPoint(player.transform.position);
     if(following.z<-6&&following.z>-22&&Mathf.Abs(following.x)<5)lane=Mathf.Clamp(lane+following.x*.25f,-3.5f,3.5f);
     foreach(var other in cars)if(other!=car){Vector3 ahead=car.transform.InverseTransformPoint(other.transform.position);if(ahead.z>0&&ahead.z<14&&Mathf.Abs(ahead.x)<3.5f)lane+=ahead.x>=0?-3.6f:3.6f;}
    }
    Vector3 aim=route[target]+Vector3.Cross(Vector3.up,tangent)*Mathf.Clamp(lane,-5,5);Vector3 direction=Vector3.ProjectOnPlane(aim-rb.position,Vector3.up);
    float angle=direction.sqrMagnitude>.1f?Vector3.Angle(car.transform.forward,direction):0;
    float desired=22-i*.4f-Mathf.Clamp(angle*.15f,0,8)+(Time.time<boosts[i]?6:0);
    speeds[i]=Mathf.MoveTowards(speeds[i],desired,7*Time.fixedDeltaTime);
    if(direction.sqrMagnitude>.1f)rotation=Quaternion.RotateTowards(rotation,Quaternion.LookRotation(direction),85*Time.fixedDeltaTime);
   }else{
    float horizontal=(Input.GetKey(KeyCode.RightArrow)||Input.GetKey(KeyCode.D)?1:0)-(Input.GetKey(KeyCode.LeftArrow)||Input.GetKey(KeyCode.A)?1:0);
    float vertical=(Input.GetKey(KeyCode.UpArrow)||Input.GetKey(KeyCode.W)?1:0)-(Input.GetKey(KeyCode.DownArrow)||Input.GetKey(KeyCode.S)?1:0);
    if(verificationInput){horizontal=verificationAxes.x;vertical=verificationAxes.y;}
    keyboardSteer=Mathf.MoveTowards(keyboardSteer,horizontal*steeringSensitivity*presentation.VehicleHandlingMultiplier,(horizontal==0?steeringReturn:steeringResponse)*Time.fixedDeltaTime);
    float limit=(Time.time<boosts[0]?boostSpeedKmh:maximumSpeedKmh)*presentation.VehicleSpeedMultiplier/3.6f;
    float desired=vertical>0?limit:vertical<0?(speeds[i]>1?0:-8*reversePower):0;
    float response=vertical>0?8:vertical<0?17:2;
    if(Input.GetKey(KeyCode.Space)){desired=0;response=21;}
    speeds[i]=Mathf.MoveTowards(speeds[i],desired,response*Time.fixedDeltaTime);
    float turn=keyboardSteer*85*Mathf.Clamp01(Mathf.Abs(speeds[i])/3)*Mathf.Sign(speeds[i]);
    rotation=Quaternion.Euler(0,rotation.eulerAngles.y+turn*Time.fixedDeltaTime,0);
   }
   var slow=car.GetComponent<CandySlow>();if(slow&&slow.enabled&&slow.remaining>0)speeds[i]=Mathf.MoveTowards(speeds[i],Mathf.Clamp(speeds[i],-5,9),28*Time.fixedDeltaTime);
   Vector3 forward=rotation*Vector3.forward;Vector3 candidate=rb.position+forward*speeds[i]*Time.fixedDeltaTime;
   Vector3 supported;Quaternion aligned;bool edge=SupportRoad(i,candidate,rotation,out supported,out aligned);candidate=supported;rotation=aligned;if(edge){speeds[i]*=.96f;if(i==0&&!demo)Message("BORDE DE PISTA / reduce el giro");}
   for(int other=0;other<cars.Length;other++)if(other!=i&&completed[other]<laps){Vector3 gap=bodies[other].position-rb.position;gap.y=0;Vector3 nextGap=bodies[other].position-candidate;nextGap.y=0;if(nextGap.sqrMagnitude<9&&Vector3.Dot(gap,forward)*Mathf.Sign(speeds[i])>0&&nextGap.sqrMagnitude<gap.sqrMagnitude){SupportRoad(i,rb.position,rotation,out supported,out aligned);candidate=supported;rotation=aligned;speeds[i]=0;break;}}
   arcadeVelocity[i]=(candidate-rb.position)/Time.fixedDeltaTime;rb.MovePosition(candidate);rb.MoveRotation(rotation);trails[i].emitting=Time.time<boosts[i];
   if(i==0){bool pad=presentation.SelectedTrack==0&&candidate.x>35&&candidate.x<49&&Mathf.Abs(candidate.z+29)<8;if((pad||(!demo&&Input.GetKey(KeyCode.F)))&&Time.time>boostReady){ActivateBoost(0,2.5f);boostReady=Time.time+7;}}
   int gateRoute=GateRouteIndex(next[i]);Vector3 dir=route[(gateRoute+1)%route.Length]-route[gateRoute];
   if(CrossedGate(previous[i],candidate,gates[next[i]],dir)){
    if(next[i]==0){completed[i]++;if(completed[i]>=laps){finishTimes[i]=elapsed;if(i==0)FinishRace();else ParkRival(i);}else if(i==0)Message("VUELTA "+(completed[i]+1)+" / "+laps);}
    Debug.Log("RACE CHECKPOINT car="+i+" gate="+next[i]+" laps="+completed[i]+" time="+elapsed);next[i]=(next[i]+1)%gates.Length;
   }
   previous[i]=candidate;if(Mathf.Abs(speeds[i])<.5f)stuck[i]+=Time.fixedDeltaTime;else stuck[i]=0;
   if(completed[i]<laps&&automatic&&stuck[i]>5)Recover(i);
  }
 }
 Vector3 ClosestRoad(Vector3 point){float best=float.MaxValue;Vector3 closest=route[0];for(int j=0;j<route.Length;j++){Vector3 a=route[j],b=route[(j+1)%route.Length],d=b-a;Vector3 horizontal=d;horizontal.y=0;Vector3 offset=point-a;offset.y=0;float fraction=horizontal.sqrMagnitude>.001f?Mathf.Clamp01(Vector3.Dot(offset,horizontal)/horizontal.sqrMagnitude):0;Vector3 candidate=Vector3.Lerp(a,b,fraction);offset=point-candidate;offset.y=0;if(offset.sqrMagnitude<best){best=offset.sqrMagnitude;closest=candidate;}}return closest;}
 void ParkRival(int i){Vector3 forward=(route[1]-route[0]).normalized;Vector3 right=Vector3.Cross(Vector3.up,forward);bodies[i].position=route[0]+forward*(25+i*7)+right*10+Vector3.up*rideOffsets[i];speeds[i]=0;arcadeVelocity[i]=Vector3.zero;}

 int GateRouteIndex(int gate){return Nearest(gates[gate]);}
 public static bool CrossedGate(Vector3 from,Vector3 to,Vector3 gate,Vector3 direction){direction.y=0;if(direction.sqrMagnitude<.0001f)return false;direction.Normalize();float a=Vector3.Dot(from-gate,direction),b=Vector3.Dot(to-gate,direction);if(a>0||b<=0)return false;Vector3 crossing=Vector3.Lerp(from,to,-a/(b-a));Vector3 off=crossing-gate;off.y=0;off-=direction*Vector3.Dot(off,direction);return off.magnitude<13&&Mathf.Abs(crossing.y-gate.y)<8;}
 void FinishRace(){if(finished)return;finalPosition=Position(0);finished=true;finalTime=elapsed;boosts[0]=0;boostUntil=0;trails[0].emitting=false;speeds[0]=0;arcadeVelocity[0]=Vector3.zero;presentation.RecordResult(finalPosition);Message("META - CARRERA COMPLETADA");Debug.Log("RACE FINISH track="+presentation.SelectedTrack+" laps="+completed[0]+" position="+finalPosition+" time="+finalTime);}
 int Position(int who){int rank=1;for(int i=0;i<cars.Length;i++)if(i!=who){if(finishTimes[i]>=0){if(finishTimes[who]<0||finishTimes[i]<finishTimes[who]||(finishTimes[i]==finishTimes[who]&&i<who))rank++;}else if(finishTimes[who]<0){int a=completed[i]*gates.Length+(next[i]==0?gates.Length:next[i]);int b=completed[who]*gates.Length+(next[who]==0?gates.Length:next[who]);if(a>b||(a==b&&Vector3.Distance(cars[i].transform.position,gates[next[i]])<Vector3.Distance(cars[who].transform.position,gates[next[who]])))rank++;}}return rank;}
 int Nearest(Vector3 p){int best=0;float d=float.MaxValue;for(int j=0;j<route.Length;j++){float n=(p-route[j]).sqrMagnitude;if(n<d){d=n;best=j;}}return best;}
 void ResetRace(){minimumCameraClearance=float.MaxValue;maxGroundError=0;minimumUpright=1;observedFrames=0;observedFrameTime=worstFrame=0;paused=false;countdown=3;elapsed=0;finalTime=0;finalPosition=0;keyboardSteer=0;notice="";noticeUntil=0;finished=false;boostUntil=boostReady=0;for(int i=0;i<cars.Length;i++){completed[i]=0;finishTimes[i]=-1;next[i]=1;stuck[i]=0;boosts[i]=0;trails[i].emitting=false;trails[i].Clear();bodies[i].position=spawnPositions[i];bodies[i].rotation=spawnRotations[i];speeds[i]=0;arcadeVelocity[i]=Vector3.zero;previous[i]=bodies[i].position;}if(items)items.ResetItems();}
 void Restart(){ResetRace();Time.timeScale=1;Debug.Log("RACE RESTART");}
 void Recover(int i){int prev=(next[i]+gates.Length-1)%gates.Length;int k=GateRouteIndex(prev);var dir=(route[(k+1)%route.Length]-route[k]).normalized;Vector3 supported;Quaternion aligned;SupportRoad(i,route[k],Quaternion.LookRotation(dir),out supported,out aligned);bodies[i].position=supported;bodies[i].rotation=aligned;speeds[i]=0;arcadeVelocity[i]=Vector3.zero;previous[i]=bodies[i].position;stuck[i]=0;if(i==0)Message("AUTO RECUPERADO");}
 void Message(string text){notice=text;noticeUntil=Time.time+2.5f;}
 void LateUpdate(){
  if(!player||!chase||InMenu)return;
  var t=player.transform;Vector3 forward=Vector3.ProjectOnPlane(t.forward,Vector3.up);if(forward.sqrMagnitude<.01f)forward=Vector3.forward;forward.Normalize();

  Vector3 desired=cameraMode==1?t.position+forward*7+Vector3.up*3:cameraMode==2?t.position-forward*3.7f+Vector3.up*2:t.position-forward*6.8f+Vector3.up*3.6f;

  Vector3 cameraGround,cameraNormal;if(TryRoadSurface(desired,out cameraGround,out cameraNormal))desired.y=Mathf.Max(desired.y,cameraGround.y+2.2f);
  float blend=(chase.transform.position-desired).sqrMagnitude>900?1:1-Mathf.Exp(-7*Time.deltaTime);
  Vector3 cameraPosition=Vector3.Lerp(chase.transform.position,desired,blend);if(TryRoadSurface(cameraPosition,out cameraGround,out cameraNormal))cameraPosition.y=Mathf.Max(cameraPosition.y,cameraGround.y+2.2f);chase.transform.position=cameraPosition;if(TryRoadSurface(cameraPosition,out cameraGround,out cameraNormal))minimumCameraClearance=Mathf.Min(minimumCameraClearance,cameraPosition.y-cameraGround.y);
  Vector3 look=t.position+(cameraMode==1?Vector3.zero:forward*6)+Vector3.up-chase.transform.position;
  if(look.sqrMagnitude>.01f)chase.transform.rotation=Quaternion.Slerp(chase.transform.rotation,Quaternion.LookRotation(look,Vector3.up),blend);
  chase.fieldOfView=Mathf.Lerp(chase.fieldOfView,Time.time<boostUntil?75:62,Time.deltaTime*3);
 }
 void OnGUI(){
  if(bodies==null||!enabled||InMenu)return;
  GUI.matrix=Matrix4x4.TRS(Vector3.zero,Quaternion.identity,new Vector3(Screen.width/1280f,Screen.height/720f,1));
  if(label==null||round==null){
   label=new GUIStyle(GUI.skin.label){fontSize=23,fontStyle=FontStyle.Bold};label.normal.textColor=new Color(.32f,.13f,.22f);
   big=new GUIStyle(label){fontSize=84,alignment=TextAnchor.MiddleCenter};big.normal.textColor=new Color(1,.96f,.88f);
   small=new GUIStyle(label){fontSize=15,fontStyle=FontStyle.Normal};small.normal.textColor=new Color(.42f,.27f,.33f);
   round=new Texture2D(64,64,TextureFormat.RGBA32,false);for(int x=0;x<64;x++)for(int y=0;y<64;y++){float dx=Mathf.Max(12-x,x-51),dy=Mathf.Max(12-y,y-51);round.SetPixel(x,y,new Color(1,1,1,Mathf.Sqrt(Mathf.Max(0,dx)*Mathf.Max(0,dx)+Mathf.Max(0,dy)*Mathf.Max(0,dy))<12?1:0));}round.Apply();
  }
  Panel(new Rect(26,26,294,124),new Color(1,.95f,.88f,.96f));
  GUI.Label(new Rect(46,37,260,35),presentation.TrackName.ToUpper(),label);
  GUI.Label(new Rect(47,69,260,24),"C A N D Y   G R A N D   P R I X",small);
  GUI.Label(new Rect(47,101,260,36),"VUELTA  "+Mathf.Min(completed[0]+1,laps)+" / "+laps,label);
  Panel(new Rect(1010,26,244,124),new Color(1,.95f,.88f,.96f));
  GUI.Label(new Rect(1030,36,205,36),Mathf.RoundToInt(arcadeVelocity[0].magnitude*3.6f)+" km/h",label);
  GUI.Label(new Rect(1030,75,205,30),TimeSpan.FromSeconds(finished?finalTime:elapsed).ToString(@"mm\:ss\.ff"),label);
  int passed=finished?gates.Length-1:next[0]==0?gates.Length-1:next[0]-1;
  for(int i=0;i<gates.Length-1;i++)Panel(new Rect(1032+i*189f/(gates.Length-1),119,Mathf.Min(18,160f/(gates.Length-1)),7),i<passed?new Color(1,.4f,.65f):new Color(.84f,.76f,.74f));
  Panel(new Rect(26,158,294,66),new Color(1,.95f,.88f,.96f));
  GUI.Label(new Rect(42,164,270,24),"POSICION: "+(finished?finalPosition:Position(0))+"º / "+cars.Length+"    Vueltas: "+completed[0],small);
  GUI.Label(new Rect(42,191,270,24),finished?"CARRERA COMPLETADA":next[0]==0?"Siguiente: META":"Siguiente: control "+next[0]+" / "+(gates.Length-1),small);
  if(demo){Panel(new Rect(454,28,372,36),new Color(1,.95f,.88f,.9f));GUI.Label(new Rect(473,34,350,26),"DEMO  /  piloto automatico",small);}
  string center=countdown>0?Mathf.CeilToInt(countdown).ToString():countdown>-1?"¡YA!":finished?"¡META!":"";
  if(center!=""){Panel(new Rect(420,236,440,150),new Color(.75f,.2f,.42f,.8f));GUI.Label(new Rect(420,236,440,150),center,big);}
  if(Time.time<noticeUntil&&!finished){Panel(new Rect(435,161,410,42),new Color(1,.95f,.88f,.95f));GUI.Label(new Rect(452,166,400,35),notice,label);}
    if(showMinimap)DrawMinimap();
  Panel(new Rect(26,235,294,78),new Color(1,.95f,.88f,.96f));
  if(items){GUI.Label(new Rect(42,242,270,28),"E / "+items.HeldName,label);GUI.Label(new Rect(42,275,270,28),items.ShieldActive?"ESCUDO ACTIVO":items.StatusMessage,small);}
  GUI.Label(new Rect(35,319,320,28),"M / musica y sonido",small);
  if(paused){Panel(new Rect(390,230,500,230),new Color(1,.95f,.88f,.98f));GUI.Label(new Rect(420,248,450,40),"PAUSA",label);if(GUI.Button(new Rect(420,300,420,38),"Continuar / ESC"))TogglePause();if(GUI.Button(new Rect(420,348,420,38),"Reiniciar carrera"))Restart();if(GUI.Button(new Rect(420,396,420,38),"Elegir vehiculo y pista"))OpenMenu();}
  Panel(new Rect(27,656,917,39),new Color(1,.95f,.88f,.93f));GUI.Label(new Rect(42,664,900,27),"FLECHAS / WASD conducir  ·  ESPACIO freno mano  ·  F turbo  ·  R recuperar  ·  F2 demo · C camara · ESC pausa",small);
  Panel(new Rect(972,607,282,89),new Color(1,.95f,.88f,.96f));
  GUI.Label(new Rect(992,618,250,30),Time.time<boostUntil?"¡SUGAR BOOST!":Time.time<boostReady?"RECARGANDO AZUCAR":"SUGAR BOOST  /  F",small);
  float fill=Time.time<boostUntil?(boostUntil-Time.time)/2.5f:Mathf.Clamp01(1-(boostReady-Time.time)/7);
  Panel(new Rect(992,660,242,12),new Color(.87f,.8f,.79f));Panel(new Rect(992,660,Mathf.Max(4,242*fill),12),new Color(1,.39f,.64f));
  if(finished){Panel(new Rect(390,405,500,110),new Color(1,.95f,.88f,.97f));GUI.Label(new Rect(420,419,480,38),"Puesto "+finalPosition+"º   Tiempo: "+TimeSpan.FromSeconds(finalTime).ToString(@"mm\:ss\.ff"),label);GUI.Label(new Rect(420,463,480,35),"ENTER / correr de nuevo",label);if(GUI.Button(new Rect(420,527,440,42),"Elegir vehiculo y pista"))OpenMenu();}
 }

 void DrawMinimap(){
  if(route==null||route.Length<2)return;
  Panel(new Rect(1010,166,244,214),new Color(1,.95f,.88f,.96f));
  GUI.Label(new Rect(1026,174,220,25),"RECORRIDO",small);
  float minX=float.MaxValue,maxX=float.MinValue,minZ=float.MaxValue,maxZ=float.MinValue;
  foreach(var p in route){minX=Mathf.Min(minX,p.x);maxX=Mathf.Max(maxX,p.x);minZ=Mathf.Min(minZ,p.z);maxZ=Mathf.Max(maxZ,p.z);}
  Rect area=new Rect(1029,209,206,128);
  float scale=Mathf.Min((area.width-20)/Mathf.Max(1,maxX-minX),(area.height-20)/Mathf.Max(1,maxZ-minZ));
  Vector2 center=new Vector2((minX+maxX)/2,(minZ+maxZ)/2);
  for(int i=0;i<route.Length;i++){
   Vector2 a=MapPoint(route[i],area,center,scale),b=MapPoint(route[(i+1)%route.Length],area,center,scale);
   MapLine(a,b,9,new Color(.4f,.28f,.36f));MapLine(a,b,5,new Color(.85f,.74f,.8f));
  }
  if(!finished)MapMarker(MapPoint(gates[next[0]],area,center,scale),new Color(1,.65f,.1f),false);
  Vector2 start=MapPoint(route[0],area,center,scale);MapLine(start+Vector2.up*7,start-Vector2.up*7,3,Color.white);
  // Posiciones reales, no progreso estimado: tambien refleja reversa y salidas de pista.
  for(int i=1;i<cars.Length;i++)MapMarker(MapPoint(cars[i].transform.position,area,center,scale),rivalMapColor,false);
  if(player)MapMarker(MapPoint(player.transform.position,area,center,scale),playerMapColor,true);
  MapMarker(new Vector2(1033,359),playerMapColor,true);GUI.Label(new Rect(1045,347,80,25),"TU",small);
  MapMarker(new Vector2(1135,359),rivalMapColor,false);GUI.Label(new Rect(1147,347,100,25),rival?"RIVAL":"SIN RIVAL",small);
 }
 Vector2 MapPoint(Vector3 p,Rect area,Vector2 center,float scale){return area.center+new Vector2((p.x-center.x)*scale,-(p.z-center.y)*scale);}
 void MapLine(Vector2 a,Vector2 b,float width,Color color){
  Matrix4x4 old=GUI.matrix;GUI.matrix=old*Matrix4x4.TRS(new Vector3(a.x,a.y,0),Quaternion.Euler(0,0,Mathf.Atan2(b.y-a.y,b.x-a.x)*Mathf.Rad2Deg),Vector3.one);
  GUI.color=color;GUI.DrawTexture(new Rect(0,-width/2,Vector2.Distance(a,b),width),Texture2D.whiteTexture);GUI.color=Color.white;GUI.matrix=old;
 }
 void MapMarker(Vector2 p,Color color,bool isPlayer){
  // Distintas formas ademas del color: jugador rombo, rival cuadrado.
  Matrix4x4 old=GUI.matrix;GUI.matrix=old*Matrix4x4.TRS(new Vector3(p.x,p.y,0),Quaternion.Euler(0,0,isPlayer?45:0),Vector3.one);
  GUI.color=new Color(.2f,.12f,.2f);GUI.DrawTexture(new Rect(-6,-6,12,12),Texture2D.whiteTexture);
  GUI.color=color;GUI.DrawTexture(new Rect(-4,-4,8,8),Texture2D.whiteTexture);GUI.color=Color.white;GUI.matrix=old;
 }
 void Panel(Rect rect,Color c){GUI.color=c;GUI.DrawTexture(rect,round);GUI.color=Color.white;}
}
