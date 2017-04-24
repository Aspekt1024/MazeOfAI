using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Minimap : MonoBehaviour {

    public PathGridSection section;

    private PathGrid grid;
    private GameObject pixel;
    private Vector2 miniMapScale;
    private GameObject currentPos;
    private Texture2D texture;

    private void Awake()
    {
        pixel = Resources.Load<GameObject>("UI/MapPixel");
        miniMapScale = GetComponent<RectTransform>().sizeDelta;
        currentPos = Instantiate(pixel, transform);
        currentPos.GetComponent<Image>().color = Color.cyan;
        currentPos.transform.localScale *= 5f;

        SetupMapTexture();
    }

    private void Update()
    {
        CheckForGridReference();
        GetCurrentPosition();
        UpdateUnitPositions();
        GetMouseInput();
    }

    private void GetMouseInput()
    {
        if (Input.GetMouseButton(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject()) return;
            if (new Rect(transform.position.x - miniMapScale.x / 2, transform.position.y - miniMapScale.y / 2, miniMapScale.x, miniMapScale.y).Contains(Input.mousePosition))
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player == null) return;

                Vector3 worldPos = GetWorldPointFromMinimap(Input.mousePosition);
                player.transform.position = new Vector3(worldPos.x, player.transform.position.y, worldPos.z);
            }
        }
    }

    private Vector3 GetWorldPointFromMinimap(Vector2 minimapPos)
    {
        float xPos = section.transform.position.x - grid.gridWorldSize.x / 2 + (Input.mousePosition.x - (transform.position.x - miniMapScale.x / 2)) * grid.gridWorldSize.x / miniMapScale.x;
        float zPos = section.transform.position.z - grid.gridWorldSize.y / 2 + (Input.mousePosition.y - (transform.position.y - miniMapScale.y / 2)) * grid.gridWorldSize.y / miniMapScale.y;

        return new Vector3(xPos, 0f, zPos);
    }

    private void SetupMapTexture()
    {
        texture = new Texture2D((int)miniMapScale.x, (int)miniMapScale.y);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, new Color(0, 0, 0, 0));
            }
        }
    }

    private void CheckForGridReference()
    {
        if (grid != null) return;
        if (section.grid.GridGenerated)
        {
            grid = section.grid;
            DisplayMinimap();
        }
    }

    private void GetCurrentPosition()
    {
        Camera camera = Camera.current;
        if (camera == null) return;
        
        Ray ray = camera.ScreenPointToRay(transform.position);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Terrain"));

        if (hit.collider == null) return;

        float xPos = transform.position.x - miniMapScale.x / 2 + miniMapScale.x * section.grid.GetNodeFromWorldPoint(hit.point).gridX / (grid.gridSizeX - 1);
        float yPos = transform.position.y - miniMapScale.y / 2 + miniMapScale.y * section.grid.GetNodeFromWorldPoint(hit.point).gridY / (grid.gridSizeY - 1);
        currentPos.transform.position = new Vector3(xPos, yPos, 0);

    }

    private void UpdateUnitPositions()
    {

    }

    private void UpdateMinimap()
    {
        // TODO call this when buildings are placed. We can feed in new grid points directly
        DisplayMinimap();

    }

    private void DisplayMinimap()
    {
        if (grid == null) return;
        
        GetComponent<RawImage>().texture = texture;
        for (int y = 0; y < grid.gridSizeY; y++)
        {
            for (int x = 0; x < grid.gridSizeX; x++)
            {
                int xPos = Mathf.RoundToInt(miniMapScale.x * x / (grid.gridSizeX - 1));
                int yPos = Mathf.RoundToInt(miniMapScale.y * y / (grid.gridSizeY - 1));
                if (!grid.grid[x, y].walkable)
                {
                    texture.SetPixel(xPos, yPos, Color.blue);
                }
                else
                {
                    texture.SetPixel(xPos, yPos, new Color(0,0,0, 0));
                }
            }
        }
        texture.Apply();
    }

}
