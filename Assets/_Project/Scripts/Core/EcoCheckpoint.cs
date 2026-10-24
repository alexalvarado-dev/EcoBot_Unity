using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EcoCheckpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<EcoPlayer>(out var p)) p.SetCheckpoint(transform.position);
    }
}
