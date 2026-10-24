using UnityEngine;

// Agua contaminada: quita un corazón y devuelve al último punto de control.
[RequireComponent(typeof(Collider2D))]
public class EcoHazard : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<EcoPlayer>(out _) && EcoGame.Instance != null) EcoGame.Instance.Hurt(true);
    }
}
