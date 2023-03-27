using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element34.StringMetrics.Similarity
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string s = "";

            s = File.ReadAllText("in.txt");
            //Console.WriteLine(s.Contains('\n'));
            s = s.ToLower();
            s = s.Replace("\n", " ");
            s = markov.utilities.clean(s, new char[] { ' ' });

            markov.model m = markov.utilities.textToModel(s, ' ');
            Console.WriteLine("Model with {0} unique states", m.states.Count);
            Console.WriteLine("'{0}' being the most popular state", m.states.First(y => y.Value.transitions.Length == m.states.Max(x => x.Value.transitions.Length)).Value.identifier);
            
            //m.currentState = m.states["the"];
            List<markov.state> states = m.genMaxTerm(1);
            states.Remove(m.terminatingState);
            
            string outp = "";

            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].identifier.Equals(m.terminatingState))
                    continue;

                outp += (string)states[i].identifier;

                if (i < states.Count - 1) 
                    outp += " ";
            }

            Console.WriteLine(outp);
        }
    }

    public static class markov
    {
        private static Random rand = new Random();

        public class state
        {
            public double[] probabilities;
            public object[] transitions;
            public object identifier;

            public state(double[] p, object[] t, object id)
            {
                probabilities = p;
                transitions = t;
                identifier = id;
            }
        }

        public class model
        {
            public Dictionary<object, state> states = new Dictionary<object, state>();
            public state currentState;
            public bool hasTerminatingState = false;
            public state terminatingState;

            public state step()
            {
                if (currentState.identifier.Equals(terminatingState.identifier))
                    throw new Exception("Tried to stepping while in terminating state");
                object to = currentState.transitions[randomArray(currentState.probabilities)];
                if (!states.ContainsKey(to))
                    throw new Exception("Tried to transition to non-existing state");
                currentState = states[to];
                return currentState;
            }

            public List<state> generate()
            {
                if (!hasTerminatingState)
                    throw new Exception("No terminating state");
                List<state> ret = new List<state>();
                ret.Add(currentState);
                while (!currentState.identifier.Equals(terminatingState.identifier))
                    ret.Add(step());
                return ret;
            }

            public List<state> genMaxTerm(int max)
            {
                if (!hasTerminatingState)
                    throw new Exception("No terminating state");
                List<state> ret = new List<state>();
                ret.Add(currentState);
                int i = 0;
                bool tryQuit = false;
                while (!currentState.Equals(terminatingState))
                {
                    state s = step();
                    i++;
                    if (i >= max) tryQuit = true;
                    if (tryQuit && s.transitions.Contains(terminatingState.identifier))
                    {
                        ret.Add(s);
                        ret.Add(terminatingState);
                        break;
                    }
                    //if (tryQuit) Console.WriteLine("Overtime");
                    ret.Add(s);
                }
                return ret;
            }

            public List<state> generate(int ni)
            {
                List<state> ret = new List<state>();
                ret.Add(currentState);
                for (int i = 0; i < ni; i++)
                {
                    ret.Add(step());
                    if (ret.Contains(terminatingState)) break;
                }
                return ret;
            }

            public void addState(state s, object identifier)
            {
                double prob = 0;
                foreach (double p in s.probabilities)
                    prob += p;
                if (!prob.Equals(1.0))
                    throw new Exception("Must have total probability of 100%");
                states.Add(identifier, s);
            }
        }

        public static class utilities
        {
            public static model textToModel(string[] s, char split)
            {
                Dictionary<string, Dictionary<string, int>> stateToStateProb = new Dictionary<string, Dictionary<string, int>>();
                List<string> str = new List<string>();
                foreach (string strng in s)
                {
                    str.AddRange(strng.Split(split));
                    str.Add("termstate");
                }

                for (int i = 0; i < str.Count - 1; i++)
                {
                    if (!stateToStateProb.ContainsKey(str[i]))
                        stateToStateProb[str[i]] = new Dictionary<string, int>();

                    Dictionary<string, int> stateProp = stateToStateProb[str[i]];
                    if (stateProp.ContainsKey(str[i + 1]))
                        stateProp[str[i + 1]] += 1;
                    else
                        stateProp.Add(str[i + 1], 1);
                    //Console.WriteLine(str[i]);
                }

                //Calculate probabilities and create Markov models
                Dictionary<string, state> states = new Dictionary<string, state>();
                for (int i = 0; i < stateToStateProb.Keys.Count; i++)
                {
                    if (stateToStateProb.ElementAt(i).Key == "termstate") continue; //Doesn't link to anything
                    int denom = 0;
                    List<double> probs = new List<double>();
                    List<string> toState = new List<string>();
                    for (int j = 0; j < stateToStateProb.ElementAt(i).Value.Count; j++)
                        denom += stateToStateProb.ElementAt(i).Value.ElementAt(j).Value;

                    state ns = new state(new double[stateToStateProb.ElementAt(i).Value.Count],
                                         new object[stateToStateProb.ElementAt(i).Value.Count],
                                        (object)stateToStateProb.ElementAt(i).Key);
                    //Console.WriteLine(denom);

                    for (int j = 0; j < stateToStateProb.ElementAt(i).Value.Count; j++)
                    {
                        ns.probabilities[j] = (double)stateToStateProb.ElementAt(i).Value.ElementAt(j).Value / denom;
                        ns.transitions[j] = stateToStateProb.ElementAt(i).Value.ElementAt(j).Key;
                        //Console.WriteLine(stateToStateProb.ElementAt(i).Value.ElementAt(j).Value / denom);
                    }

                    states.Add(stateToStateProb.Keys.ElementAt(i), ns);
                }

                model mark = new model();
                states.Add("termstate", new state(new double[0], new object[0], "termstate"));

                foreach (KeyValuePair<string, state> strstate in states)
                    mark.states.Add(strstate.Key, strstate.Value);

                mark.currentState = states.Values.ElementAt(0);
                mark.hasTerminatingState = true;
                mark.terminatingState = states["termstate"];

                return mark;
            }

            public static model textToModel(string s, char split)
            {
                string[] str = new string[1];
                str[0] = s;
                return textToModel(str, split);
            }

            public static string clean(string str, char[] ignore)
            {
                string ret = "";
                for (int i = 0; i < str.Length; i++)
                {
                    char ch = str[i];
                    if (ignore.Contains(ch)) { ret += ch; continue; }
                    if (Char.IsSymbol(ch) || Char.IsSeparator(ch) || Char.IsWhiteSpace(ch) || Char.IsPunctuation(ch)) continue;
                    ret += ch;
                }
                return ret;
            }
        }

        public static bool random(double percent)
        {
            return (rand.Next(1, 101) <= (percent * 100));
        }

        //TODO: verify this
        public static int randomArrayAdv(double[] percents)
        {
            List<KeyValuePair<int, double>> percent = new List<KeyValuePair<int, double>>();
            for (int i = 0; i < percents.Length; i++)
            {
                percent.Add(new KeyValuePair<int, double>(i, percents[i]));
            }

            while (true)
            {
                KeyValuePair<int, double> percentIndex = percent.ElementAt(percent.Count);
                percent.RemoveAt(percent.Count);
                if (random(percentIndex.Value)) percent.Insert(0, percentIndex);
                if (percent.Count == 2)
                {
                    percentIndex = percent.ElementAt(percent.Count);
                    if (random(percentIndex.Value)) return percentIndex.Key;
                    percent.RemoveAt(percent.Count);
                    percentIndex = percent.ElementAt(percent.Count);
                    return percentIndex.Key;
                }
            }
        }

        public static int randomArray(double[] percents)
        {
            for (int i = 0; i < percents.Length - 1; i++)
                if (random(percents[i])) return i;
            //Console.WriteLine(percents.Length);
            return percents.Length - 1;
        }
    }
}
