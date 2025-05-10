using UnityEngine;

namespace LB
{
    public class UILookAt : MonoBehaviour
    {
        void LateUpdate()
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up);
        }
    }
}