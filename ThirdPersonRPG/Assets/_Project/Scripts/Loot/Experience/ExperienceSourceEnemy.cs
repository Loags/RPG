using UnityEngine;

namespace LB.Loot.Experience
{
    /// <summary>
    /// Experience source specifically for enemies.
    /// Allows for enemy-specific experience scaling and configuration.
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyExperienceSource", menuName = "RPG/Experience/EnemyExperienceSource")]
    public class ExperienceSourceEnemy : ExperienceSource
    {
        [SerializeField] private float levelScaling = 1f;
        [SerializeField] private EnemyType enemyType;
        
        /// <summary>
        /// Gets the final experience amount after applying enemy-specific scaling.
        /// </summary>
        /// <returns>The calculated experience amount.</returns>
        public override float GetExperienceAmount()
        {
            return base.GetExperienceAmount() * levelScaling;
        }
    }
} 