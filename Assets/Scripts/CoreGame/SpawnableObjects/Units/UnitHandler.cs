using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHandler : MonoBehaviour {

    public Transform enemyParent;
    public Transform playerParent;

    private void Awake()
    {
        foreach(Transform tf in transform)
        {
            if (tf.name == "Enemy")
            {
                enemyParent = tf;
            }
            else if (tf.name == "Player")
            {
                playerParent = tf;
            }
        }
    }

    public Transform[] GetEnemyUnits()
    {
        return enemyParent.GetComponentsInChildren<Transform>();
    }

    public Transform[] GetPlayerUnits()
    {
        return playerParent.GetComponentsInChildren<Transform>();
    }
}
