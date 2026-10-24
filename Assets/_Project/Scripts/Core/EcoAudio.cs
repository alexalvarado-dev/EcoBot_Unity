using System.Collections.Generic;
using UnityEngine;

public class EcoAudio : MonoBehaviour
{
    static EcoAudio instance;
    static readonly Dictionary<string, AudioClip> cache = new Dictionary<string, AudioClip>();
    AudioSource sfx, music;

    static EcoAudio Ensure()
    {
        if (instance != null) return instance;
        var go = new GameObject("AudioManager");
        instance = go.AddComponent<EcoAudio>();
        instance.sfx = go.AddComponent<AudioSource>();
        instance.music = go.AddComponent<AudioSource>();
        instance.sfx.playOnAwake = false;
        instance.music.playOnAwake = false;
        instance.music.loop = true;
        instance.music.volume = 0.3f;
        return instance;
    }

    static AudioClip Clip(string name)
    {
        if (cache.TryGetValue(name, out var c) && c != null) return c;
        c = Resources.Load<AudioClip>("Audio/" + name);
        cache[name] = c;
        return c;
    }

    public static void Play(string name)
    {
        var a = Ensure();
        var c = Clip(name);
        if (c != null) a.sfx.PlayOneShot(c, 0.8f);
    }

    public static void StartMusic()
    {
        var a = Ensure();
        var c = Clip("bgm");
        if (c == null || (a.music.isPlaying && a.music.clip == c)) return;
        a.music.clip = c;
        a.music.Play();
    }
}
