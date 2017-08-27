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

    public Node[,] NodesGrid
    {
        get
        {
            return grid;
        }
    }

    void Awake()
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
                grid[x, y] = new Node(true, new Vector2(x,y), x, y);
                GameObject newSlot = Instantiate(slotPrefab);
                newSlot.transform.SetParent(transform);
                newSlot.name = "Slot[" + x + ", " + y + "]";
                newSlot.GetComponent<Slot>().node = grid[x, y];
                grid[x, y].slot = newSlot;
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0 || Mathf.Abs(x) == Mathf.Abs(y)) //Убираем диагональных соседей и центральный нод
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public bool AddBall(Ball ball)
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
                    GameController.Instance.FindSameAndColapse(grid[x, y]);
                    break;
                }
            }
            return true; //Свободное место было и шарик добавлен
        }
        return false; //Нет свободного места, шарик не добавлен
    }

    public Node NodeWorldFromPoint(Vector2 point)
    {
        int x = (int)point.x;
        int y = (int)point.y;

        return grid[x, y];
    }
}
