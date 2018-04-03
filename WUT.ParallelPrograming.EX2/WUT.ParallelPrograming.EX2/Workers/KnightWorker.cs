using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WUT.ParallelPrograming.EX2.Monitors;

namespace WUT.ParallelPrograming.EX2.Workers
{
    class KnightWorker
    {
        private bool IsKing;
        private int Id;
        private string Name
        {
            get
            {
                return IsKing ? $"|^^^| King_{Id}" : $"/ Knight_{Id}";
            }
        }

        private Random random = new Random(Guid.NewGuid().GetHashCode());

        private int HeadLevel;

        private int SleepTime, StoryTime, TimeVariation;

        public KnightWorker(int sleepTime, int storyTime, int timeVariation, int id ,bool isKing, int headLevel = 10)
        {
            Id = id;
            HeadLevel = headLevel;
            SleepTime = sleepTime;
            StoryTime = storyTime;
            TimeVariation = timeVariation;
            IsKing = isKing;
        }

        public void Work()
        {
            //Console.WriteLine($"Hello {Name}");
            for (int i = 0; i < HeadLevel; i++)
            {
                Sleep();
                TellStory();
                Drink();
            }
            //Console.WriteLine($"Goodbye {Name}");
        }
        private void Sleep()
        {
            //Console.WriteLine($"{Name} go to sleep");
            Thread.Sleep(SleepTime + random.Next() % TimeVariation);
            //Console.WriteLine($"{Name} woke up");
        }
        private void TellStory()
        {
            //Console.WriteLine($"{Name} Wait for speaking");
            TableMonitor.Instance.StartSpeak(Id);

            //Console.WriteLine($"{Name} is speaking");
            Thread.Sleep(StoryTime + random.Next() % TimeVariation);

            TableMonitor.Instance.StopSpeak(Id);
            //Console.WriteLine($"{Name} stopped speaking");
        }
        private void Drink()
        {
            //Console.WriteLine($"{Name} want to drink");
            TableMonitor.Instance.StartDrink(Id);

            //Console.WriteLine($"{Name} is Drinking");
            Thread.Sleep(StoryTime + random.Next() % TimeVariation);
            TableMonitor.Instance.StopDrink(Id);
            //Console.WriteLine($"{Name} stopped drinking");
        }
    }
}
