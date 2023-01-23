using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_EnemyCombat : MonoBehaviour
{
    private SM_EnemyController SM_EnemyController;

    [Space(15)]
    [Header("Attack stats")]
    [SerializeField] private float damage;
    [SerializeField] private float critChance;
    [SerializeField] private float critDamageMultiplier;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackRange;
    private bool canAttack;
    private bool isAttacking;
    private bool inAttackRange;
    private Coroutine attackCoroutine;
    private Coroutine attackCooldownCoroutine;
    private Coroutine inAttackRangeCoroutine;


    [Space(15)]
    [Header("Health stats")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float healthRegenRate;

    [Space(15)]
    [Header("Stamina stats")]
    [SerializeField] private float currentStamina;
    [SerializeField] private float maxStamina;
    [SerializeField] private float staminaRegenRate;
    [SerializeField] private float staminaBlockCosts;

    [Space(15)]
    [Header("Defensive stats")]
    [SerializeField] private float blockChance;
    private bool isBlocking;

    private void Awake() { SM_EnemyController = GetComponent<SM_EnemyController>(); }

    private void OnEnable()
    {
        SM_EnemyController.OnEnemyContentStateChanged += CheckAttackState;
        SM_EnemyController.OnEnemyContentStateChanged += EndInAttackRangeCheck;
    }

    private void OnDisable()
    {
        SM_EnemyController.OnEnemyContentStateChanged -= CheckAttackState;
        SM_EnemyController.OnEnemyContentStateChanged -= EndInAttackRangeCheck;
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    private void CheckAttackState()
    {
        if (SM_EnemyController.GetCurrentContentState() != EnemyContentState.ATTACK)
        {
            inAttackRange = false;
            return;
        }

        if (inAttackRangeCoroutine == null)
            SM_EnemyController.InitializeCoroutine(ref inAttackRangeCoroutine, InAttackRangeCheck()); // Start inAttackRangeCheck

        inAttackRange = true;

        if (inAttackRange && canAttack && !isAttacking)
            SM_EnemyController.InitializeCoroutine(ref attackCoroutine, Attack());
    }

    private void EndInAttackRangeCheck()
    {
        if (SM_EnemyController.GetCurrentContentState() != EnemyContentState.RESETPHASE) return;

        SM_EnemyController.TerminateCoroutine(ref inAttackRangeCoroutine, InAttackRangeCheck()); // End inAttackRangeCheck
    }


    public IEnumerator Attack()
    {
        SM_EnemyController.ChangeContentState(EnemyContentState.ATTACK);
        isAttacking = true;

        SM_EnemyController.InitializeCoroutine(ref attackCooldownCoroutine, AttackCooldown()); // Start the cooldown for AIs attack

        float critRoll = Random.Range(0f, 100f);

        if (critRoll <= critChance) ApplyDamage(damage * critDamageMultiplier); // Check for critical hit
        else ApplyDamage(damage);

        yield return new WaitForSeconds(attackDuration); // Wait until attack has finished

        isAttacking = false;

        SM_EnemyController.TerminateCoroutine(ref attackCoroutine, Attack());

        //if (inAttackRange)
        //    CheckAttackState();
    }

    private IEnumerator InAttackRangeCheck()
    {
        print("Check attack range started");
        WaitForSeconds wait = new(0.1f);
        bool continueLoop = true;
        while (continueLoop)
        {
            yield return wait;
            if (Vector3.Distance(this.transform.position, SM_EnemyController.SM_EnemyTargeting.currentTarget.transform.position) < attackRange) // Check the distance from ai to current target
            {
                if (SM_EnemyController == null) print("Controller is null");
                if (SM_EnemyController.SM_EnemyHitBox == null) print("HitBox is null");

                if (SM_EnemyController.SM_EnemyHitBox.CurrentTargetInList())
                {
                    inAttackRange = true;
                }
            }
            else
                inAttackRange = false;

            if (!inAttackRange)
            {
                SM_EnemyController.ChangeContentState(EnemyContentState.CHASE);
                SM_EnemyController.TerminateCoroutine(ref inAttackRangeCoroutine, InAttackRangeCheck()); // End inAttackRangeCheck
                continueLoop = false;
            }
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        SM_EnemyController.TerminateCoroutine(ref attackCooldownCoroutine, AttackCooldown());

        if (inAttackRange && !isAttacking)
            SM_EnemyController.InitializeCoroutine(ref attackCoroutine, Attack());
    }

    private void ApplyDamage(float _damage)
    {
        List<GameObject> potentialTargets = SM_EnemyController.SM_EnemyHitBox.GetHitObject();

        if (potentialTargets.Count <= 0) return;

        foreach (GameObject _target in potentialTargets)
        {
            if (_target.TryGetComponent(out SM_CharacterController otherController))
            {
                // Call take damage function from otherController script and apply the _damage value
                print("Attacked " + _target.name + " with an amount of: " + _damage + " damage!");
            }
        }
    }

    public void Block()
    {
        if (!isAttacking)
        {
            isBlocking = true;
        }
    }

    private bool CheckBlock()
    {
        float blockRoll = Random.Range(0f, 100f);
        return blockRoll <= blockChance;
    }
}