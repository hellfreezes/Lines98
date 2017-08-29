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
    public List<Node> freeSlotsList = new List<Node>();

    int gridSizeX;
    int gridSizeY;

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
                grid[x, y].RemoveBall();
                freeSlotsList.Add(grid[x, y]); //добавляем вновь созданный нод в список свободных слотов
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

    public Vector2 GetCoordsForBall()
    {
        int x = 0;
        int y = 0;
        bool isFree = false;
        while (!isFree)
        {
            Node randomNode = freeSlotsList[Random.Range(0, freeSlotsList.Count - 1)];
            x = randomNode.gridX;
            y = randomNode.gridY;
            isFree = randomNode.walkable;
            if (isFree)
            {
                break;
            }
        }
        return new Vector2(x, y);
    }

    public void AddPreBall(Ball ball)
    {
        int x = (int)ball.coords.x;
        int y = (int)ball.coords.y;
        grid[x, y].SetPreBall(ball);
    }

    public bool AddBall(Ball ball)
    {
        if (freeSlotsList.Count > 0)
        {
            int x, y;
            bool isFree = false;

            x = (int)ball.coords.x;
            y = (int)ball.coords.y;
            isFree = grid[x, y].walkable;

            if (isFree) //Ячейка осталась свободной поэтому можно смело добавлять туда шарик и пробовать удалить шарики в ряду
            {
                grid[x, y].SetBall(ball);
                GameController.Instance.FindSameAndColapse(grid[x, y]);
                return true; //Свободное место было и шарик добавлен
            }
            else //К моменту когда пришел момент размещать шарик, его место уже занял шар передвинутый туда пользователем
            {
                while (!isFree)
                { //генерируем ячейку в которой надо появиться заново
                    Node randomNode = freeSlotsList[Random.Range(0, freeSlotsList.Count - 1)];
                    isFree = randomNode.walkable;
                    if (isFree)
                    {
                        randomNode.SetBall(ball);
                        GameController.Instance.FindSameAndColapse(randomNode);
                        break;
                    }
                }
                return true; //Шарик в конечном счете добавлен, возвращаем true
            }
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
