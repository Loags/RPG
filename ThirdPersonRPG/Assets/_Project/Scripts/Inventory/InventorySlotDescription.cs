using TMPro;
using UnityEngine;

namespace LB.Inventory
{
    public class InventorySlotDescription : MonoBehaviour
    {
        [SerializeField] private TMP_Text descriptionHeader;
        [SerializeField] private TMP_Text descriptionContent;

        public void Initialize(string _descriptionHeader, string _descriptionContent)
        {
            descriptionHeader.text = _descriptionHeader;
            descriptionContent.text = _descriptionContent;

            if (descriptionHeader.text == string.Empty) descriptionHeader.enabled = false;
            if (descriptionContent.text == string.Empty) descriptionContent.enabled = false;
        }
    }
}