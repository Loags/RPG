using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LB.Inventory
{
    public interface IModifier
    {
        void AddValue(ref int baseValue);
    }
}