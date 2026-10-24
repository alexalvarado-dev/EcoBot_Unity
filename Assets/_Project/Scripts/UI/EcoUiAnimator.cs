using UnityEngine;
using UnityEngine.UI;

// Anima una Image de la UI (EcoBot en el menú).
public class EcoUiAnimator : MonoBehaviour
{
    public Sprite[] frames;
    public float fps = 6f;
    Image img;
    float t;
    void Awake() { img = GetComponent<Image>(); }
    void Update()
    {
        if (frames == null || frames.Length == 0 || img == null) return;
        t += Time.unscaledDeltaTime * fps;
        img.sprite = frames[(int)t % frames.Length];
    }
}
