using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulated_Annealing_for_Grasshopper
{
    class Program
    {
        static void Main()
        {
            SimulatedAnnealing simulatedAnnealing = new SimulatedAnnealing();
            double best = simulatedAnnealing.Run();
            Console.WriteLine(format: "optimal result: {0}", arg0: best.ToString());
        }
    }
}
