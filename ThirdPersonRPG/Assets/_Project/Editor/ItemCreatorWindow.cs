using LB;
using LB.Inventory;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ItemCreatorWindow : EditorWindow
{
    private string spritesRootPath = "Assets/_Project/Sprites/Items/";
    private string scriptableObjectsRootPath = "Assets/_Project/ScriptableObjects/_Items/";
    private string deletionRootPath = "Assets/_Project/ScriptableObjects/_Items/";
    private string reNameSpritesRootPath = "Assets/_Project/Sprites/Items/";
    private string itemDataBaseObjectPath = "Assets/Resources/";


    [MenuItem("Tools/Item Creator")]
    public static void ShowWindow()
    {
        GetWindow<ItemCreatorWindow>("Item Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Item Creator", EditorStyles.boldLabel);
        GUILayout.Label("Specify the path to your sprites root folder:");
        spritesRootPath = EditorGUILayout.TextField("Sprites Root Path", spritesRootPath);
        GUILayout.Label("Specify the path to your ScriptableObjects root folder for items:");
        scriptableObjectsRootPath = EditorGUILayout.TextField("ScriptableObjects Root Path", scriptableObjectsRootPath);

        GUI.enabled = !string.IsNullOrEmpty(spritesRootPath) && !string.IsNullOrEmpty(scriptableObjectsRootPath);
        if (GUILayout.Button("Create Items at ScriptableObjects Root Path"))
        {
            CreateItemsFromSprites();
        }

        GUI.enabled = true;
        GUILayout.Space(20);

        GUILayout.Label("Specify the path to clear all ScriptableObjects:");
        deletionRootPath = EditorGUILayout.TextField("Deletion Root Path", deletionRootPath);
        GUI.enabled = !string.IsNullOrEmpty(deletionRootPath);
        if (GUILayout.Button("Clear All ScriptableObjects at Deletion Root Path"))
        {
            ClearScriptableObjects();
        }

        GUI.enabled = true;
        GUILayout.Space(20);

        GUILayout.Label("Specify the path to rename sprites:");
        reNameSpritesRootPath = EditorGUILayout.TextField("ReName Sprites Root Path", reNameSpritesRootPath);
        GUI.enabled = !string.IsNullOrEmpty(reNameSpritesRootPath);
        if (GUILayout.Button("Rename Sprites at ReName Sprites Root Path"))
        {
            RenameSprites();
        }

        GUI.enabled = true;
        GUILayout.Space(20);

        GUILayout.Label("Specify the path to the Item Database object:");
        itemDataBaseObjectPath = EditorGUILayout.TextField("Item Database Object Path", itemDataBaseObjectPath);
        GUI.enabled = !string.IsNullOrEmpty(itemDataBaseObjectPath);
        if (GUILayout.Button("Update Item Database"))
        {
            UpdateItemDatabase();
        }

        GUI.enabled = true;
    }


    private void CreateItemsFromSprites()
    {
        if (!AssetDatabase.IsValidFolder(spritesRootPath))
        {
            EditorUtility.DisplayDialog("Error", "Invalid sprites root path. Please check the path.", "OK");
            Debug.LogError("Invalid sprites root path: " + spritesRootPath);
            return;
        }

        if (string.IsNullOrEmpty(scriptableObjectsRootPath))
        {
            EditorUtility.DisplayDialog("Error", "Invalid scriptableObjects root path. Please check the path.", "OK");
            Debug.LogError("ScriptableObjects root path is null or empty.");
            return;
        }

        string[] directories = Directory.GetDirectories(spritesRootPath, "*", SearchOption.AllDirectories);
        List<string> allFiles = new List<string>();

        foreach (string directory in directories)
        {
            string[] files = Directory.GetFiles(directory, "*.png", SearchOption.TopDirectoryOnly);
            allFiles.AddRange(files);
        }

        int loadedSprites = 0;
        int createdCount = 0;
        int skippedCount = 0;

        Debug.Log("Found " + allFiles.Count + " sprite files.");

        foreach (string file in allFiles)
        {
            string assetPath = file.Replace(Application.dataPath, "").Replace('\\', '/');
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

            if (sprite != null)
            {
                Debug.Log("Loaded sprite: " + sprite.name);
                loadedSprites++;
                string relativePath = file.Replace(spritesRootPath, "").Replace('\\', '/');
                string targetDirectory = Path.Combine(scriptableObjectsRootPath, Path.GetDirectoryName(relativePath))
                    .Replace('\\', '/');
                if (CreateItemObject(sprite, assetPath, targetDirectory))
                {
                    createdCount++;
                }
                else
                {
                    skippedCount++;
                }
            }
            else
            {
                Debug.LogWarning("Could not load sprite at path: " + assetPath);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success",
            $"Items creation completed.\n\nFound Sprites: {allFiles.Count}\nLoaded Sprites: {loadedSprites}\nCreated: {createdCount}\nSkipped: {skippedCount}",
            "OK");
    }

    private bool CreateItemObject(Sprite sprite, string assetPath, string targetDirectory)
    {
        if (!AssetDatabase.IsValidFolder(targetDirectory))
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, targetDirectory.Substring(7)));
            AssetDatabase.Refresh();
            Debug.Log("Created directory for items: " + targetDirectory);
        }

        string name = Path.GetFileNameWithoutExtension(assetPath);
        string itemPath = Path.Combine(targetDirectory, name + ".asset").Replace(Application.dataPath, "Assets");

        if (AssetDatabase.LoadAssetAtPath<ItemObject>(itemPath) != null)
        {
            Debug.LogWarning($"Skipping creation of {name} at {itemPath} because it already exists.");
            return false;
        }

        ItemObject itemObject = null;

        Debug.Log($"Creating item at: {itemPath}");

        if (assetPath.Contains("/Accessory/"))
        {
            AccessoryObject accessory = CreateInstance<AccessoryObject>();
            accessory.equipmentType = Equipment.Accessory;

            if (assetPath.Contains("/Ring/"))
            {
                accessory.accessoryType = Accessory.Ring;
            }
            else if (assetPath.Contains("/Earring/"))
            {
                accessory.accessoryType = Accessory.Earring;
            }
            else if (assetPath.Contains("/Necklace/"))
            {
                accessory.accessoryType = Accessory.Necklace;
            }

            // accessory.GeneratePredefinedBuffs();
            itemObject = accessory;
        }
        else if (assetPath.Contains("/Armor/"))
        {
            ArmorObject armor = CreateInstance<ArmorObject>();
            armor.equipmentType = Equipment.Armor;

            if (assetPath.Contains("/Helmet/"))
            {
                armor.armorType = Armor.Helmet;
            }
            else if (assetPath.Contains("/Chest/"))
            {
                armor.armorType = Armor.Chest;
            }
            else if (assetPath.Contains("/Gloves/"))
            {
                armor.armorType = Armor.Gloves;
            }
            else if (assetPath.Contains("/Legs/"))
            {
                armor.armorType = Armor.Legs;
            }
            else if (assetPath.Contains("/Boots/"))
            {
                armor.armorType = Armor.Boots;
            }

            // armor.GeneratePredefinedBuffs();
            itemObject = armor;
        }
        else if (assetPath.Contains("/Weapon/"))
        {
            WeaponObject weapon = CreateInstance<WeaponObject>();
            weapon.equipmentType = Equipment.Weapon;

            if (assetPath.Contains("/MeleeWeapon/"))
            {
                weapon.weaponType = Weapon.MeleeWeapon;
            }
            else
            {
                Debug.LogWarning($"Weapon sprite found outside a recognized subfolder (e.g., /MeleeWeapon/): {assetPath}. Assigning default WeaponType.NONE.");
                weapon.weaponType = Weapon.NONE;
            }

            // weapon.GeneratePredefinedBuffs();
            itemObject = weapon;
        }
        else if (assetPath.Contains("/Consumables"))
        {
            ConsumableObject consumable = CreateInstance<ConsumableObject>();
            consumable.stackable = true;
            consumable.stackAmount = 3;
            itemObject = consumable;
        }
        else if (assetPath.Contains("/Default"))
        {
            DefaultObject defaultItem = CreateInstance<DefaultObject>();
            defaultItem.stackable = true;
            defaultItem.stackAmount = 3;
            itemObject = defaultItem;
        }

        if (itemObject != null)
        {
            itemObject.icon = sprite;
            itemObject.characterDisplay = null;
            if (!itemObject.stackable)
                itemObject.stackable = false;
            if (itemObject.stackAmount <= 0)
                itemObject.stackAmount = 1;
            itemObject.description = name;
            itemObject.data.name = name;

            AssetDatabase.CreateAsset(itemObject, itemPath);
            EditorUtility.SetDirty(itemObject);

            Debug.Log($"Created {itemObject.GetType().Name} at: {itemPath}");
        }
        else
        {
            Debug.LogWarning($"Could not determine item type for sprite at: {assetPath}");
        }

        return true;
    }

    private void ClearScriptableObjects()
    {
        if (string.IsNullOrEmpty(deletionRootPath))
        {
            EditorUtility.DisplayDialog("Error", "Invalid deletion path. Please check the path.", "OK");
            Debug.LogError("Deletion path is null or empty.");
            return;
        }

        string fullPath = deletionRootPath;
        int deletedCount = 0;
        if (Directory.Exists(fullPath))
        {
            string[] files = Directory.GetFiles(fullPath, "*.asset", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string assetPath = file.Replace(Application.dataPath, "").Replace('\\', '/');
                bool successfullyDeleted = AssetDatabase.DeleteAsset(assetPath);
                if (successfullyDeleted)
                {
                    Debug.Log("Deleted asset at: " + assetPath);
                    deletedCount++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success",
                $"All ScriptableObjects cleared successfully.\n\nDeleted: {deletedCount} assets.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "Deletion path does not exist. Please check the path.", "OK");
            Debug.LogError("Deletion path does not exist: " + fullPath);
        }
    }

    private void RenameSprites()
    {
        if (string.IsNullOrEmpty(reNameSpritesRootPath) || !AssetDatabase.IsValidFolder(reNameSpritesRootPath))
        {
            EditorUtility.DisplayDialog("Error", "Invalid rename sprites root path. Please check the path.", "OK");
            Debug.LogError("Invalid rename sprites root path: " + reNameSpritesRootPath);
            return;
        }

        string[] directories = Directory.GetDirectories(reNameSpritesRootPath, "*", SearchOption.AllDirectories);
        int renamedCount = 0;

        foreach (string directory in directories)
        {
            string folderName = new DirectoryInfo(directory).Name;
            string[] files = Directory.GetFiles(directory, "*.png", SearchOption.TopDirectoryOnly);

            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                string newFileName = folderName + "_" + i + ".png";
                string newFilePath = Path.Combine(directory, newFileName);

                string assetPath = file.Replace(Application.dataPath, "").Replace('\\', '/');
                string newAssetPath = newFilePath.Replace(Application.dataPath, "").Replace('\\', '/');

                AssetDatabase.MoveAsset(assetPath, newAssetPath);
                Debug.Log($"Renamed {assetPath} to {newAssetPath}");
                renamedCount++;
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", $"Sprites renamed successfully.\n\nRenamed: {renamedCount} sprites.",
            "OK");
    }

    private void UpdateItemDatabase()
    {
        if (string.IsNullOrEmpty(itemDataBaseObjectPath))
        {
            EditorUtility.DisplayDialog("Error", "Item Database Object path is empty. Please check the path.", "OK");
            Debug.LogError("Item Database Object path is empty.");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:ItemDatabaseObject", new[] { itemDataBaseObjectPath });
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "No Item Database Object found at the specified path.", "OK");
            Debug.LogError("No Item Database Object found at the specified path: " + itemDataBaseObjectPath);
            return;
        }

        string databasePath = AssetDatabase.GUIDToAssetPath(guids[0]);
        ItemDatabaseObject itemDatabase = AssetDatabase.LoadAssetAtPath<ItemDatabaseObject>(databasePath);
        if (itemDatabase == null)
        {
            EditorUtility.DisplayDialog("Error", "Failed to load Item Database Object from the specified path.", "OK");
            Debug.LogError("Failed to load Item Database Object from the specified path: " + databasePath);
            return;
        }

        string[] itemGuids = AssetDatabase.FindAssets("t:ItemObject", new[] { scriptableObjectsRootPath });
        List<ItemObject> itemObjects = new List<ItemObject>();

        foreach (string guid in itemGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemObject itemObject = AssetDatabase.LoadAssetAtPath<ItemObject>(path);
            if (itemObject != null)
            {
                itemObjects.Add(itemObject);
            }
        }

        itemDatabase.ItemObjects = itemObjects;
        itemDatabase.UpdateID();
        itemDatabase.UpdateItemObjects();
        EditorUtility.SetDirty(itemDatabase);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Success",
            $"Item Database updated successfully.\n\nTotal items in database: {itemObjects.Count}.", "OK");
        Debug.Log("Item Database updated with " + itemObjects.Count + " items.");
    }
}