using System.Collections;
using UnityEngine;

namespace Manager_Coroutine
{
    public static class CoroutineManager
    {
        public static void InitializeCoroutine(ref Coroutine _coroutine, IEnumerator _enumerator, MonoBehaviour behaviour)
        {
            if (_coroutine != null)
            {
                behaviour.StopCoroutine(_coroutine);
                _coroutine = null;
            }
            _coroutine = behaviour.StartCoroutine(_enumerator);

            //Debug.Log("[Started Coroutine]  --->  " + _enumerator.ToString());
        }
        public static void TerminateCoroutine(ref Coroutine _coroutine, IEnumerator _enumerator, MonoBehaviour behaviour)
        {
            if (_coroutine == null) return;

            behaviour.StopCoroutine(_coroutine);
            _coroutine = null;
            //Debug.Log("[Stopped Coroutine]  --->  " + _enumerator.ToString());
        }
    }
}