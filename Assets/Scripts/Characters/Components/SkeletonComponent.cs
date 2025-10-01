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
        if (GlobalCharactersManager.Instance.Workers.Count > 0) {
            var randomIndex = Random.Range(0, GlobalCharactersManager.Instance.Workers.Count);
            target = GlobalCharactersManager.Instance.Workers[randomIndex];
        }

        else {
            model.AlreadyAttack = true;
            Despawn();
        }
    }

    protected override void Attack() {
        if (target != null) {
            Debug.Log($"{gameObject.name} is attacking {target.name}");
            target.GetComponent<WorkerComponent>().Despawn();
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
