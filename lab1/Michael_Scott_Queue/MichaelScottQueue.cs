using System;
using Akka.Util;

namespace Parallel_Labs.lab1.Michael_Scott_Queue
{
    public class Node<T>
    {
        public T data;
        public AtomicReference<Node<T>> next;
        
        public Node(T data, AtomicReference<Node<T>> next) {
            this.data = data;
            this.next = next;
        }
        
        public T GetData() {
            return data;
        }

        public void SetData(T data) {
            this.data = data;
        }

        public AtomicReference<Node<T>> GetNext() {
            return next;
        }

        public void SetNext(AtomicReference<Node<T>> next) {
            this.next = next;
        }
    }
    
    public class MichelScottQueue<T> where T: class
    { 
        private AtomicReference<Node<T>> head;
        private AtomicReference<Node<T>> tail;
        
        public MichelScottQueue()
        {
            head = new AtomicReference<Node<T>>(new Node<T>(null, new AtomicReference<Node<T>>(null)));
            tail = new AtomicReference<Node<T>>(new Node<T>(null, new AtomicReference<Node<T>>(null)));
        }

        public void Add(T data) {
            Node<T> newTail = new Node<T>(data, new AtomicReference<Node<T>>(null));
            while(true) 
            {
                Node<T> currentTail = tail.Value;
                if (currentTail.next.CompareAndSet(null, newTail)) 
                {
                    tail.CompareAndSet(currentTail, newTail);
                    return;
                } 
                else 
                {
                    tail.CompareAndSet(currentTail, currentTail.next.Value);
                }
            }
        }
        
        public Node<T> Remove() {
            while (true) 
            {
                Node<T> currentHead = head.Value;
                Node<T> currentTail = tail.Value;
                Node<T> nextForHead = currentHead.next.Value;
                if (currentHead == currentTail) 
                {
                    if (nextForHead == null) 
                    {
                        throw new InvalidOperationException("No such element"); 
                    } 
                    else 
                    {
                        tail.CompareAndSet(currentTail, currentTail.next.Value);
                    }
                } 
                else 
                {
                    if (head.CompareAndSet(currentHead, nextForHead)) 
                    {
                        return nextForHead;
                    }
                }
            }
        }
    }
}