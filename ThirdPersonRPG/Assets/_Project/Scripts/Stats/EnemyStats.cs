using UnityEngine;
using LB.Utilities;
using LB.Loot;
using LB.Loot.Experience;

namespace LB
{
    public class EnemyStats : CharacterStats
    {
        private Animator animator;

        [Header("Experience")]
        [SerializeField] private EnemyType enemyType;
        [SerializeField] private float experienceValue = 100f;
        [SerializeField] private float experienceRadius = 5f;

        private IExperienceReceiver lastDamageSource;

        public IExperienceReceiver LastDamageSource => lastDamageSource;
        public override CharacterFaction Faction => CharacterFaction.EnemyFaction;
        public EnemyType EnemyType => enemyType;
        public float ExperienceValue => experienceValue;
        public float ExperienceRadius => experienceRadius;

        public void Awake()
        {
            animator = GetComponent<Animator>();
        }

        protected override void Die()
        {
            base.Die();
            SpawnLoot();
            DistributeExperience();
            DistributeCurrency();
            Destroy(gameObject);
        }

        private void SpawnLoot()
        {
            LootManager.Instance.SpawnLoot(enemyType, transform.position);
        }

        private void DistributeExperience()
        {
            var config = LootDatabase.Instance.GetLootConfiguration(enemyType);
            if (config == null)
            {
                var template = LootDatabase.Instance.GetDefaultTemplateForEnemyType(enemyType);
                if (template != null)
                {
                    config = new LootDatabase.LootConfiguration();
                    var field = typeof(LootDatabase.LootConfiguration).GetField("enemyType",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);
                    field?.SetValue(config, enemyType);

                    field = typeof(LootDatabase.LootConfiguration).GetField("lootTemplate",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);
                    field?.SetValue(config, template);
                }
            }

            if (config?.ExperienceSource != null)
            {
                ExperienceDistributorManager.Instance.DistributeExperience(
                    config.ExperienceSource.GetExperienceAmount(),
                    transform.position,
                    LastDamageSource);
            }
            else
            {
                Debug.LogWarning($"No experience source found for {enemyType}");
            }
        }

        private void DistributeCurrency()
        {
            LootManager.Instance.DistributeCurrency(EnemyType);
        }

        public override void SetUpAttributes()
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                attributes[i].SetParent(characterStats);
            }
        }

        public override void TakeDamage(int _damage, IExperienceReceiver experienceReceiver = null)
        {
            if (experienceReceiver != null)
            {
                // Try to get the experience receiver from the damage source
                lastDamageSource = experienceReceiver;
            }
            base.TakeDamage(_damage, experienceReceiver);
        }
    }
}