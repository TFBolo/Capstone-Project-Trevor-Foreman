using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectEffects : MonoBehaviour
{
    [SerializeField] private bool conveyorBelt;
    [SerializeField] private float speed;
    private List<GameObject> touchingBelt = new List<GameObject>();

    [SerializeField] private bool playerCatch;
    [SerializeField] private GameObject climbPoint;

    [SerializeField] private bool isButton;
    [SerializeField] private GameObject buttonAct;
    private List<GameObject> onButton = new List<GameObject>();

    [SerializeField] private bool obstacle;
    [SerializeField] private Animator obstacleAnimation;
    [SerializeField] private bool oneTimeAct;

    [SerializeField] private bool startStop;
    [SerializeField] private bool differentOnOff;
    [SerializeField] private float intervalTimeOn;
    [SerializeField] private float intervalTimeOff;
    private bool timerBool = true;
    private float currentTime;

    [SerializeField] private GameObject visualEffect;
    [SerializeField] private Animator objectAnimation;

    private void Start()
    {
        if (!differentOnOff)
        {
            intervalTimeOff = intervalTimeOn;
        }
    }

    void Update()
    {
        if (conveyorBelt && timerBool)
        {
            for (int i = 0; i < touchingBelt.Count; i++)
            {
                if (touchingBelt[i].activeInHierarchy)
                {
                    if (touchingBelt[i].tag == "Player" || touchingBelt[i].tag == "Item")
                    {
                        touchingBelt[i].transform.position += transform.forward * speed * Time.deltaTime;
                    }
                    else if (touchingBelt[i].tag == "Enemy")
                    {
                        NavMeshAgent agent = touchingBelt[i].GetComponent<NavMeshAgent>();
                        if (agent.enabled)
                        {
                            agent.Move(transform.forward * speed * Time.deltaTime);
                        }
                    }
                    else if (touchingBelt[i].tag == "Pet")
                    {
                        NavMeshAgent agent = touchingBelt[i].GetComponent<NavMeshAgent>();
                        if (touchingBelt[i].GetComponent<PetAI>().command == 1)
                        {
                            touchingBelt.RemoveAt(i);
                        }
                        else if (agent.enabled)
                        {
                            agent.Move(transform.forward * speed * Time.deltaTime);
                        }
                    }
                }
                else
                {
                    touchingBelt.RemoveAt(i);
                }
            }

            if (visualEffect != null)
            {
                visualEffect.SetActive(true);
            }
            if (objectAnimation != null)
            {
                objectAnimation.Play("Animate");
            }
        }
        else if (conveyorBelt && !timerBool)
        {
            if (visualEffect != null)
            {
                visualEffect.SetActive(false);
            }
            if (objectAnimation != null)
            {
                objectAnimation.Play("New State");
            }
        }

        if (isButton)
        {
            for (int i = 0; i < onButton.Count; i++)
            {
                if (!onButton[i].activeInHierarchy)
                {
                    onButton.RemoveAt(i);
                }
                else if (onButton[i].tag == "Pet")
                {
                    if (onButton[i].GetComponent<PetAI>().command == 1)
                    {
                        onButton.RemoveAt(i);
                    }
                }
            }
            
            if (onButton.Count > 0)
            {
                buttonAct.GetComponent<ObjectEffects>().ButtonOn();
            }
            else if (onButton.Count == 0)
            {
                buttonAct.GetComponent<ObjectEffects>().ButtonOff();
            }
        }

        if (startStop)
        {
            if (timerBool)
            {
                OnTimer();
            }
            else
            {
                OffTimer();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {   
        if (!touchingBelt.Contains(other.gameObject) && conveyorBelt)
        {
            if (other.tag == "Player" || other.tag == "Pet" || other.tag == "Enemy" || other.tag == "Item")
            {
                touchingBelt.Add(other.gameObject);
            }
        }
        else if (playerCatch)
        {
            if (other.tag == "Player")
            {
                other.transform.position = climbPoint.transform.position;
            }
        }
        else if (!onButton.Contains(other.gameObject) && isButton)
        {
            if (other.tag == "Player" || other.tag == "Pet" || other.tag == "Enemy")
            {
                onButton.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (touchingBelt.Contains(other.gameObject))
        {
            touchingBelt.Remove(other.gameObject);
        }
        else if (onButton.Contains(other.gameObject))
        {
            onButton.Remove(other.gameObject);
        }
    }

    private void OnTimer()
    {
        if (currentTime < intervalTimeOn)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            timerBool = false;
            currentTime = 0;
        }
    }

    private void OffTimer()
    {
        if (currentTime < intervalTimeOff)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            timerBool = true;
            currentTime = 0;
        }
    }

    private void ButtonOn()
    {
        if (obstacleAnimation.GetCurrentAnimatorStateInfo(0).IsName("MoveReverse"))
        {
            if (obstacleAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                obstacleAnimation.Play("Move");
            }
            else
            {
                float playTime = 1f - obstacleAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime;
                obstacleAnimation.Play("Move", 0, playTime);
            }
        }
        else
        {
            obstacleAnimation.Play("Move");
        }
    }

    private void ButtonOff()
    {
        if (!oneTimeAct)
        {
            if (obstacleAnimation.GetCurrentAnimatorStateInfo(0).IsName("Move"))
            {
                if (obstacleAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    obstacleAnimation.Play("MoveReverse");
                }
                else
                {
                    float playTime = 1f - obstacleAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    obstacleAnimation.Play("MoveReverse", 0, playTime);
                }
            }
            else
            {
                obstacleAnimation.Play("MoveReverse");
            }
        }
    }
}
