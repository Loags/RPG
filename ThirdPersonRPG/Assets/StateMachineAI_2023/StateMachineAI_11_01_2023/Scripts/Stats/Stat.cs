using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private int baseValue;

    private List<int> modifiers = new();

    public int GetValue()
    {
        int finalValue = baseValue;
        modifiers.ForEach(x => finalValue += x);
        return finalValue;
    }

    public void AddModifier(int _modifier)
    {
        if (_modifier != 0)
            modifiers.Add(_modifier);
    }

    public void RemoveModifier(int _modifer)
    {
        if (_modifer != 0)
            modifiers.Remove(_modifer);
    }
}
