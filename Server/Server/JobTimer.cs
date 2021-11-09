using ServerCore;
using System;

namespace Server
{
    struct JobTimerElem : IComparable<JobTimerElem>
    {
        public int execTick; // 실행 시간
        public Action action;

        public int CompareTo(JobTimerElem other)
        {
            return other.execTick - execTick;
        }
    }

    class JobTimer
    {
        readonly PriorityQueue<JobTimerElem> pq = new();
        readonly object _lock = new();

        public static JobTimer Instance { get; } = new();

        public void Push(Action action, int tickAfter = 0)
        {
            JobTimerElem job;
            job.execTick = Environment.TickCount + tickAfter;
            job.action = action;

            lock (_lock)
            {
                pq.Push(job);
            }
        }

        public void Flush()
        {
            while (true)
            {
                int now = Environment.TickCount;

                JobTimerElem job;

                lock (_lock)
                {
                    if (pq.Count == 0)
                        break;

                    job = pq.Peek();
                    if (job.execTick > now)
                        break;

                    pq.Pop();
                }

                job.action.Invoke();
            }
        }
    }
}
