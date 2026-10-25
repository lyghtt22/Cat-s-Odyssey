using UnityEngine;
using System.Collections.Generic;

// Runtime menu presentation. The preview is an isolated copy of the actual selected kart.
public sealed class RaceMenuView : MonoBehaviour {
 RacePresentation model;RacePrototypeGame game;
 Texture2D backdrop,rounded,circle;RenderTexture preview;Camera previewCamera;GameObject studio,turntable,previewKart;
 readonly List<Material> studioMaterials=new List<Material>();
 GUIStyle logo,heading,label,small,micro,cta;
 int lastVehicle=-1,lastCharacter=-1;bool showHelp;
 static readonly Color Ink=new Color(.25f,.09f,.24f),Berry=new Color(.91f,.17f,.43f),Cream=new Color(1,.97f,.9f),Mint=new Color(.15f,.68f,.57f);
 static readonly Color[] Swatches={new Color(1,.34f,.57f),new Color(.25f,.81f,.68f),new Color(.53f,.28f,.2f)};
 static readonly string[] Vehicles={"Fresa Sprint","Menta Drift","Cocoa Comet"},Characters={"Lia Fresa","Milo Menta","Coco Robot"};
 static readonly string[] Traits={"Equilibrado","Agil en curvas","Velocidad extra"};
 void Start(){model=GetComponent<RacePresentation>();game=GetComponent<RacePrototypeGame>();backdrop=Resources.Load<Texture2D>("ProtoMenu/CandyBackdrop");BuildTextures();BuildStudio();}
 void BuildTextures(){rounded=ShapeTexture(false);circle=ShapeTexture(true);}
 Texture2D ShapeTexture(bool disc){var texture=new Texture2D(96,96,TextureFormat.RGBA32,false);texture.wrapMode=TextureWrapMode.Clamp;texture.filterMode=FilterMode.Bilinear;for(int y=0;y<96;y++)for(int x=0;x<96;x++){float distance;if(disc)distance=Vector2.Distance(new Vector2(x+.5f,y+.5f),new Vector2(48,48))-46;else{float dx=Mathf.Max(19.5f-(x+.5f),(x+.5f)-76.5f),dy=Mathf.Max(19.5f-(y+.5f),(y+.5f)-76.5f);distance=new Vector2(Mathf.Max(0,dx),Mathf.Max(0,dy)).magnitude-18;}texture.SetPixel(x,y,new Color(1,1,1,Mathf.Clamp01(.5f-distance)));}texture.Apply();return texture;}
 Material Mat(Color color){var material=new Material(Shader.Find("Standard"));material.color=color;material.SetFloat("_Glossiness",.38f);studioMaterials.Add(material);return material;}
 GameObject Part(string name,PrimitiveType type,Transform parent,Vector3 position,Vector3 scale,Material material){var obj=GameObject.CreatePrimitive(type);obj.name=name;obj.transform.SetParent(parent,false);obj.transform.localPosition=position;obj.transform.localScale=scale;var collider=obj.GetComponent<Collider>();collider.enabled=false;Destroy(collider);obj.GetComponent<Renderer>().sharedMaterial=material;return obj;}
 void Layer(Transform node){node.gameObject.layer=30;foreach(Transform child in node)Layer(child);}
 void BuildStudio(){
  studio=new GameObject("Proto3D - menu display studio");studio.transform.position=new Vector3(0,200,2500);turntable=new GameObject("Candy display");turntable.transform.SetParent(studio.transform,false);
  var biscuit=Mat(new Color(.56f,.27f,.15f));var icing=Mat(new Color(1,.69f,.79f));var vanilla=Mat(new Color(1,.91f,.72f));
  Part("Wafer turntable",PrimitiveType.Cylinder,turntable.transform,Vector3.zero,new Vector3(5.8f,.18f,5.2f),biscuit);
  Part("Strawberry frosting",PrimitiveType.Cylinder,turntable.transform,Vector3.up*.2f,new Vector3(5.85f,.03f,5.25f),icing);
  for(int i=0;i<16;i++){float a=i*Mathf.PI/8;Part("Sugar bead",PrimitiveType.Sphere,turntable.transform,new Vector3(Mathf.Sin(a)*2.65f,.27f,Mathf.Cos(a)*2.35f),Vector3.one*.17f,vanilla);}
  var cameraObject=new GameObject("Menu preview camera");cameraObject.transform.SetParent(studio.transform,false);previewCamera=cameraObject.AddComponent<Camera>();previewCamera.clearFlags=CameraClearFlags.SolidColor;previewCamera.backgroundColor=Color.clear;previewCamera.cullingMask=1<<30;previewCamera.orthographic=true;previewCamera.orthographicSize=3;previewCamera.nearClipPlane=.1f;previewCamera.farClipPlane=30;previewCamera.allowHDR=false;
  cameraObject.transform.localPosition=new Vector3(5.5f,3.7f,7.5f);cameraObject.transform.LookAt(studio.transform.position+Vector3.up*1.1f);
  preview=new RenderTexture(900,650,24,RenderTextureFormat.ARGB32);preview.antiAliasing=4;preview.Create();previewCamera.targetTexture=preview;
  var lightObject=new GameObject("Menu soft key light");lightObject.transform.SetParent(studio.transform,false);var light=lightObject.AddComponent<Light>();light.type=LightType.Directional;light.color=new Color(1,.89f,.8f);light.intensity=1.1f;light.cullingMask=1<<30;light.shadows=LightShadows.Soft;lightObject.transform.rotation=Quaternion.Euler(45,-30,0);
  Layer(turntable.transform);if(game&&game.chase)game.chase.cullingMask&=~(1<<30);
 }
 void Update(){if(!model||!studio)return;bool visible=model.MenuOpen;studio.SetActive(visible);if(!visible)return;
  if(lastVehicle!=model.SelectedVehicle||lastCharacter!=model.SelectedCharacter){RebuildKart();lastVehicle=model.SelectedVehicle;lastCharacter=model.SelectedCharacter;}
  turntable.transform.localRotation=Quaternion.Euler(0,-12+Mathf.Sin(Time.unscaledTime*.3f)*10,0);
 }
 void RebuildKart(){if(!game||!game.player)return;if(previewKart){previewKart.SetActive(false);Destroy(previewKart);}previewKart=Instantiate(game.player.gameObject);previewKart.name="MenuKartPreview";previewKart.transform.SetParent(turntable.transform,false);previewKart.transform.localPosition=Vector3.zero;previewKart.transform.localRotation=Quaternion.identity;
  foreach(var behavior in previewKart.GetComponentsInChildren<MonoBehaviour>(true))behavior.enabled=false;
  foreach(var body in previewKart.GetComponentsInChildren<Rigidbody>(true)){body.isKinematic=true;body.detectCollisions=false;body.interpolation=RigidbodyInterpolation.None;Destroy(body);}
  foreach(var collider in previewKart.GetComponentsInChildren<Collider>(true))collider.enabled=false;
  foreach(var audio in previewKart.GetComponentsInChildren<AudioSource>(true)){audio.Stop();audio.enabled=false;}
  foreach(var trail in previewKart.GetComponentsInChildren<TrailRenderer>(true)){trail.emitting=false;trail.enabled=false;}
  foreach(var renderer in previewKart.GetComponentsInChildren<Renderer>(true))if(renderer.name.Contains("Escudo"))renderer.enabled=false;
  model.ApplyVehicleVisual(previewKart);float bottom=float.MaxValue;foreach(var renderer in previewKart.GetComponentsInChildren<Renderer>())if(renderer.enabled)bottom=Mathf.Min(bottom,renderer.bounds.min.y-studio.transform.position.y);if(bottom<float.MaxValue)previewKart.transform.localPosition=Vector3.up*(.26f-bottom);Layer(previewKart.transform);
 }
 void Styles(){if(label!=null)return;label=new GUIStyle(GUI.skin.label){fontSize=18,fontStyle=FontStyle.Bold,alignment=TextAnchor.MiddleLeft};label.normal.textColor=Ink;heading=new GUIStyle(label){fontSize=26};small=new GUIStyle(label){fontSize=14,fontStyle=FontStyle.Normal};micro=new GUIStyle(small){fontSize=11};logo=new GUIStyle(label){fontSize=61,fontStyle=FontStyle.Bold,alignment=TextAnchor.UpperLeft};cta=new GUIStyle(label){fontSize=27,alignment=TextAnchor.MiddleCenter};}
 void Paint(Rect rect,Color color,bool pill=true){Color old=GUI.color;GUI.color=color;GUI.DrawTexture(rect,pill?rounded:Texture2D.whiteTexture);GUI.color=old;}
 void Text(Rect rect,string text,GUIStyle style,Color color){Color old=style.normal.textColor;style.normal.textColor=color;GUI.Label(rect,text,style);style.normal.textColor=old;}
 bool Hit(Rect rect){bool hovered=rect.Contains(Event.current.mousePosition);if(hovered)Paint(new Rect(rect.x+2,rect.y+2,rect.width-4,rect.height-4),new Color(1,1,1,.12f));return GUI.Button(rect,GUIContent.none,GUIStyle.none);}
 void Shadow(Rect r){Paint(new Rect(r.x,r.y+5,r.width,r.height),new Color(.26f,.08f,.2f,.2f));}
 void Avatar(Rect rect,int index){Color old=GUI.color;GUI.color=Swatches[index];GUI.DrawTexture(rect,circle);GUI.color=old;float cx=rect.center.x,cy=rect.center.y;Color face=index==2?new Color(1,.91f,.74f):new Color(1,.81f,.67f);Paint(new Rect(cx-11,cy-11,22,24),face);Paint(new Rect(cx-6,cy-3,3,4),Ink,false);Paint(new Rect(cx+4,cy-3,3,4),Ink,false);
  if(index==0){Paint(new Rect(cx-14,cy-17,28,10),Berry);Paint(new Rect(cx-2,cy-23,4,9),Mint,false);}else if(index==1){Paint(new Rect(cx-11,cy-26,7,18),Mint);Paint(new Rect(cx+4,cy-26,7,18),Mint);}else{Paint(new Rect(cx-1,cy-23,3,12),Ink,false);Paint(new Rect(cx-4,cy-27,9,8),new Color(1,.7f,.18f));}
 }
 void DrawVehicle(int index){Rect card=new Rect(660+index*178,143,166,112);bool selected=model.SelectedVehicle==index,available=index!=2||model.CocoaUnlocked;Shadow(card);Paint(card,selected?Swatches[index]:new Color(.91f,.85f,.85f));Paint(new Rect(card.x+2,card.y+2,card.width-4,card.height-4),selected?new Color(1,.91f,.93f):Color.white);Paint(new Rect(card.x+14,card.y+14,9,9),Swatches[index]);Text(new Rect(card.x+29,card.y+8,136,25),Vehicles[index],small,Ink);
  Text(new Rect(card.x+14,card.y+38,145,21),available?Traits[index]:"Gana para desbloquear",micro,available?new Color(.5f,.3f,.41f):Berry);
  for(int b=0;b<5;b++){Paint(new Rect(card.x+14+b*25,card.y+68,21,6),b<(index==1?3:index==2?5:4)?Swatches[index]:new Color(.91f,.86f,.86f));}
  Text(new Rect(card.x+14,card.y+82,140,20),available?(selected?"SELECCIONADO":"ELEGIR KART"):"BLOQUEADO",micro,selected?Berry:new Color(.55f,.4f,.47f));
  if(available&&Hit(card))model.SelectVehicle(index);
 }
 void DrawCharacter(int index){Rect card=new Rect(660+index*178,312,166,92);bool selected=model.SelectedCharacter==index;Shadow(card);Paint(card,selected?Berry:new Color(.94f,.89f,.89f));Paint(new Rect(card.x+2,card.y+2,162,88),selected?new Color(1,.91f,.94f):Color.white);Avatar(new Rect(card.x+12,card.y+15,52,52),index);Text(new Rect(card.x+72,card.y+17,91,35),Characters[index],small,Ink);Text(new Rect(card.x+72,card.y+46,91,20),selected?"TU PILOTO":"ELEGIR",micro,selected?Berry:new Color(.55f,.4f,.47f));if(Hit(card))model.SelectCharacter(index);}
 void DrawTrack(int index){Rect card=new Rect(660+index*268,459,256,79);bool selected=model.SelectedTrack==index;Shadow(card);Paint(card,selected?Berry:new Color(.92f,.86f,.9f));Paint(new Rect(card.x+2,card.y+2,252,75),index==0?new Color(1,.86f,.9f):new Color(.88f,.87f,1));
  Color old=GUI.color;GUI.color=index==0?new Color(.92f,.38f,.59f):new Color(.49f,.44f,.74f);GUI.DrawTexture(new Rect(card.x+14,card.y+16,53,45),circle);GUI.color=Color.white;GUI.DrawTexture(new Rect(card.x+23,card.y+24,35,29),circle);GUI.color=old;
  Text(new Rect(card.x+80,card.y+12,174,27),index==0?"Sugar Bloom":"Cocoa Cloud",label,Ink);Text(new Rect(card.x+80,card.y+40,174,22),index==0?"Jardin de dulces":"Islas de malvavisco",micro,new Color(.5f,.28f,.45f));if(Hit(card))model.SelectTrack(index);}
 void OnGUI(){if(!model||!model.MenuOpen||!rounded)return;Styles();var matrix=GUI.matrix;var color=GUI.color;int depth=GUI.depth;try{GUI.depth=-200;GUI.matrix=Matrix4x4.TRS(Vector3.zero,Quaternion.identity,new Vector3(Screen.width/1280f,Screen.height/720f,1));GUI.color=Color.white;
  if(backdrop)GUI.DrawTexture(new Rect(0,0,1280,720),backdrop,ScaleMode.ScaleAndCrop);else Paint(new Rect(0,0,1280,720),new Color(1,.75f,.83f),false);
  Paint(new Rect(0,0,1280,720),new Color(.18f,.05f,.22f,.1f),false);
  Paint(new Rect(47,28,105,29),new Color(.35f,.09f,.28f,.85f));Text(new Rect(61,31,91,23),"P R O T O 3 D",micro,Color.white);
  Text(new Rect(48,85,555,148),"CANDY\nGRAND PRIX",logo,new Color(.49f,.09f,.31f));Text(new Rect(45,78,555,148),"CANDY\nGRAND PRIX",logo,new Color(1,.98f,.91f));
  Paint(new Rect(49,238,356,32),new Color(.42f,.11f,.31f,.82f));Text(new Rect(64,240,340,29),"LA CARRERA MAS DULCE EMPIEZA AQUI",micro,Color.white);
  if(preview)GUI.DrawTexture(new Rect(17,285,606,354),preview,ScaleMode.ScaleToFit,true);
  Paint(new Rect(56,622,512,56),new Color(.34f,.08f,.29f,.85f));Text(new Rect(76,630,457,28),Characters[model.SelectedCharacter]+"  +  "+Vehicles[model.SelectedVehicle],label,Color.white);Text(new Rect(76,654,457,18),"3 vueltas  /  3 rivales  /  poderes de caramelo",micro,new Color(1,.83f,.9f));
  Rect panel=new Rect(635,58,584,604);Shadow(panel);Paint(panel,new Color(1,.98f,.94f,.97f));
  Text(new Rect(660,75,485,24),"PREPARA TU CARRERA",micro,Berry);Text(new Rect(660,99,485,35),"Elige tu estilo",heading,Ink);
  for(int i=0;i<3;i++)DrawVehicle(i);
  Text(new Rect(660,274,485,28),"TU PILOTO",label,Ink);for(int i=0;i<3;i++)DrawCharacter(i);
  Text(new Rect(660,422,485,28),"EL CIRCUITO",label,Ink);DrawTrack(0);DrawTrack(1);
  Rect start=new Rect(660,564,524,65);Shadow(start);Paint(start,Berry);Paint(new Rect(663,567,518,23),new Color(1,.44f,.61f,.65f));Text(new Rect(start.x,start.y+2,start.width,start.height),"¡A CORRER!    >",cta,Color.white);if(Hit(start))model.StartSelection();
  Text(new Rect(660,634,520,21),"ENTER para correr  ·  F2 demostracion",micro,new Color(.55f,.36f,.46f));
  Rect sound=new Rect(1042,17,176,29);Paint(sound,new Color(.36f,.11f,.29f,.86f));Text(new Rect(sound.x+12,sound.y+1,166,25),model.Muted?"SONIDO: NO  / M":"SONIDO: SI  / M",micro,Color.white);if(Hit(sound))model.SetMuted(!model.Muted);
  Rect help=new Rect(891,17,140,29);Paint(help,new Color(.36f,.11f,.29f,.86f));Text(new Rect(help.x+14,help.y+1,130,25),"COMO JUGAR",micro,Color.white);if(Hit(help))showHelp=!showHelp;
  if(showHelp){Paint(new Rect(0,0,1280,720),new Color(.2f,.06f,.2f,.65f),false);Paint(new Rect(340,157,600,380),Cream);Text(new Rect(375,182,520,50),"¡Todo listo para la salida!",heading,Ink);Text(new Rect(378,247,510,210),"WASD / Flechas     Conducir\nEspacio                 Freno de mano\nF                              Turbo\nE                              Usar objeto\nC                              Cambiar camara\nR                              Recuperar kart\nESC                         Pausa",label,Ink);Rect close=new Rect(375,475,530,41);Paint(close,Berry);Text(close,"ENTENDIDO",cta,Color.white);if(Hit(close))showHelp=false;}
 }finally{GUI.matrix=matrix;GUI.color=color;GUI.depth=depth;}}
 void OnDestroy(){if(studio)Destroy(studio);if(preview){preview.Release();Destroy(preview);}if(rounded)Destroy(rounded);if(circle)Destroy(circle);foreach(var material in studioMaterials)if(material)Destroy(material);}
}
