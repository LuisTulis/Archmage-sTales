using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Characters/Character Data")]
public class CharacterData : ScriptableObject {
    public int Id;
    public string Name;
    //public CharacterStats Stats;
    public int Speed;
}
