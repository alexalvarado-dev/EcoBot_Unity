using UnityEngine;
using UnityEngine.UI;

// Administrador de la partida: residuos, vidas, tiempo, hechos educativos, pausa y fin de juego.
public class EcoGame : MonoBehaviour
{
    public static EcoGame Instance { get; private set; }
    public enum State { Playing, Won, Lost }

    public EcoPlayer player;
    public int total, collected, lives = 3;
    public State state = State.Playing;

    [Header("UI")]
    public Text scoreText, timeText, factText, endTitle, endBody;
    public GameObject factPanel, endPanel, pausePanel;
    public Image[] hearts;
    public Sprite heartFull, heartEmpty;

    float elapsed, factTimer;
    bool paused;
    public bool Finished => state != State.Playing;

    void Awake() { Instance = this; Time.timeScale = 1f; }

    void Start() { RefreshUI(); EcoAudio.StartMusic(); }

    void OnDestroy() { if (Instance == this) Instance = null; Time.timeScale = 1f; }

    void Update()
    {
        if (state == State.Playing)
        {
            elapsed += Time.deltaTime;
            if (EcoInput.PausePressed()) TogglePause(!paused);
        }
        if (factTimer > 0f)
        {
            factTimer -= Time.deltaTime;
            if (factTimer <= 0f && factPanel != null) factPanel.SetActive(false);
        }
        if (timeText != null)
        {
            int s = (int)elapsed;
            timeText.text = string.Format("Tiempo {0:00}:{1:00}", s / 60, s % 60);
        }
    }

    public void Collect(int type)
    {
        if (state != State.Playing) return;
        collected++;
        EcoAudio.Play("collect");
        int i = Mathf.Clamp(type, 0, EcoAssets.Facts.Length - 1);
        ShowMessage(EcoAssets.ItemNames[i] + ": " + EcoAssets.Facts[i], 6f);
        RefreshUI();
    }

    public void TryDeposit()
    {
        if (state != State.Playing) return;
        if (collected >= total) Win();
        else ShowMessage("¡Aún faltan " + (total - collected) + " residuos por recoger antes de llegar a la caneca!", 3f);
    }

    public void Hurt(bool respawn)
    {
        if (state != State.Playing || player == null) return;
        if (player.invuln > 0f)
        {
            if (respawn) player.Respawn();   // caer al agua/vacío siempre devuelve al checkpoint
            return;
        }
        lives--;
        EcoAudio.Play("hit");
        RefreshUI();
        if (lives <= 0) { Lose(); return; }
        player.invuln = 1.6f;
        if (respawn) player.Respawn(); else player.Bounce(0.6f);
    }

    void Win()
    {
        state = State.Won;
        if (player != null) player.controlEnabled = false;
        EcoAudio.Play("win");
        int stars = lives >= 3 ? 3 : (lives == 2 ? 2 : 1);
        ShowEnd("¡MISIÓN CUMPLIDA!",
            "Reciclaste los " + total + " residuos.\nTiempo: " + (int)elapsed + " s    Corazones: " + lives + "/3\nEstrellas: " + stars + " / 3\n\nSeparar en la fuente ayuda a cuidar el planeta.");
    }

    void Lose()
    {
        state = State.Lost;
        if (player != null) player.controlEnabled = false;
        ShowEnd("¡SE ACABARON LOS CORAZONES!",
            "Recogiste " + collected + " de " + total + " residuos.\nEvita los slimes de contaminación y el agua sucia.\n¡Inténtalo de nuevo!");
    }

    void ShowEnd(string title, string body)
    {
        if (factPanel != null) factPanel.SetActive(false);
        if (endTitle != null) endTitle.text = title;
        if (endBody != null) endBody.text = body;
        if (endPanel != null) endPanel.SetActive(true);
    }

    void ShowMessage(string msg, float seconds)
    {
        if (factText == null || factPanel == null) return;
        factText.text = msg;
        factPanel.SetActive(true);
        factTimer = seconds;
    }

    public void TogglePause(bool value)
    {
        paused = value;
        Time.timeScale = paused ? 0f : 1f;
        if (pausePanel != null) pausePanel.SetActive(paused);
    }

    void RefreshUI()
    {
        if (scoreText != null) scoreText.text = "Residuos: " + collected + " / " + total;
        if (hearts == null) return;
        for (int i = 0; i < hearts.Length; i++)
            if (hearts[i] != null) hearts[i].sprite = i < lives ? heartFull : heartEmpty;
    }

    // Botones
    public void OnResume() { TogglePause(false); }
    public void OnRetry() { EcoBootstrap.Go(EcoBootstrap.GameScene); }
    public void OnMenu() { EcoBootstrap.Go(EcoBootstrap.MenuScene); }
}
