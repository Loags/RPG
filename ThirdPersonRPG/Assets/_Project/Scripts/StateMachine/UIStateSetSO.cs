using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State System/New UI Set")]
public class UIStateSetSO : StateSetSO<UIStateSO>
{
    public List<UIElementSO> DefaultUIElements = new List<UIElementSO>();
    public List<UIElementSO> CanvasElements = new List<UIElementSO>();
    
    public void UpdateFlowState()
    {
        foreach (UIStateSO state in StateObjects)  
        {
            state.FlowStateChanged();
        }
    }

    public void UpdateContentState()
    {
        foreach (UIStateSO state in StateObjects)  
        {
            state.ContentStateChanged();
        }
    }
}
 