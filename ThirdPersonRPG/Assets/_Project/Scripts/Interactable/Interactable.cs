using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool isFocus = false;
    public bool hasInteracted = false;

    private MeshRenderer meshRenderer;
    private Material normalMat;
    [SerializeField] private Material highlightMat;

    private Transform player;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        normalMat = meshRenderer.material;
    }

    public virtual void Interact()
    {
        Debug.DrawRay(transform.position, Vector3.up * 10, Color.blue, float.MaxValue);
        hasInteracted = true;
    }

    public void OnFocus(Transform _playerTransform)
    {
        isFocus = true;
        player = _playerTransform;
        hasInteracted = false;
        meshRenderer.material = highlightMat;
    }

    public void OnDefocus()
    {
        isFocus = false;
        hasInteracted = false;
        meshRenderer.material = normalMat;
    }

    private void OnDestroy()
    {
        if (!hasInteracted) return;
        PlayerController.instance.PlayerInteraction.RemoveInteractable(this);
    }
}
