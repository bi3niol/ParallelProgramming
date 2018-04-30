using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WUT.ParallelProgramming.EX3.Jankiel.Messages;

namespace WUT.ParallelProgramming.EX3.Jankiel
{
    public class Jankiel
    {
        internal JankielManager jankielManager { get; }

        public string Name { get; private set; }
        private string MasterName;
        private Random random;
        public Jankiel(string name, string[] neighbors, string master = "Ex3Master")
        {
            Name = name;
            MasterName = master;
            random = new Random(name.GetHashCode());
            jankielManager = new JankielManager(this);
            jankielManager.SetUpConnection(name, neighbors, master);
        }
        public async void Run()
        {
            Console.WriteLine($"Jankiel {Name} Czeka na start ...");
            jankielManager.WaitForStart();
            ElectionStatus status = ElectionStatus.None;
            int i = 0;
            //Dopoki nie bylismy wybrani to znaczy ze nie gralismy jeszcze koncertu
            while (status != ElectionStatus.Selected)
            {
                //informacja dla sąsiadów o gotowosci do rozpączecia rundy
                await jankielManager.SendMsg(new StartTourMessage(Name));
                //oczekiwanie na potwierdzenie od swoich sąsiadów
                jankielManager.WaitStartTour();

                Console.WriteLine($"{Name} Start rundy {++i}");

                //wybory ktory z jankieli będzie grał
                status = await MIS();

                Console.WriteLine($"Jankiel {Name} status : {status}");
                if (status == ElectionStatus.Selected)
                {
                    //jankiel został wybrany wiec może grać
                    Console.WriteLine($"Jankiel {Name} gra koncert !");
                    Thread.Sleep(2000);
                }

                //informacja dla sąsiadów ze jankiel skonczył turę, wraz z informacją czy w tej turze grał koncert
                // jesli tak to sąsiedzi w przyszył turach nie będą czekać na informacje pochodzące od tego jankiela
                await jankielManager.SendMsg(new FinishedTourMessage(Name, status == ElectionStatus.Selected));
                //czekanie na informacje od sąsiadów
                jankielManager.WaitFinishTour();
                Console.WriteLine($"{Name} koniec rundy {i}");
            }
            Console.WriteLine($"Jankiel {Name} skończył ...");
        }

        /// <summary>
        /// Realizacja algorytmu MIS (wybór zbioru niezależnego) 
        /// </summary>
        /// <returns>Status po zakonczeniu wyborów </returns>
        private async Task<ElectionStatus> MIS()
        {
            byte v;
            ElectionStatus status = ElectionStatus.None;
            for (int i = 0; i < jankielManager.FirstMISForLength; i++)
            {
                double probabilityOfChoose = 1.0 / Math.Pow(2, jankielManager.FirstMISForLength - i);
                for (int j = 0; j < jankielManager.SecondMISForLength; j++)
                {
                    v = 0;
                    if (random.NextDouble() < probabilityOfChoose)
                        v = 1;
                    //------Gwarancja że jesli ktos juz wie, że będzie grał------
                    //to jego status się nie zmieni
                    if (status == ElectionStatus.Selected)
                        v = 1;
                    if (status == ElectionStatus.Lose)
                        v = 0;
                    //---------------------------------------------------------
                    // Wyslanie wiadomosci do sąsiadów z informacją o swoim "głosie" ( w wyborach )
                    await jankielManager.SendMsg(new VoteMessage(Name, v));

                    // Czekanie na głosy sąsiadów, oraz sprawdzenie czy któryś z nich nie nie wysłał 1 w wiadomosci
                    bool isBRecived = jankielManager.RecivedB();
                    if (isBRecived)
                        v = 0;

                    // nikt nikt z sąsiadów nie wysłał 1, oznacza to że bierzący proces został wybrany
                    if (v == 1)
                        status = ElectionStatus.Selected;
                    //----------------------------------------------------------
                    //druga wymiana wiadomosci
                    // ponowne wysłanie v do sąsiadów by poinformować czy wartość v zmieniła się po otrzymaniu od nich wiadomosci 
                    await jankielManager.SendMsg(new VoteMessage(Name, v));
                    isBRecived = jankielManager.RecivedB();
                    // jesli v jest 0 oraz otrzymalismy od jakiegos sąsiada 1 to proces ten przegrywa wybory w przeciwnym przypadku 
                    //jesli v = 0 oraz nie otrzymalismy zadnej jedynki od sąsiadów status nadal jest nieznany
                    if (v == 0 && isBRecived)
                        status = ElectionStatus.Lose;
                }
            }
            return status;
        }

        public enum ElectionStatus
        {
            Lose = 0,
            Selected = 1,
            None = 2,
            Finished = 3
        }
    }
}
