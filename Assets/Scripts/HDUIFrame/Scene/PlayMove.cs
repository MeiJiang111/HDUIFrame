using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayMove : MonoBehaviour
{
    public NavMeshAgent agent;
    public Camera cam;
 
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { 
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
          
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.point);
                Vector3 point = hit.point;
                agent.SetDestination(point);
            }
        }
    }
}
