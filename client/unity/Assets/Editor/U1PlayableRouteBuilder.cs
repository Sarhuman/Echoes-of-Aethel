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
        RenderSettings.fogColor = new Color(0.13f, 0.15f, 0.20f);
        RenderSettings.fogDensity = 0.012f;

        Material stone = Material("U1_Stone", new Color(0.28f, 0.30f, 0.34f));
        Material forest = Material("U1_Forest", new Color(0.15f, 0.22f, 0.19f));
        Material wall = Material("U1_Wall", new Color(0.15f, 0.16f, 0.21f));
        Material wood = Material("U1_Wood", new Color(0.21f, 0.15f, 0.13f));
        Material leaves = Material("U1_Leaves", new Color(0.12f, 0.20f, 0.17f));
        Material iron = Material("U1_Iron", new Color(0.33f, 0.37f, 0.44f));
        Material blue = Material("U1_Ether", new Color(0.25f, 0.71f, 0.91f));
        Material amber = Material("U1_Amber", new Color(1f, 0.46f, 0.12f));
        Material paving = Material("U1_Paving", new Color(0.39f, 0.39f, 0.42f));
        Material trail = Material("U1_Trail", new Color(0.32f, 0.29f, 0.25f));
        Material cloak = Material("U1_Cloak", new Color(0.18f, 0.19f, 0.29f));
        Material gold = Material("U1_Gold", new Color(0.69f, 0.49f, 0.22f));
        Material shadow = Material("U1_Shadow", new Color(0.055f, 0.065f, 0.09f));

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

        // Sparse, collider-free landmarks keep the route readable without adding path traps.
        for (int row = 0; row < 8; row++)
        {
            float z = -22f + row * 2.25f;
            for (int side = -1; side <= 1; side += 2)
                Decoration("Laje da praça", PrimitiveType.Cube,
                    new Vector3(side * 1.35f, 0.016f, z), new Vector3(2.35f, 0.028f, 1.8f), paving);
        }
        for (int step = 0; step < 7; step++)
        {
            float z = -2f + step * 3.35f;
            float x = Mathf.Sin(step * 0.7f) * 0.7f;
            Decoration("Trilha de terra", PrimitiveType.Cube,
                new Vector3(x, 0.012f, z), new Vector3(4.7f, 0.02f, 2.8f), trail);
        }
        Decoration("Estandarte oeste", PrimitiveType.Cube, new Vector3(-5.3f, 2.2f, -3.4f),
            new Vector3(0.8f, 1.7f, 0.09f), cloak);
        Decoration("Estandarte leste", PrimitiveType.Cube, new Vector3(5.3f, 2.2f, -3.4f),
            new Vector3(0.8f, 1.7f, 0.09f), cloak);

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
            GameObject branch = Decoration("Galho seco", PrimitiveType.Cube,
                p + new Vector3(0.7f, 3.2f, 0f), new Vector3(1.7f, 0.18f, 0.18f), wood);
            branch.transform.rotation = Quaternion.Euler(0f, 0f, 28f);
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
        Decoration("Sombra · Bastião", PrimitiveType.Cylinder, new Vector3(0f, -0.96f, 0f),
            new Vector3(0.7f, 0.01f, 0.55f), shadow, player.transform);
        Shape("Manto", PrimitiveType.Cube, new Vector3(0f, -0.24f, -0.27f),
            new Vector3(0.65f, 1.1f, 0.12f), cloak, false, playerVisual.transform);
        Shape("Armadura", PrimitiveType.Capsule, Vector3.zero, new Vector3(0.8f, 0.9f, 0.8f), iron, false, playerVisual.transform);
        Shape("Elmo", PrimitiveType.Sphere, new Vector3(0f, 0.98f, 0.02f),
            new Vector3(0.52f, 0.48f, 0.52f), iron, false, playerVisual.transform);
        Shape("Viseira", PrimitiveType.Cube, new Vector3(0f, 0.96f, 0.29f),
            new Vector3(0.4f, 0.10f, 0.09f), gold, false, playerVisual.transform);
        foreach (float side in new[] { -1f, 1f })
            Shape("Ombreira", PrimitiveType.Sphere, new Vector3(side * 0.47f, 0.54f, 0f),
                new Vector3(0.44f, 0.24f, 0.50f), iron, false, playerVisual.transform);
        Shape("Escudo", PrimitiveType.Cube, new Vector3(-0.55f, 0.1f, 0.35f),
            new Vector3(0.12f, 0.95f, 0.75f), wall, false, playerVisual.transform);
        Shape("Emblema do escudo", PrimitiveType.Cube, new Vector3(-0.625f, 0.12f, 0.35f),
            new Vector3(0.035f, 0.48f, 0.32f), gold, false, playerVisual.transform);
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
        Decoration("Sombra · lobo", PrimitiveType.Cylinder, new Vector3(0f, -0.66f, 0f),
            new Vector3(0.85f, 0.01f, 1.1f), shadow, wolf.transform);
        GameObject wolfVisual = Shape("Tronco etéreo", PrimitiveType.Capsule, Vector3.zero,
            new Vector3(0.85f, 0.6f, 0.85f), blue, false, wolf.transform);
        wolfVisual.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        Shape("Cabeça do lobo", PrimitiveType.Cube, new Vector3(0f, 0.12f, 0.92f),
            new Vector3(0.58f, 0.43f, 0.64f), blue, false, wolf.transform);
        Shape("Focinho", PrimitiveType.Cube, new Vector3(0f, -0.02f, 1.33f),
            new Vector3(0.30f, 0.20f, 0.35f), blue, false, wolf.transform);
        foreach (float side in new[] { -1f, 1f })
        {
            Shape("Orelha", PrimitiveType.Cube, new Vector3(side * 0.23f, 0.47f, 0.94f),
                new Vector3(0.18f, 0.35f, 0.16f), blue, false, wolf.transform);
            for (int front = -1; front <= 1; front += 2)
                Shape("Pata", PrimitiveType.Cube, new Vector3(side * 0.32f, -0.35f, front * 0.48f),
                    new Vector3(0.18f, 0.48f, 0.20f), blue, false, wolf.transform);
        }
        Shape("Cauda", PrimitiveType.Cube, new Vector3(0f, 0.24f, -0.98f),
            new Vector3(0.20f, 0.22f, 0.72f), blue, false, wolf.transform);
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

        int rendererCount = UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None).Length;
        int lightCount = UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None).Length;
        if (rendererCount > 120 || lightCount > 5)
            throw new Exception($"U1 visual budget exceeded: {rendererCount} renderers, {lightCount} lights");
        Debug.Log($"AETHEL_U1_VISUAL_BUDGET renderers={rendererCount} lights={lightCount}");

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        Debug.Log("AETHEL_U1_SCENE_OK " + ScenePath);
    }

    private static Material Material(string name, Color color)
    {
        string path = MaterialFolder + "/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null)
        {
            if (material.color != color)
            {
                material.color = color;
                EditorUtility.SetDirty(material);
            }
            return material;
        }
        material = new Material(Shader.Find("Standard"));
        material.color = color;
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static GameObject Block(string name, Vector3 position, Vector3 size, Material material)
    {
        return Shape(name, PrimitiveType.Cube, position, size, material, true);
    }

    private static GameObject Decoration(string name, PrimitiveType type, Vector3 position,
        Vector3 size, Material material, Transform parent = null)
    {
        GameObject shape = Shape(name, type, position, size, material, false, parent);
        shape.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        return shape;
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
