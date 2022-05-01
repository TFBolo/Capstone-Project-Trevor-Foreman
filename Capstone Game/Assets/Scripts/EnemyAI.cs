using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float startingHealth;

    [SerializeField] public float chasingRange;
    [SerializeField] [Range(0, 360)] public float chaseAngle;
    [SerializeField] private float attackRange;
    [SerializeField] public float wanderDist;
    [SerializeField] public float speed;
    [SerializeField] public float lethargy;
    [SerializeField] public float smoothDamp;

    [SerializeField] private Transform playerTransform;

    private NavMeshAgent agent;

    private Node topNode;

    public float currentHealth;
    private float newHealth;
    public Vector3 spawn;
    private Vector3 wanderSpot;
    private bool wanderMove;
    public float wanderTime;

    public GameObject enemyWeapon;
    public Animator enemyAnimator;

    public GameObject healthBar;
    public RectTransform healthFill;

    public GameObject treasureDrop;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        agent.speed = speed;
        playerTransform = GameObject.FindWithTag("Player").transform;
        wanderMove = true;
        spawn = transform.position;
        currentHealth = startingHealth;
        newHealth = currentHealth;
        enemyWeapon.GetComponent<BoxCollider>().enabled = false;
        ConstructBehahaviourTree();
    }

    private void ConstructBehahaviourTree()
    {
        ChaseNode chaseNode = new ChaseNode(playerTransform, agent, this, attackRange);
        RangeNode chasingRangeNode = new RangeNode(chasingRange, chaseAngle, playerTransform, transform);
        RangeNode2 attackingRangeNode = new RangeNode2(attackRange, playerTransform, transform);
        AttackNode attackNode = new AttackNode(agent, this, playerTransform, smoothDamp, chaseAngle);
        RangeNode wanderRange = new RangeNode(chasingRange, chaseAngle, playerTransform, transform);
        WanderNode wanderNode = new WanderNode(agent, spawn, wanderDist, this);

        Inverter wanderRangeReversed = new Inverter(wanderRange);

        Sequence wanderInverter = new Sequence(new List<Node>{wanderRangeReversed, wanderNode});
        Sequence chaseSequence = new Sequence(new List<Node>{chasingRangeNode, chaseNode});
        Sequence attackSequence = new Sequence(new List<Node>{attackingRangeNode, attackNode});

        topNode = new Selector(new List<Node>{chaseSequence, attackSequence, wanderInverter });
    }

    private void Update()
    {
        topNode.Evaluate();
        if (topNode.nodeState == NodeState.FAILURE)
        {
            agent.isStopped = true;
        }

        if (currentHealth < startingHealth)
        {
            lethargy = 1f;
        }

        if (currentHealth <= 0)
        {
            treasureDrop.SetActive(true);
            treasureDrop.GetComponent<Item>().amount = Random.Range(1, 4);
            treasureDrop.transform.parent = null;
            gameObject.SetActive(false);
        }

        if (healthBar.activeInHierarchy)
        {
            healthFill.sizeDelta = new Vector2(160 * (currentHealth / startingHealth), 10);

            healthBar.transform.LookAt(Camera.main.transform);
            healthBar.transform.Rotate(0, 180, 0);
        }
        else if (!healthBar.activeInHierarchy && currentHealth < startingHealth)
        {
            healthBar.SetActive(true);
        }

        if (agent.velocity.magnitude > 0f && Vector3.Distance(playerTransform.position, transform.position) > attackRange)
        {
            //enemyWeapon.SetActive(false);
            enemyAnimator.Play("Move");
        }
        else if (agent.velocity.magnitude <= 0f && !wanderMove)
        {
            //enemyWeapon.SetActive(false);
            enemyAnimator.Play("Idle2");
        }

        WanderWait();
    }

    public void SetWanderSpot(Vector3 wanderSpot)
    {
        if (currentHealth < newHealth)
        {
            this.wanderSpot = playerTransform.position /*+ Random.insideUnitSphere * (wanderDist / 2)*/;
            //this.wanderSpot.y = playerTransform.position.y;
            SetAggro();
        }
        else
        {
            this.wanderSpot = wanderSpot;
        }
    }

    public void SetAggro()
    {
        newHealth = currentHealth;
    }

    public Vector3 GetWanderSpot()
    {
        return wanderSpot;
    }

    public void SetWanderMoveFalse()
    {
        this.wanderMove = false;
        //StartCoroutine("WanderWait");
        wanderTime = 0f;
    }

    public bool WanderMove()
    {
        return wanderMove;
    }

    public void WanderWait()
    {
        if (wanderTime < lethargy)
        {
            wanderTime += Time.deltaTime;
        }
        else
        {
            wanderMove = true;
        }
    }

    /*public IEnumerator WanderWait()
    {
        yield return new WaitForSeconds(lethargy);
        wanderMove = true;
    }*/

    public void Attack1()
    {
        if (!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack") && enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            //enemyWeapon.SetActive(true);
            enemyAnimator.Play("BasicAttack");
        }
        else if (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            //enemyWeapon.SetActive(false);
            enemyAnimator.Play("Idle");
        }
    }
}
