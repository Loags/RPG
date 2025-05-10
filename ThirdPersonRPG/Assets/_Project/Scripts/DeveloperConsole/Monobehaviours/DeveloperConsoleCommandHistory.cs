using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeveloperConsoleCommandHistory : MonoBehaviour
{
    [SerializeField] private Transform consoleHistory;
    [SerializeField] private Transform consoleDebugHistory;
    [SerializeField] private GameObject developerCommandHistoryValue;

    [TextArea(5, 10)] [SerializeField] private string prefixHistory;
    [TextArea(5, 10)] [SerializeField] private string prefixDebugHistory;

    private List<GameObject> consoleHistoryValues = new();
    private List<GameObject> consoleDebugHistoryValues = new();

    public void AddConsoleHistoryEntry(string inputValue) => SpawnPrefab(inputValue, false);
    public void AddConsoleDebugHistoryEntry(string inputValue) => SpawnPrefab(inputValue, true);

    private void SpawnPrefab(string inputValue, bool isDebugEntry)
    {
        GameObject spawnedObject;
        string prefix = string.Empty;

        if (isDebugEntry)
        {
            spawnedObject = Instantiate(developerCommandHistoryValue, consoleDebugHistory);
            consoleDebugHistoryValues.Add(spawnedObject);
            prefix = prefixHistory;
        }
        else
        {
            spawnedObject = Instantiate(developerCommandHistoryValue, consoleHistory);
            consoleHistoryValues.Add(spawnedObject);
            prefix = prefixDebugHistory;
        }

        spawnedObject.GetComponentInChildren<TMP_Text>().text = prefix + inputValue;
    }
}