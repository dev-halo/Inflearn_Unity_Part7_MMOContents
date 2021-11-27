using ServerCore;
using System;

namespace Server.Game
{
    struct JobTimerElem : IComparable<JobTimerElem>
    {
        public int execTick; // 실행 시간
        public IJob job;

        public int CompareTo(JobTimerElem other)
        {
            return other.execTick - execTick;
        }
    }

    public class JobTimer
    {
        readonly PriorityQueue<JobTimerElem> pq = new();
        readonly object _lock = new();

        public void Push(IJob job, int tickAfter = 0)
        {
            JobTimerElem jobElement;
            jobElement.execTick = Environment.TickCount + tickAfter;
            jobElement.job = job;

            lock (_lock)
            {
                pq.Push(jobElement);
            }
        }

        public void Flush()
        {
            while (true)
            {
                int now = Environment.TickCount;

                JobTimerElem jobElement;

                lock (_lock)
                {
                    if (pq.Count == 0)
                        break;

                    jobElement = pq.Peek();
                    if (jobElement.execTick > now)
                        break;

                    pq.Pop();
                }

                jobElement.job.Execute();
            }
        }
    }
}
