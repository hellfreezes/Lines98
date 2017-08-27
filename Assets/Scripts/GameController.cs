using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    [SerializeField]
    Ball[] balls;
    [SerializeField]
    GameObject gameDesk;
    [SerializeField]
    int ballsAtTurn = 3;
    [SerializeField]
    int mustBeSameColor = 5;
    [SerializeField]
    Text scoreField;

    int score = 0;

    private Slot hand = null;

    private static GameController instance;

    public static GameController Instance
    {
        get
        {
            return instance;
        }
    }

    // Use this for initialization
    void Start () {
        if (instance != null)
        {
            Debug.LogWarning("ЛАЖА!");
        }
        instance = this;


        GenerateBalls(); //Генерируем первые шары
    }
	
	// Update is called once per frame
	void Update () {
        OnSlotClicked();
    }

    void UpdateGameDesk()
    {
        for (int x = 0; x < Grid.instance.GridSizeX; x++)
        {
            for (int y = 0; y < Grid.instance.GridSizeY; y++)
            {

            }
        }
    }

    public bool GenerateBalls()
    {
        hand = null;
        int generated = 0; //считаем сколько шаров получилось сгенерировать
        for (int i = 1; i<= ballsAtTurn; i++)
        {
            int newBall = Random.Range(0, balls.Length);
            if (!Grid.instance.AddBall(balls[newBall]))
            {
                break;
            }
            generated++;
        }
        if (generated < 3)
            return false; //возвращаем ложь если смогли сгенерировать только один шар
        else
            return true;
    }

    void OnSlotClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {         
            GameObject go = EventSystem.current.currentSelectedGameObject;
            if (go == null || go.transform.parent == null)
                return;
            GameObject parent = go.transform.parent.gameObject;
            Slot slot = parent.GetComponent<Slot>();

            if (slot != null && !slot.node.walkable)
            {
                //Кликнули по шарику
                if (hand == null || (hand != null && hand != slot))
                {
                    //Шарик не был выбран до этого
                    //Или шарик был выбран, но клинули по другому шарику
                    //выбираем шарик
                    hand = slot;
                }
                else
                {
                    //Рука уже была с шариком
                    if (hand == slot)
                    {
                        //кликнули по томуже шарику
                        //снять выделение
                        hand = null;
                    }
                }

            }
            else
            {
                //Кликнули по пустому слоту
                if (hand != null)
                {
                    //Шарик выбран, пытаемся переместить
                    List<Node> path = Pathfinding.Instance.FindPath(hand.node, slot.node);
                    if (path != null)
                    {
                        slot.node.SetBall(hand.node.ball);
                        hand.node.RemoveBall();
                        if (!FindSameAndColapse(slot.node))
                        {
                            if (!GenerateBalls())
                            {
                                GameOver();
                            }
                        }
                        hand = null;
                    }
                    else
                    {
                        Debug.Log("Путь не найден");
                        hand = null;
                    }
                }
            }
        }
    }

    private void GameOver()
    {
        Debug.Log("Игра окончена!");
    }

    //Проверяем количество смежных шаров цветом идентичном шару from.
    public bool FindSameAndColapse(Node from)
    {
        List<Node> horizontal = new List<Node>();
        List<Node> vertical = new List<Node>();
        List<Node> diagonal1 = new List<Node>();
        List<Node> diagonal2 = new List<Node>();

        //Проверяем по горизонтали
        horizontal = CheckForMatch(horizontal, new Vector2(1, 0), from);
        horizontal = CheckForMatch(horizontal, new Vector2(-1, 0), from);

        //Проверяем по вертикали
        vertical = CheckForMatch(vertical, new Vector2(0, 1), from);
        vertical = CheckForMatch(vertical, new Vector2(0, -1), from);

        //Проверяем по диагонали 1
        diagonal1 = CheckForMatch(diagonal1, new Vector2(1, 1), from);
        diagonal1 = CheckForMatch(diagonal1, new Vector2(-1, -1), from);

        //Проверяем по диагонали 2
        diagonal2 = CheckForMatch(diagonal2, new Vector2(-1, 1), from);
        diagonal2 = CheckForMatch(diagonal2, new Vector2(1, -1), from);

        List<Node> toColapse = new List<Node>();
        if (horizontal.Count >= mustBeSameColor)
            toColapse = UnionNodes(toColapse, horizontal);
        if (vertical.Count >= mustBeSameColor)
            toColapse = UnionNodes(toColapse, vertical);
        if (diagonal1.Count >= mustBeSameColor)
            toColapse = UnionNodes(toColapse, diagonal1);
        if (diagonal2.Count >= mustBeSameColor)
            toColapse = UnionNodes(toColapse, diagonal2);


        if (toColapse.Count >= mustBeSameColor)
        {
            foreach (Node n in toColapse)
            {
                n.RemoveBall();
                score += 1;
                scoreField.text = "Score: " + score;
            }

            return true;
        }

        return false;

    }

    List<Node> CheckForMatch(List<Node> nodesList, Vector2 direction, Node startNode)
    {
        int xDir = (int)direction.x;
        int yDir = (int)direction.y;
        int x = startNode.gridX;
        int y = startNode.gridY;
        Node currentNode = startNode;
        while (currentNode.walkable == false && currentNode.ball.code == startNode.ball.code)
        {
            if (!nodesList.Contains(currentNode))
            {
                nodesList.Add(currentNode);
            }
            x += xDir;
            y += yDir;
            if (!IsInRange(x, y))
                break;
            currentNode = Grid.instance.NodesGrid[x, y];
        }

        return nodesList;
    }

    List<Node> UnionNodes(List<Node> addTo, List<Node> addFrom)
    {
        foreach(Node node in addFrom)
        {
            if (!addTo.Contains(node))
            {
                addTo.Add(node);
            }
        }
        return addTo;
    }

    bool IsInRange(int x, int y)
    {
        if (x >= 0 && y >= 0 && x <= Grid.instance.GridSizeX-1 && y <= Grid.instance.GridSizeY-1)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
