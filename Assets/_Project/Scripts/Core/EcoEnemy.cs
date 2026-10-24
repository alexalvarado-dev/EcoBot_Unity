using UnityEngine;

// Slime contaminante: patrulla. Si el jugador cae encima lo limpia; si lo toca de lado, pierde un corazón.
[RequireComponent(typeof(Collider2D))]
public class EcoEnemy : MonoBehaviour
{
    public float speed = 1.8f;
    public float leftX, rightX;
    int dir = 1;
    Collider2D col;
    float baseScale = 1f;

    void Awake() { col = GetComponent<Collider2D>(); baseScale = Mathf.Abs(transform.localScale.x); }

    void Update()
    {
        transform.position += Vector3.right * (dir * speed * Time.deltaTime);
        if (transform.position.x > rightX) dir = -1;
        else if (transform.position.x < leftX) dir = 1;
        transform.localScale = new Vector3(baseScale * (dir > 0 ? 1f : -1f), transform.localScale.y, 1f);
    }

    void OnTriggerEnter2D(Collider2D other) { Handle(other); }
    void OnTriggerStay2D(Collider2D other) { Handle(other); }

    void Handle(Collider2D other)
    {
        if (!other.TryGetComponent<EcoPlayer>(out var p)) return;
        bool stomp = p.VerticalSpeed < 0.1f && other.bounds.min.y > col.bounds.center.y;
        if (stomp)
        {
            EcoAudio.Play("hit");
            p.Bounce(0.7f);
            Destroy(gameObject);
        }
        else if (EcoGame.Instance != null) EcoGame.Instance.Hurt(false);
    }
}
