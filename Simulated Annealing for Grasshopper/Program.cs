﻿using Newtonsoft.Json;
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

            string outMessage = JsonConvert.SerializeObject(new DataExchange(best, 0, 0));
            string response = SynchronousClientSocket.RequestResponse(11000, outMessage);
            Console.WriteLine(response);

            try
            {
                DataExchange incoming = JsonConvert.DeserializeObject<DataExchange>(response);
                Console.WriteLine(incoming.value.ToString());
            }
            catch
            {
                Console.WriteLine( "Unexpected data format received");
            }
        }
    }

    class DataExchange
    {
        public double value { get; set; }
        public double record1 { get; set; }
        public double record2 { get; set; }

        public DataExchange(double v, double r1, double r2)
        {
            value = v;
            record1 = r1;
            record2 = r2;
        }
    }
}
