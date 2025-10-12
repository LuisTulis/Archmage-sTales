using UnityEngine;

public class BaseWorkerComponent : CharacterComponent
{

    protected RandomWalkLocomotion locomotion;
    protected BaseWorkerModel model;
    public bool isWorking;

    protected virtual void Awake()
    {
        locomotion = GetComponent<RandomWalkLocomotion>();
        model = GetComponent<BaseWorkerModel>();
    }

    protected virtual void Update()
    {
        locomotion.WalkingAnimation();

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
        if (Vector3.Distance(this.transform.position, this.model.AsignatedStation.workerPosition.transform.position) < 3)
        {
            if (this.model.AsignatedStation.clientUsing == 2)
            {
                model.AsignatedStation.accessToWork(this.model.CharacterName);
                this.isWorking = true;

                this.GetIntoWorkingPosition(this.model.AsignatedStation.workerPosition);
            }
            else
            {
                this.isWorking = false;
                locomotion.SittingAnimation(false);
            }

        }
    }

    private void GetIntoWorkingPosition(Transform targetPosition)
    {
        Vector3 targetPos = this.model.AsignatedStation.workerPosition.position;
        targetPos.y = transform.position.y;
        transform.position = targetPos;

        Vector3 direction = this.model.AsignatedStation.workDirection.position - transform.position;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(direction);

        locomotion.SittingAnimation(this.model.AsignatedStation.sittingWorkstation);
    }

}
