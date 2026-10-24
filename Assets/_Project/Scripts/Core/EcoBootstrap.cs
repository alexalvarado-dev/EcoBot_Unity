using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Punto de entrada: construye el menú o el nivel según la escena cargada (MainMenu / GameScene).
public static class EcoBootstrap
{
    public const string MenuScene = "MainMenu";
    public const string GameScene = "GameScene";
    static bool hooked;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Auto()
    {
        if (!hooked) { hooked = true; SceneManager.sceneLoaded += (s, m) => BuildFor(s.name); }
        BuildFor(SceneManager.GetActiveScene().name);
    }

    static void BuildFor(string sceneName)
    {
        if (Object.FindFirstObjectByType<EcoGame>() != null || Object.FindFirstObjectByType<MainMenuManager>() != null) return;
        if (sceneName == GameScene) EcoLevelBuilder.Build(); else EcoMenuBuilder.Build();
    }

    public static void Go(string sceneName)
    {
        Time.timeScale = 1f;
        if (Application.CanStreamedLevelBeLoaded(sceneName)) { SceneManager.LoadScene(sceneName); return; }
        // Si las escenas no están en Build Settings, se reconstruye en la escena actual.
        var runner = new GameObject("EcoRunner").AddComponent<EcoRunner>();
        runner.StartCoroutine(runner.Run(sceneName));
    }

    public static Camera EnsureCamera()
    {
        var cam = Camera.main != null ? Camera.main : Object.FindFirstObjectByType<Camera>();
        if (cam == null)
        {
            var g = new GameObject("Main Camera") { tag = "MainCamera" };
            cam = g.AddComponent<Camera>();
            g.AddComponent<AudioListener>();
        }
        if (cam.GetComponent<AudioListener>() == null && Object.FindFirstObjectByType<AudioListener>() == null)
            cam.gameObject.AddComponent<AudioListener>();
        cam.orthographic = true;
        cam.orthographicSize = 7f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.45f, 0.72f, 0.92f);
        cam.transform.position = new Vector3(0, 2, -10);
        cam.transform.rotation = Quaternion.identity;
        return cam;
    }
}

public class EcoRunner : MonoBehaviour
{
    public IEnumerator Run(string sceneName)
    {
        foreach (var go in SceneManager.GetActiveScene().GetRootGameObjects())
            if (go != gameObject) Destroy(go);
        yield return null;
        yield return null;
        if (sceneName == EcoBootstrap.GameScene) EcoLevelBuilder.Build(); else EcoMenuBuilder.Build();
        Destroy(gameObject);
    }
}
