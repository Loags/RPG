using System.Collections.Generic;
using UnityEngine;

namespace LB.Utilities
{
    public static class LBMath
    {
        /// <summary>
        /// Returns a random number between the specified minimum and maximum values,
        /// clamped by the count of the provided collection.
        /// </summary>
        /// <param name="collection">The collection used to determine the maximum possible count.</param>
        /// <param name="min">The minimum desired value.</param>
        /// <param name="max">The maximum desired value.</param>
        /// <returns>A random integer between clamped minimum and maximum values, inclusive.</returns>
        public static int GetRandomClampedCount<T>(ICollection<T> collection, int min, int max)
        {
            int maxLimit = Mathf.Max(1, collection.Count);

            int clampedMin = Mathf.Clamp(min, 1, maxLimit);
            int clampedMax = Mathf.Clamp(max, clampedMin, maxLimit);

            return Random.Range(clampedMin, clampedMax + 1);
        }

    }
}