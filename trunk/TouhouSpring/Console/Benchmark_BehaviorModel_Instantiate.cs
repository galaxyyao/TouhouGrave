using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TouhouSpring
{
    class Benchmark_BehaviorModel_Instantiate : ICommandlet
    {
        public string Tag
        {
            get { return "benchmark.0"; }
        }

        public void Execute(params string[] parms)
        {
            Console.WriteLine("Benchmark: BehaviorModel.Instantiate");

            // generate an array of random behavior models
            var rnd = new Random();
            var cardModels = Program.CardFactory.CardModels;
            Behaviors.IBehaviorModel[] bhvModels = new Behaviors.IBehaviorModel[1000];
            for (int i = 0; i < bhvModels.Length; ++i)
            {
                var bhvlist = cardModels[rnd.Next(cardModels.Count)].Behaviors;
                bhvModels[i] = bhvlist[rnd.Next(bhvlist.Count)];
            }
            Behaviors.IBehavior[] bhvs = new Behaviors.IBehavior[bhvModels.Length];

            long start, end, freq;
            QueryPerformanceFrequency(out freq);
            QueryPerformanceCounter(out start);

            for (int i = 0; i < bhvModels.Length; ++i)
            {
                bhvs[i] = bhvModels[i].CreateInitialized();
            }

            QueryPerformanceCounter(out end);

            Console.WriteLine("Time taken: {0:0.3}ms", (end - start) / (double)freq * 1000.0);
        }

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);
    }
}
