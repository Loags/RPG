using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// It maps UIElements to a target state and is responsible for loading them.
/// </summary>
[CreateAssetMenu(menuName = "State System/New UI State")]
public class UIStateSO : ScriptableObject
{
    public bool IsContentDependent = false;

    public AppStates.ContentStates TargetContentState;
    public AppStates.FlowStates TargetFlowState;

    public List<UIElementSO> UIItems = new List<UIElementSO>();

    public void ContentStateChanged()
    {
        if(IsContentDependent)
            UpdateUIItems(StateManager.Instance.CurrentContentState == TargetContentState);
    }

    public void FlowStateChanged()
    {
        if(!IsContentDependent)
            UpdateUIItems(StateManager.Instance.CurrentFlowState == TargetFlowState);
    }

    private void UpdateUIItems(bool inTargetState)
    {
        if (UIItems.Count < 1)
            return;
        //TODO: Change this to use SOs instead. Loop through all SOs and Instantiate items, then show.
        //TODO: Objects now need to be destroyed after fading out
        foreach (var item in UIItems.Where(item => item != null))
            item.UpdateItem(inTargetState);
    }
}
