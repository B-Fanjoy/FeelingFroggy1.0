using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretDoorController : MonoBehaviour
{

    public GameObject secretDoor;
    public GameObject secretDoorText;
   
    void Start()
    {
        secretDoor.SetActive(true);
        secretDoorText.SetActive(false);
    }

    public void OpenDoor()
    {
        StartCoroutine(OpenDoorInternal());
    }

    private IEnumerator OpenDoorInternal()
    {
        Destroy(secretDoor);
        secretDoorText.SetActive(true);

        yield return new WaitForSeconds(3);

        secretDoorText.SetActive(false);
    }
}
