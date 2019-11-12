using Newtonsoft.Json;
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
            SynchronousClientSocket.SendNoResponse(JsonConvert.SerializeObject(new DataExchange(new List<double> { -1, -1 }, 0, 0)));
            SimulatedAnnealing simulatedAnnealing = new SimulatedAnnealing();
            double best = simulatedAnnealing.Run();
            SynchronousClientSocket.SendNoResponse(JsonConvert.SerializeObject(new DataExchange(new List<double> { best }, 0, 0)));

            Console.WriteLine(format: "optimal result: {0}", arg0: best.ToString());
            Console.WriteLine("optimal state {0}", simulatedAnnealing.state_best.ToString());
            
            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();
        }

        private static void OptimizeCombinations()
        {
            for (int volume = 0; volume < 2; ++volume)
            {
                for (int combination = 0; combination < 40; ++combination)
                {
                    if (volume == 0 && combination <= 17) { continue; }
                    SimulatedAnnealing simulatedAnnealing = new SimulatedAnnealing();
                    // update context
                    // TODO: make a nicer way of sending this message instead of repurposing this list
                    string message = JsonConvert.SerializeObject(new DataExchange(new List<double> { volume, combination }, 0, 0));
                    SynchronousClientSocket.SendNoResponse(message);
                    double best = simulatedAnnealing.Run();
                    message = JsonConvert.SerializeObject(new DataExchange(new List<double> { best }, 0, 0));
                    SynchronousClientSocket.SendNoResponse(message);

                }
            }
        }
    }


    class DataExchange
    {
        public IList<double> values { get; set; }
        public double record1 { get; set; }
        public double record2 { get; set; }

        public DataExchange()
        {

        }

        public DataExchange(List<double> v, double r1, double r2)
        {
            values = new List<double>(v);
            record1 = r1;
            record2 = r2;
        }
    }
}
