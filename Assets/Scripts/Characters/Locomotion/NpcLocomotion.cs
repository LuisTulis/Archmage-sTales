public class NpcLocomotion : CharacterLocomotion
{

    public void talkingAnimation()
    {
        if (animator == null) return;
        animator.SetBool("talking", true);
    }

    public void IdleAnimation()
    {
        if (animator == null) return;
        animator.SetBool("Idle", true);
    }

    public void WaveAnimation()
    {
        if (animator == null) return;
        animator.SetTrigger("wave");
    }

}
