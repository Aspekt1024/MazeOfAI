using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementGrid : MonoBehaviour {
    
    private PathGrid worldGrid = null;

    private GameObject building;
    private Vector3 halfExtents;

    private const int numPieces = 16;
    private Transform[] gridPieces = new Transform[numPieces];

    private void Awake()
    {
        CreateGridPieces();
    }

    private void CreateGridPieces()
    {
        GameObject prefab = Resources.Load<GameObject>("Buildings/PlacementGrid");
        for (int i = 0; i < numPieces; i++)
        {
            gridPieces[i] = Instantiate(prefab, transform).transform;
            gridPieces[i].position = Vector3.down * 20f;
        }
    }

    private void Update()
    {
        if (building == null) return;
        
        if (worldGrid == null)
        {
            worldGrid = FindObjectOfType<PathGrid>();
            if (worldGrid == null) return;
        }

        PlaceBuildGrid();
    }

    public void SetNewBounds(GameObject obj)
    {
        building = obj;
        foreach(Transform tf in building.transform)
        {
            if (tf.gameObject.layer == LayerMask.NameToLayer("BuildingBounds"))
            {
                Collider collider = tf.GetComponent<Collider>();
                if (collider != null)
                    halfExtents = collider.bounds.extents;
                return;
            }
        }

        halfExtents = Vector2.zero;
    }

    public void Disable()
    {
        building = null;
        foreach (Transform gridPiece in gridPieces)
        {
            gridPiece.position = Vector3.down * 20f;
        }
    }

    private void PlaceBuildGrid()
    {
        PathNode topRight = worldGrid.GetNodeFromWorldPoint(building.transform.position + halfExtents);
        PathNode bottomLeft = worldGrid.GetNodeFromWorldPoint(building.transform.position - halfExtents);

        int width = topRight.gridX - bottomLeft.gridX + 1;
        int height = topRight.gridY - bottomLeft.gridY + 1;
        
        int pieceNum = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridPieces[pieceNum].position = bottomLeft.worldPosition + new Vector3(x, 0.01f, y);
                SetGridPieceColor(gridPieces[pieceNum]);
                pieceNum++;
                if (pieceNum == numPieces)
                    break;
            }
            for (int i = pieceNum; i < numPieces; i++)
            {
                gridPieces[i].position = Vector3.down * 20f;
            }
        }
    }

    private void SetGridPieceColor(Transform gridPiece)
    {
        Color colorToSet = Color.green;
        if (!worldGrid.GetNodeFromWorldPoint(gridPiece.position).walkable)
        {
            colorToSet = Color.red;
        }

        gridPiece.GetComponent<MeshRenderer>().material.SetColor(Shader.PropertyToID("_Color"), colorToSet);
    }

}
