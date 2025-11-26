using UnityEngine;

public class ThugComponent_Prototipo : EnemyComponent_Prototipo
{

    protected ThugModel model;

    private void Start()
    {
        model = GetComponent<ThugModel>();
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

        if (distance > model.AttackRange + .75f)
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
        if (WorkstationManager_Prototipo.Instance.activeStations.Count > 0)
        {
            var randomIndex = Random.Range(0, WorkstationManager_Prototipo.Instance.activeStations.Count);
            var targetStation = WorkstationManager_Prototipo.Instance.activeStations[randomIndex];
            target = targetStation.clientPosition.gameObject;
        }
        else
        {
            Debug.Log("No workstations available to attack.");
            model.AlreadyAttack = true;
            Despawn();
        }
    }

    protected override void Attack()
    {
        if (target != null)
        {
            Debug.Log($"{gameObject.name} is attacking {target.name}");
            GameManager_Prototipo.Instance.removeGold(model.StealAmount);
        }
        else
        {
            Debug.Log($"{gameObject.name} has no target to attack.");
        }

        model.AlreadyAttack = true;
        Despawn();
    }

    public override void Despawn()
    {
        if (locomotion != null)
        {
            StartCoroutine(MoveToDespawnAndDestroy());
        }
    }

    private System.Collections.IEnumerator MoveToDespawnAndDestroy()
    {
        Vector3 despawnPos = GlobalLocomotionManager.Instance.despawnPoint.position;

        locomotion.MoveTo(despawnPos);

        while (Vector3.Distance(transform.position, despawnPos) > 0.1f)
        {
            yield return null;
        }

        base.Despawn();
    }
}
