using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCatch : MonoBehaviour
{
    public Vector3 returnPoint = new Vector3(19f, 2.5f, -40f);

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.transform.position = returnPoint;
        }
    }
}
