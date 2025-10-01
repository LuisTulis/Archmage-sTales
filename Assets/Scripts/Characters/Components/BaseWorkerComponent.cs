using UnityEngine;

public class BaseWorkerComponent : CharacterComponent
{

    protected RandomWalkLocomotion locomotion;
    protected CharacterModel model;

    private void Awake() {
        locomotion = GetComponent<RandomWalkLocomotion>();
        model = GetComponent<CharacterModel>();
        
    }

    private void Update()
    {
        if (model.AsignatedStation != null)
        {
            locomotion.MoveTo(model.AsignatedStation.workerPosition.position);
            Work();
        }
        else
        {
            locomotion.IdleRandomWalk();
        }
    }

    private void Work()
    {
        if (Vector3.Distance(this.transform.position, this.model.AsignatedStation.workerPosition.transform.position) < 2)
        {
            if (this.model.AsignatedStation.clientUsing == 2)
            {
                model.AsignatedStation.accessToWork(this.model.CharacterName);
                //this.transform.Rotate(new Vector3(0, 180 * Time.deltaTime, 0));
            }
            else
            {
                //this.transform.rotation = model.AsignatedStation.workerPosition.rotation;
            }

        }
    }

}
