using UnityEngine;

public class ShakeState : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset any trigger that was set during the current animation
        animator.ResetTrigger("Shake");
    }
}
