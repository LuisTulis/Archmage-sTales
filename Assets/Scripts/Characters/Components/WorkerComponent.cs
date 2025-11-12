using System.Collections;
using UnityEditor.Experimental;
using UnityEngine;

public class WorkerComponent : BaseWorkerComponent
{
    private Animator animator;
    private string isPulling = "isPulling";
    private string isOpening = "isOpening";

    private float animChance = 0.5f;

    private Coroutine workRoutine;
    private bool prevIsWorking = false;

    public bool isFired = false;
    public bool isKidnapped = false;
    private Transform kidnapper;

    protected override void Awake()
    {
        base.Awake();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        if(model is WorkerModel wm)
        {
            if(wm.mental < 0)
            {
                BeKidnapped(GlobalLocomotionManager.Instance.despawnPoint);
                float distanceToDespawn = Vector3.Distance(transform.position, GlobalLocomotionManager.Instance.despawnPoint.position);
                if (distanceToDespawn < 2f)
                {
                    Despawn();
                }
            }
        }
        if (isKidnapped && kidnapper != null)
        {
            locomotion.MoveTo(kidnapper.position - new Vector3(0, 0, 1.5f));
            float distanceToDespawn = Vector3.Distance(transform.position, GlobalLocomotionManager.Instance.despawnPoint.position);
            if (distanceToDespawn < 1f)
            {
                Despawn();
            }
            return;
        }
        if (isFired) {
            locomotion.MoveTo(GlobalLocomotionManager.Instance.despawnPoint.position);
            float distanceToDespawn = Vector3.Distance(transform.position, GlobalLocomotionManager.Instance.despawnPoint.position);
            if (distanceToDespawn < 1f) {
                Despawn();
            }
            return;
        }

        if (isWorking != prevIsWorking)
        {
            prevIsWorking = isWorking;

            if (isWorking)
            {
                if (!model.AsignatedStation.sittingWorkstation)
                {
                    workRoutine = StartCoroutine(RandomWorkRoutine());
                }
            }
            else
            {
                if (workRoutine != null)
                {
                    StopCoroutine(workRoutine);
                    workRoutine = null;
                }
            }
        }
    }

    private IEnumerator RandomWorkRoutine()
    {
        while (isWorking)
        {
            animator.SetBool(isPulling, false);
            animator.SetBool(isOpening, false);

            bool playPull = Random.value <= animChance;
            if (playPull) animator.SetBool(isPulling, true);
            else animator.SetBool(isOpening, true);

            float animLength = GetAnimationLength(playPull ? "Pulling Lever" : "Opening");
            yield return new WaitForSeconds(animLength);

            animator.SetBool(isPulling, false);
            animator.SetBool(isOpening, false);

            yield return new WaitForSeconds(5f);
        }
    }

    private float GetAnimationLength(string animName)
    {
        if (animator.runtimeAnimatorController != null)
        {
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == animName)
                    return clip.length;
            }
        }
        return 5f;
    }

    public void BeKidnapped(Transform skeleton)
    {
        if (workRoutine != null)
        {
            StopCoroutine(workRoutine);
            workRoutine = null;
        }
        isWorking = false;
        isKidnapped = true;
        kidnapper = skeleton;

        LeaveWorkStation();
    }

    public void BeFired() {
        if (workRoutine != null) {
            StopCoroutine(workRoutine);
            workRoutine = null;
        }
        isWorking = false;
        isFired = true;

        LeaveWorkStation();
    }

    public override void Despawn()
    {
        GlobalCharactersManager.Instance.FireWorker(this.gameObject.GetComponent<WorkerModel>());
        base.Despawn();
    }
}

