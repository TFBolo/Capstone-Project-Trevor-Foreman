using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectIdentify : MonoBehaviour
{
    [SerializeField] private int objectNum;
    public GameObject interactText;
    public Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) < 5f)
        {
            interactText.SetActive(true);
        }
        else
        {
            interactText.SetActive(false);
        }
        if (interactText.activeInHierarchy)
        {
            interactText.transform.LookAt(Camera.main.transform);
            interactText.transform.Rotate(0, 180, 0);
        }
    }

    public void PlayerCall()
    {
        GameObject.FindGameObjectWithTag("UI").GetComponent<InteractableController>().OpenInteraction(objectNum);
    }
}
