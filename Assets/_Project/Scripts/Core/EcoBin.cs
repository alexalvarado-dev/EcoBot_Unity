using UnityEngine;

// Caneca de reciclaje: meta del nivel.
[RequireComponent(typeof(Collider2D))]
public class EcoBin : MonoBehaviour
{
    SpriteRenderer sr;
    void Awake() { sr = GetComponent<SpriteRenderer>(); }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<EcoPlayer>(out _)) return;
        if (sr != null) sr.sprite = EcoAssets.Bin[1];
        if (EcoGame.Instance != null) EcoGame.Instance.TryDeposit();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<EcoPlayer>(out _) && sr != null && !(EcoGame.Instance != null && EcoGame.Instance.Finished))
            sr.sprite = EcoAssets.Bin[0];
    }
}
