using System;
using System.Collections.Generic;
using System.Linq;

namespace LB.Utilities
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets all individual flags from a flags enum
        /// </summary>
        public static IEnumerable<T> GetFlags<T>(this T flags) where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Where(flag => flags.HasFlag(flag));
        }
    }
} 