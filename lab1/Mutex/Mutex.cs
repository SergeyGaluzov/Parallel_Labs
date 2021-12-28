using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Parallel_Labs.lab1.Mutex
{
    public class Mutex
    {
        private Thread _currentThread;
        private List<Thread> _waitingThreads;
        private int _isLocked = 0;

        public Mutex()
        {
            _waitingThreads = new List<Thread>();
        }

        public void Lock()
        {
            Console.WriteLine("lock");
            Interlocked.Exchange(ref _isLocked, 1);
            while (Interlocked.CompareExchange(ref _currentThread, Thread.CurrentThread, null) == null)
            {
                Thread.Yield();
            }
        }

        public void Unlock()
        {
            Console.WriteLine("unlock");
            Interlocked.Exchange(ref _currentThread, null);
            Interlocked.Exchange(ref _isLocked, 0);
        }


        public void Wait()
        {
            Console.WriteLine("wait");
            _waitingThreads.Add(Thread.CurrentThread);
            Unlock();

            while (_isLocked == 0)
            {
                Thread.Yield();
            }
            
            Lock();
            Interlocked.Exchange(ref _isLocked, 1);
        }

        public void Notify()
        {
            Console.WriteLine("notify");
            _currentThread = Thread.CurrentThread;
            if (_waitingThreads.Count > 0)
            {
                _waitingThreads.RemoveAt(_waitingThreads.Count - 1);
                Interlocked.Exchange(ref _isLocked, 0);
            } 
            else
            {
                throw new NullReferenceException("No threads in waiting list");
            }
        }

        public void NotifyAll()
        {
            Console.WriteLine("notify all");
            _waitingThreads.Clear();
        }
    }
}
