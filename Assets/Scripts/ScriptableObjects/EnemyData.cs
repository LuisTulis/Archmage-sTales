using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Characters/Enemy Data")]
public class EnemyData : ScriptableObject {
    public int Id;
    public int Speed;
    public float AttackRange;
    public int stealAmount;
}
