using UnityEngine;

public class ThugModel : EnemyModel
{
    [SerializeField] private int stealAmount;

    public int StealAmount { get => stealAmount; set => stealAmount = value; }
}
