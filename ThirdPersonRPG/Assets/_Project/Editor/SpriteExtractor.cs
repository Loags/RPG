using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class SpriteSheetExtractorWindow : EditorWindow
{
    private string spriteSheetPath;
    private string extractedSpritesPath;
    private Texture2D spriteSheet;
    private Sprite[] sprites;

    [MenuItem("Tools/Sprite Sheet Extractor")]
    public static void ShowWindow()
    {
        GetWindow<SpriteSheetExtractorWindow>("Sprite Sheet Extractor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sprite Sheet Extractor", EditorStyles.boldLabel);

        spriteSheetPath = EditorGUILayout.TextField("Sprite Sheet Path", spriteSheetPath);

        if (GUILayout.Button("Load Sprite Sheet"))
        {
            LoadSpriteSheet();
        }

        if (sprites != null && sprites.Length > 0)
        {
            if (GUILayout.Button("Extract and Save Sprites"))
            {
                ExtractAndSaveSprites();
            }
        }
    }

    private void LoadSpriteSheet()
    {
        spriteSheet = AssetDatabase.LoadAssetAtPath<Texture2D>(spriteSheetPath);

        if (spriteSheet == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not load sprite sheet. Please check the path.", "OK");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(spriteSheet);
        Object[] objects = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        sprites = objects.OfType<Sprite>().ToArray();

        if (sprites.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "No sprites found in the specified sprite sheet.", "OK");
        }
    }

    private void ExtractAndSaveSprites()
    {
        if (!AssetDatabase.IsValidFolder(extractedSpritesPath))
        {
            EditorUtility.DisplayDialog("Error", "Invalid extraction path. Please check the path.", "OK");
            return;
        }

        string path = Path.Combine(Application.dataPath, extractedSpritesPath.Substring("Assets/".Length));
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        for (int i = 0; i < sprites.Length; i++)
        {
            SaveSpriteToFile(sprites[i], Path.Combine(path, "sprite_" + i + ".png"));
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", "Sprites extracted and saved successfully.", "OK");
    }

    private void SaveSpriteToFile(Sprite sprite, string filePath)
    {
        Texture2D texture = sprite.texture;
        Rect rect = sprite.rect;
        Texture2D newTexture = new Texture2D((int)rect.width, (int)rect.height);
        Color[] pixels = texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
        newTexture.SetPixels(pixels);
        newTexture.Apply();

        byte[] bytes = newTexture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
    }
}