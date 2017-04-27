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
