using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LB.Inventory
{
    [CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
    public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
    {
        private static ItemDatabaseObject _instance;

        public static ItemDatabaseObject Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.Log("Loading ItemDatabaseObject from Resources/Database...");
                    _instance = Resources.Load<ItemDatabaseObject>("Database");

                    if (_instance == null)
                        Debug.LogError("Failed to load ItemDatabaseObject from Resources/Database!");
                    else
                        Debug.Log("Successfully loaded ItemDatabaseObject.");
                }

                return _instance;
            }
            private set => _instance = value;
        }

        public List<ItemObject> ItemObjects;

        [Space(10)] [Header("Item Type Objects")] [SerializeField]
        private List<EquipmentObject> equipmentItemObjects = new();

        [SerializeField] private List<ConsumableObject> consumableItemObjects = new();
        [SerializeField] private List<DefaultObject> defaultItemObjects = new();

        [Space(10)] [Header("Equipment Type Objects")] [SerializeField]
        private List<WeaponObject> weaponItemObjects = new();

        [SerializeField] private List<ArmorObject> armorItemObjects = new();
        [SerializeField] private List<AccessoryObject> accessoryItemObjects = new();

        [Space(10)] [Header("Weapon Type Objects")] [SerializeField]
        private List<WeaponObject> meleeWeaponItemObjects = new();

        [Space(10)] [Header("Armor Type Objects")] [SerializeField]
        private List<ArmorObject> helmetItemObjects = new();

        [SerializeField] private List<ArmorObject> chestItemObjects = new();
        [SerializeField] private List<ArmorObject> glovesItemObjects = new();
        [SerializeField] private List<ArmorObject> legsItemObjects = new();
        [SerializeField] private List<ArmorObject> bootsItemObjects = new();

        [Space(10)] [Header("Accessory Type Objects")] [SerializeField]
        private List<AccessoryObject> necklaceItemObjects = new();

        [SerializeField] private List<AccessoryObject> earringItemObjects = new();
        [SerializeField] private List<AccessoryObject> ringItemObjects = new();


        public IReadOnlyList<EquipmentObject> EquipmentItemObjects => equipmentItemObjects.AsReadOnly();
        public IReadOnlyList<ConsumableObject> ConsumableItemObjects => consumableItemObjects.AsReadOnly();
        public IReadOnlyList<DefaultObject> DefaultItemObjects => defaultItemObjects.AsReadOnly();
        public IReadOnlyList<WeaponObject> WeaponItemObjects => weaponItemObjects.AsReadOnly();
        public IReadOnlyList<ArmorObject> ArmorItemObjects => armorItemObjects.AsReadOnly();
        public IReadOnlyList<AccessoryObject> AccessoryItemObjects => accessoryItemObjects.AsReadOnly();
        public IReadOnlyList<WeaponObject> MeleeWeaponItemObjects => meleeWeaponItemObjects.AsReadOnly();
        public IReadOnlyList<ArmorObject> HelmetItemObjects => helmetItemObjects.AsReadOnly();
        public IReadOnlyList<ArmorObject> ChestItemObjects => chestItemObjects.AsReadOnly();
        public IReadOnlyList<ArmorObject> GlovesItemObjects => glovesItemObjects.AsReadOnly();
        public IReadOnlyList<ArmorObject> LegsItemObjects => legsItemObjects.AsReadOnly();
        public IReadOnlyList<ArmorObject> BootsItemObjects => bootsItemObjects.AsReadOnly();
        public IReadOnlyList<AccessoryObject> NecklaceItemObjects => necklaceItemObjects.AsReadOnly();
        public IReadOnlyList<AccessoryObject> EarringItemObjects => earringItemObjects.AsReadOnly();
        public IReadOnlyList<AccessoryObject> RingItemObjects => ringItemObjects.AsReadOnly();


        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
                UpdateID();
                UpdateItemObjects();
            }
            else if (Instance != this)
            {
                Debug.LogWarning("Multiple instances of ItemDatabaseObject detected. Ensure only one instance exists.");
            }
        }


        public EquipmentObject GetRandomEquipment()
        {
            return equipmentItemObjects.TakeRandom();
        }

        public ConsumableObject GetRandomConsumable()
        {
            return consumableItemObjects.TakeRandom();
        }

        public DefaultObject GetRandomDefault()
        {
            return defaultItemObjects.TakeRandom();
        }

        public ItemObject GetRandomItemObject()
        {
            int randomIndex = Random.Range(0, 2);
            switch (randomIndex)
            {
                case 0:
                    return GetRandomEquipment();
                case 1:
                    return GetRandomConsumable();
                case 2:
                    return GetRandomDefault();
                default:
                    return null;
            }
        }

        public List<ItemObject> GetRandomItemObjects(int _amount)
        {
            List<ItemObject> itemObjects = new();
            for (int i = 0; i < _amount; i++)
            {
                itemObjects.Add(GetRandomItemObject());
            }

            return itemObjects;
        }

        [ContextMenu("Update ID's")]
        public void UpdateID()
        {
            for (int i = 0; i < ItemObjects.Count; i++)
            {
                if (ItemObjects[i].data.Id != i)
                    ItemObjects[i].data.Id = i;
            }
        }

        [ContextMenu("Update Item Objects")]
        public void UpdateItemObjects()
        {
            ClearAllLists();

            foreach (ItemObject itemObject in ItemObjects)
            {
                switch (itemObject)
                {
                    case EquipmentObject equipmentObject:
                        AddToEquipmentLists(equipmentObject);
                        break;
                    case ConsumableObject consumableObject:
                        consumableItemObjects.Add(consumableObject);
                        break;
                    case DefaultObject defaultObject:
                        defaultItemObjects.Add(defaultObject);
                        break;
                }
            }
        }

        private void ClearAllLists()
        {
            equipmentItemObjects.Clear();
            consumableItemObjects.Clear();
            defaultItemObjects.Clear();
            weaponItemObjects.Clear();
            armorItemObjects.Clear();
            accessoryItemObjects.Clear();
            meleeWeaponItemObjects.Clear();
            helmetItemObjects.Clear();
            chestItemObjects.Clear();
            glovesItemObjects.Clear();
            legsItemObjects.Clear();
            bootsItemObjects.Clear();
            necklaceItemObjects.Clear();
            earringItemObjects.Clear();
            ringItemObjects.Clear();
        }

        private void AddToEquipmentLists(EquipmentObject equipmentObject)
        {
            equipmentItemObjects.Add(equipmentObject);
            switch (equipmentObject)
            {
                case WeaponObject weaponObject:
                    AddToWeaponLists(weaponObject);
                    break;
                case ArmorObject armorObject:
                    AddToArmorLists(armorObject);
                    break;
                case AccessoryObject accessoryObject:
                    AddToAccessoryLists(accessoryObject);
                    break;
            }
        }

        private void AddToWeaponLists(WeaponObject weaponObject)
        {
            weaponItemObjects.Add(weaponObject);
            switch (weaponObject.weaponType)
            {
                case Weapon.MeleeWeapon:
                    meleeWeaponItemObjects.Add(weaponObject);
                    break;
                case Weapon.Unarmed:
                case Weapon.NONE:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddToArmorLists(ArmorObject armorObject)
        {
            armorItemObjects.Add(armorObject);
            switch (armorObject.armorType)
            {
                case Armor.Helmet:
                    helmetItemObjects.Add(armorObject);
                    break;
                case Armor.Chest:
                    chestItemObjects.Add(armorObject);
                    break;
                case Armor.Gloves:
                    glovesItemObjects.Add(armorObject);
                    break;
                case Armor.Legs:
                    legsItemObjects.Add(armorObject);
                    break;
                case Armor.Boots:
                    bootsItemObjects.Add(armorObject);
                    break;
                case Armor.NONE:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddToAccessoryLists(AccessoryObject accessoryObject)
        {
            accessoryItemObjects.Add(accessoryObject);
            switch (accessoryObject.accessoryType)
            {
                case Accessory.Ring:
                    ringItemObjects.Add(accessoryObject);
                    break;
                case Accessory.Earring:
                    earringItemObjects.Add(accessoryObject);
                    break;
                case Accessory.Necklace:
                    necklaceItemObjects.Add(accessoryObject);
                    break;
                case Accessory.NONE:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void OnValidate()
        {
            UpdateItemObjects();
        }

        public void OnAfterDeserialize()
        {
            UpdateID();
            UpdateItemObjects();
        }

        public void OnBeforeSerialize()
        {
        }
    }
}