using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

public class BuildScript
{
    public static void BuildWebGL()
    {
        string outputPath = "Builds/WebGL";
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        // Asegurar que la escena está cargada
        Scene current = EditorSceneManager.GetActiveScene();
        if (current.name != "CyberDrifter")
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CyberDrifter.unity");
        }

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = new[] { "Assets/Scenes/CyberDrifter.unity" };
        options.locationPathName = outputPath;
        options.target = BuildTarget.WebGL;
        options.targetGroup = BuildTargetGroup.WebGL;
        options.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("✅ BUILD WEBGL EXITOSO");
            Debug.Log("📁 Tamaño total: " + summary.totalSize + " bytes");
            Debug.Log("⏱️ Tiempo: " + summary.totalTime);
        }
        else
        {
            Debug.LogError("❌ BUILD FALLÓ: " + summary.result);
        }
    }
}
