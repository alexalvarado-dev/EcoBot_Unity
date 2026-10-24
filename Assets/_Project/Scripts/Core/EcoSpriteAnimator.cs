using UnityEngine;

// Animación cuadro por cuadro (equivale al Animator Idle/Walk con parámetro Speed del taller).
[RequireComponent(typeof(SpriteRenderer))]
public class EcoSpriteAnimator : MonoBehaviour
{
    public Sprite[] idle, walk, jump;
    public float idleFps = 6f, walkFps = 12f;
    [HideInInspector] public float speed;      // parámetro "Speed"
    [HideInInspector] public bool airborne, rising;

    SpriteRenderer sr;
    float t;

    void Awake() { sr = GetComponent<SpriteRenderer>(); }

    void Update()
    {
        if (airborne && jump != null && jump.Length >= 2)
        {
            sr.sprite = rising ? jump[0] : jump[1];
            return;
        }
        Sprite[] f; float fps;
        if (speed > 0.01f) { f = walk; fps = walkFps; } else { f = idle; fps = idleFps; }
        if (f == null || f.Length == 0) return;
        t += Time.deltaTime * fps;
        sr.sprite = f[(int)t % f.Length];
    }
}
