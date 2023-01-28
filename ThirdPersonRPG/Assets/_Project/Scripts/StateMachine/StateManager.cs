using UnityEngine;
using UnityEngine.SceneManagement;
using Events;


public class StateManager : MonoBehaviour
{
    public static StateManager Instance;

    [SerializeField]
    private ApplicationEvent OnStateChangedEvent;

    public delegate void FlowStateChanged();
    public event FlowStateChanged OnFlowStateChanged;

    public delegate void ContentStateChanged();
    public event ContentStateChanged OnContentStateChanged;

    public AppStates.FlowStates CurrentFlowState;
    public AppStates.ContentStates CurrentContentState;

    public UIStateSetSO StateSet;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        SceneManager.sceneLoaded += LoadDefaultUI;
#if UNITY_EDITOR
        LoadDefaultUI();
#endif
    }

    private void LoadDefaultUI(Scene loadedScene, LoadSceneMode mode)
    {
        if (loadedScene.name != "StartScene")
            return;

        LoadDefaultUI();
    }

    private void LoadDefaultUI()
    {
        ChangeAppFlowState(AppStates.FlowStates.INITIALIZATION);

        foreach (UIElementSO canvasElement in StateSet.CanvasElements)
        {
            canvasElement.UpdateItem(true);
        }
        foreach (UIElementSO stateSetDefaultUIElement in StateSet.DefaultUIElements)
        {
            stateSetDefaultUIElement.UpdateItem(true);
        }

        ChangeAppFlowState(AppStates.FlowStates.MAINMENU);
    }

    public void ChangeAppFlowState(int stateIndex) => ChangeAppFlowState((AppStates.FlowStates)stateIndex);

    public void ChangeAppFlowState(AppStates.FlowStates stateToChangeTo)
    {
        if (stateToChangeTo == CurrentFlowState)
            return;

        CurrentFlowState = stateToChangeTo;
        OnStateChangedEvent.Raise();
        OnFlowStateChanged?.Invoke();
        StateSet.UpdateFlowState();
    }

    public void ChangeAppContentState(int stateIndex) => ChangeAppContentState((AppStates.ContentStates)stateIndex);

    public void ChangeAppContentState(AppStates.ContentStates stateToChangeTo)
    {
        if (stateToChangeTo == CurrentContentState)
            return;

        CurrentContentState = stateToChangeTo;
        OnContentStateChanged?.Invoke();
        StateSet.UpdateContentState();
    }
}
