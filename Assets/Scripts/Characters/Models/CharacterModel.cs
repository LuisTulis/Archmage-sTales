using UnityEngine;

public class CharacterModel : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private string name;
    [SerializeField] private int speed;
    [SerializeField] private bool idle = true;

    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public int Speed { get => speed; set => speed = value; }
    public bool Idle { get => idle; set => idle = value; }
    //public Station AsignatedStation { get; set; }
    //public Stats Stats { get; set; }
}
