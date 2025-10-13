using UnityEngine;

public abstract class EnemyComponent : CharacterComponent {

    protected EnemyLocomotion locomotion;
    [SerializeField] protected GameObject target;


    protected override void Awake() {
        base.Awake();
        locomotion = GetComponent<EnemyLocomotion>();
    }


    protected abstract void Attack();
    protected abstract void SetTarget();

}

