using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experience : MonoBehaviour {

    public int experience;
    public int level;

    private const int xpMax = 300;

    public void AddExperience(int xp)
    {
        experience += xp;
        if (experience > xpMax) experience = xpMax;


        if (experience >= 100 && level == 1)
        {
            StartCoroutine(LevelUpSequence());
        }
        else if (experience >= 300 && level == 2)
        {
            StartCoroutine(LevelUpSequence());
        }
    }

    private void Start()
    {
        experience = 0;
        level = 1;
    }

    private IEnumerator LevelUpSequence()
    {
        level++;
        yield return null;
        GetComponent<Unit>().UpdateAttributesForLevel(level);
    }
    
    public int GetNextLevelXP()
    {
        switch(level)
        {
            case 1:
                return 100;
            case 2:
                return 300;
            case 3:
                return 300;
            default:
                return 9999;
        }
    }
}
