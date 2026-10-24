using UnityEngine;

// Cámara con zona muerta (Dead Zone) y límites del escenario (Clamping).
public class EcoCameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 8f;
    public Vector3 offset = new Vector3(0f, 2f, -10f);
    public float deadZoneX = 1.5f;
    public bool useLimits = true;
    public float minX = 0f;   // borde izquierdo del nivel
    public float maxX = 93f;  // borde derecho del nivel

    Camera cam;

    void Awake() { cam = GetComponent<Camera>(); }

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 goal = transform.position;
        float dx = target.position.x - transform.position.x;
        if (Mathf.Abs(dx) > deadZoneX) goal.x = target.position.x - Mathf.Sign(dx) * deadZoneX;
        goal.y = target.position.y + offset.y;
        goal.z = offset.z;

        if (useLimits && cam != null)
        {
            float half = cam.orthographicSize * cam.aspect;
            float lo = minX + half, hi = maxX - half;
            goal.x = lo > hi ? (minX + maxX) * 0.5f : Mathf.Clamp(goal.x, lo, hi);
        }
        goal.y = Mathf.Max(goal.y, 1.5f);
        transform.position = Vector3.Lerp(transform.position, goal, smoothSpeed * Time.deltaTime);
    }

    // Coloca la cámara de inmediato (al reiniciar)
    public void Snap()
    {
        if (target == null) return;
        transform.position = new Vector3(target.position.x, target.position.y + offset.y, offset.z);
    }
}
