﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WUT.Zad1.Lib;
using WUT.Zad1.Lib.Alchemists;
using WUT.Zad1.Lib.Wizards;

namespace WUT.Zad1.Console
{
    class Program
    {
        const int PbFacId = 0;
        const int SFacId = 1;
        const int HgFacId = 2;
        private static bool Working = true;

        private static Factory[] factories;
        private static List<Wizard> wizards;

        static void Main(string[] args)
        {
            SetUp();
            Start();
            System.Console.ReadLine();
            Stop();

            System.Console.ReadLine();
        }
        private static void Start()
        {
            new Thread(AlchemistSpamer).Start();
            Storehouse.StartWorkAsync();
            foreach (var factory in factories)
                new Thread(factory.Run).Start();
            foreach (var wizard in wizards)
                new Thread(wizard.Run).Start();
        }

        private static void Stop()
        {
            Working = false;
            foreach (var wizar in wizards)
                wizar.Stop();
            foreach (var factory in factories)
                factory.Stop();
            Storehouse.StopWork();
        }

        private static void SetUp()
        {
            factories = new Factory[]
            {
                new Factory("Pb Factory",LibSettings.Default.PbProductionTime,LibSettings.Default.ProductionInterval),
                new Factory("S Factory",LibSettings.Default.SProductionTime,LibSettings.Default.ProductionInterval),
                new Factory("Hg Factory",LibSettings.Default.HgProductionTime,LibSettings.Default.ProductionInterval)
            };

            Storehouse.Hg_Factory = factories[HgFacId];
            Storehouse.S_Factory = factories[SFacId];
            Storehouse.Pb_Factory = factories[PbFacId];

            wizards = new List<Wizard>();
            for (int i = 0; i < LibSettings.Default.EvilWizardsCount; i++)
                wizards.Add(new EvilWizard(factories, LibSettings.Default.CastEvilCharmTime, LibSettings.Default.CharmInterval));
            for (int i = 0; i < LibSettings.Default.GoodWizardsCount; i++)
                wizards.Add(new GoodWizard(factories, LibSettings.Default.CastGoodCharmTime, LibSettings.Default.CharmInterval));

        }

        private static void AlchemistSpamer()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            var delta = LibSettings.Default.MaxAlchemistRespTime - LibSettings.Default.MinAlchemistRespTime;
            var minVal = LibSettings.Default.MinAlchemistRespTime;
            System.Console.WriteLine("Spammer Begin");
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
            System.Console.WriteLine("Spammer stop...");
        }
    }
}
