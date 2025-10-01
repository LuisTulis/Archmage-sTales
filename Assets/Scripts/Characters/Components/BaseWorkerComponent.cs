using UnityEngine;

public class BaseWorkerComponent : CharacterComponent
{

    protected RandomWalkLocomotion locomotion;
    protected CharacterModel model;
    public bool isWorking;

    protected virtual void Awake() {
        locomotion = GetComponent<RandomWalkLocomotion>();
        model = GetComponent<CharacterModel>();
        
    }

    protected virtual void Update() {
        locomotion.WalkingAnimation();

        if (model.AsignatedStation != null) {
            locomotion.MoveTo(model.AsignatedStation.workerPosition.position);
            Work();
        } else {
            locomotion.IdleRandomWalk();
        }
    }

    private void Work() {
        if (Vector3.Distance(this.transform.position, this.model.AsignatedStation.workerPosition.transform.position) < 2) {
            if (this.model.AsignatedStation.clientUsing == 2) {
                model.AsignatedStation.accessToWork(this.model.CharacterName);
                this.isWorking = true;
            } else {
                this.isWorking = false;
            }

        }
    }

}
