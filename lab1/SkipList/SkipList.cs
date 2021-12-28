using System;
using Akka;
using Akka.Util;

namespace Parallel_Labs.lab1.SkipList
{
    public class Node<T>
    { 
        private AtomicReference<Node<T>> rightNode;
        private AtomicReference<Node<T>> lowerNode;
        private T value;
        private bool toBeDeleted;

        public Node(T value, int height)
        {
            this.value = value;
            rightNode = new AtomicReference<Node<T>>(null);
            lowerNode = new AtomicReference<Node<T>>(null);
            toBeDeleted = false;
        }

        public T GetValue()
        {
            return value;
        }

        public Node<T> GetRightNode()
        {
            return rightNode.Value;
        }

        public void SetRightNode(Node<T> rightNode)
        {
            this.rightNode.GetAndSet(rightNode);
        }

        public Node<T> GetLowerNode()
        {
            return lowerNode.Value;
        }
        
        public void SetLowerNode(Node<T> lowerNode)
        {
            this.lowerNode.GetAndSet(lowerNode);
        }
        
        public void SetToBeDeleted(bool toBeDeleted) {
            this.toBeDeleted = toBeDeleted;
        }
        
        public bool CompareAndSetRightNode(Node<T> expected, Node<T> next)
        {
            return !toBeDeleted && rightNode.CompareAndSet(expected, next);
        }
        
        public bool SetLowerNode(Node<T> expected, Node<T> next) {
            return lowerNode.CompareAndSet(expected, next);
        }
        
    }
    
    public class SkipList<T> where T : class
    {
        private int curMaxHeight;
        private Node<T> upperHeadNode;
        private static int MAX_HEIGHT = 24;

        public SkipList()
        {
            curMaxHeight = MAX_HEIGHT;
            Node<T> initialNode = new Node<T>(null, MAX_HEIGHT);
            upperHeadNode = initialNode;
            InitHeadNode(upperHeadNode);
        }
        
        private int СompareNodesData(T data1 , T data2)
        {
            if (data1 is int i && data2 is int j)
            {
                return i.CompareTo(j);
            }
            return 0;
        }
        
        private void InitHeadNode(Node<T> initialNode){
            for (int i = 1; i < curMaxHeight; i++) {
                var nextNode = new Node<T>(null, curMaxHeight);
                initialNode.SetLowerNode(null, nextNode);
                initialNode = nextNode;
            }
        }
        
        private Node<T> ReturnHeadNodeForLevel(int index) {
            Node<T> head = upperHeadNode;
            for (int i = curMaxHeight - 1; i > index; i--) {
                head = head.GetLowerNode();
            }
            return head;
        }

        private int GetTowerHeight() {
            int nodeTowerLevel = 1;
            while (nodeTowerLevel < curMaxHeight && Math.Abs(new Random().Next()) % 2 == 0) 
            {
                nodeTowerLevel++;
            }
            return nodeTowerLevel;
        }
        
        public void Add(T elem) {
            int randomHeight = GetTowerHeight();
            Node<T> previousNode = null;
            Node<T> headCurrentLevelNode = ReturnHeadNodeForLevel(randomHeight - 1);

            while (headCurrentLevelNode != null) 
            {
                Node<T> headRightTowerNode = headCurrentLevelNode.GetRightNode();
                if (headRightTowerNode != null && СompareNodesData(headRightTowerNode.GetValue(), elem) == -1)
                {
                    headCurrentLevelNode = headRightTowerNode;
                }
                else 
                {
                    Node<T> newTowerNode = new Node<T>(elem, randomHeight);
                    newTowerNode.SetRightNode(headRightTowerNode);
                    if (headCurrentLevelNode.CompareAndSetRightNode(headRightTowerNode, newTowerNode)) 
                    {
                        headCurrentLevelNode = headCurrentLevelNode.GetLowerNode();
                        if (previousNode != null) 
                        {
                            previousNode.SetLowerNode(newTowerNode);
                        }
                        previousNode = newTowerNode;
                    }
                }
            }
        }
        
        
    }
    
}