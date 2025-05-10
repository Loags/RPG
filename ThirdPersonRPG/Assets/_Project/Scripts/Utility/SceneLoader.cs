using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using LB_Manager_Coroutine;
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

        OnFlowStateChanged.Raise(8); // Switch to Inventory State so the Equipment can be load on start

        CoroutineManager.TerminateCoroutine(ref LoadScene, LoadAsyncScene(_sceneName), monoBehaviour);
    }
}
