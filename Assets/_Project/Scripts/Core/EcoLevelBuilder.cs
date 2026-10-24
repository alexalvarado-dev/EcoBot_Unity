using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// Construye la escena GameScene: Tilemap con Composite Collider, jugador, residuos, enemigos, meta, cámara y HUD.
public static class EcoLevelBuilder
{
    public const int Width = 93;

    static Tilemap ground, water;

    public static void Build()
    {
        Time.timeScale = 1f;
        EcoAssets.Load();
        var cam = EcoBootstrap.EnsureCamera();
        EcoUI.EnsureEventSystem();
        EcoAudio.StartMusic();

        var gm = new GameObject("GameManager").AddComponent<EcoGame>();

        BuildTilemaps();
        BuildHazardsAndCheckpoints();
        var player = BuildPlayer();
        gm.player = player;
        BuildCollectibles(gm);
        BuildEnemies();
        BuildBin();
        SetupCamera(cam, player);
        BuildHud(gm);
    }

    // ---------------- Tilemap (Grid + Tilemap + TilemapCollider2D + CompositeCollider2D) ----------------
    static Tilemap MakeMap(Transform grid, string name, int order, bool solid)
    {
        var go = new GameObject(name);
        go.transform.SetParent(grid, false);
        var tm = go.AddComponent<Tilemap>();
        var tr = go.AddComponent<TilemapRenderer>();
        tr.sortingOrder = order;
        if (solid)
        {
            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;           // ajuste vital: Static
            var tc = go.AddComponent<TilemapCollider2D>();
            go.AddComponent<CompositeCollider2D>();
            tc.compositeOperation = Collider2D.CompositeOperation.Merge;
        }
        return tm;
    }

    static void T(Tilemap m, Tile t, int x, int y) { m.SetTile(new Vector3Int(x, y, 0), t); }

    static void BuildTilemaps()
    {
        var gridGo = new GameObject("Grid");
        gridGo.AddComponent<Grid>().cellSize = Vector3.one;
        ground = MakeMap(gridGo.transform, "Tilemap_Ground", 0, true);
        water = MakeMap(gridGo.transform, "Tilemap_Water", 1, false);

        int[,] segs = { { 0, 23 }, { 28, 50 }, { 55, Width - 1 } };
        for (int s = 0; s < segs.GetLength(0); s++)
            for (int x = segs[s, 0]; x <= segs[s, 1]; x++)
            {
                T(ground, EcoAssets.Grass, x, -1);
                for (int y = -4; y <= -2; y++) T(ground, EcoAssets.Dirt, x, y);
            }

        // Muros laterales
        for (int y = -4; y <= 13; y++) { T(ground, EcoAssets.Dirt, -1, y); T(ground, EcoAssets.Dirt, Width, y); }

        // Agua contaminada en los fosos
        int[,] pits = { { 24, 27 }, { 51, 54 } };
        for (int p = 0; p < pits.GetLength(0); p++)
            for (int x = pits[p, 0]; x <= pits[p, 1]; x++) { T(water, EcoAssets.Water, x, -3); T(water, EcoAssets.Water, x, -4); }

        // Plataformas metálicas (x izquierdo, y de celda, ancho)
        int[,] plats =
        {
            { 6, 2, 3 }, { 11, 4, 3 }, { 16, 2, 3 },
            { 31, 2, 3 }, { 36, 4, 3 }, { 41, 2, 3 }, { 46, 4, 3 },
            { 52, 0, 2 },
            { 59, 2, 3 }, { 64, 4, 3 }, { 69, 6, 3 }, { 74, 4, 3 }, { 79, 2, 3 }
        };
        for (int i = 0; i < plats.GetLength(0); i++)
            for (int dx = 0; dx < plats[i, 2]; dx++) T(ground, EcoAssets.Metal, plats[i, 0] + dx, plats[i, 1]);
    }

    static void BuildHazardsAndCheckpoints()
    {
        var root = new GameObject("Hazards");
        float[,] pits = { { 24, 27 }, { 51, 54 } };
        for (int i = 0; i < pits.GetLength(0); i++)
        {
            var h = new GameObject("Agua" + (i + 1));
            h.transform.SetParent(root.transform);
            h.transform.position = new Vector3((pits[i, 0] + pits[i, 1] + 1) * 0.5f, -3f, 0);
            var bc = h.AddComponent<BoxCollider2D>();
            bc.isTrigger = true;
            bc.size = new Vector2(pits[i, 1] - pits[i, 0] + 1, 2.5f);
            h.AddComponent<EcoHazard>();
        }
        float[] cps = { 29f, 56f };
        for (int i = 0; i < cps.Length; i++)
        {
            var c = new GameObject("Checkpoint" + (i + 1));
            c.transform.SetParent(root.transform);
            c.transform.position = new Vector3(cps[i], 0.4f, 0);
            var bc = c.AddComponent<BoxCollider2D>();
            bc.isTrigger = true; bc.size = new Vector2(1.5f, 20f);
            c.AddComponent<EcoCheckpoint>();
        }
    }

    // ---------------- Jugador ----------------
    static EcoPlayer BuildPlayer()
    {
        var p = new GameObject("Player");
        p.transform.position = new Vector3(2f, 0.5f, 0f);
        var sr = p.AddComponent<SpriteRenderer>();
        sr.sprite = EcoAssets.BotIdle[0];
        sr.sortingOrder = 5;

        var rb = p.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.freezeRotation = true;                                   // Freeze Rotation Z
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        var col = p.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.8f, 1.6f);
        col.offset = new Vector2(0f, 1.05f);
        col.sharedMaterial = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };

        var gc = new GameObject("GroundCheck");
        gc.transform.SetParent(p.transform, false);
        gc.transform.localPosition = new Vector3(0f, 0.2f, 0f);

        var an = p.AddComponent<EcoSpriteAnimator>();
        an.idle = EcoAssets.BotIdle; an.walk = EcoAssets.BotWalk; an.jump = EcoAssets.BotJump;

        var pc = p.AddComponent<EcoPlayer>();
        pc.groundCheck = gc.transform;
        return pc;
    }

    // ---------------- Residuos ----------------
    static void BuildCollectibles(EcoGame gm)
    {
        var root = new GameObject("Residuos");
        // x, y, tipo
        float[,] items =
        {
            { 7.5f, 3.8f, 0 }, { 12.5f, 5.8f, 1 }, { 17.5f, 3.8f, 2 }, { 22f, 0.8f, 3 },
            { 32.5f, 3.8f, 4 }, { 37.5f, 5.8f, 5 }, { 42.5f, 3.8f, 0 }, { 47.5f, 5.8f, 1 },
            { 60.5f, 3.8f, 2 }, { 65.5f, 5.8f, 3 }, { 70.5f, 7.8f, 4 }, { 75.5f, 5.8f, 5 }
        };
        for (int i = 0; i < items.GetLength(0); i++)
        {
            int type = (int)items[i, 2];
            var go = new GameObject("Residuo" + (i + 1) + "_" + EcoAssets.ItemNames[type]);
            go.transform.SetParent(root.transform);
            go.transform.position = new Vector3(items[i, 0], items[i, 1], 0);
            go.transform.localScale = Vector3.one * 1.3f;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = EcoAssets.Items[type];
            sr.sortingOrder = 3;
            var cc = go.AddComponent<CircleCollider2D>();
            cc.isTrigger = true; cc.radius = 0.55f;
            go.AddComponent<EcoCollectible>().type = type;
        }
        gm.total = items.GetLength(0);
    }

    // ---------------- Enemigos ----------------
    static void BuildEnemies()
    {
        var root = new GameObject("Enemigos");
        float[,] e = { { 12, 9, 21 }, { 34, 30, 45 }, { 62, 58, 72 }, { 83, 81, 88 } };
        for (int i = 0; i < e.GetLength(0); i++)
        {
            var go = new GameObject("Slime" + (i + 1));
            go.transform.SetParent(root.transform);
            go.transform.position = new Vector3(e[i, 0], 0f, 0f);
            go.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = EcoAssets.Slime[0];
            sr.sortingOrder = 4;
            var an = go.AddComponent<EcoSpriteAnimator>();
            an.idle = EcoAssets.Slime; an.idleFps = 3f;
            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            var bc = go.AddComponent<BoxCollider2D>();
            bc.isTrigger = true; bc.size = new Vector2(0.8f, 0.6f); bc.offset = new Vector2(0, 0.35f);
            var en = go.AddComponent<EcoEnemy>();
            en.leftX = e[i, 1]; en.rightX = e[i, 2];
        }
    }

    // ---------------- Caneca (meta) ----------------
    static void BuildBin()
    {
        var go = new GameObject("Caneca");
        go.transform.position = new Vector3(88.5f, 0f, 0f);
        go.transform.localScale = new Vector3(1.6f, 1.6f, 1f);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = EcoAssets.Bin[0];
        sr.sortingOrder = 2;
        var bc = go.AddComponent<BoxCollider2D>();
        bc.isTrigger = true; bc.size = new Vector2(1.4f, 1.1f); bc.offset = new Vector2(0, 0.6f);
        go.AddComponent<EcoBin>();
    }

    // ---------------- Cámara ----------------
    static void SetupCamera(Camera cam, EcoPlayer player)
    {
        var follow = cam.GetComponent<EcoCameraFollow2D>();
        if (follow == null) follow = cam.gameObject.AddComponent<EcoCameraFollow2D>();
        follow.target = player.transform;
        follow.minX = -1f; follow.maxX = Width + 1f;
        follow.Snap();

        var bg = new GameObject("Fondo");
        bg.transform.SetParent(cam.transform, false);
        bg.transform.localPosition = new Vector3(0, 0, 20f);
        var sr = bg.AddComponent<SpriteRenderer>();
        sr.sprite = EcoAssets.Background;
        sr.sortingOrder = -100;
        bg.AddComponent<EcoBackground>();
    }

    // ---------------- HUD ----------------
    static void BuildHud(EcoGame gm)
    {
        var canvas = EcoUI.CreateCanvas("Canvas");
        var topLeft = new Vector2(0, 1);

        gm.heartFull = EcoAssets.Items[6];
        gm.heartEmpty = EcoAssets.Items[7];
        gm.hearts = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            var h = EcoUI.MakeImage(canvas, "Heart" + (i + 1), gm.heartFull, Color.white);
            EcoUI.Place(h.rectTransform, topLeft, new Vector2(30 + i * 90, -24), new Vector2(80, 80));
            h.raycastTarget = false;
            gm.hearts[i] = h;
        }

        gm.scoreText = EcoUI.MakeLabel(canvas, "ScoreText", "", 54, Color.white, TextAnchor.MiddleCenter);
        EcoUI.Place(gm.scoreText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0, -24), new Vector2(800, 80));
        gm.timeText = EcoUI.MakeLabel(canvas, "TimeText", "", 46, Color.white, TextAnchor.MiddleRight);
        EcoUI.Place(gm.timeText.rectTransform, new Vector2(1f, 1f), new Vector2(-30, -30), new Vector2(500, 70));
        var help = EcoUI.MakeLabel(canvas, "HelpText", "A/D o flechas: mover   |   Espacio: saltar   |   Esc: pausa   |   Salta sobre los slimes", 32, new Color(1, 1, 1, 0.85f), TextAnchor.MiddleCenter);
        EcoUI.Place(help.rectTransform, new Vector2(0.5f, 0f), new Vector2(0, 14), new Vector2(1800, 50));

        // Panel de datos educativos (aparece al recoger un residuo)
        var fact = EcoUI.MakeImage(canvas, "FactPanel", null, new Color(0.02f, 0.12f, 0.08f, 0.85f));
        EcoUI.Place(fact.rectTransform, new Vector2(0.5f, 0f), new Vector2(0, 80), new Vector2(1500, 150));
        fact.raycastTarget = false;
        gm.factText = EcoUI.MakeLabel(fact.transform, "FactText", "", 38, Color.white, TextAnchor.MiddleCenter);
        EcoUI.Stretch(gm.factText.rectTransform);
        gm.factText.rectTransform.offsetMin = new Vector2(30, 10); gm.factText.rectTransform.offsetMax = new Vector2(-30, -10);
        gm.factPanel = fact.gameObject;
        gm.factPanel.SetActive(false);

        // Pausa
        gm.pausePanel = MakePanel(canvas, "PausePanel", "PAUSA", out _, out _);
        EcoUI.MakeButton(gm.pausePanel.transform, "Btn_Resume", "CONTINUAR", new Vector2(0, 20), new Vector2(560, 110), gm.OnResume, 56);
        EcoUI.MakeButton(gm.pausePanel.transform, "Btn_Menu", "MENÚ PRINCIPAL", new Vector2(0, -130), new Vector2(560, 110), gm.OnMenu, 50);
        gm.pausePanel.SetActive(false);

        // Fin de partida
        gm.endPanel = MakePanel(canvas, "EndPanel", "", out gm.endTitle, out gm.endBody);
        EcoUI.MakeButton(gm.endPanel.transform, "Btn_Retry", "REINTENTAR", new Vector2(-300, -330), new Vector2(520, 110), gm.OnRetry, 52);
        EcoUI.MakeButton(gm.endPanel.transform, "Btn_EndMenu", "MENÚ", new Vector2(300, -330), new Vector2(520, 110), gm.OnMenu, 52);
        gm.endPanel.SetActive(false);
    }

    static GameObject MakePanel(Transform canvas, string name, string title, out Text titleText, out Text bodyText)
    {
        var p = EcoUI.MakeImage(canvas, name, null, new Color(0, 0, 0, 0.8f));
        EcoUI.Stretch(p.rectTransform);
        titleText = EcoUI.MakeLabel(p.transform, "Title", title, 90, new Color(0.7f, 1f, 0.75f), TextAnchor.MiddleCenter);
        EcoUI.Place(titleText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0, 330), new Vector2(1600, 130));
        bodyText = EcoUI.MakeLabel(p.transform, "Body", "", 48, Color.white, TextAnchor.UpperCenter);
        EcoUI.Place(bodyText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0, 90), new Vector2(1500, 400));
        return p.gameObject;
    }
}
