using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
public class SceneBuilder
{
    [MenuItem("Cyber Drifter/Build Scene")]
    public static void BuildScene()
    {
        // Crear nueva escena
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        newScene.name = "CyberDrifter";

        // ===== CÁMARA =====
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        camObj.tag = "MainCamera";
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.02f, 0.0f, 0.05f); // negro violáceo
        cam.fieldOfView = 70f;
        camObj.transform.position = new Vector3(0, 5, -8);
        camObj.transform.rotation = Quaternion.Euler(20, 0, 0);
        CameraFollow camFollow = camObj.AddComponent<CameraFollow>();

        // Light
        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = new Color(0.8f, 0.6f, 1f);
        light.intensity = 1.2f;
        light.shadows = LightShadows.Soft;
        lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);

        // Luz ambiental
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.1f, 0.05f, 0.2f);
        RenderSettings.ambientEquatorColor = new Color(0.05f, 0.0f, 0.1f);
        RenderSettings.ambientGroundColor = new Color(0.02f, 0.0f, 0.05f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.05f, 0.0f, 0.1f);
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.025f;

        // ===== SUELO =====
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(3f, 1f, 30f);
        Renderer groundRend = ground.GetComponent<Renderer>();
        Material groundMat = new Material(Shader.Find("Standard"));
        groundMat.color = new Color(0.05f, 0.0f, 0.1f);
        groundMat.SetColor("_EmissionColor", new Color(0.2f, 0.0f, 0.4f) * 0.3f);
        groundMat.EnableKeyword("_EMISSION");
        groundRend.material = groundMat;
        ground.transform.position = new Vector3(0, 0, 50f);
        GroundScroller groundScroller = ground.AddComponent<GroundScroller>();

        // Líneas de neón decorativas en el suelo
        for (int i = 0; i < 6; i++)
        {
            GameObject lineObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lineObj.name = "NeonLine_" + i;
            lineObj.transform.localScale = new Vector3(0.05f, 0.05f, 60f);
            lineObj.transform.position = new Vector3(i == 0 ? -4f : (i == 5 ? 4f : (i - 2.5f) * 1.5f), 0.05f, 50f);
            Renderer lineRend = lineObj.GetComponent<Renderer>();
            Material lineMat = new Material(Shader.Find("Standard"));
            Color lineColor = (i == 0 || i == 5) ? new Color(0f, 1f, 1f) : new Color(1f, 0f, 1f);
            lineMat.color = lineColor;
            lineMat.SetColor("_EmissionColor", lineColor * 2f);
            lineMat.EnableKeyword("_EMISSION");
            lineRend.material = lineMat;
            lineObj.transform.SetParent(ground.transform);
        }

        // ===== JUGADOR =====
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag = "Player";
        player.transform.localScale = new Vector3(0.8f, 1f, 0.8f);
        player.transform.position = new Vector3(0, 1f, 0);
        Renderer playerRend = player.GetComponent<Renderer>();
        Material playerMat = new Material(Shader.Find("Standard"));
        playerMat.color = new Color(0.1f, 0.8f, 1f);
        playerMat.SetColor("_EmissionColor", new Color(0f, 1f, 1f) * 1.5f);
        playerMat.EnableKeyword("_EMISSION");
        playerMat.SetFloat("_Metallic", 0.8f);
        playerMat.SetFloat("_Glossiness", 0.9f);
        playerRend.material = playerMat;
        // Quitar collider default (usaremos trigger)
        UnityEngine.Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        PlayerController pc = player.AddComponent<PlayerController>();
        pc.rb = rb;

        // Trail
        TrailRenderer trail = player.AddComponent<TrailRenderer>();
        trail.time = 0.5f;
        trail.startWidth = 0.5f;
        trail.endWidth = 0.05f;
        trail.material = new Material(Shader.Find("Standard"));
        trail.material.color = new Color(0f, 1f, 1f);
        trail.material.SetColor("_EmissionColor", new Color(0f, 1f, 1f) * 3f);
        trail.material.EnableKeyword("_EMISSION");
        pc.trail = trail;

        // ===== SPAWN MANAGER =====
        GameObject spawnObj = new GameObject("SpawnManager");
        spawnObj.transform.position = Vector3.zero;
        SpawnManager sm = spawnObj.AddComponent<SpawnManager>();

        // Crear prefabs de obstáculos básicos (cubo)
        GameObject obstaclePrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obstaclePrefab.name = "ObstaclePrefab";
        obstaclePrefab.transform.localScale = new Vector3(1.5f, 2f, 1f);
        Renderer obsRend = obstaclePrefab.GetComponent<Renderer>();
        Material obsMat = new Material(Shader.Find("Standard"));
        obsMat.color = new Color(1f, 0.2f, 0.6f);
        obsMat.SetColor("_EmissionColor", new Color(1f, 0.2f, 0.6f) * 2f);
        obsMat.EnableKeyword("_EMISSION");
        obsRend.material = obsMat;
        // Hacerlo prefab y destruir la instancia
        string obstaclePath = "Assets/Prefabs/Obstacle.prefab";
        GameObject obstacleAsset = PrefabUtility.SaveAsPrefabAsset(obstaclePrefab, obstaclePath);
        UnityEngine.Object.DestroyImmediate(obstaclePrefab);

        // Crear prefab de orbe (esfera)
        GameObject orbPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        orbPrefab.name = "OrbPrefab";
        orbPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Renderer orbRend = orbPrefab.GetComponent<Renderer>();
        Material orbMat = new Material(Shader.Find("Standard"));
        orbMat.color = new Color(1f, 0.8f, 0f);
        orbMat.SetColor("_EmissionColor", new Color(1f, 0.8f, 0f) * 3f);
        orbMat.EnableKeyword("_EMISSION");
        orbRend.material = orbMat;
        UnityEngine.Object.DestroyImmediate(orbPrefab.GetComponent<SphereCollider>());
        SphereCollider orbCollider = orbPrefab.AddComponent<SphereCollider>();
        orbCollider.isTrigger = true;
        string orbPath = "Assets/Prefabs/Orb.prefab";
        GameObject orbAsset = PrefabUtility.SaveAsPrefabAsset(orbPrefab, orbPath);
        UnityEngine.Object.DestroyImmediate(orbPrefab);

        sm.obstaclePrefabs = new GameObject[] { obstacleAsset };
        sm.orbPrefab = orbAsset;

        // ===== BUILDING SPAWNER =====
        GameObject buildingObj = new GameObject("BuildingSpawner");
        buildingObj.transform.position = Vector3.zero;
        BuildingSpawner bs = buildingObj.AddComponent<BuildingSpawner>();
        GameObject buildingPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        buildingPrefab.name = "BuildingPrefab";
        Renderer bRend = buildingPrefab.GetComponent<Renderer>();
        Material bMat = new Material(Shader.Find("Standard"));
        bMat.color = new Color(0.05f, 0.0f, 0.1f);
        bMat.SetColor("_EmissionColor", new Color(0.3f, 0f, 0.6f) * 0.5f);
        bMat.EnableKeyword("_EMISSION");
        bRend.material = bMat;
        string buildingPath = "Assets/Prefabs/Building.prefab";
        GameObject buildingAsset = PrefabUtility.SaveAsPrefabAsset(buildingPrefab, buildingPath);
        UnityEngine.Object.DestroyImmediate(buildingPrefab);
        bs.buildingPrefabs = new GameObject[] { buildingAsset };

        // ===== GAME MANAGER =====
        GameManager gm = spawnObj.AddComponent<GameManager>();
        gm.player = pc;
        gm.spawnManager = sm;
        gm.ground = ground;

        camFollow.target = player.transform;
        pc.gm = gm;

        // ===== UI =====
        CreateUI(gm);

        // Guardar escena
        EditorSceneManager.SaveScene(newScene, "Assets/Scenes/CyberDrifter.unity");

        Debug.Log("✅ CYBER DRIFTER scene built!");
    }

    static void CreateUI(GameManager gm)
    {
        // Canvas
        GameObject canvasObj = new GameObject("UI_Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>().uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // EventSystem
        if (GameObject.Find("EventSystem") == null)
        {
            GameObject evtObj = new GameObject("EventSystem");
            evtObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            evtObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Panel Menú
        GameObject menuPanel = CreatePanel(canvasObj.transform, "MenuPanel", new Color(0.02f, 0.0f, 0.05f, 0.95f));
        CreateText(menuPanel.transform, "Title", "CYBER\nDRIFTER", 80, TextAnchor.MiddleCenter, new Vector2(0, 50));
        CreateText(menuPanel.transform, "Subtitle", "Tap or arrow keys to dodge", 24, TextAnchor.MiddleCenter, new Vector2(0, -30));
        CreateText(menuPanel.transform, "HighScore", "HI: 0", 24, TextAnchor.MiddleCenter, new Vector2(0, -90));
        Button playBtn = CreateButton(menuPanel.transform, "PlayButton", "▶  PLAY", new Vector2(0, -160));

        // Panel Gameplay (HUD)
        GameObject gameplayPanel = CreatePanel(canvasObj.transform, "GameplayPanel", new Color(0, 0, 0, 0));
        gameplayPanel.GetComponent<UnityEngine.UI.Image>().enabled = false;
        CreateText(gameplayPanel.transform, "Score", "0", 64, TextAnchor.UpperLeft, new Vector2(40, -30));

        // Panel Game Over
        GameObject gameOverPanel = CreatePanel(canvasObj.transform, "GameOverPanel", new Color(0.02f, 0.0f, 0.05f, 0.85f));
        gameOverPanel.SetActive(false);
        CreateText(gameOverPanel.transform, "GameOverTitle", "GAME OVER", 70, TextAnchor.MiddleCenter, new Vector2(0, 50));
        CreateText(gameOverPanel.transform, "ScoreText", "SCORE: 0", 36, TextAnchor.MiddleCenter, new Vector2(0, -30));
        CreateText(gameOverPanel.transform, "HighText", "HI: 0", 36, TextAnchor.MiddleCenter, new Vector2(0, -80));
        Button restartBtn = CreateButton(gameOverPanel.transform, "RestartButton", "↻  RESTART", new Vector2(0, -160));

        // UIManager
        UIManager ui = canvasObj.AddComponent<UIManager>();
        ui.gm = gm;
        ui.menuPanel = menuPanel;
        ui.gameplayPanel = gameplayPanel;
        ui.gameOverPanel = gameOverPanel;
        ui.playButton = playBtn;
        ui.restartButton = restartBtn;
        ui.scoreText = gameplayPanel.transform.Find("Score").GetComponent<Text>();
        ui.highScoreText = menuPanel.transform.Find("HighScore").GetComponent<Text>();
        ui.gameOverScoreText = gameOverPanel.transform.Find("ScoreText").GetComponent<Text>();
        ui.gameOverHighText = gameOverPanel.transform.Find("HighText").GetComponent<Text>();

        gm.ui = ui;
    }

    static GameObject CreatePanel(Transform parent, string name, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        UnityEngine.UI.Image img = panel.AddComponent<UnityEngine.UI.Image>();
        img.color = color;
        return panel;
    }

    static Text CreateText(Transform parent, string name, string text, int size, TextAnchor anchor, Vector2 pos)
    {
        GameObject txtObj = new GameObject(name);
        txtObj.transform.SetParent(parent);
        RectTransform rt = txtObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(800, 100);
        rt.anchoredPosition = pos;
        Text txt = txtObj.AddComponent<Text>();
        txt.text = text;
        txt.fontSize = size;
        txt.alignment = anchor;
        txt.color = new Color(0f, 1f, 1f);
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txtObj.AddComponent<Shadow>().effectColor = new Color(1f, 0f, 1f, 0.7f);
        txtObj.AddComponent<Outline>().effectColor = new Color(0.5f, 0f, 1f, 0.5f);
        return txt;
    }

    static Button CreateButton(Transform parent, string name, string label, Vector2 pos)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent);
        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(280, 70);
        rt.anchoredPosition = pos;
        UnityEngine.UI.Image img = btnObj.AddComponent<UnityEngine.UI.Image>();
        img.color = new Color(0f, 0.8f, 1f, 0.9f);
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;
        ColorBlock cb = btn.colors;
        cb.highlightedColor = new Color(0.5f, 1f, 1f, 1f);
        cb.pressedColor = new Color(1f, 0f, 1f, 1f);
        btn.colors = cb;

        Text btnText = CreateText(btnObj.transform, "Label", label, 32, TextAnchor.MiddleCenter, Vector2.zero);
        btnText.color = Color.black;
        btnText.fontStyle = FontStyle.Bold;

        return btn;
    }
}
#endif