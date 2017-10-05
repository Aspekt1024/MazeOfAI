using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGridManager : MonoBehaviour {

    private Section[] sections;
    public List<Vector3> PathPoints;

    private void Start()
    {
        gameObject.AddComponent<PathRequestManager>();
        sections = FindObjectsOfType<Section>();
        DeploySections();
        CreateGrids();
    }

    public PathGridSection GetSectionFromWorldPoint(Vector3 point)
    {
        foreach (Section section in sections)
        {
            if (!section.IsType<PathGridSection>()) continue;
            if (((PathGridSection)section).grid.GridGenerated)
            {
                if (section.lowerBound.x < point.x && section.upperBound.x > point.x
                    && section.lowerBound.y < point.z && section.upperBound.y > point.z)
                {
                    return (PathGridSection)section;
                }
            }
        }
        return null;
    }

    private void DeploySections()
    {
        foreach (Section section in sections)
        {
            section.DeploySection();
        }
    }

    private void CreateGrids()
    {
        foreach (Section section in sections)
        {
            if (section.IsType<PathGridSection>())
                ((PathGridSection)section).GenerateGrid();
        }
    }
}
