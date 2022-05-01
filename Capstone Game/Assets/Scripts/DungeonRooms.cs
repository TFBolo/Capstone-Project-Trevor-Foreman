using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRooms : MonoBehaviour
{
    public GameObject[] walls;
    public GameObject[] doors;

    public void UpdateRooms(bool[] status)
    {
        for (int i=0; i < status.Length; i++)
        {
            doors[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
        }
    }
}
