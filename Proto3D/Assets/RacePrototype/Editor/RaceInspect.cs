using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Text;
 public static class RaceInspect {

 [MenuItem("Race Prototype/Inspeccionar escena") ]
 static void Inspect(){
 if(EditorApplication.isPlayingOrWillChangePlaymode) return;
 var s=EditorSceneManager.GetActiveScene(); var b=new StringBuilder(s.path+"\n");
 foreach(var g in s.GetRootGameObjects()) { b.AppendLine("ROOT "+g.name+" "+g.transform.position+" "+g.transform.eulerAngles+" active="+g.activeSelf);
 foreach(var r in g.GetComponentsInChildren<Renderer>(true)) if(g.name.Contains("oval")||r.name.Contains("start")) b.AppendLine(r.name+" center="+r.bounds.center+" size="+r.bounds.size); }
 File.WriteAllText(Path.GetFullPath(Path.Combine(Application.dataPath,"../Temp/scene-inspect.txt")),b.ToString());
 }
}
