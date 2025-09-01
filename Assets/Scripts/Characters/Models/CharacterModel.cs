[System.Serializable]
public class CharacterModel
{
    int Id { get; set; }
    string Name { get; set; }
    int Speed { get; set; }
    bool Idle { get; set; } = true;
    //Station AsignatedStation { get; set; }
    //Stats Stats { get; set; }
}
