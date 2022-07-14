using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    
    public GameObject objectToDrop;

    // Start is called before the first frame update
    void Start()
    {
        //script to come!
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            objectToDrop.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
