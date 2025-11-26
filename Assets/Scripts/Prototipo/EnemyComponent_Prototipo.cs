using UnityEngine;

public abstract class EnemyComponent_Prototipo : CharacterComponent_Prototipo
{

    protected EnemyLocomotion locomotion;
    [SerializeField] protected GameObject target;


    private void Awake() {
        locomotion = GetComponent<EnemyLocomotion>();
    }


    protected abstract void Attack();
    protected abstract void SetTarget();

}

