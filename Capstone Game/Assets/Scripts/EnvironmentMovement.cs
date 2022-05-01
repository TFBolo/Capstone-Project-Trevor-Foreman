using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentMovement : MonoBehaviour
{
    public Transform platform;
    // Update is called once per frame
    void Update()
    {
        platform.position += new Vector3(0.5f, 0f, 0f) * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.transform.SetParent(gameObject.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if  (other.tag == "Player")
        {
            gameObject.transform.DetachChildren();
        }
    }
}
