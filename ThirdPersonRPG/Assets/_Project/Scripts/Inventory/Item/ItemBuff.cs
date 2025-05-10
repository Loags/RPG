using System;
using Random = UnityEngine.Random;

namespace LB.Inventory
{
    [Serializable]
    public class ItemBuff : IModifier
    {
        public Attributes attribute;
        public int value;
        public int min;
        public int max;

        public ItemBuff(int _min, int _max)
        {
            this.min = _min;
            this.max = _max;
            GenerateValue();
        }

        public void AddValue(ref int baseValue) => baseValue += value;

        public void GenerateValue() => value = Random.Range(min, max);
    }
}