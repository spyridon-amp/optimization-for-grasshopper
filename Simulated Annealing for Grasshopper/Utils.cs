﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulated_Annealing_for_Grasshopper
{
    class State
    {
        private static Random random = new Random(23);

        public List<double> Values { get; private set; }
        public int Dim { get; private set; }

        public State(List<double> values)
        {
            Values = new List<double>();
            Values.AddRange(values);
            Dim = values.Count;
        }

        public State()
        {
            Values = new List<double>();
            Dim = 0;
        }

        public void ClampState(double min, double max)
        {
            for (int i = 0; i < Dim; ++i)
            {
                Values[i] = Utils.Clamp(Values[i], min, max);
            }
        }

        public State copy()
        {
            return new State(Values);
        }

        public State SampleNormal(double mean, double stDev)
        {
            List<double> vv = new List<double>();
            for (int i = 0; i < Dim; ++i)
            {
                vv.Add(Utils.SampleNormalDistribution(mean, stDev));
            }
            return new State(vv);
        }

        public static State SampleUniform(double min, double max, int size)
        {
            List<double> vv = new List<double>();
            for (int i = 0; i < size; ++i)
            {
                double v = random.NextDouble() * (max - min) + min;
                vv.Add(v);
            }
            return new State(vv);
        }

        /// <summary>
        /// Increment state1 by state2
        /// </summary>
        /// <param name="st1"></param>
        /// <param name="st2"></param>
        /// <returns>new State, piecewise addition of values</returns>
        public static State Add(State st1, State st2)
        {
            if (st1.Dim != st2.Dim)
            {
                //Print("Cannot add states with dimensions " + st1.Dim + " and " + st2.Dim);
                return new State();
            }

            List<double> vv = new List<double>();
            for (int i = 0; i < st1.Dim; ++i)
            {
                vv.Add(st1.Values[i] + st2.Values[i]);
            }
            return new State(vv);
        }

        /// <summary>
        /// Scale a state by a number
        /// </summary>
        /// <param name="state"></param>
        /// <param name="scale"></param>
        /// <returns>new State, all values multiplied by scale, bounds do not change</returns>
        public static State Scale(State state, double scale)
        {
            List<double> vv = new List<double>();
            for (int i = 0; i < state.Dim; ++i)
            {
                vv.Add(state.Values[i] * scale);
            }
            return new State(vv);
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < Dim; ++i)
            {
                s = s + Values[i] + " ";
            }
            return s;
        }

    }



    class Utils
    {

        public Utils()
        {

        }

        private static Random random = new Random(23);

        /// <summary>
        /// Performance function
        /// </summary>
        /// <param name="state">State to evaluate</param>
        /// <returns>Performance of state</returns>
        public static double evaluate(State state)
        {
            //return (state.Values[0] + state.Values[1]) * (state.Values[0] + state.Values[1]);
            double x1 = state.Values[0];
            double x2 = state.Values[1];

            // performance function from: http://apmonitor.com/me575/index.php/Main/SimulatedAnnealing
            return 0.2 + x1 * x1 + x2 * x2 - 0.1 * Math.Cos(6 * Math.PI * x1) - 0.1 * Math.Cos(6 * Math.PI * x2);
        }


        public static double Clamp(double val, double min, double max)
        {
            if (val > max) val = max;
            if (val < min) val = min;
            return val;
        }

        public static double SampleNormalDistribution(double mean, double stdDev)
        {
            double u1 = 1.0 - random.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
              Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
              mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;
        }
    }
}