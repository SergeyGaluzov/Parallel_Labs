using System;
using System.Collections.Generic;
using Akka.Util;

namespace Parallel_Labs.lab1.Harris_LinkedList_LockFree
{
    public class BoolClass
    {
        private bool value;

        public BoolClass(bool value)
        {
            this.value = value;
        }

        public bool BoolValue
        {
            get => value;
            set => this.value = value;
        }
    }
    public class Node<T> 
    {
        public T data;
        public AtomicReference<Node<T>> next;
        public AtomicReference<BoolClass> isLogicallyDeleted;

        public Node(T data, AtomicReference<Node<T>> next) {
            this.data = data;
            this.next = next;
        }

        public Node(T data) {
            this.data = data;
        }

        public BoolClass canNodeBeDeleted() {
            return isLogicallyDeleted.Value;
        }

        public AtomicReference<Node<T>> getNext() {
            return next;
        }

        public AtomicReference<BoolClass> getIsLogicallyDeleted() {
            return isLogicallyDeleted;
        }

        public void SetIsLogicallyDeleted(BoolClass isLogicallyDeleted) {
            this.isLogicallyDeleted.GetAndSet(isLogicallyDeleted);
        }

        public void SetNext(AtomicReference<Node<T>> next) {
            this.next = next;
        }
    }

    public class HarrisLinkedListLockFree<T>
    {
        private volatile AtomicReference<Node<T>> head;
        private int count;

        public HarrisLinkedListLockFree()
        {
            head = new AtomicReference<Node<T>>(null);
            count = 0;
        }
        
        public void Add(T elem) 
        {
            var newNode = new Node<T>(elem);
            while (true) 
            {
                if (head.Value == null) 
                {
                    if (head.CompareAndSet(null, newNode)) 
                    {
                        count++;    
                        break;
                    }
                } 
                else 
                {
                    var curHead = head.Value;
                    var next = curHead.getNext();
                    newNode.SetNext(next);
                    if (!curHead.isLogicallyDeleted.Value.BoolValue && curHead.getNext().CompareAndSet(next.Value, newNode))
                    {
                        count++;
                        break;
                    }
                }
            }
        }
        
        private Node<T> IndexAt(int index) {
            Node<T> curHead = head.Value;
            int atIndex = 0;
            while (atIndex < index && curHead != null) 
            {
                curHead = curHead.getNext().Value;
                atIndex++;
            }
            if (atIndex == index) {
                return curHead;
            } 
            return null;
        }
        
        public bool RemoveAt(int index) 
        {
            if (index > count || index < 0) 
            {
                return false;
            }

            while (true) 
            {
                var nodeAtIndex = IndexAt(index);
                nodeAtIndex.SetIsLogicallyDeleted(new BoolClass(true));
                if (index != 0) 
                {
                    var prevNode = IndexAt(index - 1);
                    var nextNode = nodeAtIndex.getNext();
                    if (!prevNode.isLogicallyDeleted.Value.BoolValue && prevNode.next.CompareAndSet(nodeAtIndex, nextNode.Value)) 
                    {
                        count--;
                        break;
                    }
                } 
                else 
                {
                    if (!head.Value.isLogicallyDeleted.Value.BoolValue && head.CompareAndSet(nodeAtIndex, nodeAtIndex.getNext().Value)) 
                    {
                        count--;
                        break;
                    }
                }
                nodeAtIndex.SetIsLogicallyDeleted(new BoolClass(false));
            }
            return true;
        }
        
        public String Traverse() 
        {
            Node<T> currNode = head.Value;
            List<string> outputList = new List<string>();

            while (currNode != null) {
                outputList.Add(currNode.getNext().ToString());
                currNode = currNode.getNext().Value;
            }

            return string.Join(", ", outputList);
        }
    }
}
