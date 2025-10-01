using System.Collections;
using UnityEngine;

public class WorkerComponent : CharacterComponent
{
    private Animator animator;
    private string isPulling = "isPulling";
    private string isOpening = "isOpening";

    private float minDelay = 3f;
    private float maxDelay = 6f;

    private float animChance = 0.5f;

    private Coroutine workRoutine;
    private bool prevIsWorking = false;

    private void Start()
    {
        base.Start();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    private void Update()
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

            string animName = playPull ? "Pulling Lever" : "Opening";
            float animLength = GetAnimationLength(animName);
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
}

