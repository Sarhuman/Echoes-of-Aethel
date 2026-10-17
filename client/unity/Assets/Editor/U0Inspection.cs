using System;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class U0Inspection
{
    private const string ScenePath = "Assets/Scenes/U0_Inspection.unity";

    public static void CreateSceneAndBuild()
    {
        PlayerSettings.companyName = "Sarhuman";
        PlayerSettings.productName = "Echoes of Aethel";
        CreateScene();

        var output = "Build/EchoesOfAethel-U0.exe";
        var options = new BuildPlayerOptions
        {
            scenes = new[] { ScenePath },
            locationPathName = output,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };
        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new Exception("U0 Windows inspection build failed: " + report.summary.result);
        }

        Debug.Log("U0 Windows inspection build succeeded: " + output);
    }

    private static void CreateScene()
    {
        System.IO.Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Inspection Floor";
        floor.transform.localScale = new Vector3(2f, 1f, 2f);

        var marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        marker.name = "Scale Marker (2 m)";
        marker.transform.position = new Vector3(0f, 1f, 0f);
        marker.transform.localScale = new Vector3(1f, 2f, 1f);

        var cameraObject = new GameObject("Inspection Camera");
        var camera = cameraObject.AddComponent<Camera>();
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(9f, 8f, -9f);
        cameraObject.transform.LookAt(new Vector3(0f, 0.5f, 0f));
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.07f, 0.08f, 0.12f);

        var lightObject = new GameObject("Inspection Light");
        var light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
        lightObject.transform.rotation = Quaternion.Euler(45f, -35f, 0f);

        new GameObject("Runtime Diagnostics").AddComponent<U0RuntimeDiagnostics>();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        Debug.Log("U0 inspection scene saved: " + ScenePath);
    }
}
