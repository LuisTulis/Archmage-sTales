using UnityEngine;

public abstract class CharacterComponent : MonoBehaviour
{

    public bool isWorking;

    public void Start()
    {
        locomotion = GetComponent<CharacterLocomotion>();
        model = GetComponent<CharacterModel>();
        locomotion.InitializePatrolPoints();
    }

    public void Update()
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
            public virtual void Despawn() {
        Destroy(this.gameObject);
    }

}
