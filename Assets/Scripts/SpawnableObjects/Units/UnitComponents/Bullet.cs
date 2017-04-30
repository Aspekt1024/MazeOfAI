using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private Unit owner;
    
    public void SetOwner(Unit owningUnit)
    {
        owner = owningUnit;
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit otherUnit = GetUnit(other.transform);
        if (otherUnit == null || otherUnit == owner) return;

        if (owner == null || owner.Friendly != otherUnit.Friendly)
        {
            otherUnit.Hit(1);
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
        }
    }

    private Unit GetUnit(Transform tf)
    {
        Unit unit = tf.GetComponent<Unit>();
        if (unit != null)
        {
            return unit;
        }

        unit = tf.GetComponentInParent<Unit>();
        if (unit != null)
        {
            return unit;
        }

        unit = tf.GetComponentInParent<Transform>().GetComponentInParent<Unit>();
        if (unit != null)
        {
            return unit;
        }
        return null;
    }
}
