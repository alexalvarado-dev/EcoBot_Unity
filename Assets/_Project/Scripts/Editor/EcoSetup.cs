#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Crea la estructura de carpetas del taller, las escenas MainMenu y GameScene y las registra en Build Settings.
public static class EcoSetup
{
    const string Root = "Assets/_Project";
    const string MenuPath = "Assets/Scenes/MainMenu.unity";
    const string GamePath = "Assets/Scenes/GameScene.unity";

    static readonly string[] Folders =
    {
        "Animations/Player", "Audio", "Materials", "Prefabs", "Scripts", "Sprites", "Tilemaps"
    };

    [InitializeOnLoadMethod]
    static void AutoSetup()
    {
        if (File.Exists(MenuPath) && File.Exists(GamePath)) return;
        if (SessionState.GetBool("EcoSetupRan", false)) return;
        SessionState.SetBool("EcoSetupRan", true);
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode) return;
            Setup();
        };
    }

    [MenuItem("EcoBot/Preparar proyecto (carpetas + escenas + Build Settings)")]
    public static void Setup()
    {
        try
        {
            EditorSettings.defaultBehaviorMode = EditorBehaviorMode.Mode2D;
            foreach (var f in Folders) Directory.CreateDirectory($"{Root}/{f}");
            Directory.CreateDirectory("Assets/Scenes");
            AssetDatabase.Refresh();

            if (!File.Exists(MenuPath) || !File.Exists(GamePath))
            {
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
                var s1 = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(s1, GamePath);
                var s2 = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(s2, MenuPath);
            }

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(MenuPath, true),   // índice 0
                new EditorBuildSettingsScene(GamePath, true)    // índice 1
            };
            AssetDatabase.SaveAssets();
            if (!EditorApplication.isPlaying) EditorSceneManager.OpenScene(MenuPath);
            Debug.Log("EcoBot: proyecto listo. Abre MainMenu y presiona Play.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("EcoBot: no se pudo preparar el proyecto -> " + e + "\nPuedes igual presionar Play en cualquier escena.");
        }
    }
}
#endif
