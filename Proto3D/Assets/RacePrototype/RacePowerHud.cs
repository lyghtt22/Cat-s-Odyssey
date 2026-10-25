using UnityEngine;

// Original readable candy icons, drawn locally without external artwork.
public sealed class RacePowerHud : MonoBehaviour {
 RacePrototypeGame game;RaceCandyItems items;Texture2D[] icons;Texture2D panel;GUIStyle title,body,badge,alert;
 static readonly Color Ink=new Color(.3f,.1f,.23f),Cream=new Color(1,.96f,.89f),Berry=new Color(.85f,.12f,.4f);
 void Start(){game=GetComponent<RacePrototypeGame>();items=GetComponent<RaceCandyItems>();icons=new Texture2D[5];for(int i=0;i<5;i++)icons[i]=Icon(i);panel=Round();}
 static bool Disk(float x,float y,float cx,float cy,float r){return (x-cx)*(x-cx)+(y-cy)*(y-cy)<r*r;}
 static bool Polygon(float x,float y,Vector2[] p){bool inside=false;for(int i=0,j=p.Length-1;i<p.Length;j=i++){if((p[i].y>y)!=(p[j].y>y)&&x<(p[j].x-p[i].x)*(y-p[i].y)/(p[j].y-p[i].y)+p[i].x)inside=!inside;}return inside;}
 Texture2D Icon(int id){
  var t=new Texture2D(80,80,TextureFormat.RGBA32,false);t.filterMode=FilterMode.Bilinear;
  Vector2[] bolt={new Vector2(43,7),new Vector2(19,44),new Vector2(36,44),new Vector2(31,72),new Vector2(62,32),new Vector2(44,32)};
  Vector2[] shield={new Vector2(40,7),new Vector2(12,21),new Vector2(16,48),new Vector2(25,63),new Vector2(40,74),new Vector2(55,63),new Vector2(64,48),new Vector2(68,21)};
  for(int y=0;y<80;y++)for(int x=0;x<80;x++){float py=79-y;Color c=Color.clear;
   if(Disk(x,py,40,40,38))c=id==1?new Color(1,.85f,.66f):id==2?new Color(.77f,.94f,1):id==3?new Color(1,.8f,.86f):new Color(.9f,.83f,1);
   if(id==0){if((x>34&&x<46&&py>17&&py<44)||Disk(x,py,40,58,6))c=Berry;}
   if(id==1&&Polygon(x,py,bolt))c=new Color(1,.25f,.42f);
   if(id==2&&Polygon(x,py,shield)){c=new Color(.22f,.64f,.84f);if(Disk(x,py,33,33,9)||Disk(x,py,47,33,9)||(py>30&&py<54&&Mathf.Abs(x-40)<(54-py)*.8f))c=Color.white;}
   if(id==3&&Disk(x,py,40,42,27)){c=new Color(.36f,.15f,.09f);if(Disk(x,py,31,30,8))c=new Color(.8f,.49f,.28f);if(py>49&&py<56)c=new Color(.98f,.8f,.58f);}
   if(id==4&&(Disk(x,py,25,45,15)||Disk(x,py,40,32,20)||Disk(x,py,56,43,16)||(py>43&&py<59&&x>22&&x<60)))c=new Color(1,.97f,.91f);
   t.SetPixel(x,y,c);
  }t.Apply();return t;
 }
 Texture2D Round(){var t=new Texture2D(48,48,TextureFormat.RGBA32,false);for(int y=0;y<48;y++)for(int x=0;x<48;x++){float dx=Mathf.Max(10-x,x-37),dy=Mathf.Max(10-y,y-37);t.SetPixel(x,y,new Color(1,1,1,new Vector2(Mathf.Max(0,dx),Mathf.Max(0,dy)).magnitude<10?1:0));}t.Apply();return t;}
 void Box(Rect r,Color c){GUI.color=c;GUI.DrawTexture(r,panel);GUI.color=Color.white;}
 void Styles(){if(title!=null)return;title=new GUIStyle(GUI.skin.label){fontSize=20,fontStyle=FontStyle.Bold};title.normal.textColor=Ink;body=new GUIStyle(title){fontSize=16,fontStyle=FontStyle.Normal,wordWrap=true};badge=new GUIStyle(title){fontSize=17,alignment=TextAnchor.MiddleCenter};badge.normal.textColor=Color.white;alert=new GUIStyle(title){fontSize=24,alignment=TextAnchor.MiddleCenter,wordWrap=true};alert.normal.textColor=Color.white;}
 void OnGUI(){
  if(!game||!items||!panel||!game.IsRacing)return;Styles();Matrix4x4 old=GUI.matrix;Color color=GUI.color;int depth=GUI.depth;
  try{GUI.depth=-20;GUI.matrix=Matrix4x4.TRS(Vector3.zero,Quaternion.identity,new Vector3(Screen.width/1280f,Screen.height/720f,1));GUI.color=Color.white;
   Box(new Rect(26,505,350,139),Cream);Box(new Rect(36,515,72,72),new Color(1,.88f,.91f));GUI.DrawTexture(new Rect(36,515,72,72),icons[items.HeldId]);
   GUI.Label(new Rect(120,517,245,28),items.HeldName,title);GUI.Label(new Rect(120,549,240,44),RaceCandyItems.ItemHelp(items.HeldId),body);
   Box(new Rect(40,604,320,28),items.HeldId==0?new Color(.6f,.38f,.5f):Berry);GUI.Label(new Rect(40,604,320,28),items.HeldId==0?"CAJA ROSA ? = PODER ALEATORIO":"PULSA E PARA USARLO",badge);
   if(items.ShieldActive){Box(new Rect(26,453,350,43),new Color(.12f,.48f,.64f,.97f));GUI.Label(new Rect(36,458,330,30),"ESCUDO ACTIVO  /  "+items.ShieldSeconds.ToString("0.0")+" s",badge);}
   if(items.HasAlert){Box(new Rect(342,155,654,92),new Color(.45f,.08f,.25f,.96f));Box(new Rect(348,160,7,80),new Color(1,.68f,.27f));GUI.Label(new Rect(363,160,613,80),items.Alert,alert);}
  }finally{GUI.matrix=old;GUI.color=color;GUI.depth=depth;}
 }
 void OnDestroy(){if(icons!=null)foreach(var icon in icons)if(icon)Destroy(icon);if(panel)Destroy(panel);}
}
