using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleController : MonoBehaviour
{
    float radius=30;
    float speed=0.4f;
    float angle=0;
    Vector3 lastDirection;

    Animator anim;

    void Start ()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
        lastDirection = new Vector3(1, 0, 0);
        lastDirection.Normalize();
    }

    void Update()
    {
        anim.SetInteger ("AnimationPar", 1);

        angle += speed * Time.deltaTime;
        angle = angle % (2*Mathf.PI);

        float newX = radius * (float)Mathf.Cos(angle);
        float newZ = radius * (float)Mathf.Sin(angle);

        Vector3 newPosition = new Vector3(newX, transform.position.y, newZ);
        Vector3 newDirection = newPosition - transform.position;

        newDirection.Normalize();

        float rAngle = -Vector3.Angle(lastDirection, newDirection);
        transform.Rotate(Vector3.up, rAngle, Space.World);

        transform.position = newPosition;
        lastDirection = newDirection;
    }
}
