using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Unit {
    
    protected override void SetupAttributes()
    {
        Name = "Drone";
        elevation = 0.5f;
    }

}
