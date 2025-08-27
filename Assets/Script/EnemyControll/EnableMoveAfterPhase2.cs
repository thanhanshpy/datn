using UnityEngine;

public class EnableMoveAfterPhase2 : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var stateControl = animator.GetComponent<BossStateManager>();
        if (stateControl != null)
        {
            stateControl.canMove = true;
            stateControl.canFlip = true;
        }
    }
}
