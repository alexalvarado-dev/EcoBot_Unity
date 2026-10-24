using UnityEngine;

// Administrador del menú principal (mismo contrato que el taller: PlayGame, OpenCredits, CloseCredits, QuitGame).
public class MainMenuManager : MonoBehaviour
{
    [Header("Paneles de Interfaz")]
    [SerializeField] private GameObject creditsPanel;

    public void Init(GameObject credits) { creditsPanel = credits; }

    public void PlayGame() { EcoBootstrap.Go(EcoBootstrap.GameScene); }

    public void OpenCredits() { if (creditsPanel != null) creditsPanel.SetActive(true); }

    public void CloseCredits() { if (creditsPanel != null) creditsPanel.SetActive(false); }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
