using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulated_Annealing_for_Grasshopper
{
    // template reference: https://codereview.stackexchange.com/questions/70310/simple-simulated-annealing-template-in-c11
    // other reference: https://www.gnu.org/software/gsl/doc/html/siman.html
    class SimulatedAnnealing
    {
        private static Random random= new Random(23);

        private int maxK = 400;
        private int dwell = 5;  // TODO: reconsider this (20) in https://github.com/CISMM/SimulatedAnnealing/blob/master/simann.cxx
        private double tscale = 1.8; // maybe 0.1 according to https://github.com/CISMM/SimulatedAnnealing/blob/master/simann.cxx


        private int dimensions = 25;

        private double upper = 1.0;
        private double lower = 0.0;
        // estimate starting Temperature as 1.2 times the largest cost-function deviation over random points
        // in the box-shaped region specified by the lower, upper input parameters
        private double T = 0;
        private double T0;
        private double learn_rate = 0.15;  // scipy defaul is 0.5 from: https://docs.scipy.org/doc/scipy-0.15.1/reference/generated/scipy.optimize.anneal.html
        private double k_boltzmann = 0.2; // scipy default is 1.0
        private double Tfinal = Math.Pow(10, -12);

        private double energy_old;
        private double energy_new;
        private State state_old;

        public State state_best { get; private set; } = new State();
        public double energy_best { get; private set; }
        public List<State> states_log { get; private set; } = new List<State>();
        public List<double> energies_log { get; private set; } = new List<double>();

        public SimulatedAnnealing()
        {

        }

        public SimulatedAnnealing(int randomSeed)
        {
            random = new Random(randomSeed);
        }

        private void Initialize()
        {
            // run a few iterations randomly sampling the whole space in order to initialize temperature, energy_best, state_best
            List<State> randomStates = new List<State>();
            List<double> randomPerformances = new List<double>();
            for (int i = 0; i < 10; ++i)
            {
                State s = State.SampleUniform(lower, upper, dimensions);
                double e = Utils.evaluate(s);
                randomStates.Add(s);
                randomPerformances.Add(e);
            }

            double emin = Double.MaxValue;
            double emax = Double.MinValue;
            for (int i = 0; i < randomPerformances.Count; ++i)
            {
                if (randomPerformances[i] < emin)
                {
                    emin = randomPerformances[i];
                    state_best = randomStates[i];
                }
                if (randomPerformances[i] > emax)
                {
                    emax = randomPerformances[i];
                }
            }

            T0 = 1.2 * (emax - emin);
            T = T0;
            energy_best = emin;
            energy_old = emin;
            state_old = new State((upper - lower)*0.5, dimensions);
            Console.WriteLine("Initial temperature: " + T0);
        }

        private void Anneal()
        {
            double performance;

            // iteration of outer loop
            for (int k = 1; k <= maxK; k += 2)
            {

                // update temperature using Boltzmann schedule
                T = T0 / Math.Log(1 + k, 2);

                //update temperature using Cauchy schedule
                //T = T0 / (1 + k * tscale);

                for (int il = 0; il < dwell; ++il)
                {

                    // sample new state using Boltzmann schedule
                    double std = Math.Min(Math.Sqrt(T), (upper - lower) / (9 * learn_rate));  // TODO: this was 3, changed to 5 to make steps smaller
                    //State y = state_old.SampleNormal(0, std);
                    double drift = 1.0 - k / (4.0*maxK);
                    State y = state_old.SampleNormalGalapagosInspired(0, std, drift);
                    State state_new = State.Add(state_old, State.Scale(y, learn_rate));
                    state_new.ClampState(lower, upper);  // TODO: this might prove tricky, check later if it causes algo to get stuck on boundaries
                    

                    // sample new state using Cauchy schedule
                    /*State state_new = State.SampleCauchy(state_old, learn_rate, T);
                    state_new.ClampState(lower, upper);  
                    */

                    // then evaluate performance of the new state
                    // ... output current state to proceed
                    performance = Utils.evaluate(state_new);

                    // when performance received
                    energy_new = performance;

                    // keep track of best state
                    if (energy_new < energy_best)
                    {
                        energy_best = energy_new;
                        state_best = state_new;
                    }

                    if (energy_new < energy_old)
                    {
                        energy_old = energy_new;
                        state_old = state_new.copy();

                        // also add to log:
                        states_log.Add(state_new);
                        energies_log.Add(energy_new);
                    }
                    else
                    {
                        double p = Math.Exp(-(energy_new - energy_old) / (T * k_boltzmann));
                        if (random.NextDouble() < p)
                        {
                            energy_old = energy_new;
                            state_old = state_new.copy();

                            // also add to log:
                            states_log.Add(state_new);
                            energies_log.Add(energy_new);
                        }
                    }
                } // end of inner loop over dwell

                // break if system has cooled down
                if (T <= Tfinal)
                    break;

            }  // end of outer loop over maxK
        }

        public double Run()
        {
            // run Initialize to determine initial temperature
            Initialize();
            // run optimization
            Anneal();
           
            return energy_best;
        }

    }
}
