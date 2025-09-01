[System.Serializable]
public class CharacterModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Speed { get; set; }
    public bool Idle { get; set; } = true;
    //public Station AsignatedStation { get; set; }
    //public Stats Stats { get; set; }
}
