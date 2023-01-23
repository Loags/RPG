using UnityEngine;

[CreateAssetMenu(fileName = "UI Element", menuName = "New UI Element")]
public class UIElementSO : ScriptableObject
{ 
    public GameObject UIPrefab;

    public bool InstantiateAsNewCanvas;

    public string TargetTag;
    private GameObject _spawnedPrefab;

    private void Spawn(Transform target)
    {
        if (InstantiateAsNewCanvas)
        {
            _spawnedPrefab = Instantiate(UIPrefab);
            return;
        }
        _spawnedPrefab = Instantiate(UIPrefab, target);
    }

    private void Remove()
    {
        if (!_spawnedPrefab)
        {
            return;
        }
        
        MainCanvasUIItem item = _spawnedPrefab.GetComponent<MainCanvasUIItem>();
        
        if(item)
            item.HideAndDestroyItem(false);
        
        _spawnedPrefab = null;
    }

    public void UpdateItem(bool inTargetState)
    {
        if (InstantiateAsNewCanvas && inTargetState)
        {
            Spawn(null);
            return;
        }

        GameObject targetObject = GameObject.FindWithTag(TargetTag);

        if (!targetObject)
            return;

        Transform target = targetObject.transform;
        
        if (inTargetState)
        {
            Spawn(target);
            return;
        }
        
        Remove();
    }
}
