using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    [SerializeField]
    Sprite[] balls;
    [SerializeField]
    GameObject gameDesk;
    [SerializeField]
    int ballsAtTurn = 3;
    [SerializeField]
    int mustBeSameColor = 5;
    [SerializeField]
    Text scoreField;
    [SerializeField]
    float animationMoveSpeedDelay = 0.1f;
    [SerializeField]
    Image[] nextBalls;

    bool isAnimationInProcess = false;

    private List<Ball> preBalls = new List<Ball>();

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

        GenerateBalls(ballsAtTurn); //Генерируем первые шары
        PlaceBalls();
        GenerateBalls(ballsAtTurn);
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

    public void GenerateBalls(int maxBalls)
    {
        int freeBalls = Grid.instance.freeSlotsList.Count;
        //Debug.Log("Сободных ячеек: " + freeBalls);
        for (int i = 0; i < maxBalls; i++)
        {
            if (freeBalls > 0)
            {
                int code = Random.Range(0, balls.Length);
                Vector2 coords = Grid.instance.GetCoordsForBall();
                Ball b = new Ball();
                b.code = code;
                b.icon = balls[code];
                nextBalls[i].sprite = balls[code];
                b.coords = coords;
                preBalls.Add(b); //Генерируем новый шарик на будущее и добавляем их в список сгенерированных
                Grid.instance.AddPreBall(b);
                //freeBalls--; //Контролирум количество будущих свободных слотов (и не трогаем число свободных слотов в настоящее время)
            }
            else
            { //свободные слоты кончилис. прервать цикл
                break;
            }
        }
    }

    bool PlaceBalls()
    {
        hand = null;
        int generated = 0; //считаем сколько шаров получилось сгенерировать

        foreach (Ball ball in preBalls)
        {
            if (!Grid.instance.AddBall(ball)) //Пробуем разместить шарик
            {
                break; //Если не получается, то прерываем цикл
            }
            generated++; //считаем сколько разместили шариков
        }
        preBalls.Clear(); //Чистим список преШаров
        if (generated < 3)
            return false; //возвращаем ложь если смогли сгенерировать 
        else
            return true;
    }

    void OnSlotClicked()
    {
        if (Input.GetMouseButtonDown(0) && !isAnimationInProcess)
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
                    if (hand != null)
                    {   //Если шарик до этого был выбран
                        hand.DeselectBall();
                    }
                    hand = slot;
                    slot.SelectBall();
                }
                else
                {
                    //Рука уже была с шариком
                    if (hand == slot)
                    {
                        //кликнули по томуже шарику
                        //снять выделение
                        hand = null;
                        slot.DeselectBall();
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
                        //Старт перемещения шарика. После перемещения вызывается проверка на 5 в ряд.
                        StartCoroutine(MoveBall(hand.node, slot.node, path));
                    }
                    else
                    {
                        Debug.Log("Путь не найден");
                        hand.DeselectBall();
                        hand = null;
                    }
                }
            }
        }
    }

    IEnumerator MoveBall(Node startNode, Node endNode, List<Node> path)
    {
        isAnimationInProcess = true; //Говорим что началась анимация (чтобы блокировать в ее время любые дейситвия)
        Ball tempBall = null; //Если до этого в ячейке был предварительный шар, то на всякий сохраняем его в темп переменную
        if (endNode.preBall != null)
        {
            tempBall = endNode.preBall;
        }
        int i = -1;
        
        Node currentNode = startNode;
        Ball startBall = startNode.ball.CopyBall();
        startNode.RemoveBall();

        while (currentNode != endNode) {
            i++;
            currentNode = path[i];

            path[i].slot.GetComponent<Slot>().pathImage.gameObject.SetActive(true);

            yield return new WaitForSeconds(animationMoveSpeedDelay);
        }
        isAnimationInProcess = false; //Конец анимации
        endNode.SetBall(startBall);
        currentNode = path[0];
        foreach(Node p in path)
        {
            p.slot.GetComponent<Slot>().pathImage.gameObject.SetActive(false);
        }
        if (!FindSameAndColapse(endNode)) // <--------------------------------ПРОВЕРКА на 5 в ряд!!!!!!!!!!!!!
        {
            //Шар которым мы только что управляли не будет удален поэтому,
            if (endNode.preBall != null) //На месте куда встал шар, должен был появится шар в новом ходу, то надо убрать оттуда пребалл и переназначит его в другое место
            {
                endNode.RemovePreBall();
                GenerateBalls(1);
            }
            if (!PlaceBalls()) //Пробуем разместить шарики. Если вернулось false то
            {
                GameOver(); //Игра окончена
            } //если true то продолжаем
            if (Grid.instance.freeSlotsList.Count <= 0)
            {
                GameOver(); //Игра окончена если не осталось свободных слотов
            }
            GenerateBalls(ballsAtTurn); //Генерируем шарики на след ход
        }
        else
        {
            //Удаление было успешно. Нужно проверить а был ли в ключевой ячейке до этого преБалл
            //И если был то вернуть его назад
            if (tempBall != null)
            {
                endNode.SetPreBall(tempBall);
            }
        }
        hand = null; //освобождаем руку
    }

    private void GameOver()
    {
        //Debug.Log("Игра окончена!");
        ScenesController.Instance.GameOver();
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
                n.slot.GetComponent<Slot>().DieAndRemoveBall();
                ScoreController.Instance.AddScore(1);
                scoreField.text = "Score: " + ScoreController.Instance.Score;
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
