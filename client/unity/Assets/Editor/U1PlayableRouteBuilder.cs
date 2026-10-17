using System;
using Aethel.Campaign;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class U1PlayableRouteBuilder
{
    private const string ScenePath = "Assets/Scenes/U1_PlayableRoute.unity";
    private const string MaterialFolder = "Assets/Materials";

    public static void CreateSceneAndBuild()
    {
        if (CombatMath.DirectDamage(42, 0, false, 15000, 5, 0, 0) != 40)
            throw new Exception("Domain combat damage changed");

        CreateScene();
        var options = new BuildPlayerOptions
        {
            scenes = new[] { ScenePath },
            locationPathName = "Build/EchoesOfAethel-U1.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };
        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("U1 Windows build failed: " + report.summary.result);
        Debug.Log("AETHEL_U1_BUILD_OK Build/EchoesOfAethel-U1.exe");
    }

    private static void CreateScene()
    {
        if (!AssetDatabase.IsValidFolder(MaterialFolder)) AssetDatabase.CreateFolder("Assets", "Materials");
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientLight = new Color(0.24f, 0.25f, 0.32f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.12f, 0.12f, 0.19f);
        RenderSettings.fogDensity = 0.018f;

        Material stone = Material("U1_Stone", new Color(0.28f, 0.30f, 0.34f));
        Material forest = Material("U1_Forest", new Color(0.15f, 0.22f, 0.19f));
        Material wall = Material("U1_Wall", new Color(0.15f, 0.16f, 0.21f));
        Material wood = Material("U1_Wood", new Color(0.21f, 0.15f, 0.13f));
        Material leaves = Material("U1_Leaves", new Color(0.12f, 0.20f, 0.17f));
        Material iron = Material("U1_Iron", new Color(0.33f, 0.37f, 0.44f));
        Material blue = Material("U1_Ether", new Color(0.25f, 0.71f, 0.91f));
        Material amber = Material("U1_Amber", new Color(1f, 0.46f, 0.12f));

        Block("Cidadela · praça", new Vector3(0f, -0.25f, -15f), new Vector3(24f, 0.5f, 20f), stone);
        Block("Portão · passagem", new Vector3(0f, -0.25f, -4f), new Vector3(7f, 0.5f, 3f), stone);
        Block("Bosque · rota", new Vector3(0f, -0.25f, 10f), new Vector3(24f, 0.5f, 28f), forest);
        Block("Muralha oeste", new Vector3(-12.4f, 1.4f, -15f), new Vector3(0.8f, 2.8f, 21f), wall);
        Block("Muralha leste", new Vector3(12.4f, 1.4f, -15f), new Vector3(0.8f, 2.8f, 21f), wall);
        Block("Muralha sul", new Vector3(0f, 1.4f, -25.4f), new Vector3(25f, 2.8f, 0.8f), wall);
        Block("Bosque · limite oeste", new Vector3(-12.4f, 1f, 10f), new Vector3(0.8f, 2f, 29f), wall);
        Block("Bosque · limite leste", new Vector3(12.4f, 1f, 10f), new Vector3(0.8f, 2f, 29f), wall);
        Block("Bosque · limite norte", new Vector3(0f, 1f, 24.4f), new Vector3(25f, 2f, 0.8f), wall);
        Block("Portão oeste", new Vector3(-7.7f, 1.4f, -4f), new Vector3(8.5f, 2.8f, 0.8f), wall);
        Block("Portão leste", new Vector3(7.7f, 1.4f, -4f), new Vector3(8.5f, 2.8f, 0.8f), wall);
        Block("Arco do portão", new Vector3(0f, 3.7f, -4f), new Vector3(7.4f, 0.8f, 1f), wall);

        foreach (Vector3 p in new[] {
            new Vector3(-8f, 0f, -19f), new Vector3(8f, 0f, -19f),
            new Vector3(-8f, 0f, -8f), new Vector3(8f, 0f, -8f) })
        {
            Block("Coluna da Cidadela", p + Vector3.up * 1.4f, new Vector3(1f, 2.8f, 1f), wall);
            Block("Braseiro", p + Vector3.up * 2.95f, new Vector3(0.8f, 0.3f, 0.8f), iron);
            GameObject flame = Shape("Chama", PrimitiveType.Sphere, p + Vector3.up * 3.3f,
                new Vector3(0.35f, 0.55f, 0.35f), amber, false);
            Light fire = flame.AddComponent<Light>();
            fire.type = LightType.Point; fire.range = 6f; fire.intensity = 2f;
            fire.color = new Color(1f, 0.48f, 0.2f);
        }

        foreach (Vector3 p in new[] {
            new Vector3(-8f, 0f, 1f), new Vector3(8f, 0f, 2f),
            new Vector3(-8f, 0f, 10f), new Vector3(8f, 0f, 13f),
            new Vector3(-6f, 0f, 20f), new Vector3(7f, 0f, 21f) })
        {
            Shape("Árvore morta · tronco", PrimitiveType.Cylinder, p + Vector3.up * 1.8f,
                new Vector3(0.65f, 1.8f, 0.65f), wood, true);
            Shape("Copa escura", PrimitiveType.Sphere, p + Vector3.up * 3.9f,
                new Vector3(2.8f, 2.1f, 2.8f), leaves, false);
        }

        var sunlight = new GameObject("Luar").AddComponent<Light>();
        sunlight.type = LightType.Directional;
        sunlight.color = new Color(0.58f, 0.66f, 0.96f);
        sunlight.intensity = 0.8f;
        sunlight.transform.rotation = Quaternion.Euler(48f, -25f, 0f);

        var player = new GameObject("Bastião · ator técnico");
        player.transform.position = new Vector3(0f, 1f, -15f);
        CharacterController playerCollider = player.AddComponent<CharacterController>();
        playerCollider.radius = 0.42f; playerCollider.height = 1.8f;
        playerCollider.stepOffset = 0.25f;
        U1Player playerScript = player.AddComponent<U1Player>();
        var playerVisual = new GameObject("Visual provisório");
        playerVisual.transform.SetParent(player.transform, false);
        Shape("Armadura", PrimitiveType.Capsule, Vector3.zero, new Vector3(0.8f, 0.9f, 0.8f), iron, false, playerVisual.transform);
        Shape("Escudo", PrimitiveType.Cube, new Vector3(-0.55f, 0.1f, 0.35f),
            new Vector3(0.12f, 0.95f, 0.75f), wall, false, playerVisual.transform);
        Shape("Machado · cabo", PrimitiveType.Cube, new Vector3(0.55f, 0f, 0.3f),
            new Vector3(0.1f, 1.1f, 0.1f), wood, false, playerVisual.transform);
        Shape("Machado · lâmina", PrimitiveType.Cube, new Vector3(0.55f, 0.5f, 0.3f),
            new Vector3(0.7f, 0.34f, 0.1f), iron, false, playerVisual.transform);
        playerScript.Visual = playerVisual.transform;

        var wolf = new GameObject("Lobo Etéreo · ator técnico");
        wolf.transform.position = new Vector3(0f, 0.7f, 8f);
        CharacterController wolfCollider = wolf.AddComponent<CharacterController>();
        wolfCollider.radius = 0.5f; wolfCollider.height = 1.3f;
        wolfCollider.stepOffset = 0.2f;
        U1Enemy wolfScript = wolf.AddComponent<U1Enemy>();
        GameObject wolfVisual = Shape("Silhueta etérea", PrimitiveType.Capsule, Vector3.zero,
            new Vector3(0.9f, 0.65f, 1.3f), blue, false, wolf.transform);
        wolfScript.Visual = wolfVisual.GetComponent<Renderer>();
        wolfScript.Player = playerScript;
        playerScript.Enemy = wolfScript;

        var cameraObject = new GameObject("Câmera isométrica");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 8.5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.08f, 0.09f, 0.15f);
        U1Camera follow = cameraObject.AddComponent<U1Camera>();
        follow.Target = player.transform;
        cameraObject.transform.position = player.transform.position + follow.Offset;
        cameraObject.transform.LookAt(player.transform.position + Vector3.up * 0.7f);
        playerScript.ViewCamera = camera;

        new GameObject("Diagnóstico de inicialização U1").AddComponent<U1RuntimeSmoke>();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        Debug.Log("AETHEL_U1_SCENE_OK " + ScenePath);
    }

    private static Material Material(string name, Color color)
    {
        string path = MaterialFolder + "/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null) return material;
        material = new Material(Shader.Find("Standard"));
        material.color = color;
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static GameObject Block(string name, Vector3 position, Vector3 size, Material material)
    {
        return Shape(name, PrimitiveType.Cube, position, size, material, true);
    }

    private static GameObject Shape(string name, PrimitiveType type, Vector3 position, Vector3 size,
        Material material, bool collision, Transform parent = null)
    {
        GameObject shape = GameObject.CreatePrimitive(type);
        shape.name = name;
        if (parent != null) shape.transform.SetParent(parent, false);
        shape.transform.localPosition = position;
        shape.transform.localScale = size;
        shape.GetComponent<Renderer>().sharedMaterial = material;
        if (!collision) UnityEngine.Object.DestroyImmediate(shape.GetComponent<Collider>());
        return shape;
    }
}
