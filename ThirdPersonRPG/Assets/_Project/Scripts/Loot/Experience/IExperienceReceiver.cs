using UnityEngine;

namespace LB.Loot.Experience
{
    public interface IExperienceReceiver
    {
        void AddExperience(float amount);
        bool IsAlive { get; }
        Transform transform { get; }
        
        /// <summary>
        /// The faction this character belongs to
        /// </summary>
        CharacterFaction Faction { get; }
    }
} 