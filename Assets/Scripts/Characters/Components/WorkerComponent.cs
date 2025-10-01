using System.Collections;
using UnityEngine;

public class WorkerComponent : BaseWorkerComponent
{
    private Animator animator;
    private string isPulling = "isPulling";
    private string isOpening = "isOpening";

    private float animChance = 0.5f;

    private Coroutine workRoutine;
    private bool prevIsWorking = false;

    protected override void Awake()
    {
        base.Awake();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        if (isWorking != prevIsWorking)
        {
            prevIsWorking = isWorking;

            if (isWorking)
            {
                workRoutine = StartCoroutine(RandomWorkRoutine());
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

    public override void Despawn()
    {
        GlobalCharactersManager.Instance.FireWorker(this.gameObject.GetComponent<WorkerModel>());
        base.Despawn();
    }
}

