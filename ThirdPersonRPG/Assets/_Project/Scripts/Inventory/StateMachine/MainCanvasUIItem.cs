using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class MainCanvasUIItem : MonoBehaviour
{
    [SerializeField]private CanvasGroup _canvasGroup;

    [SerializeField]
    private bool EnabledOnStart;

    [SerializeField]
    private float FadeTime = 1.0f;

    [SerializeField]
    private bool SetBeforeStarting;

    private void Start()
    {

        _canvasGroup = GetComponent<CanvasGroup>();

        if (SetBeforeStarting) return;
        
        //This is a convenience step, so that we do not need to manually set the canvas groups of every item in the editor!
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        HandleCanvasGroup(Convert.ToInt32(EnabledOnStart), EnabledOnStart, EnabledOnStart);
    }

    public virtual void ShowItem(bool inTargetState) => HandleCanvasGroup(Convert.ToInt32(inTargetState), inTargetState, inTargetState);
    
    public virtual void HideAndDestroyItem(bool inTargetState) => HandleCanvasGroup(Convert.ToInt32(inTargetState), inTargetState, inTargetState, true);

    public virtual void ShowItemWithReverseBool(bool inTargetState) => HandleCanvasGroup(Convert.ToInt32(!inTargetState), !inTargetState, !inTargetState);
    
    private void HandleCanvasGroup(float alpha, bool interactable, bool blocksRaycasts, bool destroyAfterFade = false)
    {
        if (!_canvasGroup)
            return;
        SetBeforeStarting = true;
        // Create Fade In / Out 
        _canvasGroup.interactable = interactable;
        _canvasGroup.blocksRaycasts = blocksRaycasts;
    }

    private void DestroyItem(bool destroy)
    {
        if (!destroy)
            return;
        
        Destroy(gameObject);
    }
}
