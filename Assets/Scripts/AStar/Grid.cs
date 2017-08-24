using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    [SerializeField]
    private GameObject slotPrefab;
    [SerializeField]
    private Vector2 gridWorldSize;

    public static Grid instance;

    Node[,] grid;

    int gridSizeX;
    int gridSizeY;

    int freeSlots;

    public int GridSizeX
    {
        get
        {
            return gridSizeX;
        }
    }

    public int GridSizeY
    {
        get
        {
            return gridSizeY;
        }
    }

    public int FreeSlots
    {
        get
        {
            return freeSlots;
        }

        set
        {
            freeSlots = value;
        }
    }

    void Start()
    {
        if (instance != null)
        {
            Debug.LogWarning("Обнаружено две копии Grid");
        }
        instance = this;

        gridSizeX = (int)gridWorldSize.x;
        gridSizeY = (int)gridWorldSize.y;
        freeSlots = gridSizeX * gridSizeY;
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
                grid[x, y].slot = newSlot;
            }
        }
    }

    public void AddBall(Ball ball)
    {
        if (freeSlots > 0)
        {
            int x, y;
            bool isFree = false;
            while (!isFree)
            {
                x = Random.Range(0, gridSizeX);
                y = Random.Range(0, gridSizeY);
                isFree = grid[x, y].walkable;
                if (isFree)
                {
                    grid[x, y].SetBall(ball);
                    break;
                }
            }
        }
    }
}
