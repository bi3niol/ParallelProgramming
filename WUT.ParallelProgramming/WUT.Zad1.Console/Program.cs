using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WUT.Zad1.Lib;
using WUT.Zad1.Lib.Alchemists;

namespace WUT.Zad1.Console
{
    class Program
    {
        private static bool Working = true;
        static void Main(string[] args)
        {
            new Thread(AlchemistSpamer).Start();
            Storehouse.StartWorkAsync();
            System.Console.ReadLine();
            Working = false;
        }

        private static void AlchemistSpamer()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            var delta = LibSettings.Default.MaxAlchemistRespTime - LibSettings.Default.MinAlchemistRespTime;
            var minVal = LibSettings.Default.MinAlchemistRespTime;
            while (Working)
            {
                Thread.Sleep(minVal + random.Next() % delta);
                var Alchemist = random.Next() % 4;
                switch (Alchemist)
                {
                    case 0:
                        {
                            new Thread(new A_Alchemist().Run).Start();
                            break;
                        }
                    case 1:
                        {
                            new Thread(new B_Alchemist().Run).Start();
                            break;
                        }
                    case 2:
                        {
                            new Thread(new C_Alchemist().Run).Start();
                            break;
                        }
                    case 3:
                        {
                            new Thread(new D_Alchemist().Run).Start();
                            break;
                        }
                    default:
                        break;
                }
                Thread.Sleep(0);
            }
        }
    }
}
