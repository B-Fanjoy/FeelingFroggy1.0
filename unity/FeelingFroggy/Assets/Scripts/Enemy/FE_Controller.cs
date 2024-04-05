using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FE_Controller : MonoBehaviour
{
    public NavMeshAgent AirAgent;
    public Transform target;
    bool chase = false;
    float time = 0.0f;
    //float smooth = 1f;
    //Vector3 newAngle;

    // Update is called once per frame
    void Update()
    {
        time+=Time.deltaTime;

        if(chase==true)
        {
            transform.LookAt(target.position);  
            AirAgent.SetDestination(target.position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            chase=true;
    }

    void OnTriggerExit(Collider other)
    {
        chase=false;
    }
}
