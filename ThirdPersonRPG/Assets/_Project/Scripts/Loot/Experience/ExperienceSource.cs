using UnityEngine;

namespace LB.Loot.Experience
{
    /// <summary>
    /// Base class for all experience sources in the game.
    /// This ScriptableObject allows for easy configuration of experience rewards
    /// from different sources like enemies, quests, or items.
    /// </summary>
    public abstract class ExperienceSource : ScriptableObject
    {
        [SerializeField] protected float baseExperienceAmount;
        [SerializeField] protected float experienceMultiplier = 1f;
        
        /// <summary>
        /// Gets the final experience amount after applying all multipliers.
        /// </summary>
        /// <returns>The calculated experience amount.</returns>
        public virtual float GetExperienceAmount()
        {
            return baseExperienceAmount * experienceMultiplier;
        }
    }
} 