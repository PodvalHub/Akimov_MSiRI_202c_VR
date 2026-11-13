import re
p = "Assets/Editor/VRPanelSetup.cs"
t = open(p, encoding="utf-8").read()

old_btn = """        var go = CreateUIGo(name, parent);
        var img = go.AddComponent<Image>();
        img.color = color;
        go.AddComponent<Button>();
        var colors = go.AddComponent<ButtonColors>();
        colors.highlightedColor = Color.white;
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
        colors.fadeDuration = 0.1f;
"""
new_btn = """        var go = CreateUIGo(name, parent);
        var img = go.AddComponent<Image>();
        img.color = color;
        go.AddComponent<Button>();
        var block = go.GetComponent<Selectable>().colors;
        block.highlightedColor = Color.white;
        block.pressedColor = new Color(0.7f, 0.7f, 0.7f);
        block.fadeDuration = 0.1f;
        go.GetComponent<Selectable>().colors = block;
"""
assert old_btn in t, "btn block not found"
t = t.replace(old_btn, new_btn)

start = t.index("    /// <summary>\n    /// Шрифт с кириллицей")
end = t.index("    static TMP_FontAsset SaveFont")
new_font = """    /// <summary>
    /// Шрифт с кириллицей: готовый TMP Font Asset или создание
    /// из системного Arial (кириллица есть).
    /// </summary>
    static TMP_FontAsset GetCyrillicFont()
    {
        var existing = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
        if (existing != null && existing.Length > 0)
            return existing[0];

        var font = Font.CreateDynamicFontFromOSFont("Arial", 20);
        if (font == null)
            return null;

        var asset = TMP_FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024);
        if (asset == null)
            return null;

        string dir = "Assets/Fonts";
        if (!AssetDatabase.IsValidFolder(dir))
            AssetDatabase.CreateFolder("Assets", "Fonts");
        string path = dir + "/ArialCyrillic SDF.asset";
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        Debug.Log("VRPanelSetup: создан Font Asset " + path);
        return asset;
    }

"""
t = t[:start] + new_font + t[end:]
t = t[:t.index("    static TMP_FontAsset SaveFont")] + "}\n"
open(p, "w", encoding="utf-8").write(t)
print("fixed")
