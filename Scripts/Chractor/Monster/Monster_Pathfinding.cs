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
        public findDirection dir; //�ش� ��尡 �˻縦 �簳�ؾ��� ����
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
        //        //��ã�� �׽�Ʈ//////////////////////////////////////////////////////////////////////////
        //        startPos = new Vector2Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.y);
        //        targetPos = new Vector2Int((int)hit.point.x, (int)hit.point.y);
        //        print("���콺 ��ġ : " + targetPos);
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

        if (targetPos.x < 0 || targetPos.x > mapSize.x || targetPos.y < 0 || targetPos.y > mapSize.y) //�׽�Ʈ �� ����
            return;

        path.Clear();
        openList.Clear();
        closeList.Clear();
        bArrivedTarget = false;

        //���۳�� ������ �켱���� ť�� ����
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
            
            //���ο� ��� �켱���� ť���� ����ġ ���� ������ Dequeue�� currentNode�� ����
            currentNode = openList.Dequeue();
            closeList.Enqueue(currentNode, currentNode.f);

            //8���� Ž��
            if (currentNode.dir == findDirection.All) //���۳���� 8���� Ž��
            {
                //print("8���� Ž��===============================");

                for (int i = 0; i < 8; i++)
                {
                    if (i < 4) //���� Ž��
                    {
                        //print("Ž�� ���� ��ġ : " + currentNode.searchPos);
                        //print("Ž�� ���� ���� : " + currentNode.dir);
                        SearchStraight(currentNode, currentNode.searchPos, (findDirection)i);
                    }
                    else //�밢�� Ž��
                    {
                        //print("Ž�� ���� ��ġ : " + currentNode.searchPos);
                        //print("Ž�� ���� ���� : " + currentNode.dir);
                        Searchdiagonal(currentNode, currentNode.searchPos, (findDirection)i);
                    }
                    if (bArrivedTarget) break;
                    count = 0;
                    bArrivedCorner = false;
                    bExtendedSearch = false;
                    bSetSearchPos = false;
                }
            }
            else //���۳�尡 �ƴ϶�� �θ����� ������ �̾ Ž��
            {
                //print("Ư������ Ž��==============================");

                int dirNum = (int)currentNode.dir;
            
                if (dirNum < 4) //���� Ž��
                {
                    //print("Ž�� ���� ��ġ : " + currentNode.searchPos);
                    //print("Ž�� ���� ���� : " + currentNode.dir);
                    SearchStraight(currentNode, currentNode.searchPos, currentNode.dir);
                }
                else //�밢�� Ž��
                {
                    //print("Ž�� ���� ��ġ : " + currentNode.searchPos);
                    //print("Ž�� ���� ���� : " + currentNode.dir);
                    Searchdiagonal(currentNode, currentNode.searchPos, currentNode.dir);
                }
                if (bArrivedTarget) break;
                count = 0;
                bArrivedCorner = false;
                bExtendedSearch = false;
                bSetSearchPos = false;
            }
        }

        //Ÿ���� ã�Ҵٸ� path ����
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

    //��� ����(�ڳ� ������X)
    private Node CreateNode(Vector2Int pos, findDirection dir, ref Node parent)
    {
        //��� ����(�ڳʸ� �߰��� ������), �� ����� �θ� ���� Ž�����۳��
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


    //���� Ž��
    private void SearchStraight(Node searchNode, Vector2Int pos, findDirection dir)
    {
        Vector2Int tempPos = pos;
        
        while (true)
        {
            //�ڳ� �̿��� ���� ����
            if (tempPos.x > mapSize.x || tempPos.x < 0 || tempPos.y > mapSize.y || tempPos.y < 0) //�� ���� ����
            {
                return;
            }

            if (tempPos == targetPos) //�������� : ������ġ�� ���������
            {
                //print("������ ����");
                
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
                        //�ڳ� ���� ��ֹ��� ����
                        if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y + 1, 0)) || tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y, 0))) 
                            return;
        
                        //if (count > 0)
                        //{
                            //TODO : tempPos.y + 1�κе� �˻��ؾ� ���� ����
                            if (tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y + 1, 0))) && tempPos.y + 1 < mapSize.y) //�ڳ�(�����̿�) Ž��
                            {
                                //print("�ڳ� �߰�");
                                //print("�ڳ� ��ġ : " + tempPos);

                                if (bExtendedSearch)
                                {
                                    Node testNode;
                                    //�ڳʸ� �߰��� ����(���� �ڳ��������� �밢�� Ž��)
                                    if (currentNode.pos == searchNode.pos)
                                    {
                                        testNode = CreateNode(tempPos, findDirection.LeftTop, ref currentNode);
                                        bSetSearchPos = true;
                                    }
                                    else
                                    {
                                        testNode = CreateNode(tempPos, findDirection.LeftTop, ref searchNode);
                                    }
                                    //print("�밢�� ����Ž�� �ڳ� �߰����� �޸���ƽ : " + testNode.f);
    
                                    bArrivedCorner = true;
                                }
                                else
                                {
                                    //�ڳ� �߰� ������ ��������
                                    Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);
                                    //�ڳʸ� �߰��� ����(���� �ڳ��������� �밢�� Ž��)
                                    CreateNode(new Vector2Int(tempPos.x - 1, tempPos.y + 1), findDirection.LeftTop, ref newNode);
                                }
                                
                                return;
                            }
                            else if (tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y + 1, 0))) && tempPos.y + 1 < mapSize.y) //�ڳ�(�����̿�) Ž��
                            {
                                //print("�ڳ� �߰�");
                                //print("�ڳ� ��ġ : " + tempPos);
 
                                //�ڳʸ� �߰��� ����(���� �ڳ��������� �밢�� Ž��)
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
                                    //print("�밢�� ����Ž�� �ڳ� �߰����� �޸���ƽ : " + testNode.f);
    
                                    bArrivedCorner = true;
                                }
                                else
                                {
                                    Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);
                                    //�ڳ� ����
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
                        if (tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y, 0)) || tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y, 0))) //�ڳ� ���� ��ֹ��� ����
                            return;

                        //if (count > 0)
                        //{
                            if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y + 1, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y + 1, 0)))) //�ڳ�(�����̿�) Ž��
                            {
                                //print("�ڳ� �߰�");
                                //print("�ڳ� ��ġ : " + tempPos);
    
                                if (bExtendedSearch)
                                {
                                    Node testNode;
                                    //�ڳʸ� �߰��� ����(���� �ڳ��������� �밢�� Ž��)
                                    if (currentNode.pos == searchNode.pos)
                                    {
                                        testNode = CreateNode(tempPos, findDirection.LeftTop, ref currentNode);
                                        bSetSearchPos = true;
                                    }
                                    else
                                    {
                                        testNode = CreateNode(tempPos, findDirection.LeftTop, ref searchNode);
                                    }
                                    //print("�밢�� ����Ž�� �ڳ� �߰����� �޸���ƽ : " + testNode.f);
    
                                    bArrivedCorner = true;
                                }
                                else
                                {
                                    Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);
                                    //�ڳ� ����
                                    CreateNode(new Vector2Int(tempPos.x - 1, tempPos.y + 1), findDirection.LeftTop, ref newNode);
                                }
                                
                                return;
                            }
                            else if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y - 1, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y - 1, 0)))) //�ڳ�(�����̿�) Ž��
                            {
                                //print("�ڳ� �߰�");
                                //print("�ڳ� ��ġ : " + tempPos);
    
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
                                    //print("�밢�� ����Ž�� �ڳ� �߰����� �޸���ƽ : " + testNode.f);
    
                                    //�ڳʸ� �߰��� ����(���� �ڳ��������� �밢�� Ž��)
                                    bArrivedCorner = true;
                                }
                                else
                                {
                                    Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);
                                    //�ڳ� ����
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
                        if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y - 1, 0)) || tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y, 0))) //�ڳ� ���� ��ֹ��� ����
                            return;

                        //if (count > 0)
                        //{
                            if (tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y - 1, 0)))) //�ڳ�(�����̿�) Ž��
                            {
                                //print("�ڳ� �߰�");
                                //print("�ڳ� ��ġ : " + tempPos);
    
                                //�ڳʸ� �߰��� ����(���� �ڳ��������� �밢�� Ž��)
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
                                    //print("�밢�� ����Ž�� �ڳ� �߰����� �޸���ƽ : " + testNode.f);
    
                                    bArrivedCorner = true;
                                }
                                else
                                {
                                    Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);
                                    //�ڳ� ����
                                    CreateNode(new Vector2Int(tempPos.x - 1, tempPos.y - 1), findDirection.LeftBottom, ref newNode);
                                }

                                return;
                            }
                            else if (tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y - 1, 0)))) //�ڳ�(�����̿�) Ž��
                            {
                                //print("�ڳ� �߰�");
                                //print("�ڳ� ��ġ : " + tempPos);
    
                                //�ڳʸ� �߰��� ����(���� �ڳ��������� �밢�� Ž��)
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
                                    //print("�밢�� ����Ž�� �ڳ� �߰����� �޸���ƽ : " + testNode.f);
    
                                    bArrivedCorner = true;
                                }
                                else
                                {
                                    Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);
                                    //�ڳ� ����
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
                        if (tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y, 0)) || tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y, 0))) //�ڳ� ���� ��ֹ��� ����
                            return;
        
                        //if(count > 0)
                        //{
                            if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y + 1, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y + 1, 0)))) //�ڳ�(�����̿�) Ž��
                            {
                                //print("�ڳ� �߰�");
                                //print("�ڳ� ��ġ : " + tempPos);

                                Node testNode;
                                //�ڳʸ� �߰��� ����(���� �ڳ��������� �밢�� Ž��)
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
                                    //print("�밢�� ����Ž�� �ڳ� �߰����� �޸���ƽ : " + testNode.f);
                                    bArrivedCorner = true;
                                }
                                else
                                {
                                    Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);
                                    //�ڳ� ����
                                    CreateNode(new Vector2Int(tempPos.x + 1, tempPos.y + 1), findDirection.RightTop, ref newNode);
                                }

                                return;
                            }
                            else if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y - 1, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y - 1, 0)))) //�ڳ�(�����̿�) Ž��
                            {
                                //print("�ڳ� �߰�");
                                //print("�ڳ� ��ġ : " + tempPos);
    
                                //�ڳʸ� �߰��� ����(���� �ڳ��������� �밢�� Ž��)
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
                                    //print("�밢�� ����Ž�� �ڳ� �߰����� �޸���ƽ : " + testNode.f);
                                    bArrivedCorner = true;
                                }
                                else
                                {
                                    Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);
                                    //�ڳ� ����
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

    //TODO : ����Ž���� Ž������ ���ǹ߻��� �밢�� Ž���� ����ǵ��� �߰�
    //�밢�� Ž��
    private void Searchdiagonal(Node searchNode, Vector2Int pos, findDirection dir)
    {
        bExtendedSearch = true;

        Vector2Int tempPos = pos;
        
        Node tempNode = new Node();
        tempNode.pos = tempPos;
        tempNode.dir = dir;
        tempNode.parent = searchNode;
        
        //�밢�� Ž��
        while (true)
        {
            if (tempPos == targetPos) //�������� : ������ġ�� ���������
            {
                //print("������ ����");

                lastNode.pos = tempPos;
                lastNode.dir = findDirection.None;
                lastNode.parent = searchNode;
                lastNode.g = Mathf.Sqrt(Mathf.Pow((startPos.x - tempPos.x), 2) + Mathf.Pow((startPos.y - tempPos.y), 2));
                lastNode.h = Mathf.Abs(targetPos.x - tempPos.x) + Mathf.Abs(targetPos.y - tempPos.y);
                lastNode.f = lastNode.g + lastNode.h;

                bArrivedTarget = true;

                return;
            }

            if (tempPos.x > mapSize.x || tempPos.x < 0 || tempPos.y > mapSize.y || tempPos.y < 0) //�� ���� ����
                return;
            
            if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y, 0))) //�ڳ� ���� ��ֹ��� ����
                return;

            switch (dir)
            {
                case findDirection.LeftTop: //�� Ž��
                    {
                        //if (count > 0)
                        //{
                            if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y - 1, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y - 1, 0)))) //�ڳ�(�����̿�) Ž��
                            {
                                Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);

                                //�ڳ� ����
                                Node testNode = CreateNode(new Vector2Int(tempPos.x - 1, tempPos.y - 1), findDirection.LeftBottom, ref newNode);
                                //print("�ڳ� �߰����� ��� �޸���ƽ : " + newNode.f);
                                //print("�ڳ����� ��� �޸���ƽ : " + testNode.f);
                                return;
                            }
                            else if (tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y + 1, 0)))) //�ڳ�(�����̿�) Ž��
                            {
                                Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);

                                //�ڳ� ����
                                Node testNode = CreateNode(new Vector2Int(tempPos.x + 1, tempPos.y + 1), findDirection.RightTop, ref newNode);
                                //print("�ڳ� �߰����� ��� �޸���ƽ : " + newNode.f);
                                //print("�ڳ����� ��� �޸���ƽ : " + testNode.f);
                                return;
                            }
                        //}

                        //����Ž��
                        //count = 0;
                        //print("���� ���� Ž�� ���� ��ġ : " + new Vector2Int(tempPos.x - 1, tempPos.y));
                        SearchStraight(tempNode, new Vector2Int(tempPos.x - 1, tempPos.y), findDirection.Left);
                        //count = 0;
                        //print("���� ���� Ž�� ���� ��ġ : " + new Vector2Int(tempPos.x, tempPos.y +1));
                        SearchStraight(tempNode, new Vector2Int(tempPos.x, tempPos.y + 1), findDirection.Top);
                        //����Ž�� ������ ����
                        if (bArrivedTarget)
                        {
                            //print("����Ž�� ������ ����");
                            return;
                        }
                        //����Ž�� �ڳ� ����
                        if (bArrivedCorner)
                        {
                            //print("����Ž�� �ڳ� ����");
                            //if (bSetSearchPos)
                                tempNode.searchPos = new Vector2Int(tempPos.x - 1, tempPos.y + 1);
                            //else
                            //    tempNode.searchPos = tempPos;

                            tempNode.g = Mathf.Sqrt(Mathf.Pow((startPos.x - tempPos.x), 2) + Mathf.Pow((startPos.y - tempPos.y), 2));
                            tempNode.h = Mathf.Abs(targetPos.x - tempPos.x) + Mathf.Abs(targetPos.y - tempPos.y);
                            tempNode.f = tempNode.g + tempNode.h;

                            openList.Enqueue(tempNode, tempNode.f);
                            //print("�밢�� ����Ž�� ������ �޸���ƽ : " + tempNode.f);

                            return;
                        }

                        if (tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y + 1, 0))) //�ڳ� ���� ��ֹ��� ����
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
                            if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y + 1, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y + 1, 0)))) //�ڳ�(�����̿�) Ž��
                            {
                                //print("�ڳ� �߰�");

                                Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);

                                //�ڳ� ����
                                Node testNode = CreateNode(new Vector2Int(tempPos.x - 1, tempPos.y + 1), findDirection.LeftTop, ref newNode);
                                //print("�ڳ� �߰����� ��� �޸���ƽ : " + newNode.f);
                                //print("�ڳ����� ��� �޸���ƽ : " + testNode.f);
                                return;
                            }
                            else if (tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y - 1, 0)))) //�ڳ�(�����̿�) Ž��
                            {
                                print("�ڳ� �߰�");

                                Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);

                                //�ڳ� ����
                                Node testNode = CreateNode(new Vector2Int(tempPos.x + 1, tempPos.y - 1), findDirection.RightBottom, ref newNode);
                                //print("�ڳ� �߰����� ��� �޸���ƽ : " + newNode.f);
                                //print("�ڳ����� ��� �޸���ƽ : " + testNode.f);
                                return;
                            }
                        //}
                        //����Ž��
                        //count = 0;
                        //print("���� ���� Ž�� ���� ��ġ : " + new Vector2Int(tempPos.x - 1, tempPos.y));
                        SearchStraight(tempNode, new Vector2Int(tempPos.x - 1, tempPos.y), findDirection.Left);
                        //count = 0;
                        //print("�Ʒ��� ���� Ž�� ���� ��ġ : " + new Vector2Int(tempPos.x, tempPos.y - 1));
                        SearchStraight(tempNode, new Vector2Int(tempPos.x, tempPos.y - 1), findDirection.Bottom);
                        //����Ž�� ������ ����
                        if (bArrivedTarget)
                        {
                            //print("����Ž�� ������ ����");
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
                            //print("�밢�� ����Ž�� ������ �޸���ƽ : " + tempNode.f);

                            return;
                        }

                        if (tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y - 1, 0))) //�ڳ� ���� ��ֹ��� ����
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
                            if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y + 1, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y + 1, 0)))) //�ڳ�(�����̿�) Ž��
                            {
                                //print("�ڳ� �߰�");
                                Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);

                                //�ڳ� ����
                                Node testNode = CreateNode(new Vector2Int(tempPos.x + 1, tempPos.y + 1), findDirection.RightTop, ref newNode);
                                //print("�ڳ� �߰����� ��� �޸���ƽ : " + newNode.f);
                                //print("�ڳ����� ��� �޸���ƽ : " + testNode.f);


                                return;
                            }
                            else if (tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y - 1, 0)))) //�ڳ�(�����̿�) Ž��
                            {
                                //print("�ڳ� �߰�");
                                Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);

                                //�ڳ� ����
                                Node testNode = CreateNode(new Vector2Int(tempPos.x - 1, tempPos.y - 1), findDirection.LeftBottom, ref newNode);
                                //print("�ڳ� �߰����� ��� �޸���ƽ : " + newNode.f);
                                //print("�ڳ����� ��� �޸���ƽ : " + testNode.f);
                                return;
                            }
                        //}

                        //����Ž��
                        //count = 0;
                        //print("������ ���� Ž�� ���� ��ġ : " + new Vector2Int(tempPos.x + 1, tempPos.y));
                        SearchStraight(tempNode, new Vector2Int(tempPos.x + 1, tempPos.y), findDirection.Right);
                        //count = 0;
                        //print("�Ʒ��� ���� Ž�� ���� ��ġ : " + new Vector2Int(tempPos.x, tempPos.y - 1));
                        SearchStraight(tempNode, new Vector2Int(tempPos.x, tempPos.y - 1), findDirection.Bottom);
                        //����Ž�� ������ ����
                        if (bArrivedTarget)
                        {
                            //print("����Ž�� ������ ����");
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
                            //print("�밢�� ����Ž�� ������ �޸���ƽ : " + tempNode.f);

                            return;
                        }

                        if (tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y - 1, 0))) //�ڳ� ���� ��ֹ��� ����
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
                            if (tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x - 1, tempPos.y + 1, 0)))) //�ڳ�(�����̿�) Ž��
                            {
                                //print("�ڳ� �߰�");

                                Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);

                                //�ڳ� ����
                                Node testNode = CreateNode(new Vector2Int(tempPos.x - 1, tempPos.y + 1), findDirection.LeftTop, ref newNode);
                                //print("�ڳ� �߰����� ��� �޸���ƽ : " + newNode.f);
                                //print("�ڳ����� ��� �޸���ƽ : " + testNode.f);
                                    
                                return;
                            }
                            else if (tilemap.HasTile(new Vector3Int(tempPos.x, tempPos.y - 1, 0)) && !(tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y - 1, 0)))) //�ڳ�(�����̿�) Ž��
                            {
                                //print("�ڳ� �߰�");

                                Node newNode = CreateNode(tempPos, searchNode.dir, ref searchNode);

                                //�ڳ� ����
                                Node testNode = CreateNode(new Vector2Int(tempPos.x + 1, tempPos.y - 1), findDirection.RightBottom, ref newNode);
                                //print("�ڳ� �߰����� ��� �޸���ƽ : " + newNode.f);
                                //print("�ڳ����� ��� �޸���ƽ : " + testNode.f);

                            return;
                            }
                        //}

                        //����Ž��
                        //count = 0;
                        //print("������ ���� Ž�� ���� ��ġ : " + new Vector2Int(tempPos.x + 1, tempPos.y));
                        SearchStraight(tempNode, new Vector2Int(tempPos.x + 1, tempPos.y), findDirection.Right);
                        //count = 0;
                        //print("���� ���� Ž�� ���� ��ġ : " + new Vector2Int(tempPos.x, tempPos.y + 1));
                        SearchStraight(tempNode, new Vector2Int(tempPos.x, tempPos.y + 1), findDirection.Top);
                        //����Ž�� ������ ����
                        if (bArrivedTarget)
                        {
                            //print("����Ž�� ������ ����");
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

                            //print("�밢�� ����Ž�� ������ �޸���ƽ : " + tempNode.f);
                            return;
                        }

                        if (tilemap.HasTile(new Vector3Int(tempPos.x + 1, tempPos.y + 1, 0))) //�ڳ� ���� ��ֹ��� ����
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

            // ó������ Ž���Ѵ�. 'O(N)'
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