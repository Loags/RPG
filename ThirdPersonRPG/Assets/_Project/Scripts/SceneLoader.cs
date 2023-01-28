using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Manager_Coroutine;
using Events;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private IntEvent OnFlowStateChanged;
    [SerializeField] private float delaySceneChange;
    private Coroutine LoadScene;
    private MonoBehaviour monoBehaviour;

    private void Start()
    {
        monoBehaviour = GetComponent<MonoBehaviour>();
    }

    public void LoadSceneAsync(string _sceneName) // Called by Event Listener
    {
        CoroutineManager.InitializeCoroutine(ref LoadScene, LoadAsyncScene(_sceneName), monoBehaviour);
    }

    IEnumerator LoadAsyncScene(string _sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneName);

        while (!asyncLoad.isDone) // Wait until the asynchronous scene fully loads
        {
            yield return null;
        }

        yield return new WaitForSeconds(delaySceneChange);

        OnFlowStateChanged.Raise(7); // Switch to InGame State

        CoroutineManager.TerminateCoroutine(ref LoadScene, LoadAsyncScene(_sceneName), monoBehaviour);
    }
}
