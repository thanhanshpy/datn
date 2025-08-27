using UnityEngine;

public class EndAttackFlipReset : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var boss = animator.GetComponent<BossStateManager>();
        if (boss != null)
        {
            boss.canFlip = true;
        }
    }
}
