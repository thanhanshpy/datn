using UnityEngine;

public class DarkKnightMoveSet : StateMachineBehaviour
{
    [SerializeField] private float phase1Speed = 2.5f;
    [SerializeField] private float phase2Speed = 5f;

    public float moveSpeed;

    public float attackRange = 3f;

    private bool hasEnteredPhase2 = false;
    public float attackCooldown;
    private float cooldownTimer = 0f;
    private bool isCoolingDown = false;

    Transform player;
    Rigidbody2D rb;
    FlipToPlayer flip;
    BossStateManager boss;
    Health bossHealth;

    [SerializeField] private float phase1Cooldown = 2f;
    [SerializeField] private float phase2Cooldown = 1.5f;

    public WeightedAttack[] phase1Attacks;
    public WeightedAttack[] phase2Attacks;

    public WeightedAttack[] currentAttackOptions;

    private bool hasAttacked = false;

    [System.Serializable]
    public class WeightedAttack
    {
        public string attackTrigger;
        public int weight;
    }

    private string GetWeightedRandomAttack()
    {
        if (currentAttackOptions == null || currentAttackOptions.Length == 0)
        {
            return null;
        }
            
        int totalWeight = 0;
        foreach (var attack in currentAttackOptions)
        {
            totalWeight += attack.weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var attack in currentAttackOptions)
        {
            currentWeight += attack.weight;
            if (randomValue < currentWeight)
            {
                return attack.attackTrigger;
            }
        }

        return currentAttackOptions[0].attackTrigger; // fallback
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        Debug.Log("Dark Knight is moving. Speed: " + moveSpeed);
        Debug.Log("Dark Knight is moving. CoolDown: " + attackCooldown);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        flip = animator.GetComponent<FlipToPlayer>();
        boss = animator.GetComponent<BossStateManager>();
        bossHealth = animator.GetComponent<Health>();

        if (phase1Attacks == null || phase1Attacks.Length == 0)
        {
            phase1Attacks = new WeightedAttack[]
            {
                new WeightedAttack { attackTrigger = "Thrust", weight = 40 },
                new WeightedAttack { attackTrigger = "Swing", weight = 50 },
                new WeightedAttack { attackTrigger = "Slash", weight = 10 },
                new WeightedAttack { attackTrigger = "Cut", weight = 40 }
            };
        }

        if (phase2Attacks == null || phase2Attacks.Length == 0)
        {
            phase2Attacks = new WeightedAttack[]
            {
                new WeightedAttack { attackTrigger = "Thrust", weight = 20 },
                new WeightedAttack { attackTrigger = "Swing", weight = 30 },
                new WeightedAttack { attackTrigger = "Slash", weight = 50 },
                new WeightedAttack { attackTrigger = "Cut", weight = 20 }
            };
        }

        if (!hasEnteredPhase2)
        {
            currentAttackOptions = phase1Attacks;
            attackCooldown = phase1Cooldown;
            moveSpeed = phase1Speed;
        }

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null || rb == null) return;
        
        if (boss.canMove)
        {
            Vector2 target = new Vector2(player.position.x, player.position.y + 1.5f);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

        }

        if (boss != null && boss.canFlip)
        {
            flip.LookAtPlayer();
        }

        if (isCoolingDown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isCoolingDown = false;
            }
            return;
        }

        if (Vector2.Distance(player.position, rb.position) <= attackRange)
        {
            isCoolingDown = true;
            cooldownTimer = attackCooldown;

            boss.canFlip = false;

            string chosenAttack = GetWeightedRandomAttack();
            animator.SetTrigger(chosenAttack);
        }

        if (!hasEnteredPhase2 && bossHealth.Hp <= bossHealth.MaxHp / 2)
        {
            hasEnteredPhase2 = true;
            currentAttackOptions = phase2Attacks;
            attackCooldown = phase2Cooldown;
            moveSpeed = phase2Speed;

            var stateControl = animator.GetComponent<BossStateManager>();
            if (stateControl != null)
            {
                stateControl.canFlip = false;
                stateControl.canMove = false;
            }

            animator.SetTrigger("Phase2");
            //return;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //isCoolingDown = false;
        //cooldownTimer = 0f;
    }
}
