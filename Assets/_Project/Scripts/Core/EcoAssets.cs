using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Carga las texturas de Resources/Sprites y las corta en sprites (Grid By Cell Size) por código.
public static class EcoAssets
{
    public static Sprite[] BotIdle, BotWalk, BotJump, Slime, Items, Bin;
    public static Sprite Background;
    public static Tile Grass, Dirt, Metal, Water;
    static bool loaded;
    static readonly Dictionary<string, Texture2D> texCache = new Dictionary<string, Texture2D>();

    public static void Load()
    {
        if (loaded && BotIdle != null && BotIdle[0] != null) return;

        var center = new Vector2(0.5f, 0.5f);
        var bottom = new Vector2(0.5f, 0f);

        BotIdle = Cut("Ecobot_Sheet", 32, 32, 0, 4, bottom);
        BotWalk = Cut("Ecobot_Sheet", 32, 32, 1, 6, bottom);
        BotJump = Cut("Ecobot_Sheet", 32, 32, 2, 2, bottom);
        Slime = Cut("Slime", 16, 16, 0, 2, bottom);
        Items = Cut("Items", 16, 16, 0, 8, center);
        Bin = Cut("Bin", 32, 32, 0, 2, bottom);

        var bg = Tex("Background");
        if (bg != null)
            Background = Sprite.Create(bg, new Rect(0, 0, bg.width, bg.height), center, 16f, 0, SpriteMeshType.FullRect);

        var tiles = Cut("Tileset", 16, 16, 0, 4, center);
        Grass = MakeTile(tiles[0], true);
        Dirt = MakeTile(tiles[1], true);
        Metal = MakeTile(tiles[2], true);
        Water = MakeTile(tiles[3], false);
        loaded = true;
    }

    static Texture2D Tex(string name)
    {
        if (texCache.TryGetValue(name, out var t) && t != null) return t;
        t = Resources.Load<Texture2D>("Sprites/" + name);
        if (t == null) { Debug.LogError("EcoBot: no se encontró Resources/Sprites/" + name); return null; }
        t.filterMode = FilterMode.Point;
        t.wrapMode = TextureWrapMode.Clamp;
        texCache[name] = t;
        return t;
    }

    static Sprite[] Cut(string texName, int cw, int ch, int rowFromTop, int count, Vector2 pivot)
    {
        var t = Tex(texName);
        var arr = new Sprite[count];
        if (t == null) return arr;
        for (int i = 0; i < count; i++)
        {
            var rect = new Rect(i * cw, t.height - (rowFromTop + 1) * ch, cw, ch);
            arr[i] = Sprite.Create(t, rect, pivot, 16f, 0, SpriteMeshType.FullRect);
            arr[i].name = texName + "_" + rowFromTop + "_" + i;
        }
        return arr;
    }

    static Tile MakeTile(Sprite s, bool solid)
    {
        var tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = s;
        tile.colliderType = solid ? Tile.ColliderType.Grid : Tile.ColliderType.None;
        return tile;
    }

    // Datos educativos de cada residuo (índice = sprite en Items)
    public static readonly string[] ItemNames = { "Botella plástica", "Lata de aluminio", "Papel", "Pila usada", "Frasco de vidrio", "Resto de manzana" };
    public static readonly string[] Facts =
    {
        "Una botella de plástico puede tardar cientos de años en degradarse. Vacíala, aplástala y llévala a la caneca blanca.",
        "El aluminio se puede reciclar muchas veces sin perder calidad y ahorra gran parte de la energía de fabricar una lata nueva.",
        "El papel limpio y seco se puede reciclar varias veces. Si está sucio de grasa o comida, ya no sirve para reciclar.",
        "Las pilas contienen metales pesados que contaminan el agua y el suelo. Nunca van a la basura común: llévalas a puntos de recolección.",
        "El vidrio se recicla una y otra vez sin perder calidad. Enjuaga el frasco antes de depositarlo en la caneca blanca.",
        "Los residuos orgánicos van a la caneca verde y pueden convertirse en compost, un abono natural para las plantas."
    };
}
