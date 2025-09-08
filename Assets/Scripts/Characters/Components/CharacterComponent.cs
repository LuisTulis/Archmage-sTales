using UnityEngine;

public class CharacterComponent : MonoBehaviour
{
    private CharacterLocomotion locomotion;
    private CharacterModel model;

    private void Start() {
        locomotion = GetComponent<CharacterLocomotion>();
        model = GetComponent<CharacterModel>();
        locomotion.InitializePatrolPoints();
    }

    private void Update() {
        if(model.AsignatedStation != null)
        {
            locomotion.MoveTo(model.AsignatedStation.transform.position);
            Work();
        }
        else
        {
            locomotion.IdleRandomWalk();
        }
    }

    private void Work()
    {            
        model.AsignatedStation.accessToWork(this.model.Name);
    }

}
