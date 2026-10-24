using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Ayudantes para construir la interfaz (Canvas, Image, Text, Button) por código.
public static class EcoUI
{
    static Font font;
    public static Font DefaultFont
    {
        get
        {
            if (font != null) return font;
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null) font = Font.CreateDynamicFontFromOSFont("Arial", 16);
            return font;
        }
    }

    public static Transform CreateCanvas(string name)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var c = go.GetComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        var s = go.GetComponent<CanvasScaler>();
        s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;   // Scale With Screen Size
        s.referenceResolution = new Vector2(1920, 1080);              // 1920 x 1080
        s.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        s.matchWidthOrHeight = 0.5f;                                  // Match 0.5
        return go.transform;
    }

    public static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null) return;
        var go = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        go.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
        go.AddComponent<StandaloneInputModule>();
#endif
    }

    public static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    public static void Place(RectTransform rt, Vector2 anchor, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = anchor; rt.anchorMax = anchor; rt.pivot = anchor;
        rt.anchoredPosition = pos; rt.sizeDelta = size;
    }

    public static Image MakeImage(Transform parent, string name, Sprite sprite, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.sprite = sprite; img.color = color;
        return img;
    }

    public static Text MakeLabel(Transform parent, string name, string text, int size, Color color, TextAnchor align)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var t = go.GetComponent<Text>();
        t.font = DefaultFont; t.text = text; t.fontSize = size; t.color = color; t.alignment = align;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        var sh = go.AddComponent<Shadow>();
        sh.effectColor = new Color(0, 0, 0, 0.7f);
        sh.effectDistance = new Vector2(3, -3);
        return t;
    }

    public static Button MakeButton(Transform parent, string name, string label, Vector2 pos, Vector2 size, UnityAction onClick, int fontSize = 52)
    {
        var img = MakeImage(parent, name, null, Color.white);
        Place(img.rectTransform, new Vector2(0.5f, 0.5f), pos, size);
        var b = img.gameObject.AddComponent<Button>();
        b.targetGraphic = img;
        var cb = b.colors;
        cb.normalColor = new Color(0.16f, 0.55f, 0.30f);
        cb.highlightedColor = new Color(0.32f, 0.75f, 0.44f);
        cb.pressedColor = new Color(0.09f, 0.36f, 0.19f);
        cb.selectedColor = new Color(0.16f, 0.55f, 0.30f);
        cb.fadeDuration = 0.08f;
        b.colors = cb;
        var ol = img.gameObject.AddComponent<Outline>();
        ol.effectColor = new Color(0.05f, 0.18f, 0.1f, 1f);
        ol.effectDistance = new Vector2(4, -4);
        var t = MakeLabel(img.transform, "Text", label, fontSize, Color.white, TextAnchor.MiddleCenter);
        Stretch(t.rectTransform);
        b.onClick.AddListener(() => EcoAudio.Play("click"));
        if (onClick != null) b.onClick.AddListener(onClick);
        return b;
    }
}
