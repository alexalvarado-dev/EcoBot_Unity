using UnityEngine;

// Fondo pegado a la cámara: se escala para cubrir siempre toda la pantalla.
public class EcoBackground : MonoBehaviour
{
    Camera cam;
    void Awake() { cam = Camera.main != null ? Camera.main : GetComponentInParent<Camera>(); }
    void LateUpdate()
    {
        if (cam == null) return;
        float h = cam.orthographicSize * 2f, w = h * cam.aspect;
        float s = Mathf.Max(h / 16.875f, w / 30f) * 1.02f;
        transform.localScale = new Vector3(s, s, 1f);
    }
}
