using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonComponent : EnemyComponent
{

    protected SkeletonModel model;
    private Animator animator;

    private void Start()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        model = GetComponent<SkeletonModel>();
        SetTarget();
    }

    private void Update()
    {
        if (model.AlreadyAttack) return;

        if (target == null)
        {
            SetTarget();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance > model.AttackRange)
        {
            locomotion.MoveTo(target.transform.position);
        }
        else if (!model.AlreadyAttack)
        {
            locomotion.StopMovement();
            Attack();
        }
    }

    protected override void SetTarget()
    {
        bool targetAssigned = false;

        float chance = Random.value;

        if (chance <= 0.3f && GlobalCustomerManager.Instance.customers.Count > 0)
        {
            int randomIndex = Random.Range(0, GlobalCustomerManager.Instance.customers.Count);
            target = GlobalCustomerManager.Instance.customers[randomIndex].gameObject;
            targetAssigned = true;
        }

        if (!targetAssigned && GlobalCharactersManager.Instance.Workers.Count > 0)
        {
            int randomIndex = Random.Range(0, GlobalCharactersManager.Instance.Workers.Count);
            target = GlobalCharactersManager.Instance.Workers[randomIndex];
            targetAssigned = true;
        }

        if (!targetAssigned)
        {
            model.AlreadyAttack = true;
            Despawn();
        }
    }

    protected override void Attack()
    {
        if (target != null)
        {
            Debug.Log($"{gameObject.name} is attacking {target.name}");
            StartCoroutine(PerformAttackAndDespawn());
        }
        else
        {
            Debug.Log($"{gameObject.name} has no target to attack.");
        }
    }

    public override void Despawn()
    {
        if (locomotion != null)
        {
            animator.SetBool("isAttacking", false);
            StartCoroutine(MoveToDespawnAndDestroy());
        }
    }

    private void ASD()
    {
        var workerComp = target.GetComponent<WorkerComponent>();
        var customerComp = target.GetComponent<CustomerComponent>();

        if (workerComp != null && !workerComp.isKidnapped)
        {
            workerComp.BeKidnapped(this.transform);
        }
        else if (customerComp != null)
        {
            customerComp.LeaveWithoutBuy();
        }
        else
        {
            Debug.LogWarning($"{target.name} no tiene componente Worker ni Customer.");
        }
    }

    private IEnumerator MoveToDespawnAndDestroy()
    {
        Vector3 despawnPos = GlobalLocomotionManager.Instance.despawnPoint.position;

        locomotion.MoveTo(despawnPos);

        while (Vector3.Distance(transform.position, despawnPos) > 0.1f)
        {
            yield return null;
        }

        base.Despawn();
    }

    private IEnumerator PerformAttackAndDespawn()
    {
        model.AlreadyAttack = true;
        animator.SetBool("isAttacking", true);
        float wait = GetAnimationLength("Slash");

        Debug.Log("wait: " + wait);

        yield return new WaitForSeconds(wait);

        animator.SetBool("isAttacking", false);

        yield return new WaitForSeconds(0.05f);

        ASD();
        Despawn();
    }

    private float GetAnimationLength(string animName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animName)
                return clip.length;
        }
        return 5f;
    }

    public override Dictionary<string, object> GetStats() {
        var stats = new Dictionary<string, object>
        {
            { "Name", model.CharacterName },
            { "Speed", model.Speed.ToString("F1") },
            { "Icon", model.Icon }
        };
        return stats;
    }
}
