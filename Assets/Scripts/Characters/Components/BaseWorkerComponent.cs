using System.Collections.Generic;
using UnityEngine;

public class BaseWorkerComponent : CharacterComponent
{

    protected RandomWalkLocomotion locomotion;
    public BaseWorkerModel model;
    public bool isWorking;
    protected override void Awake() {
        base.Awake();
        locomotion = GetComponent<RandomWalkLocomotion>();
        model = GetComponent<BaseWorkerModel>();
    }

    protected virtual void Update()
    {
        locomotion.WalkingAnimation(isWorking);
        
        // Fixme cuando tengamos un boton de deseleccionar trabajador, (que vuelva a estar idle)
        // debrai llamar a LeaveWorkStation

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
        if (Vector3.Distance(this.transform.position, this.model.AsignatedStation.workerPosition.transform.position) < 3) {
            if (this.model.AsignatedStation.clientUsing == 2) {
                if(Random.Range(0,9) == 1)
                {
                    switch (this.model.AsignatedStation.type.ToString())
                    {
                        case "adivinacion":
                            if(this.model.Stats.adivinationStat < 5)
                            {
                                this.model.Stats.adivinationStat += 1;
                            }
                            break;
                        case "invocacion":
                            if (this.model.Stats.summonStat < 5)
                            {
                                this.model.Stats.summonStat += 1;
                            }
                            break;
                        case "caldero":
                            if (this.model.Stats.alchemyStat < 5)
                            {
                                this.model.Stats.alchemyStat += 1;
                            }
                            break;
                        case "encantamiento":
                            if (this.model.Stats.enchantStat < 5)
                            {
                                this.model.Stats.enchantStat += 1;
                            }
                            break;
                    }
                }
                model.AsignatedStation.accessToWork(this);
                this.isWorking = true;
                this.GetIntoWorkingPosition(this.model.AsignatedStation.workerPosition);
            } else {
                this.isWorking = false;
                locomotion.SittingAnimation(false);
            }

        }
    }

    public void LeaveWorkStation() {
        if (model.AsignatedStation != null) {
            model.AsignatedStation.StopAllCoroutines();
            model.AsignatedStation.fx.SetWorking(false);
            model.AsignatedStation.status = "Idle";
            model.AsignatedStation.assignedWorkerName = null;
            if (model.AsignatedStation.actualProgress != null)
            {
                Destroy(model.AsignatedStation.actualProgress);
            }
            model.AsignatedStation = null;
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

    public override Dictionary<string, object> GetStats() {
        var stats = new Dictionary<string, object>
        {
            { "Name", model.CharacterName },
            { "Speed", model.Speed.ToString("F1") },
            { "Summoning", model.Stats.summonStat.ToString() },
            { "Adivination", model.Stats.adivinationStat.ToString() },
            { "Alchemy", model.Stats.alchemyStat.ToString() },
            { "Enchanting", model.Stats.enchantStat.ToString() },
            { "Working", isWorking ? "Yes" : "No" },
            { "Icon", model.Icon }
        };

        Debug.Log("Getting stats for worker: " + model.CharacterName);
        return stats;
    }

}
