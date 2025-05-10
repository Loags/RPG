using System.Collections;
using UnityEngine;
using LB.Utilities;
using LB.Loot.Experience;
using LB.Inventory;

namespace LB
{
    public abstract class CharacterStats : MonoBehaviour, IExperienceReceiver
    {
        [SerializeField] protected bool debugDamage;
        public Attribute[] attributes;
        protected CharacterStats characterStats;

        public delegate void OnModifierChanged();
        public OnModifierChanged onModifierChanged;

        [Header("Health")]
        [SerializeField][ReadOnly] private float HealthRegenerationIntervals = 2f;
        [ReadOnly] public bool IsCharacterDead;
        [field: SerializeField] public int CurrentHealth { get; private set; }
        [field: SerializeField] public int CurrentHealthRegeneration { get; private set; }

        [Header("Stamina")]
        [SerializeField][ReadOnly] private float StaminaRegenerationIntervals = 2f;
        [field: SerializeField] public int CurrentStamina { get; private set; }
        [field: SerializeField] public int CurrentStaminaRegeneration { get; private set; }

        [Header("Experience Settings")]
        [field: SerializeField] protected float baseExperienceRequired { get; private set; } = 100f;
        [field: SerializeField] protected float experienceMultiplier { get; private set; } = 1.5f;

        public event System.Action<int> OnLevelUp;
        public event System.Action<float> OnExperienceGained;


        public float GetCurrentExperience => GetMaxValueOfAttributeType(Attributes.Experience);
        public int GetCurrentLevel => GetMaxValueOfAttributeType(Attributes.Level);
        public float GetExperienceToNextLevel => CalculateExperienceForLevel(GetCurrentLevel + 1);

        public bool IsAlive => !IsCharacterDead;
        public abstract CharacterFaction Faction { get; }

        public virtual void Start()
        {
            characterStats = this;
            SetUpAttributes();

            CurrentHealth = GetMaxValueOfAttributeType(Attributes.Health);
            CurrentStamina = GetMaxValueOfAttributeType(Attributes.Stamina);
            CurrentHealthRegeneration = GetMaxValueOfAttributeType(Attributes.RegenerationHealth);
            CurrentStaminaRegeneration = GetMaxValueOfAttributeType(Attributes.RegenerationStamina);

            StartCoroutine(RegenerateHealth());
            StartCoroutine(RegenerateStamina());
        }

        private void OnDestroy()
        {
            StopCoroutine(RegenerateHealth());
            StopCoroutine(RegenerateStamina());
        }

        public virtual void TakeDamage(int _damage, IExperienceReceiver experienceReceiver = null)
        {
            if (IsCharacterDead) return;
            _damage -= GetMaxValueOfAttributeType(Attributes.Armor);
            _damage = Mathf.Clamp(_damage, 0, int.MaxValue);

            if (debugDamage)
            {
                Debug.Log($"Character {gameObject}" +
                          $"\nHealth: {CurrentHealth}" +
                          $"\nArmor: {GetMaxValueOfAttributeType(Attributes.Armor)}" +
                          $"\nDamage: {_damage}" +
                          $"\nHealth after damage: {CurrentHealth - _damage}");
            }

            CurrentHealth -= _damage;

            TakeDamageAnimation();

            if (CurrentHealth <= 0)
            {
                Die();
                IsCharacterDead = true;
            }

            onModifierChanged?.Invoke();
        }

        public void TakeStaminaDamage(int _damage)
        {
            if (IsCharacterDead) return;

            CurrentStamina -= _damage;
            onModifierChanged?.Invoke();
        }

        public void AddHealth(int _amount)
        {
            _amount = Mathf.Abs(_amount);
            if (IsCharacterDead) return;

            CurrentHealth += _amount;
            if (CurrentHealth > GetMaxValueOfAttributeType(Attributes.Health))
                CurrentHealth = GetMaxValueOfAttributeType(Attributes.Health);
            onModifierChanged?.Invoke();
        }

        public void AddStamina(int _amount)
        {
            _amount = Mathf.Abs(_amount);
            if (IsCharacterDead) return;

            CurrentStamina += _amount;
            if (CurrentStamina > GetMaxValueOfAttributeType(Attributes.Stamina))
                CurrentStamina = GetMaxValueOfAttributeType(Attributes.Stamina);
            onModifierChanged?.Invoke();
        }

        public virtual void TakeDamageAnimation()
        {
            // Override this func to apply animation
        }

        protected virtual void Die()
        {
            // Override this func to apply animation
        }

        public virtual void SetUpAttributes()
        {
            // Override this func to apply correct attribute stats ( Check PlayerStats )
        }

        public virtual void Respawn()
        {
            IsCharacterDead = false;
            CurrentHealth = GetMaxValueOfAttributeType(Attributes.Health);
            CurrentStamina = GetMaxValueOfAttributeType(Attributes.Stamina);
            // Override this func to modify the respawn of different character types. But always include the base.Respawn();
        }

        protected int GetIndexOfAttributeInList(Attributes _type)
        {
            for (int i = 0; i < attributes.Length; i++)
                if (_type == attributes[i].type)
                    return i;

            return -1;
        }

        public int GetMaxValueOfAttributeType(Attributes _type)
        {
            int index = GetIndexOfAttributeInList(_type);

            if (index < 0) return -1;

            return attributes[index].value.ModifiedValue;
        }

        // This is being called everytime the modified playerstats changes
        public void AttributeModified(Attribute attribute)
        {
            onModifierChanged?.Invoke();
        }

        private IEnumerator RegenerateHealth()
        {
            while (!IsCharacterDead)
            {
                yield return new WaitForSeconds(HealthRegenerationIntervals);
                AddHealth(CurrentHealthRegeneration);
            }
        }

        private IEnumerator RegenerateStamina()
        {
            while (!IsCharacterDead)
            {
                yield return new WaitForSeconds(StaminaRegenerationIntervals);
                AddStamina(CurrentStaminaRegeneration);
            }
        }

        protected void SetAttributeValue(Attributes type, int value)
        {
            int index = GetIndexOfAttributeInList(type);
            if (index >= 0)
            {
                attributes[index].value.BaseValue = value;
            }
        }

        public void AddExperience(float amount)
        {
            if (IsCharacterDead) return;

            amount *= GetMaxValueOfAttributeType(Attributes.ExperienceMultiplier);
            SetAttributeValue(Attributes.Experience, GetMaxValueOfAttributeType(Attributes.Experience) + (int)amount);

            OnExperienceGained?.Invoke(amount);

            if (GetCurrentExperience >= GetExperienceToNextLevel)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            int currentExp = GetMaxValueOfAttributeType(Attributes.Experience);
            SetAttributeValue(Attributes.Experience, currentExp - (int)GetExperienceToNextLevel);

            int currentLevel = GetMaxValueOfAttributeType(Attributes.Level);
            SetAttributeValue(Attributes.Level, currentLevel + 1);

            OnLevelUp?.Invoke(currentLevel + 1);
        }

        private float CalculateExperienceForLevel(int level)
        {
            return baseExperienceRequired * Mathf.Pow(experienceMultiplier, level - 1);
        }
    }

    [System.Serializable]
    public class Attribute
    {
        [System.NonSerialized] public CharacterStats parent;
        public Attributes type;
        public ModifiableInt value;

        public void SetParent(CharacterStats _characterStats)
        {
            parent = _characterStats;
            int baseValue = value.BaseValue;
            value = new ModifiableInt(AttributeModified);
            value.BaseValue = baseValue;
        }

        public void AttributeModified()
        {
            parent.AttributeModified(this);
        }
    }
}