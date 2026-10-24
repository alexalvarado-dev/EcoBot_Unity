using UnityEngine;
#if !ENABLE_LEGACY_INPUT_MANAGER && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Lee el teclado con el sistema de entrada activo del proyecto (Input Manager clásico o Input System).
public static class EcoInput
{
    public static float Horizontal()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetAxisRaw("Horizontal");
#elif ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current; if (kb == null) return 0f;
        float h = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) h -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h += 1f;
        return h;
#else
        return 0f;
#endif
    }

    public static bool JumpPressed()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
#elif ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current; if (kb == null) return false;
        return kb.spaceKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame;
#else
        return false;
#endif
    }

    public static bool PausePressed()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P);
#elif ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current; if (kb == null) return false;
        return kb.escapeKey.wasPressedThisFrame || kb.pKey.wasPressedThisFrame;
#else
        return false;
#endif
    }
}
