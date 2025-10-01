using UnityEngine;

public class EnemyModel : CharacterModel
{
    [SerializeField] private float attackRange;
    [SerializeField] private bool alreadyAttack;

    public float AttackRange { get => attackRange; set => attackRange = value; }
    public bool AlreadyAttack { get => alreadyAttack; set => alreadyAttack = value; }

}
