using UnityEngine;

public class SkeletonComponent : EnemyComponent {

    protected SkeletonModel model;

    private void Start() {
        model = GetComponent<SkeletonModel>();
        SetTarget();
    }

    private void Update() {
        if (model.AlreadyAttack) return;

        if (target == null) {
            SetTarget();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance > model.AttackRange) {
            locomotion.MoveTo(target.transform.position);
        }
        else if (!model.AlreadyAttack) {
            locomotion.StopMovement();
            Attack();
        }
    }

    protected override void SetTarget() {
        bool targetAssigned = false;

        float chance = Random.value;

        if (chance <= 0.3f && GlobalCustomerManager.Instance.customers.Count > 0) {
            int randomIndex = Random.Range(0, GlobalCustomerManager.Instance.customers.Count);
            target = GlobalCustomerManager.Instance.customers[randomIndex].gameObject;
            targetAssigned = true;
        }

        if (!targetAssigned && GlobalCharactersManager.Instance.Workers.Count > 0) {
            int randomIndex = Random.Range(0, GlobalCharactersManager.Instance.Workers.Count);
            target = GlobalCharactersManager.Instance.Workers[randomIndex];
            targetAssigned = true;
        }

        if (!targetAssigned) {
            model.AlreadyAttack = true;
            Despawn();
        }
    }

    protected override void Attack() {
        if (target != null) {
            Debug.Log($"{gameObject.name} is attacking {target.name}");

            var workerComp = target.GetComponent<WorkerComponent>();
            var customerComp = target.GetComponent<Customer>();

            if (workerComp != null && !workerComp.isKidnapped) {
                workerComp.BeKidnapped(this.transform);
            } else if (customerComp != null) {
                customerComp.LeaveWithoutBuy();
            } else {
                Debug.LogWarning($"{target.name} no tiene componente Worker ni Customer.");
            }
        } else {
            Debug.Log($"{gameObject.name} has no target to attack.");
        }

        model.AlreadyAttack = true;
        Despawn();
    }

    public override void Despawn() {
        if (locomotion != null) {
            StartCoroutine(MoveToDespawnAndDestroy());
        }
    }

    private System.Collections.IEnumerator MoveToDespawnAndDestroy() {
        Vector3 despawnPos = GlobalLocomotionManager.Instance.despawnPoint.position;

        locomotion.MoveTo(despawnPos);

        while (Vector3.Distance(transform.position, despawnPos) > 0.1f) {
            yield return null;
        }

        base.Despawn();
    }
}
