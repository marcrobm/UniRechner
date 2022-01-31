using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace LinAlg
{
    class TH1
    {
        public class AUTOMAT
        {
            public AUTOMAT(string Input)
            {
                Input = Input.Replace(">", ">:");
                string [] lines = Regex.Split(Input,Environment.NewLine);    
                foreach (string line in lines){
                    if (line == "")
                    {
                        break;
                    }
                    string type = Regex.Split(line, ":")[0];//Format: A B a // from A to B using a
                    string [] values = Regex.Split(Regex.Split(line, ":")[1], " ");
                    if (type == "STARTNODE")
                    {
                        start_node = values[0];
                    }
                    if (type == "NODES")
                    {
                        Nodes.AddRange(values);
                    }else
                    if (type == "FINALNODES")
                    {
                        FinalNodes.AddRange(values);
                    }
                    else
                    if (type == ">")
                    {
                        if (values[0] != "" && values[1] != "" && values[2] != "")
                        {
                            transitions.Add(new Tuple<string, string, char>(values[0], values[2], values[1].ElementAt(0)));
                            if (!Alphabet.Contains(values[1].ElementAt(0)))
                            {
                                Alphabet.Add(values[1].ElementAt(0));
                            }
                        }
                        else
                        {
                            throw new Exception("invalid transitions, line:"+line);
                        }
                    }
                    else
                    {
                        //Error
                    }
                }
            }
            string start_node;
            public List<Tuple<string,string,char>> transitions = new List<Tuple<string, string, char>>();
            public List<string> Nodes = new List<string>();
            public List<string> FinalNodes = new List<string>();
            public List<char> Alphabet = new List<char>();
        }
        public static string TableFilling(AUTOMAT A)
        {
            string[][] table = new string[A.Nodes.Count+1][];
            // 1.initial: null = unmarkiert
            for (int i = 0; i < table.Length; i++)
            {
                table[i] = new string[A.Nodes.Count+1];
            }
            // label table with nodes
            for(int i = 0; i < A.Nodes.Count; i++)
            {
                table[0][i+1] = A.Nodes[i];
                table[i+1][0] = A.Nodes[i];
            }
            // 2. Markiere alle Paare {q, q′} mit q ∈ QF und q′ nicht ∈ QF .
                for (int c = 1; c < table.Length; c++)
                {
                    for (int r = 1; r < c+1; r++)
                    {
                        if (A.FinalNodes.Contains(A.Nodes[r - 1]) != A.FinalNodes.Contains(A.Nodes[c - 1]))
                        {
                            table[r][c] = "0";
                        }
                    }
                }
            // 3. loop
            bool iterate = true;
            int run = 0;
            while (iterate)
            {
                iterate = false;
                run++;
                for (int c = 1; c < table.Length; c++)
                {
                    for (int r = 1; r < c; r++)
                    {
                        if (table[r][c] == null)
                        {
                            //find reached field for every tuple
                            foreach(char a in A.Alphabet)
                            {
                                // found a unmarked pair
                                // calculat e reachable fields
                                try
                                {
                                    int reach1 = A.Nodes.IndexOf(A.transitions.Where(t => t.Item1 == table[r][0] && t.Item3 == a).First().Item2);
                                    int reach2 = A.Nodes.IndexOf(A.transitions.Where(t => t.Item1 == table[0][c] && t.Item3 == a).First().Item2);
                                    if (table[reach1 + 1][reach2 + 1] != null || table[reach2 + 1][reach1 + 1] != null)
                                    {
                                        //is marked, mark current
                                        table[r][c] = run.ToString();
                                        iterate = true;
                                    }
                                }catch(Exception ex)
                                {

                                }
                            }
                        }
                    }
                }
            }


            return Utils.StringArrToLatexMatrix(table,"X");
        }
    }
}
