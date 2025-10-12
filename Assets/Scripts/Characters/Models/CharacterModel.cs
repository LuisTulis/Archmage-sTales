using UnityEngine;

public class CharacterModel : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private string characterName;
    [SerializeField] private int speed;
    [SerializeField] private bool idle = true;

    public int Id { get => id; set => id = value; }
    public string CharacterName { get => characterName; set => characterName = value; }
    public int Speed { get => speed; set => speed = value; }
    public bool Idle { get => idle; set => idle = value; }
}
