//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PriorityQueue<T> : MonoBehaviour
//{
//    private class Node
//    {
//        public T Data { get; private set; }
//        public int Priority { get; set; } = 0;
//
//        public Node prve;
//        public Node next;
//
//        public Node(T data, int priority)
//        {
//            this.Data = data;
//            this.Priority = priority;
//        }
//    }
//
//    private Node head = null;
//    private Node tail = null;
//
//    public int Count = 0;
//
//    public void Enqueue(T data, int priority)
//    {
//        ++Count;
//        Node newNode = new Node(data, priority);
//        if (head != null)
//        {
//            Node prev = null;
//            Node node = head;
//
//            //////////////////////////////
//            /// 처음부터 탐색한다. 'O(N)'
//            do
//            {
//                if (node.Priority > priority) break;
//                prev = node;
//                node = node.next;
//            } while (node != null);
//            //////////////////////////////
//
//            if (prev != null)
//                prev.next = newNode;
//
//            newNode.prve = prev;
//
//            if (node != null)
//            {
//                if (node.Priority > priority)
//                {
//                    newNode.next = node;
//                    node.prve = newNode;
//                }
//                else
//                {
//                    node.next = newNode;
//                    newNode.prve = node;
//                }
//            }
//
//            if (newNode.prve == null)
//                head = newNode;
//
//            if (newNode.next == null)
//                tail = newNode;
//        }
//        else
//        {
//            head = newNode;
//            tail = head;
//        }
//    }
//
//    public T Dequeue()
//    {
//        --Count;
//        Node temp = tail;
//        tail = tail.prve;
//        if (tail == null) head = null;
//
//        if (tail != null && tail.prve != null)
//            tail.prve.next = null;
//
//        if (temp != null)
//            return temp.Data;
//        return default(T);
//    }
//
//    public T Peek()
//    {
//        if (tail != null)
//            return tail.Data;
//        return default(T);
//    }
//}