using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
[InitializeOnLoad] public static class RaceBuild {
 static string request=Path.GetFullPath(Path.Combine(Application.dataPath,"../Temp/RaceBuild.request"));
 static RaceBuild(){EditorApplication.update+=Tick;}
 static void Tick(){if(EditorApplication.isPlayingOrWillChangePlaymode||EditorApplication.isCompiling||EditorApplication.isUpdating||!File.Exists(request))return;string output=File.ReadAllText(request).Trim();File.Delete(request);Directory.CreateDirectory(output);try{
  var options=new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/RacePrototype.unity"},locationPathName=Path.Combine(output,"Proto3D.exe"),target=BuildTarget.StandaloneWindows64,options=BuildOptions.None};
  var report=BuildPipeline.BuildPlayer(options);File.WriteAllText(Path.Combine(output,"build-result.txt"),"Result="+report.summary.result+"\nErrors="+report.summary.totalErrors+"\nWarnings="+report.summary.totalWarnings+"\nBytes="+report.summary.totalSize+"\nDuration="+report.summary.totalTime);
 }catch(Exception ex){File.WriteAllText(Path.Combine(output,"build-error.txt"),ex.ToString());}}
}
