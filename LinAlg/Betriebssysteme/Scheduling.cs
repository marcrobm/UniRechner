using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace LinAlg.Betriebssysteme
{
    class Scheduling
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string psz1, string psz2);
        struct Task
        {
            public string id;
            public uint start;
            public uint duration;
        }
        List<Task> tasks = new List<Task>();
        public string[][] Schedule;//row:core, collom:time
        uint cores = 1;
       public Scheduling(String formattedInput)
        {
            Regex inputFormat = new Regex(@"\((?<id>\w+),(?<start>\d+),(?<duration>\d+)\)");
            foreach(Match match in inputFormat.Matches(formattedInput))
            {
                Task A = new Task();
                A.id = match.Groups["id"].ToString();
                A.start = uint.Parse(match.Groups["start"].ToString());
                A.duration = uint.Parse(match.Groups["duration"].ToString());
                tasks.Add(A);
            }
            tasks.Sort((x, y) => StrCmpLogicalW(x.id,y.id));
            Regex CPUFormat = new Regex("\\(CPU=(\\d+)\\)");
            var matchcpu = CPUFormat.Match(formattedInput).Groups[1];
            cores = uint.Parse(matchcpu.ToString());
            Schedule = new string[cores][];
            for(int i = 0; i < cores; i++)
            {
                Schedule[i] = new string[1];
            }
        }
        public void ScheduleFCFS() //TODO: OUTPUT
        {
            List<Task> tempTasks = new List<Task>(tasks);
            int current_slot = 0;
            var waitingTasks = new List<Task>(tempTasks.Where((task) => task.start <= current_slot));
            while (tempTasks.Count>0)
            {
                waitingTasks.Sort((A,B)=>((int)A.start-(int)B.start));
                //Iterate through waiting tasks
                foreach (Task t in waitingTasks)
                {
                    //Iterate through each core
                    for(int currentCore = 0;currentCore<cores;currentCore++)
                        if (Schedule[currentCore][current_slot] == null) {//is this core free?
                            //schedule this thread in next available core
                            for (int i = 0; i < t.duration; i++)
                            {
                                Utils.setCollumCount(ref Schedule, Math.Max((current_slot + i), (Schedule[0].Count()))+1);
                                Schedule[currentCore][current_slot + i] = t.id;
                            }
                            tempTasks.RemoveAll((x)=> (x.id == t.id));
                            break;
                    }
                }          
                current_slot++;
                Utils.setCollumCount(ref Schedule, current_slot+1);
                waitingTasks = new List<Task>(tempTasks.Where((task) => task.start <= current_slot));
            }   
        }
       public string TasksToLatex()
        {
            String s = "Rechenkerne:"+cores+"\\\\";
            s += @"\pmatrix{";
            //Add row for Processes
            s += "Process" + " & ";
            for (int j = 0; j < tasks.Count; j++)
            {
                s += tasks[j].id + " & ";
            }
            s = s.Substring(0, s.Length - 3);// last remove &
            s += @"\\";

            //Add row for duration
            s += "Bedienzeit" + " & ";
            for (int j = 0; j < tasks.Count; j++)
            {
                s += tasks[j].duration + " & ";
            }
            s = s.Substring(0, s.Length - 3);// last remove &
            s += @"\\";

            //Add row for start
            s += "Ankunftszeit" + " & ";
            for (int j = 0; j < tasks.Count; j++)
            {
                s += tasks[j].start + " & ";
            }
            s = s.Substring(0, s.Length - 3);// last remove &
            s += @"\\";
      


            s = s.Substring(0, s.Length - 2); //remove last //
            s += "}";

            return (s);
        }

    }
}
