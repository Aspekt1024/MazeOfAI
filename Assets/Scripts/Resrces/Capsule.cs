using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capsule : MonoBehaviour {

    public bool Assigned;
    public bool Landed;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Building" && !Landed)
        {
            if (other.collider.GetComponentInParent<Facility>() != null)
            {
                Debug.Log(other.collider.GetComponentInParent<Building>().name);    // TODO fix this activating for capsule springs
                GameData.Instance.Capsules++;
                Destroy(gameObject);
            }
        }
        else if (other.collider.tag == "Terrain")
        {
            Landed = true;
        }
    }
}
