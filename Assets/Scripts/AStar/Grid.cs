using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    [SerializeField]
    private GameObject slotPrefab;
    [SerializeField]
    private Vector2 gridWorldSize;



    Node[,] grid;

    int gridSizeX;
    int gridSizeY;

    void Start()
    {
        gridSizeX = (int)gridWorldSize.x;
        gridSizeY = (int)gridWorldSize.y;
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                grid[x, y] = new Node(true, new Vector2(x,y));
                GameObject newSlot = Instantiate(slotPrefab);
                newSlot.transform.SetParent(transform);
                newSlot.name = "Slot[" + x + ", " + y + "]";
            }
        }
    }
}
