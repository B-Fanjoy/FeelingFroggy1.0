using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHover : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.color = Color.black;
    }

    void OnMousePlayButton(){
        GetComponent<Renderer>().material.color = Color.red;
    }

    void OnMouseOptionsButton(){
        GetComponent<Renderer>().material.color = Color.black;
    }

    void OnMouseExitButton(){
         GetComponent<Renderer>().material.color = Color.black;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
