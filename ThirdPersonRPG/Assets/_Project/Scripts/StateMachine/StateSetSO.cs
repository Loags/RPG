using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSetSO<T> : ScriptableObject
{
    public List<T> StateObjects = new List<T>();
}
 