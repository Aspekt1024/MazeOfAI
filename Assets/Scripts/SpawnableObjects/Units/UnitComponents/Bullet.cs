using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Drone>() != null)
        {
            other.GetComponentInParent<Drone>().Hit(1);
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
