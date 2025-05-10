using System;
using UnityEngine;
using System.IO;
using UnityEditor;
using LB.Utilities;

namespace LB.Inventory
{
    public enum InterfaceType
    {
        Inventory,
        Equipment,
        StorageBox
    }


    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
    public class InventoryObject : ScriptableObject
    {
        public string savePath;

        private const string itemDatabaseResourcePath = "Database"; // Path to the ItemDatabaseObject in Resources folder

        [NonSerialized] public ItemDatabaseObject database;
        public InterfaceType type;
        public Inventory Container;

        [SerializeField][ReadOnly] private bool isBatchSaveMode = false;
        [SerializeField][ReadOnly] private string folderPath = "Assets/_Project/ScriptableObjects/_Inventory/AllowedItems";
        [SerializeField][ReadOnly] private AllowedItem[] loadedScriptableObjects;

        private void OnValidate()
        {
            if (type == InterfaceType.Equipment)
            {
                LoadAllowedItems();
            }
        }

        public InventorySlot[] GetSlots
        {
            get { return Container.Slots; }
        }

        public bool AddItem(Item _item, int _amount)
        {
            int remainingAmount = TryAddItem(_item, _amount);
            return remainingAmount == 0;
        }

        public int TryAddItem(Item _item, int _amount)
        {
            int maxStackAmount = database.ItemObjects[_item.Id].stackAmount;
            int remainingAmount = _amount;

            // Check if item is stackable
            if (database.ItemObjects[_item.Id].stackable)
            {
                InventorySlot slot = FindItemWithSpace(_item, maxStackAmount);

                while (slot != null && remainingAmount > 0)
                {
                    int availableSpace = maxStackAmount - slot.amount;
                    int amountToAdd = Math.Min(remainingAmount, availableSpace);

                    slot.AddAmount(amountToAdd);
                    remainingAmount -= amountToAdd;

                    if (remainingAmount > 0)
                    {
                        slot = FindItemWithSpace(_item, maxStackAmount);
                    }
                }
            }

            // If item is not stackable or remaining amount still exists, find empty slots
            while (remainingAmount > 0 && EmptySlotCount > 0)
            {
                int amountToAdd = Math.Min(remainingAmount, maxStackAmount);
                SetEmptySlot(_item, amountToAdd);
                remainingAmount -= amountToAdd;
            }

            Save();
            return remainingAmount; // Return the amount that could not be added
        }

        public int EmptySlotCount
        {
            get
            {
                int counter = 0;
                for (int i = 0; i < GetSlots.Length; i++)
                {
                    if (GetSlots[i].item.Id <= -1)
                        counter++;
                }

                return counter;
            }
        }

        public InventorySlot FindItemOnInventory(Item _item)
        {
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.Id == _item.Id)
                {
                    return GetSlots[i];
                }
            }

            return null;
        }

        public InventorySlot FindItemWithSpace(Item _item, int maxStackAmount)
        {
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.Id == _item.Id && GetSlots[i].amount < maxStackAmount)
                {
                    return GetSlots[i];
                }
            }

            return null;
        }

        public InventorySlot SetEmptySlot(Item _item, int _amount)
        {
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.Id <= -1)
                {
                    GetSlots[i].UpdateSlot(_item, _amount);
                    return GetSlots[i];
                }
            }

            // TODO: Inventory full
            return null;
        }

        public int GetNextFreeInventorySlot()
        {
            for (int i = 0; i < Container.Slots.Length; i++)
            {
                InventorySlot slot = Container.Slots[i];
                if (slot.item.Id <= -1)
                    return i;
            }

            return -1;
        }

        public void SwapItems(InventorySlot _item1, InventorySlot _item2)
        {
            if (_item2.CanPlaceInSlot(_item1.ItemObject) && _item1.CanPlaceInSlot(_item2.ItemObject))
            {
                InventorySlot temp = new(_item2.item, _item2.amount);
                _item2.UpdateSlot(_item1.item, _item1.amount);
                _item1.UpdateSlot(temp.item, temp.amount);
            }

            Save();
        }

        public void RemoveItem(Item _item)
        {
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item == _item)
                {
                    GetSlots[i].UpdateSlot(new Item(), 0);
                }
            }

            Save();
        }

        public void StartBatchSave()
        {
            isBatchSaveMode = true;
        }

        public void EndBatchSave()
        {
            isBatchSaveMode = false;
            Save();
        }


        #region ContextMenu Functions

        [ContextMenu("Save")]
        public void Save()
        {
            if (isBatchSaveMode) return;

            // Convert the InventoryObject to JSON
            string jsonData = JsonUtility.ToJson(this, true);

            // Define the save path (use Path.Combine for platform-independent paths)
            string saveFilePath = Application.persistentDataPath + savePath;

            // Write the JSON data to a file
            File.WriteAllText(saveFilePath, jsonData);

            Debug.Log($"Saved Data to {saveFilePath}");

            #region Previous Save Method

            // #if UNITY_EDITOR
            //             EditorUtility.SetDirty(this);
            //             AssetDatabase.SaveAssets();
            //             AssetDatabase.Refresh();
            // #endif
            //
            //             // Make the data editable
            //             string saveData = JsonUtility.ToJson(this, true);
            //             BinaryFormatter bf = new();
            //             FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
            //             bf.Serialize(file, saveData);
            //             file.Close();
            //
            //             // Make the data NOT editable
            //             //IFormatter formatter = new BinaryFormatter();
            //             //Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
            //             //formatter.Serialize(stream, Container);
            //             //stream.Close();

            #endregion
        }

        [ContextMenu("Load")]
        public void Load()
        {
            // Load the ItemDatabaseObject at runtime
            database = Resources.Load<ItemDatabaseObject>(itemDatabaseResourcePath);

            // Check if the database has been loaded
            if (database != null)
            {
                Debug.Log("ItemDatabaseObject loaded successfully!");
            }
            else
            {
                Debug.LogError("ItemDatabaseObject could not be loaded!");
            }

            // Define the load path
            string loadFilePath = Application.persistentDataPath + savePath;

            // Check if the save file exists
            if (File.Exists(loadFilePath))
            {
                // Read the JSON data from the file
                string jsonData = File.ReadAllText(loadFilePath);

                // Deserialize the JSON data into this InventoryObject
                JsonUtility.FromJsonOverwrite(jsonData, this);
                RestoreAllowedItems();
            }

            #region Previous Load Method

            // if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
            // {
            //     // Make the data editable
            //     BinaryFormatter bf = new BinaryFormatter();
            //     FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            //     JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            //     file.Close();
            //
            //     // Make the data NOT editable
            //     //IFormatter formatter = new BinaryFormatter();
            //     //Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            //     //Inventory newContainer = (Inventory)formatter.Deserialize(stream);
            //     //for (int i = 0; i < Container.Items.Length; i++)
            //     //{
            //     //    Container.Items[i].UpdateSlot(newContainer.Items[i].item, newContainer.Items[i].amount);
            //     //}
            //     //stream.Close();
            // }

            #endregion
        }


        [ContextMenu("Clear")]
        public void Clear()
        {
            Container.Clear();
        }

        private void RestoreAllowedItems()
        {
            if (type == InterfaceType.Equipment)
            {
                LoadAllowedItems();
            }
        }

        [ContextMenu("Load Allowed Items")]
        public void LoadAllowedItems()
        {
            // Ensure folder path is valid
            if (string.IsNullOrEmpty(folderPath))
            {
                Debug.LogError("Folder path is empty or null.");
                return;
            }

            // Get all asset guids in the specified folder
            string[] assetGuids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { folderPath });

            // Load all ScriptableObjects from the asset guids
            loadedScriptableObjects = new AllowedItem[assetGuids.Length];
            for (int i = 0; i < assetGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
                loadedScriptableObjects[i] = AssetDatabase.LoadAssetAtPath<AllowedItem>(assetPath);
            }


            for (int index = 0; index < Container.Slots.Length; index++)
            {
                Container.Slots[index].AllowedItems.Clear();
                switch (index)
                {
                    case 0:
                        Container.Slots[index].AllowedItems.Add(loadedScriptableObjects[6]);
                        break;
                    case 1:
                        Container.Slots[index].AllowedItems.Add(loadedScriptableObjects[4]);
                        break;
                    case 2:
                        Container.Slots[index].AllowedItems.Add(loadedScriptableObjects[5]);
                        break;
                    case 3:
                        Container.Slots[index].AllowedItems.Add(loadedScriptableObjects[7]);
                        break;
                    case 4:
                        Container.Slots[index].AllowedItems.Add(loadedScriptableObjects[3]);
                        break;
                    case 5:
                        Container.Slots[index].AllowedItems.Add(loadedScriptableObjects[1]);
                        break;
                    case 6:
                        Container.Slots[index].AllowedItems.Add(loadedScriptableObjects[0]);
                        break;
                    case 7:
                        Container.Slots[index].AllowedItems.Add(loadedScriptableObjects[2]);
                        break;
                    case 8:
                        Container.Slots[index].AllowedItems.Add(loadedScriptableObjects[8]);
                        Container.Slots[index].AllowedItems.Add(loadedScriptableObjects[9]);
                        break;
                }
            }
        }

        #endregion ContextMenu Functions
    }

    public delegate void SlotUpdated(InventorySlot _slot);
}