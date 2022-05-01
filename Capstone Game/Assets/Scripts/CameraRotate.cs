using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public Transform cam;
    public Transform player;

    public List<RaycastHit> raycastHits = new();

    private Vector3 desiredPosition;
    private LayerMask groundMask;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        groundMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        desiredPosition = player.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * cam.eulerAngles.y) * 0.75f, 0.75f, -Mathf.Sin(Mathf.Deg2Rad * cam.eulerAngles.y) * 0.75f);
        
        Vector3 startVector = player.position + new Vector3(0f, 0.75f, 0f);
        if (Physics.Linecast(startVector, desiredPosition, out RaycastHit wallPosition, groundMask))
        {
            desiredPosition = wallPosition.point + (cam.right * -0.2f);
        }
        transform.position = desiredPosition;
        
        if (raycastHits.Count > 0)
        {
            foreach (RaycastHit i in raycastHits)
            {
                GameObject wall = i.transform.gameObject;
                if (((1 << wall.layer) & groundMask) != 0)
                {
                    wall.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
            }
        }
        

        RaycastHit[] rayHitArray = Physics.RaycastAll(transform.position, cam.position - transform.position, Vector3.Distance(cam.position, transform.position), groundMask);
        raycastHits.Clear();
        foreach (RaycastHit i in rayHitArray)
        {
            if (i.transform.gameObject.GetComponent<MeshRenderer>() != null)
            {
                raycastHits.Add(i);
            }
        }

        if (raycastHits.Count > 0)
        {
            foreach (RaycastHit i in raycastHits)
            {
                GameObject wall = i.transform.gameObject;
                if (((1 << wall.layer) & groundMask) != 0)
                {
                    wall.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
            }
        }
    }
}
