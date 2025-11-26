using UnityEngine;

public class CharacterModel_Prototipo : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private string characterName;
    [SerializeField] private int speed;
    [SerializeField] private bool idle = true;
    [SerializeField] private WorkStationBehaviour_Prototipo workStation;

    public int Id { get => id; set => id = value; }
    public string CharacterName { get => characterName; set => characterName = value; }
    public int Speed { get => speed; set => speed = value; }
    public bool Idle { get => idle; set => idle = value; }
    public WorkStationBehaviour_Prototipo AsignatedStation { get => workStation; set => workStation = value; }
    //public Stats Stats { get; set; }
}
