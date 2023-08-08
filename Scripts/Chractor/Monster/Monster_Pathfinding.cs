using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Tilemaps;

class Monster_Pathfinding : MonoBehaviour
{
    public enum findDirection
    {
        None = -2,
        All = -1,
        Top = 0,
        Left,
        Bottom,
        Right,
        LeftTop,
        LeftBottom,
        RightBottom,
        RightTop,
    };

    private class Node
    {
        public Vector2Int pos;
        public Vector2Int searchPos;
        public float g, h, f;
        public Node parent;
        public findDirection dir; //해당 노드가 검사를 재개해야할 방향
    }
    
    public GameObject target;
    new public Camera camera;
    public Grid grid;
    public Tilemap tilemap;
    
    Vector2Int startPos;
    Vector2Int targetPos;
    Vector2Int mapSize;

    bool bArrivedTarget;
    bool bArrivedCorner;
    bool bExtendedSearch;
    bool bSetSearchPos;
    [HideInInspector] public bool bReset;
    int count;

    PriorityQueue<Node> openList;
    PriorityQueue<Node> closeList;
    Node startNode;
    Node lastNode;
    Node currentNode;

    [HideInInspector]
    public List<Vector2Int> path;

    void Awake()
    {
        bArrivedTarget = false;
        bArrivedCorner = false;
        bExtendedSearch = false;
        bSetSearchPos = false;
        bReset = false;
        count = 0;

        startNode = new Node();
        lastNode = new Node();
        currentNode = new Node();

        openList = new PriorityQueue<Node>();
        closeList = new PriorityQueue<Node>();
        path = new List<Vector2Int>();
        //Test
        mapSize = new Vector2Int(40, 20);
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        //    Vector3Int mousePosInt = new Vector3Int((int)Input.mousePosition.x, (int)Input.mousePosition.y, (int)Input.mousePosition.z);
        //    
        //    RaycastHit hit;
        //    if (Physics.Raycast(ray.origin, ray.direction, out hit))
        //    {
        //        Vector3Int hitPosInt = new Vector3Int(Mathf.FloorToInt(hit.point.x), Mathf.FloorToInt(hit.point.y), 0);
        //        
        //        //길찾기 테스트//////////////////////////////////////////////////////////////////////////
        //        startPos = new Vector2Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.y);
        //        targetPos = new Vector2Int((int)hit.point.x, (int)hit.point.y);
        //        print("마우스 위치 : " + targetPos);
        //        JPS();
        //    }
        //}
    }

    public List<Vector2Int> FindPath(Vector2 target)
    {
        startPos = new Vector2Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.y);
        targetPos = new Vector2Int((int)target.x, (int)target.y);

        JPS();

        return path;
    }

    public List<Vector2Int> ReFindPath(Vector2 target)
    {
        bReset = true;

        return FindPath(target);
    }


    public void SetMapSize(Vector2Int size)
    {
        mapSize = size;
    }

    public void SetMapSize(Vector2 size)
    {
        Vector2Int sizeInt = new Vector2Int((int)size.x, (int)size.y);
        mapSize = sizeInt;
    }

    private void JPS()
    {
        if (startPos == targetPos)
            return;

        if (targetPos.x < 0 || targetPos.x > mapSize.x || targetPos.y < 0 || targetPos.y > mapSize.y) //테스트 후 삭제
            return;

        path.Clear();
        openList.Clear();
        closeList.Clear();
        bArrivedTarget = false;

        //시작노드 생성후 우선순위 큐에 삽입
        startNode.pos = startPos;
        startNode.searchPos = startPos;
        startNode.g = 0;
        startNode.h = targetPos.x + targetPos.y;
        startNode.f = startNode.h;
        startNode.parent = null;
        startNode.dir = findDirection.All;
        openList.Enqueue(startNode, startNode.f);
        
        while (true)
        {
            if (openList.Count == 0 || bArrivedTarget == true) break;

            //if (bReset == true)
            //{
            //    bReset = false;
            //    JPS();
            //    return;
            //}
            
            //새로운 노드 우선순위 큐에서 가중치 높은 순으로 Dequeue후 currentNode에 대입
            currentNode = openList.Dequeue();
            closeList.Enqueue(currentNode, currentNode.f);

            //8방향 탐색
            if (currentNode.dir == findDirection.All) //시작노드라면 8방향 탐색
            {
                //print("8방향 탐색===============================");

                for (int i = 0; i < 8; i++)
                {
                    if (i < 4) //직선 탐색
                    {
                        //print("탐색 시작 위치 : " + currentNode.searchPos);
                        //print("탐색 시작 방향 : " + currentNode.dir);
                        SearchStraight(currentNode, currentNode.searchPos, (findDirection)i);
                    }
                    else //대각선 탐색
                    {
                        //print("탐색 시작 위치 : " + currentNode.searchPos);
                        //print("탐색 시작 방향 : " + currentNode.dir);
                        Searchdiagonal(currentNode, currentNode.searchPos, (findDirection)i);
                    }
                    if (bArrivedTarget) break;
                    count = 0;
                    bArrivedCorner = false;
                    bExtendedSearch = false;
                    bSetSearchPos = false;
                }
            }
            else //시작노드가 아니라면 부모노드의 방향대로 이어서 탐색
            {
                //print("특정방향 탐색==============================");

                int dirNum = (int)currentNode.dir;
            
                if (dirNum < 4) //직선 탐색
                {
                    //print("탐색 시작 위치 : " + currentNode.searchPos);
                    //print("탐색 시작 방향 : " + currentNode.dir);
                    SearchStraight(currentNode, currentNode.searchPos, currentNode.dir);
                }
                else //대각선 탐색
                {
                    //print("탐색 시작 위치 : " + currentNode.searchPos);
                    //print("탐색 시작 방향 : " + currentNode.dir);
                    Searchdiagonal(currentNode, currentNode.searchPos, currentNode.dir);
                }
                if (bArrivedTarget) break;
                count = 0;
                bArrivedCorner = false;
                bExtendedSearch = false;
                bSetSearchPos = false;
            }
        }

        //타겟을 찾았다면 path 생성
        if (bArrivedTarget)
        {
            Node tempNode = lastNode;
            while(tempNode.parent != null)
            {
                path.Add(tempNode.pos);
                tempNode = tempNode.parent;
            }
        }
        path.Reverse();

        ////test
        //print("Path Count : " + path.Count);
        //for (int i = 0; i < path.Count; i++)
        //    print("Path[" + i + "] : " + path[i]);
    }

    //노드 생성(코너 데이터X)
    private Node CreateNode(Vector2Int pos, findDirection dir, ref Node parent)
    {
        //노드 생성(코너를 발견한 지점에), 새 노드의 부모 노드는 탐색시작노드
        Node newNode = new Node();

        newNode.pos = pos;
        newNode.searchPos = pos;
        newNode.dir = dir;
        newNode.parent = parent;
        newNode.g = Mathf.Sqrt(Mathf.Pow((startPos.x - pos.x), 2) + Mathf.Pow((startPos.y - pos.y), 2));
        newNode.h = Mathf.Abs(targetPos.x - pos.x) + Mathf.Abs(targetPos.y - pos.y);
        newNode.f = newNode.g + newNode.h;
        
        openList.Enqueue(newNode, newNode.f);

        return newNode;
    }


    //직선 탐색
    private void SearchStraight(Node searchNode, Vector2Int pos, findDirection dir)
    {
        Vector2Int tempPos = pos;
        
        while (true)
        {
            //코너 이외의 종료 조건
            if (tempPos.x > mapSize.x || tempPos.x < 0 || tempPos.y > mapSize.y || tempPos.y < 0) //맵 끝에 도달
            {
                return;
            }

            if (tempPos == targetPos) //종료조건 : 현재위치가 목적지라면
            {
                //print("목적지 도달");
                
                lastNode.pos = tempPos;
                lastNode.dir = findDirection.None;
                lastNode.parent = searchNode;
                lastNode.g = Mathf.Sqrt(Mathf.Pow((startPos.x - tempPos.x), 2) + Mathf.Pow((startPos.y - tempPos.y), 2));
                lastNode.h = Mathf.Abs(targetPos.x - tempPos.x) + Mathf.Abs(targetPos.y - tempPos.y);
                lastNode.f = lastNode.g + lastNode.h;
                
                bArrivedTarget = true;
                return;
            }

            switch (dir)
            {
                case findDirection.Top:
                    {
                        //코너 없는 장애물에 도달
                        if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y + 1, 0)) || tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y, 0))) 
                            return;
        
                        //if (count > 0)
                        //{
                            //TODO : tempPos.y + 1부분도 검사해야 할지 볼것
                            if (tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y + 1, 0))) && tempPos.y + 1 < mapSize.y) //코너(강제이웃) 탐지
                            {
                                //print("코너 발견");
                                //print("코너 위치 : " + tempPos);

                                if (bExtendedSearch)
                                {
                                    Node testNode;
                                    //코너를 발견한 지점(에서 코너지점으로 대각선 탐색)
                                    if (currentNode.pos == searchNode.pos)
                                    {
                                        testNode = CreateNode(tempPos, findDirection.LeftTop, ref currentNode);
                                        bSetSearchPos = true;
                                    }
                                    else
                                    {
                                        testNode = CreateNode(tempPos, findDirection.LeftTop, ref searchNode);
                                    }
                                    //print("대각선 보조탐색 코너 발견지점 휴리스틱 : " + testNode.f);
    
                                    bArrivedCorner = true;
                                }
                                else
                                {
                                    //코너 발견 지점의 시작지점
                                    Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);
                                    //코너를 발견한 지점(에서 코너지점으로 대각선 탐색)
                                    CreateNode(new Vector2Int(tempPos.x - 1, tempPos.y + 1), findDirection.LeftTop, ref newNode);
                                }
                                
                                return;
                            }
                            else if (tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y + 1, 0))) && tempPos.y + 1 < mapSize.y) //코너(강제이웃) 탐지
                            {
                                //print("코너 발견");
                                //print("코너 위치 : " + tempPos);
 
                                //코너를 발견한 지점(에서 코너지점으로 대각선 탐색)
                                if (bExtendedSearch)
                                {
                                    Node testNode;
                                    if (currentNode.pos == searchNode.pos)
                                    {
                                        testNode = CreateNode(tempPos, findDirection.RightTop, ref currentNode);
                                        bSetSearchPos = true;

                                    }
                                    else
                                    {
                                        testNode = CreateNode(tempPos, findDirection.RightTop, ref searchNode);
                                    }
                                    //print("대각선 보조탐색 코너 발견지점 휴리스틱 : " + testNode.f);
    
                                    bArrivedCorner = true;
                                }
                                else
                                {
                                    Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);
                                    //코너 지점
                                    CreateNode(new Vector2Int(tempPos.x + 1, tempPos.y + 1), findDirection.RightTop, ref newNode);
                                }

                                return;
                            }
                        //}
        
                        tempPos.y++;
                        count++;
                        break;
                    }
                case findDirection.Left:
                    {
                        if (tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y, 0)) || tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y, 0))) //코너 없는 장애물에 도달
                            return;

                        //if (count > 0)
                        //{
                            if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y + 1, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y + 1, 0)))) //코너(강제이웃) 탐지
                            {
                                //print("코너 발견");
                                //print("코너 위치 : " + tempPos);
    
                                if (bExtendedSearch)
                                {
                                    Node testNode;
                                    //코너를 발견한 지점(에서 코너지점으로 대각선 탐색)
                                    if (currentNode.pos == searchNode.pos)
                                    {
                                        testNode = CreateNode(tempPos, findDirection.LeftTop, ref currentNode);
                                        bSetSearchPos = true;
                                    }
                                    else
                                    {
                                        testNode = CreateNode(tempPos, findDirection.LeftTop, ref searchNode);
                                    }
                                    //print("대각선 보조탐색 코너 발견지점 휴리스틱 : " + testNode.f);
    
                                    bArrivedCorner = true;
                                }
                                else
                                {
                                    Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);
                                    //코너 지점
                                    CreateNode(new Vector2Int(tempPos.x - 1, tempPos.y + 1), findDirection.LeftTop, ref newNode);
                                }
                                
                                return;
                            }
                            else if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y - 1, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y - 1, 0)))) //코너(강제이웃) 탐지
                            {
                                //print("코너 발견");
                                //print("코너 위치 : " + tempPos);
    
                                if (bExtendedSearch)
                                {
                                    Node testNode;
                                    if (currentNode.pos == searchNode.pos)
                                    {
                                        testNode = CreateNode(tempPos, findDirection.LeftBottom, ref currentNode);
                                        bSetSearchPos = true;
                                    }
                                    else
                                    {
                                        testNode = CreateNode(tempPos, findDirection.LeftBottom, ref searchNode);
                                    }
                                    //print("대각선 보조탐색 코너 발견지점 휴리스틱 : " + testNode.f);
    
                                    //코너를 발견한 지점(에서 코너지점으로 대각선 탐색)
                                    bArrivedCorner = true;
                                }
                                else
                                {
                                    Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);
                                    //코너 지점
                                    CreateNode(new Vector2Int(tempPos.x - 1, tempPos.y - 1), findDirection.LeftBottom, ref newNode);
                                }
                            
                                return;
                            }
                        //}
        
                        tempPos.x--;
                        count++;
                        break;
                    }
                case findDirection.Bottom:
                    {
                        if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y - 1, 0)) || tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y, 0))) //코너 없는 장애물에 도달
                            return;

                        //if (count > 0)
                        //{
                            if (tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y - 1, 0)))) //코너(강제이웃) 탐지
                            {
                                //print("코너 발견");
                                //print("코너 위치 : " + tempPos);
    
                                //코너를 발견한 지점(에서 코너지점으로 대각선 탐색)
                                if (bExtendedSearch)
                                {
                                    Node testNode;
                                    if (currentNode.pos == searchNode.pos)
                                    {
                                        testNode = CreateNode(tempPos, findDirection.LeftBottom, ref currentNode);
                                        bSetSearchPos = true;
                                    }
                                    else
                                    {
                                        testNode = CreateNode(tempPos, findDirection.LeftBottom, ref searchNode);
                                    }
                                    //print("대각선 보조탐색 코너 발견지점 휴리스틱 : " + testNode.f);
    
                                    bArrivedCorner = true;
                                }
                                else
                                {
                                    Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);
                                    //코너 지점
                                    CreateNode(new Vector2Int(tempPos.x - 1, tempPos.y - 1), findDirection.LeftBottom, ref newNode);
                                }

                                return;
                            }
                            else if (tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y - 1, 0)))) //코너(강제이웃) 탐지
                            {
                                //print("코너 발견");
                                //print("코너 위치 : " + tempPos);
    
                                //코너를 발견한 지점(에서 코너지점으로 대각선 탐색)
                                if (bExtendedSearch)
                                {
                                    Node testNode;
                                    if (currentNode.pos == searchNode.pos)
                                    {
                                        testNode = CreateNode(tempPos, findDirection.RightBottom, ref currentNode);
                                        bSetSearchPos = true;
                                    }
                                    else
                                    {
                                        testNode = CreateNode(tempPos, findDirection.RightBottom, ref searchNode);
                                    }
                                    //print("대각선 보조탐색 코너 발견지점 휴리스틱 : " + testNode.f);
    
                                    bArrivedCorner = true;
                                }
                                else
                                {
                                    Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);
                                    //코너 지점
                                    CreateNode(new Vector2Int(tempPos.x + 1, tempPos.y - 1), findDirection.RightBottom, ref newNode);
                                }

                                return;
                            }
                        //}
        
                        tempPos.y--;
                        count++;
                        break;
                    }
                case findDirection.Right:
                    {                        
                        if (tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y, 0)) || tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y, 0))) //코너 없는 장애물에 도달
                            return;
        
                        //if(count > 0)
                        //{
                            if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y + 1, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y + 1, 0)))) //코너(강제이웃) 탐지
                            {
                                //print("코너 발견");
                                //print("코너 위치 : " + tempPos);

                                Node testNode;
                                //코너를 발견한 지점(에서 코너지점으로 대각선 탐색)
                                if (bExtendedSearch)
                                {
                                    if (currentNode.pos == searchNode.pos)
                                    {
                                        testNode = CreateNode(tempPos, findDirection.RightTop, ref currentNode);
                                        bSetSearchPos = true;
                                    }
                                    else
                                    {
                                        testNode = CreateNode(tempPos, findDirection.RightTop, ref searchNode);
                                    }
                                    //print("대각선 보조탐색 코너 발견지점 휴리스틱 : " + testNode.f);
                                    bArrivedCorner = true;
                                }
                                else
                                {
                                    Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);
                                    //코너 지점
                                    CreateNode(new Vector2Int(tempPos.x + 1, tempPos.y + 1), findDirection.RightTop, ref newNode);
                                }

                                return;
                            }
                            else if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y - 1, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y - 1, 0)))) //코너(강제이웃) 탐지
                            {
                                //print("코너 발견");
                                //print("코너 위치 : " + tempPos);
    
                                //코너를 발견한 지점(에서 코너지점으로 대각선 탐색)
                                if (bExtendedSearch)
                                {
                                    Node testNode;
                                    if (currentNode.pos == searchNode.pos)
                                    {
                                        testNode = CreateNode(tempPos, findDirection.RightBottom, ref currentNode);
                                        bSetSearchPos = true;
                                    }
                                    else
                                    {
                                        testNode = CreateNode(tempPos, findDirection.RightBottom, ref searchNode);
                                    }
                                    //print("대각선 보조탐색 코너 발견지점 휴리스틱 : " + testNode.f);
                                    bArrivedCorner = true;
                                }
                                else
                                {
                                    Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);
                                    //코너 지점
                                    CreateNode(new Vector2Int(tempPos.x + 1, tempPos.y - 1), findDirection.RightBottom, ref newNode);
                                }
                                
                                return;
                            }
                        //}
        
                        tempPos.x++;
                        count++;
                        break;
                    }
            }
        }
    }

    //TODO : 보조탐색중 탐색종료 조건발생시 대각선 탐색도 종료되도록 추가
    //대각선 탐색
    private void Searchdiagonal(Node searchNode, Vector2Int pos, findDirection dir)
    {
        bExtendedSearch = true;

        Vector2Int tempPos = pos;
        
        Node tempNode = new Node();
        tempNode.pos = tempPos;
        tempNode.dir = dir;
        tempNode.parent = searchNode;
        
        //대각선 탐색
        while (true)
        {
            if (tempPos == targetPos) //종료조건 : 현재위치가 목적지라면
            {
                //print("목적지 도달");

                lastNode.pos = tempPos;
                lastNode.dir = findDirection.None;
                lastNode.parent = searchNode;
                lastNode.g = Mathf.Sqrt(Mathf.Pow((startPos.x - tempPos.x), 2) + Mathf.Pow((startPos.y - tempPos.y), 2));
                lastNode.h = Mathf.Abs(targetPos.x - tempPos.x) + Mathf.Abs(targetPos.y - tempPos.y);
                lastNode.f = lastNode.g + lastNode.h;

                bArrivedTarget = true;

                return;
            }

            if (tempPos.x > mapSize.x || tempPos.x < 0 || tempPos.y > mapSize.y || tempPos.y < 0) //맵 끝에 도달
                return;
            
            if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y, 0))) //코너 없는 장애물에 도달
                return;

            switch (dir)
            {
                case findDirection.LeftTop: //주 탐색
                    {
                        //if (count > 0)
                        //{
                            if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y - 1, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y - 1, 0)))) //코너(강제이웃) 탐지
                            {
                                Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);

                                //코너 지점
                                Node testNode = CreateNode(new Vector2Int(tempPos.x - 1, tempPos.y - 1), findDirection.LeftBottom, ref newNode);
                                //print("코너 발견지점 노드 휴리스틱 : " + newNode.f);
                                //print("코너지점 노드 휴리스틱 : " + testNode.f);
                                return;
                            }
                            else if (tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y + 1, 0)))) //코너(강제이웃) 탐지
                            {
                                Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);

                                //코너 지점
                                Node testNode = CreateNode(new Vector2Int(tempPos.x + 1, tempPos.y + 1), findDirection.RightTop, ref newNode);
                                //print("코너 발견지점 노드 휴리스틱 : " + newNode.f);
                                //print("코너지점 노드 휴리스틱 : " + testNode.f);
                                return;
                            }
                        //}

                        //보조탐색
                        //count = 0;
                        //print("왼쪽 보조 탐색 시작 위치 : " + new Vector2Int(tempPos.x - 1, tempPos.y));
                        SearchStraight(tempNode, new Vector2Int(tempPos.x - 1, tempPos.y), findDirection.Left);
                        //count = 0;
                        //print("위쪽 보조 탐색 시작 위치 : " + new Vector2Int(tempPos.x, tempPos.y +1));
                        SearchStraight(tempNode, new Vector2Int(tempPos.x, tempPos.y + 1), findDirection.Top);
                        //보조탐색 목적지 도달
                        if (bArrivedTarget)
                        {
                            //print("보조탐색 목적지 도달");
                            return;
                        }
                        //보조탐색 코너 도달
                        if (bArrivedCorner)
                        {
                            //print("보조탐색 코너 도달");
                            //if (bSetSearchPos)
                                tempNode.searchPos = new Vector2Int(tempPos.x - 1, tempPos.y + 1);
                            //else
                            //    tempNode.searchPos = tempPos;

                            tempNode.g = Mathf.Sqrt(Mathf.Pow((startPos.x - tempPos.x), 2) + Mathf.Pow((startPos.y - tempPos.y), 2));
                            tempNode.h = Mathf.Abs(targetPos.x - tempPos.x) + Mathf.Abs(targetPos.y - tempPos.y);
                            tempNode.f = tempNode.g + tempNode.h;

                            openList.Enqueue(tempNode, tempNode.f);
                            //print("대각선 보조탐색 시작점 휴리스틱 : " + tempNode.f);

                            return;
                        }

                        if (tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y + 1, 0))) //코너 없는 장애물에 도달
                            return;

                        tempPos.x--;
                        tempPos.y++;
                        tempNode.pos = tempPos;
                        count++;
                        break;
                    }
                case findDirection.LeftBottom:
                    {
                        //if (count > 0)
                        //{
                            if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y + 1, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y + 1, 0)))) //코너(강제이웃) 탐지
                            {
                                //print("코너 발견");

                                Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);

                                //코너 지점
                                Node testNode = CreateNode(new Vector2Int(tempPos.x - 1, tempPos.y + 1), findDirection.LeftTop, ref newNode);
                                //print("코너 발견지점 노드 휴리스틱 : " + newNode.f);
                                //print("코너지점 노드 휴리스틱 : " + testNode.f);
                                return;
                            }
                            else if (tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y - 1, 0)))) //코너(강제이웃) 탐지
                            {
                                print("코너 발견");

                                Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);

                                //코너 지점
                                Node testNode = CreateNode(new Vector2Int(tempPos.x + 1, tempPos.y - 1), findDirection.RightBottom, ref newNode);
                                //print("코너 발견지점 노드 휴리스틱 : " + newNode.f);
                                //print("코너지점 노드 휴리스틱 : " + testNode.f);
                                return;
                            }
                        //}
                        //보조탐색
                        //count = 0;
                        //print("왼쪽 보조 탐색 시작 위치 : " + new Vector2Int(tempPos.x - 1, tempPos.y));
                        SearchStraight(tempNode, new Vector2Int(tempPos.x - 1, tempPos.y), findDirection.Left);
                        //count = 0;
                        //print("아래쪽 보조 탐색 시작 위치 : " + new Vector2Int(tempPos.x, tempPos.y - 1));
                        SearchStraight(tempNode, new Vector2Int(tempPos.x, tempPos.y - 1), findDirection.Bottom);
                        //보조탐색 목적지 도달
                        if (bArrivedTarget)
                        {
                            //print("보조탐색 목적지 도달");
                            return;
                        }

                        if (bArrivedCorner)
                        {
                            //if (bSetSearchPos)
                                tempNode.searchPos = new Vector2Int(tempPos.x - 1, tempPos.y - 1);
                            //else
                            //    tempNode.searchPos = tempPos;

                            tempNode.g = Mathf.Sqrt(Mathf.Pow((startPos.x - tempPos.x), 2) + Mathf.Pow((startPos.y - tempPos.y), 2));
                            tempNode.h = Mathf.Abs(targetPos.x - tempPos.x) + Mathf.Abs(targetPos.y - tempPos.y);
                            tempNode.f = tempNode.g + tempNode.h;

                            openList.Enqueue(tempNode, tempNode.f);
                            //print("대각선 보조탐색 시작점 휴리스틱 : " + tempNode.f);

                            return;
                        }

                        if (tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y - 1, 0))) //코너 없는 장애물에 도달
                            return;
        
                        tempPos.x--;
                        tempPos.y--;
                        count++;
                        tempNode.pos = tempPos;
                        break;
                    }
                case findDirection.RightBottom:
                    {                        
                        //if (count > 0)
                        //{
                            if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y + 1, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y + 1, 0)))) //코너(강제이웃) 탐지
                            {
                                //print("코너 발견");
                                Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);

                                //코너 지점
                                Node testNode = CreateNode(new Vector2Int(tempPos.x + 1, tempPos.y + 1), findDirection.RightTop, ref newNode);
                                //print("코너 발견지점 노드 휴리스틱 : " + newNode.f);
                                //print("코너지점 노드 휴리스틱 : " + testNode.f);


                                return;
                            }
                            else if (tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y - 1, 0)))) //코너(강제이웃) 탐지
                            {
                                //print("코너 발견");
                                Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);

                                //코너 지점
                                Node testNode = CreateNode(new Vector2Int(tempPos.x - 1, tempPos.y - 1), findDirection.LeftBottom, ref newNode);
                                //print("코너 발견지점 노드 휴리스틱 : " + newNode.f);
                                //print("코너지점 노드 휴리스틱 : " + testNode.f);
                                return;
                            }
                        //}

                        //보조탐색
                        //count = 0;
                        //print("오른쪽 보조 탐색 시작 위치 : " + new Vector2Int(tempPos.x + 1, tempPos.y));
                        SearchStraight(tempNode, new Vector2Int(tempPos.x + 1, tempPos.y), findDirection.Right);
                        //count = 0;
                        //print("아래쪽 보조 탐색 시작 위치 : " + new Vector2Int(tempPos.x, tempPos.y - 1));
                        SearchStraight(tempNode, new Vector2Int(tempPos.x, tempPos.y - 1), findDirection.Bottom);
                        //보조탐색 목적지 도달
                        if (bArrivedTarget)
                        {
                            //print("보조탐색 목적지 도달");
                            return;
                        }

                        if (bArrivedCorner)
                        {
                            //if (bSetSearchPos)
                                tempNode.searchPos = new Vector2Int(tempPos.x + 1, tempPos.y - 1);
                            //else
                            //    tempNode.searchPos = tempPos;

                            tempNode.g = Mathf.Sqrt(Mathf.Pow((startPos.x - tempPos.x), 2) + Mathf.Pow((startPos.y - tempPos.y), 2));
                            tempNode.h = Mathf.Abs(targetPos.x - tempPos.x) + Mathf.Abs(targetPos.y - tempPos.y);
                            tempNode.f = tempNode.g + tempNode.h;

                            openList.Enqueue(tempNode, tempNode.f);
                            //print("대각선 보조탐색 시작점 휴리스틱 : " + tempNode.f);

                            return;
                        }

                        if (tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y - 1, 0))) //코너 없는 장애물에 도달
                            return;

                        tempPos.x++;
                        tempPos.y--;
                        count++;
                        tempNode.pos = tempPos;
                        break;
                    }
                case findDirection.RightTop:
                    {                        
                        //if (count > 0)
                        //{
                            if (tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y + 1, 0)))) //코너(강제이웃) 탐지
                            {
                                //print("코너 발견");

                                Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);

                                //코너 지점
                                Node testNode = CreateNode(new Vector2Int(tempPos.x - 1, tempPos.y + 1), findDirection.LeftTop, ref newNode);
                                //print("코너 발견지점 노드 휴리스틱 : " + newNode.f);
                                //print("코너지점 노드 휴리스틱 : " + testNode.f);
                                    
                                return;
                            }
                            else if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y - 1, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y - 1, 0)))) //코너(강제이웃) 탐지
                            {
                                //print("코너 발견");

                                Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);

                                //코너 지점
                                Node testNode = CreateNode(new Vector2Int(tempPos.x + 1, tempPos.y - 1), findDirection.RightBottom, ref newNode);
                                //print("코너 발견지점 노드 휴리스틱 : " + newNode.f);
                                //print("코너지점 노드 휴리스틱 : " + testNode.f);

                            return;
                            }
                        //}

                        //보조탐색
                        //count = 0;
                        //print("오른쪽 보조 탐색 시작 위치 : " + new Vector2Int(tempPos.x + 1, tempPos.y));
                        SearchStraight(tempNode, new Vector2Int(tempPos.x + 1, tempPos.y), findDirection.Right);
                        //count = 0;
                        //print("위쪽 보조 탐색 시작 위치 : " + new Vector2Int(tempPos.x, tempPos.y + 1));
                        SearchStraight(tempNode, new Vector2Int(tempPos.x, tempPos.y + 1), findDirection.Top);
                        //보조탐색 목적지 도달
                        if (bArrivedTarget)
                        {
                            //print("보조탐색 목적지 도달");
                            return;
                        }

                        if (bArrivedCorner)
                        {
                            //if (bSetSearchPos)
                                tempNode.searchPos = new Vector2Int(tempPos.x + 1, tempPos.y + 1);
                            //else
                            //    tempNode.searchPos = tempPos;

                            tempNode.g = Mathf.Sqrt(Mathf.Pow((startPos.x - tempPos.x), 2) + Mathf.Pow((startPos.y - tempPos.y), 2));
                            tempNode.h = Mathf.Abs(targetPos.x - tempPos.x) + Mathf.Abs(targetPos.y - tempPos.y);
                            tempNode.f = tempNode.g + tempNode.h;

                            openList.Enqueue(tempNode, tempNode.f);

                            //print("대각선 보조탐색 시작점 휴리스틱 : " + tempNode.f);
                            return;
                        }

                        if (tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y + 1, 0))) //코너 없는 장애물에 도달
                            return;

                        tempPos.x++;
                        tempPos.y++;
                        count++;
                        tempNode.pos = tempPos;
                        break;
                    }
            } //switch(searchNode.dir)
        } //while(true)
    }
}


public class PriorityQueue<T>
{
    private class Node
    {
        public T Data { get; private set; }
        public float Priority { get; set; } = 0;

        public Node prev;
        public Node next;

        public Node(T data, float priority)
        {
            this.Data = data;
            this.Priority = priority;
        }
    }

    private Node head = null;
    private Node tail = null;

    public int Count = 0;

    public void Enqueue(T data, float priority)
    {
        Count++;
        Node newNode = new Node(data, priority);
        if (head != null)
        {
            Node prev = null;
            Node node = head;

            // 처음부터 탐색한다. 'O(N)'
            do
            {
                if (node.Priority < priority) break;
                prev = node;
                node = node.next;
            } while (node != null);
            //////////////////////////////

            if (prev != null)
                prev.next = newNode;

            newNode.prev = prev;

            if (node != null)
            {
                if (node.Priority <= priority)
                {
                    newNode.next = node;
                    node.prev = newNode;
                }
                else
                {
                    node.next = newNode;
                    newNode.prev = node;
                }
            }

            if (newNode.prev == null)
                head = newNode;

            if (newNode.next == null)
                tail = newNode;
        }
        else
        {
            head = newNode;
            tail = head;
        }
    }

    public T Dequeue()
    {
        if (Count == 0)
        {
            head = null;
            tail = null;
            return default(T);
        }

        Count--;
        Node temp = tail;

        if(tail.prev != null)
        {
            tail = tail.prev;
            if(tail.prev != null)
               tail.prev.next = tail;
        }
        else
        {
            head = null;
            tail = null;
            Count = 0;
        }

        return temp.Data;
    }

    public void Clear()
    {
        while(true)
        {
            if (Count == 0) break;

            Dequeue();
        }
    }

    public T Peek()
    {
        if (tail != null)
            return tail.Data;
        return default(T);
    }
}