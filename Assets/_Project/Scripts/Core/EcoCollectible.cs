using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EcoCollectible : MonoBehaviour
{
    public int type;
    Vector3 start;
    float phase;

    void Start() { start = transform.position; phase = Random.value * 6f; }

    void Update()
    {
        transform.position = start + Vector3.up * (Mathf.Sin(Time.time * 3f + phase) * 0.12f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<EcoPlayer>(out _)) return;
        if (EcoGame.Instance != null) EcoGame.Instance.Collect(type);
        Destroy(gameObject);
    }
}
