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
        static int Main(String[] args)
        {
            int port;
            int vol_from;
            int vol_to;
            int combination_from;
            int combination_to;

            // Test if input arguments were supplied.
            if (args.Length == 0)
            {
                port = 11000;
                vol_from = 0;
                vol_to = 0;
                combination_from = 0;
                combination_to = 0;
            }
            else if (args.Length == 5)
            {
                // Try to convert the input arguments to numbers. This will throw
                // an exception if the argument is not a number.
                bool success = int.TryParse(args[0], out port);
                success = int.TryParse(args[1], out vol_from) && success;
                success = int.TryParse(args[2], out vol_to) && success;
                success = int.TryParse(args[3], out combination_from) && success;
                bool success_comb = int.TryParse(args[4], out combination_to) && success;
                if (!success)
                {
                    Console.WriteLine("Error in input arguments, need: [port, vol_from, vol_to, combination_from, combination_to");
                    return 1;
                }
            }
            else
            {
                Console.WriteLine("Wrong number of arguments");
                return 1;
            }

            SynchronousClientSocket.port = port;

            for (int volume = vol_from; volume < vol_to; ++volume)
            {
                for (int combination = combination_from; combination < combination_to; ++combination)
                {
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


            /*SimulatedAnnealing simulatedAnnealing = new SimulatedAnnealing();
            double best = simulatedAnnealing.Run();
            Console.WriteLine(format: "optimal result: {0}", arg0: best.ToString());

            Console.WriteLine("optimal state {0}", simulatedAnnealing.state_best.ToString());
            */
            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();

            return 0;
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
