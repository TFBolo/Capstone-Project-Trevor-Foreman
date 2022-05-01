using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PetAI : MonoBehaviour
{
    [SerializeField] private GameObject playerTransform;
    [SerializeField] private float attackRange;
    [SerializeField] private GameObject weapon;
    [Range(1.5f, 2.5f)]
    [SerializeField] public float baseSpeed; //save
    private float modifiedSpeed;
    [Range(5f, 15f)]
    [SerializeField] public float basePower; //save
    private float modifiedPower;
    [Range(2f, 5f)]
    [SerializeField] public float baseBond; //save
    [Range(5f, 10f)]
    [SerializeField] public float baseEndurance; //save

    public Camera cam;
    private NavMeshAgent agent;
    private LayerMask playerMask;
    public BoxCollider interact;
    private LayerMask enemyMask;
    public Animator petAnimations;

    private Node topNode;
    private Vector3 petDestination;
    private Transform enemyTransform;
    /*
    private Vector3 attackStart;
    private Vector3 attackEnd;
    */
    private int attackType;
    private int chosenAttack;
    public int command;
    public bool cooldown;
    public bool followEnemy;
    public bool holdAttack;
    public float damage;
    public float commandWait;
    public float maxStamina;
    public float currentStamina;
    public float staminaModifier;
    //private Coroutine wait = null;
    public float timeWait;
    public float commandTime;
    public float followTime;
    public List<int> attackList = new List<int>(); //save
    public bool obeyAnyway;
    public int attackEffect;

    //Stats
    public LevelSystem power; //weapon damage
    public LevelSystem speed; //agent's speed
    public LevelSystem bond; //obey time, number of attacks it can learn, modifier of speed and power max 25%, and Critical Chance
    public LevelSystem endurance; //stamina and modifier of speed and power max 25%

    public float powerModifier;
    public float speedModifier;

    private void Awake()
    {
        enemyMask = LayerMask.GetMask("Enemy");
        interact = GetComponent<BoxCollider>();
        playerMask = LayerMask.GetMask("Player");
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player");
        cam = Camera.main;
    }

    private void Start()
    {
        if (SaveData.instance.shouldLoad)
        {
            power = new LevelSystem(SaveData.instance.powerLevel, SaveData.instance.powerExp);
            speed = new LevelSystem(SaveData.instance.speedLevel, SaveData.instance.speedExp);
            bond = new LevelSystem(SaveData.instance.bondLevel, SaveData.instance.bondExp);
            endurance = new LevelSystem(SaveData.instance.enduranceLevel, SaveData.instance.enduranceExp);

            attackList = SaveData.instance.petAttacks;

            basePower = SaveData.instance.startPower;
            baseSpeed = SaveData.instance.startSpeed;
            baseBond = SaveData.instance.startBond;
            baseEndurance = SaveData.instance.startEndurance;

            currentStamina = SaveData.instance.staminaCurrent;
        }
        else
        {
            power = new LevelSystem();
            speed = new LevelSystem();
            bond = new LevelSystem();
            endurance = new LevelSystem();
            
            attackList.Add(1);
            attackList.Add(2);
            attackList.Add(3);
            attackList.Add(4);
        }
        maxStamina = baseEndurance + ((float)endurance.GetLevelNumber() / 100) * baseEndurance;
        if(!SaveData.instance.shouldLoad) currentStamina = maxStamina;

        holdAttack = false;
        followEnemy = false;
        cooldown = false;
        attackType = attackList[0];
        command = 0; // 0=stop   1=carried  2=move  3=call  4=attack
        ConstructBehahaviourTree();
    }

    private void ConstructBehahaviourTree()
    {
        ObeyNode obeyNode = new ObeyNode(this, 2);
        DestinationNode destinationNode = new DestinationNode(this, agent);
        ObeyNode followNode = new ObeyNode(this, 3);
        CallNode callNode = new CallNode(agent, playerTransform.transform);
        ObeyNode obeyAttackNode = new ObeyNode(this, 4);
        PetAttackNode petAttackNode = new PetAttackNode(transform, this, agent, attackRange);

        Sequence attackSequence = new(new List<Node> { obeyAttackNode, petAttackNode});
        Sequence callSequence = new Sequence(new List<Node> { followNode, callNode });
        Sequence destinationSequence = new Sequence(new List<Node> { obeyNode, destinationNode });

        topNode = new Selector(new List<Node> { destinationSequence, callSequence, attackSequence });
    }

    private void Update()
    {
        // endurance
        maxStamina = baseEndurance + ((float)endurance.GetLevelNumber() / 100) * baseEndurance;
        if (currentStamina < 0) currentStamina = 0;
        else if (currentStamina > maxStamina) currentStamina = maxStamina;
        staminaModifier = currentStamina / baseEndurance;

        // bond
        commandWait = 10 - (((float)bond.GetLevelNumber() / 100) * 10);
        if (commandWait < 0.5f) commandWait = 0.5f;

        // speed
        speedModifier = ((float)speed.GetLevelNumber() / 100) + (0.25f * ((float)bond.GetLevelNumber() / 100));
        modifiedSpeed = baseSpeed + (speedModifier * (baseSpeed * 3));
        modifiedSpeed = modifiedSpeed + (modifiedSpeed * (0.25f * (staminaModifier - 1)));
        agent.speed = modifiedSpeed;
        agent.angularSpeed = 120 + (((float)speed.GetLevelNumber() / 100) * 120);
        agent.acceleration = agent.speed * 3;

        // power
        powerModifier = ((float)power.GetLevelNumber() / 100) + (0.25f * ((float)bond.GetLevelNumber() / 100));
        modifiedPower = basePower + (powerModifier * (basePower * 5));
        modifiedPower = modifiedPower + (modifiedPower * (0.25f * (staminaModifier - 1)));
        damage = modifiedPower;

        if (Time.timeScale == 0) return;

        if (petAnimations.GetCurrentAnimatorStateInfo(0).IsName("PetTackle") && petAnimations.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            interact.enabled = true;
            agent.enabled = true;
            agent.isStopped = false;
            currentStamina -= 0.1f;

            obeyAnyway = false;
            followEnemy = true;
            petAnimations.Play("New State");
        }
        else if (petAnimations.GetCurrentAnimatorStateInfo(0).IsName("PetSweep") && petAnimations.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            interact.enabled = true;
            agent.enabled = true;
            agent.isStopped = false;
            currentStamina -= 0.2f;

            obeyAnyway = false;
            followEnemy = true;
            petAnimations.Play("New State");
        }

        if (Input.GetButtonDown("Destination") && command != 1)
        {
            
            //if (wait != null) StopCoroutine(wait);
            
            //wait = StartCoroutine(OrderWait());
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit petPlace))
            {
                followEnemy = false;
                agent.enabled = true;
                command = 2;
                timeWait = 0f;
                petDestination = petPlace.point;
                holdAttack = false;
            }
        }
        if (Input.GetButtonDown("Call") && command != 1)
        {
            followEnemy = false;
            agent.enabled = true;
            //if (wait != null) StopCoroutine(wait);
            command = 3;
            timeWait = 0f;
            holdAttack = false;
            //wait = StartCoroutine(OrderWait());
        }
        if (Input.GetButtonDown("Pick Up") && Physics.CheckSphere(gameObject.transform.position, 1.5f, playerMask))
        {
            holdAttack = false;
            followEnemy = false;
            //if (wait != null) StopCoroutine(wait);
            if (command == 1)
            {
                command = 0;
                agent.enabled = true;
                agent.isStopped = false;
                gameObject.transform.parent = null;
                interact.enabled = true;
            }
            else if (command != 1)
            {
                command = 1;
                agent.enabled = true;
                agent.isStopped = true;
                agent.enabled = false;
                gameObject.transform.SetParent(playerTransform.transform);
                interact.enabled = false;
                transform.localPosition = new Vector3(0f, 0.5f, 0.3f);
                transform.rotation = playerTransform.transform.rotation * Quaternion.Euler(0, 90, 0);
            }
        }
        if (Input.GetButtonDown("Attack1") /*&& command != 4*/)
        {
            attackType = attackList[0];
        }
        else if (Input.GetButtonDown("Attack2") /*&& command != 4*/ && bond.GetLevelNumber() >= 15)
        {
            attackType = attackList[1];
        }
        else if (Input.GetButtonDown("Attack3") /*&& command != 4*/ && bond.GetLevelNumber() >= 30)
        {
            attackType = attackList[2];
        }
        else if (Input.GetButtonDown("Attack4") /*&& command != 4*/ && bond.GetLevelNumber() >= 45)
        {
            attackType = attackList[3];
        }
        /*if (!holdAttack)
        {*/       //Physics.CheckSphere(playerTransform.transform.position, 15f, enemyMask
            if (Input.GetButtonDown("Fire1") && command != 1 && Physics.OverlapSphere(playerTransform.transform.position, 15f, enemyMask).Length != 0)
            {
                if (!cooldown)
                {
                    followEnemy = false;
                    //if (wait != null) StopCoroutine(wait);
                    command = 4;
                    //timeWait = 0f;
                    obeyAnyway = true;
                    //wait = StartCoroutine(OrderWait());
                    Collider[] colliders = Physics.OverlapSphere(playerTransform.transform.position, 16f, enemyMask);
                    enemyTransform = colliders[0].transform;
                    chosenAttack = attackType;
                    holdAttack = true;
                }
                /*else
                {
                    holdAttack = true;
                }*/
            }
        //}
        

        if (followEnemy && Vector3.Distance(transform.position, enemyTransform.position) > 2f)
        {
            agent.SetDestination(enemyTransform.position);
        }

        topNode.Evaluate();
        if (topNode.nodeState == NodeState.FAILURE)
        {

        }

        OrderWait();
        CommandWait();
    }

    public int GetAttackType()
    {
        return chosenAttack;
    }

    public Transform GetEnemy()
    {
        return enemyTransform;
    }

    public int GetOrder()
    {
        return command;
    }

    public Vector3 GetPetDestination()
    {
        return petDestination;
    }

    public void CommandWait()
    {
        if (commandTime < commandWait)
        {
            commandTime += Time.deltaTime;
        }
    }

    /*public IEnumerator CommandWait()
    {
        yield return new WaitForSeconds(commandWait);
    }*/

    public IEnumerator CooldownWait()
    {
        yield return new WaitForSeconds(2f);
        if (holdAttack && command != 1 && enemyTransform.gameObject.activeInHierarchy)
        {
            followEnemy = false;
            //if (wait != null) StopCoroutine(wait);
            command = 4;
            //timeWait = 0f;
            obeyAnyway = true;
            //wait = StartCoroutine(OrderWait());
            //Collider[] colliders = Physics.OverlapSphere(playerTransform.transform.position, 16f, enemyMask);
            //enemyTransform = colliders[0].transform;
            chosenAttack = attackType;
        }
        //holdAttack = false;
        cooldown = false;
    }

    /*public IEnumerator OrderWait()
    {
        yield return new WaitForSeconds(baseBond + ((float)bond.GetLevelNumber() / 100) * (baseBond * 19));
        command = 0;
    }*/

    public void OrderWait()
    {
        if (timeWait < (baseBond + ((float)bond.GetLevelNumber() / 100) * (baseBond * 19)))
        {
            timeWait += Time.deltaTime;
        }
        else if (timeWait >= (baseBond + ((float)bond.GetLevelNumber() / 100) * (baseBond * 19)) && command != 1 && !obeyAnyway)
        {
            command = 0;
        }
    }

    /*public IEnumerator FollowEnemyAfterAttack()
    {
        yield return new WaitForSeconds(1.75f);
        followEnemy = false;
    }*/

    /*public IEnumerator Attack1Wait()
    {
        transform.LookAt(enemyTransform, Vector3.up);
        while (Vector3.Distance(transform.position, attackEnd) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, attackEnd, 0.2f);
            yield return new WaitForSeconds(0.01f);
        }
        while (Vector3.Distance(transform.position, attackStart) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, attackStart, 0.2f);
            yield return new WaitForSeconds(0.01f);
        }

        interact.enabled = true;
        agent.enabled = true;
        agent.isStopped = false;
        weapon.SetActive(false);
        currentStamina -= 0.1f;

        obeyAnyway = false;
        followEnemy = true;
    }*/

    public void AttackType1()
    {
        cooldown = true;
        StartCoroutine("CooldownWait");
        command = 0;
        /*
        attackStart = transform.position;
        attackEnd = enemyTransform.position;
        */
        attackEffect = 0;
        agent.isStopped = true;
        agent.enabled = false;
        interact.enabled = false;

        transform.LookAt(enemyTransform, Vector3.up);
        petAnimations.Play("PetTackle");
        //StartCoroutine("Attack1Wait");
    }
    public void AttackType2()
    {
        cooldown = true;
        StartCoroutine("CooldownWait");
        command = 0;

        attackEffect = 1;
        agent.isStopped = true;
        agent.enabled = false;
        interact.enabled = false;

        petAnimations.Play("PetSweep");
    }
    public void AttackType3()
    {
        cooldown = true;
        StartCoroutine("CooldownWait");
        command = 0;
    }
    public void AttackType4()
    {
        cooldown = true;
        StartCoroutine("CooldownWait");
        command = 0;
    }
}
