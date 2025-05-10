using LB;
using UnityEngine;

public class RPGCharacterPreLoadBlackScreen : MonoBehaviour
{
    [SerializeField] private GameObject blackScreenImage;
    private RPGCharacterController rPGCharacterController;

    private void Awake()
    {
        rPGCharacterController = GetComponentInParent<RPGCharacterController>();
        rPGCharacterController.OnPreLoad += ToggleBlackScreen;
    }

    private void ToggleBlackScreen()
    {
        blackScreenImage.SetActive(!rPGCharacterController.IsPreLoadFinished);
        rPGCharacterController.rpgCharacterInventoryController.ToggleInventory();
    }
}