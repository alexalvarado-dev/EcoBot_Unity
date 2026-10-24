#if UNITY_EDITOR
using UnityEditor;

// Ajustes de importación para pixel art en Resources/Sprites: Point (no filter), Compression None.
public class EcoPostprocessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (!assetPath.Replace("\\", "/").Contains("/Resources/Sprites/")) return;
        var ti = (TextureImporter)assetImporter;
        ti.filterMode = UnityEngine.FilterMode.Point;
        ti.textureCompression = TextureImporterCompression.Uncompressed;
        ti.mipmapEnabled = false;
        ti.npotScale = TextureImporterNPOTScale.None;
        ti.alphaIsTransparency = true;
        ti.wrapMode = UnityEngine.TextureWrapMode.Clamp;
        ti.maxTextureSize = 2048;
    }
}
#endif
