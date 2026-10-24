using UnityEngine;
using UnityEngine.UI;

// Construye la escena MainMenu: Canvas, fondo, título, 3 botones y panel de créditos.
public static class EcoMenuBuilder
{
    const string CreditsText =
        "============================================\n" +
        "ECOBOT: MISIÓN RECICLAJE\n" +
        "Versión Prototipo - 2026\n" +
        "============================================\n" +
        "1. PROPÓSITO EDUCATIVO Y COMPETENCIAS\n" +
        "- Área: Educación ambiental (separación y reciclaje de residuos).\n" +
        "- Competencia: identifica residuos aprovechables, aprende datos sobre su impacto\n" +
        "  y practica la separación en la fuente.\n\n" +
        "2. EQUIPO DE DESARROLLO\n" +
        "- Diseño y lógica de juego: Alexander Alvarado y Reyner Acosta\n" +
        "- Programación e integración: Alexander Alvarado y Reyner Acosta\n" +
        "- Arte, animación e interfaces: Alexander Alvarado y Reyner Acosta\n\n" +
        "3. FUENTES Y REFERENCIAS ACADÉMICAS\n" +
        "- [Referencia 1: guía oficial de separación en la fuente de tu país o ciudad]\n" +
        "- [Referencia 2: documento oficial o enlace de consulta]\n\n" +
        "4. ATRIBUCIÓN DE RECURSOS DE TERCEROS\n" +
        "- Arte, música y sonidos: generados por código para este prototipo.\n\n" +
        "AVISO: prototipo con fines formativos y académicos, sin fines comerciales.";

    public static void Build()
    {
        Time.timeScale = 1f;
        EcoAssets.Load();
        var cam = EcoBootstrap.EnsureCamera();
        cam.backgroundColor = new Color(0.08f, 0.14f, 0.18f);
        EcoUI.EnsureEventSystem();
        EcoAudio.StartMusic();

        var canvas = EcoUI.CreateCanvas("Canvas");

        // 1) Fondo (primera posición del Canvas)
        var bg = EcoUI.MakeImage(canvas, "BackgroundImage", EcoAssets.Background, Color.white);
        EcoUI.Stretch(bg.rectTransform);
        var shade = EcoUI.MakeImage(canvas, "Shade", null, new Color(0, 0.05f, 0.08f, 0.25f));
        EcoUI.Stretch(shade.rectTransform);
        shade.raycastTarget = false;

        // 2) Título
        var title = EcoUI.MakeLabel(canvas, "Title", "ECOBOT", 170, new Color(0.7f, 1f, 0.75f), TextAnchor.MiddleCenter);
        EcoUI.Place(title.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0, 330), new Vector2(1400, 220));
        var sub = EcoUI.MakeLabel(canvas, "Subtitle", "Misión Reciclaje", 70, Color.white, TextAnchor.MiddleCenter);
        EcoUI.Place(sub.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0, 200), new Vector2(1400, 100));

        // Mascota animada
        var bot = EcoUI.MakeImage(canvas, "EcobotImage", EcoAssets.BotIdle[0], Color.white);
        EcoUI.Place(bot.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(-640, -170), new Vector2(380, 380));
        bot.raycastTarget = false;
        var ua = bot.gameObject.AddComponent<EcoUiAnimator>();
        ua.frames = EcoAssets.BotIdle; ua.fps = 5f;

        // 3) Panel de menú + 3 botones
        var panel = new GameObject("MenuPanel", typeof(RectTransform));
        panel.transform.SetParent(canvas, false);
        EcoUI.Place((RectTransform)panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(10, 10));

        var manager = new GameObject("MenuManager").AddComponent<MainMenuManager>();

        EcoUI.MakeButton(panel.transform, "Btn_Play", "JUGAR", new Vector2(0, -50), new Vector2(560, 120), manager.PlayGame, 62);
        EcoUI.MakeButton(panel.transform, "Btn_Credits", "CRÉDITOS", new Vector2(0, -200), new Vector2(560, 120), manager.OpenCredits, 62);
        EcoUI.MakeButton(panel.transform, "Btn_Quit", "SALIR", new Vector2(0, -350), new Vector2(560, 120), manager.QuitGame, 62);

        var tip = EcoUI.MakeLabel(canvas, "Tip", "Recoge residuos, aprende a reciclarlos y llévalos a la caneca.", 40, Color.white, TextAnchor.MiddleCenter);
        EcoUI.Place(tip.rectTransform, new Vector2(0.5f, 0f), new Vector2(0, 30), new Vector2(1700, 60));

        // 4) Panel de créditos (oculto al inicio)
        var credits = EcoUI.MakeImage(canvas, "CreditsPanel", null, new Color(0, 0, 0, 0.88f));
        EcoUI.Stretch(credits.rectTransform);
        var ct = EcoUI.MakeLabel(credits.transform, "CreditsTitle", "INFORMACIÓN Y CRÉDITOS", 66, new Color(0.7f, 1f, 0.75f), TextAnchor.MiddleCenter);
        EcoUI.Place(ct.rectTransform, new Vector2(0.5f, 1f), new Vector2(0, -30), new Vector2(1600, 90));
        var body = EcoUI.MakeLabel(credits.transform, "Credito", CreditsText, 29, Color.white, TextAnchor.UpperLeft);
        EcoUI.Place(body.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0, -20), new Vector2(1640, 800));
        EcoUI.MakeButton(credits.transform, "Btn_CloseCredits", "VOLVER", new Vector2(0, -470), new Vector2(420, 90), manager.CloseCredits, 50);
        credits.gameObject.SetActive(false);
        manager.Init(credits.gameObject);
    }
}
