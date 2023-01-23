using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager_Coroutine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int inventorySpace;
    public List<Item> items = new();

    [SerializeField] private GameObject inventoryPrefab;
    [SerializeField] private GameObject inventorySlotPrefab;
    [HideInInspector] public GameObject activeInventory;
    private GameObject inventoryAnchor;
    private InventoryUI InventoryUI;
    private Coroutine loadSlotsCoroutine;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        inventoryAnchor = GameObject.FindGameObjectWithTag("UI");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) ToggleInventory();

        #region Debug
        if (Input.GetKeyDown(KeyCode.O)) AddRandomItems();
        #endregion
    }

    private void ToggleInventory()
    {
        if (activeInventory != null)
        {
            Destroy(activeInventory);
            activeInventory = null;
        }
        else
        {
            activeInventory = Instantiate(inventoryPrefab, inventoryAnchor.transform);
            InventoryUI = activeInventory.GetComponent<InventoryUI>();
            CoroutineManager.InitializeCoroutine(ref loadSlotsCoroutine, LoadSlots(), PlayerController.instance.myMonoBehaviour);
        }

        Cursor.visible = activeInventory;
        Cursor.lockState = activeInventory ? CursorLockMode.None : CursorLockMode.Locked;
        PlayerController.instance.ToggleCameraInput();
    }

    private IEnumerator LoadSlots()
    {
        for (int i = 0; i < inventorySpace; i++)
        {
            if (i > 40)
                yield return new WaitForEndOfFrame(); // After the visible items in inventory have been loaded, delay the rest so it wont be laggy

            GameObject slot = Instantiate(inventorySlotPrefab, InventoryUI.ItemsParent);


            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
            InventoryUI.AssignInventorySlot(i, inventorySlot);

            if (i >= items.Count) continue;

            inventorySlot.AddItem(items[i]);
        }

        CoroutineManager.TerminateCoroutine(ref loadSlotsCoroutine, LoadSlots(), PlayerController.instance.myMonoBehaviour);
    }

    public bool Add(Item _item)
    {
        if (!_item.isDefaultItem)
        {
            if (items.Count >= inventorySpace)
            {
                Debug.Log("Not enough room in inventory");
                return false;
            }
            items.Add(_item);

            onItemChangedCallback?.Invoke();
        }
        return true;
    }

    public void Remove(Item _item)
    {
        items.Remove(_item);
        onItemChangedCallback?.Invoke();
    }


    #region Debug
    [SerializeField] private List<Item> possibleItems = new();

    private void AddRandomItems()
    {
        for (int i = 0; i < Random.Range(1, inventorySpace); i++)
            if (!Add(possibleItems[Random.Range(0, possibleItems.Count - 1)]))
                break;
    }
    #endregion
}
