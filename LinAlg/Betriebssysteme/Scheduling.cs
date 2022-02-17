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
            public static bool operator ==(Task A,Task B)
            {
                return A.id==B.id;
            }
            public static bool operator !=(Task A, Task B)
            {
                return A.id != B.id;
            }
        }
        List<Task> tasks = new List<Task>();
        public string[][] Schedule;//row:core, collom:time
        uint cores = 1;
        public Scheduling(String formattedInput)
        {
            /*
            Regex inputFormat = new Regex(@"\((?<id>\w+),(?<start>\d+),(?<duration>\d+)\)");
            foreach(Match match in inputFormat.Matches(formattedInput))
            {
                Task A = new Task();
                A.id = match.Groups["id"].ToString();
                A.start = uint.Parse(match.Groups["start"].ToString());
                A.duration = uint.Parse(match.Groups["duration"].ToString());
                tasks.Add(A);
            }*/
            String[] input = formattedInput.Split(new string[] { Environment.NewLine },StringSplitOptions.None);
            String[] name = input[0].Split(' ');
            String[] dauer = input[1].Split(' ');
            String[] start = input[2].Split(' ');
            for (int i = 0; i < name.Length;i++) {
                Task A = new Task();
                A.id = name[i];
                A.start = uint.Parse(start[i]);
                A.duration = uint.Parse(dauer[i]);
                tasks.Add(A);
            }
            tasks.Sort((x, y) => StrCmpLogicalW(x.id,y.id));
            Regex CPUFormat = new Regex("\\(CPU=(\\d+)\\)");
            var matchcpu = CPUFormat.Match(formattedInput).Groups[1];
            cores = uint.Parse(matchcpu.ToString());
            Schedule = new string[cores+1][];
            Schedule[0] = new string[1];
            Schedule[0][0] = "Zeitpunkt";
            for (int i = 1; i < cores+1; i++)
            {
                Schedule[i] = new string[1];
                Schedule[i][0] = "Kern_" + i;
            }
        }
        public void fillTimeLatex()
        {
            for (int i = 1; i < Schedule[0].Length; i++)
            {
                Schedule[0][i] = i.ToString();
            }
        }
        public string ScheduleFCFS()
        {
            List<Task> tempTasks = new List<Task>(tasks);
            int current_slot = 1;
            var waitingTasks = new List<Task>(tempTasks.Where((task) => task.start <= current_slot));
            while (tempTasks.Count>0)
            {
                foreach (Task t in waitingTasks.OrderBy((A) => (int)A.start))
                {
                    // Iterate through each core
                    int cpu = nextAvailableCpu();
                    int start_slot = nextAvailableSlot(cpu);
                    //Utils.setCollumCount(ref Schedule, Math.Max(start_slot + 1,Schedule[0].Count())+1);
                    ScheduleTask(cpu, start_slot, t.duration, t.id);
                    tempTasks.RemoveAll((x)=> (x.id == t.id));
                }
                current_slot++;
                waitingTasks = new List<Task>(tempTasks.Where((task) => task.start <= current_slot));
            }
            fillTimeLatex();
            return ("Input\\\\" + TasksToLatex() + Utils.Ltx("\\\\FCFS(rows:core0-coreX, collums time)\\\\") + Utils.StringArrToLatexMatrix(Schedule, "\\emptyset"));
        }
        public string ScheduleSPN() 
        {
            // TODO
            List<Task> tempTasks = new List<Task>(tasks);
            int current_slot = 1;
            var waitingTasks = new List<Task>(tempTasks.Where((task) => task.start <= current_slot));
            while (tempTasks.Count > 0)
            {
                foreach (Task t in waitingTasks.OrderBy(A => A.duration))
                {
                    int cpu = nextAvailableCpu();
                    int start_slot = nextAvailableSlot(cpu);
                    ScheduleTask(cpu, start_slot, t.duration, t.id);
                    tempTasks.RemoveAll((x) => (x.id == t.id));
                }
                current_slot = nextAvailableSlot(nextAvailableCpu());
                waitingTasks = new List<Task>(tempTasks.Where((task) => task.start <= current_slot));
            }
            fillTimeLatex();
            return ("Input\\\\" + TasksToLatex() + Utils.Ltx("\\\\SPN(rows:core0-coreX, collums time)\\\\") + Utils.StringArrToLatexMatrix(Schedule, "\\emptyset"));
        }     
        List<Task> GetAvailableTasks(int start_time,List<Task> tasks)
        {
            List<Task> available = new List<Task>();
            foreach(Task t in tasks)
            {
                if (t.start >= start_time)
                {
                    available.Add(t);
                }
            }
            return available;
        }



        public void ScheduleTask(int core,int start,uint duration,string id)
        {
            Console.WriteLine("Scheduled: " + id + " from " + start + " to " + (start + duration));
            for (int i = start; i<start+duration; i++)
            {
                Utils.setCollumCount(ref Schedule, Math.Max(i+1, Schedule[0].Count()));
                Schedule[core+1][i] = id;
            }
        }
        public int nextAvailableSlot(int core)
        { 
            int pos = Schedule[0].Length;
            for (int i = Schedule[0].Length - 1; i >= 0; i--)
            {
                if (Schedule[core+1][i] != null)
                {
                    break;
                }
                pos = i;
            }
            return pos;
        }
        public int nextAvailableCpu()
        {
            int time = int.MaxValue;
            int core = 0;
            for(int i = 0; i < cores; i++)
            {
                if (nextAvailableSlot(i) < time)
                {
                    time = nextAvailableSlot(i);
                    core = i;
                }
            }
            return core;
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
