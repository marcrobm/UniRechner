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
            public uint end;
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
        uint timeFrameLength = 1;
        public List<Tuple<string, string>> Dependancies = new List<Tuple<string, string>>(); // Format: <parent,child>
        List<Task> finishedTasks = new List<Task>();
        public Scheduling(String formattedInput,String dependConstraints)
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

            Regex DependanciesRegEx = new Regex(@"(?<Parent>\w+)\>(?<Child>\w+)");
            var matches = DependanciesRegEx.Matches(dependConstraints);
            foreach (Match match in matches)
            {
                Dependancies.Add(new Tuple<string,string>(match.Groups["Parent"].ToString(), match.Groups["Child"].ToString()));
            }
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
            Regex slotLength = new Regex("\\(FRAME=(\\d+)\\)");
            var matchslot = slotLength.Match(formattedInput).Groups[1];
            timeFrameLength = uint.Parse(matchslot.ToString());

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
            var waitingTasks = new List<Task>(tempTasks.Where((task) => (task.start <= current_slot) && IsAvailable(task,(uint)current_slot)));
            while (tempTasks.Count>0)
            {
                foreach (Task t in waitingTasks.OrderBy((A) => (int)A.start))
                {
                    // Iterate through each core
                    int cpu = nextAvailableCpu(current_slot);
                    int start_slot = nextAvailableSlot(cpu,current_slot);
                    //Utils.setCollumCount(ref Schedule, Math.Max(start_slot + 1,Schedule[0].Count())+1);
                    ScheduleTask(cpu, start_slot, t.duration, t.id);
                    tempTasks.RemoveAll((x)=> (x.id == t.id));
                    var ttemp = t;
                    ttemp.end = (uint)start_slot + t.duration;
                    finishedTasks.Add(ttemp);
                }
                current_slot++;
                waitingTasks = new List<Task>(tempTasks.Where((task) => (task.start <= current_slot) && IsAvailable(task, (uint)current_slot)));
            }
            fillTimeLatex();
            return ("Input\\\\" + TasksToLatex() + Utils.Ltx("\\\\FCFS\\\\") + Utils.StringArrToLatexMatrix(Schedule, "\\emptyset"));
        }
        public string ScheduleSRTF(bool optimizeSwap) 
        {
            int intervalLength = 1;
            int maxloops = 50;
            List<Task> tempTasks = new List<Task>(tasks);
            int current_slot = 1;
            var waitingTasks = new List<Task>(tempTasks.Where((task) => (task.start <= current_slot) &&  IsAvailable(task,(uint)current_slot)));
            Dictionary<string, int> remainingtime = new Dictionary<string, int>();
            foreach(var v in tempTasks)
            {
                remainingtime[v.id] = (int)v.duration;
            }
            String TimeLatexInfo = Utils.Ltx("Remaining Timeframes per Step" + Environment.NewLine+"      "+ Utils.DictToKeyString(remainingtime))+"\\\\";
            //while there are tasks to schedule
            while (tempTasks.Count > 0 && maxloops-->0)
            {
                TimeLatexInfo +=  Utils.Ltx(current_slot.ToString("D2") + ". "+ Utils.DictToValueString(remainingtime)) + "\\\\";
                waitingTasks.Sort((x, y) => (int)remainingtime[x.id] - (int)remainingtime[y.id]);
                List<int> availablecpu = new List<int>(Enumerable.Range(0,(int)cores));

                for (int cpu = 0;cpu<cores;cpu++)
                {
                    //pick the Task with the shortest remaining timeframe            
                    if (waitingTasks.Count > 0)
                    {
                        var ct = waitingTasks.First();
                        Console.WriteLine(ct.id + " needs " + remainingtime[ct.id]);
                        waitingTasks.RemoveAll(x => x.id == ct.id);

                        // select the best cpu
                        int cpupos = availablecpu.First();
                        if (optimizeSwap)
                        {
                            foreach (var s in availablecpu)
                            {
                                if (Schedule[s + 1][current_slot - 1] == ct.id)
                                {
                                    cpupos = s;
                                    break;
                                }
                            }
                        }
                        availablecpu.Remove(cpupos);

                        ScheduleTask(cpupos,current_slot, (uint)Math.Min((uint)remainingtime[ct.id],intervalLength),ct.id);// todo test this
                        if (remainingtime[ct.id] <= intervalLength)
                        {
                            // if the task is finished, remove it
                            tempTasks.RemoveAll((x) => (x.id == ct.id));
                            ct.end = (uint)(current_slot + remainingtime[ct.id]);
                            remainingtime[ct.id] = 0;
                            finishedTasks.Add(ct);
                        }
                        else
                        {
                            Console.WriteLine(ct.id + " decremented to " + remainingtime[ct.id]);
                            // else reduce the remaining time
                            remainingtime[ct.id] -= (int)intervalLength;
                        }
                    }
                }
                current_slot += (int)intervalLength;
                waitingTasks = new List<Task>(tempTasks.Where((task) => (task.start <= current_slot) && IsAvailable(task, (uint)current_slot)));
            }
            TimeLatexInfo += Utils.Ltx(current_slot.ToString("D2") +". "+ Utils.DictToValueString(remainingtime)) + "\\\\";
            fillTimeLatex();
            return ("Input\\\\" + TasksToLatex() + Utils.Ltx("\\\\SRTF " + (optimizeSwap ? "(minimum Swaps)" : "") + "\\\\") + Utils.StringArrToLatexMatrix(Schedule, "\\emptyset") + "\\\\ "+ TimeLatexInfo);
        }
        public string FilterReadyList(string list)
        {
            string filtered = list.Replace("||", "|").Replace("|,", "|").Replace(",|", "|").Replace(",,", ",");
            if (filtered == "")
            {
                return filtered;
            }else
            if (filtered.Last() == '|')
            {
                filtered = filtered.Remove(filtered.Length - 1);
            }else
            if (filtered.First() == '|')
            {
                filtered = filtered.Remove(0,1);
            }else
            if (filtered.First() == ',')
            {
                filtered = filtered.Remove(0, 1);
            }else
            if (filtered.Last() == ',')
            {
                filtered = filtered.Remove(filtered.Length - 1);
            }

            if (filtered != list)
            {
                return FilterReadyList(filtered);
            }
            else
            {
                return list;
            }
        }
        public string ScheduleRR(bool optimizeSwap)
        { 
            // TODO testing
            int maxloops = 20;
            List<Task> tempTasks = new List<Task>(tasks); // stores all tasks
            int current_slot = 1;
            var waitingTasks = new List<Task>(tempTasks.Where((task) => (task.start <= current_slot) && IsAvailable(task, (uint)current_slot)));// stores tasks which still have to be executed
            String ReadyLatexInfo = Utils.Ltx("Ready Tasks" + Environment.NewLine);
            string ReadyList = ""; // format: a,b,c|d,e|d means a,b,c at highest priority, d,e one lower d at lowest priority
            // for each task the remaining execution time
            Dictionary<string, int> remainingtime = new Dictionary<string, int>();
            foreach (var v in tempTasks)
            {
                remainingtime[v.id] = (int)v.duration; // Task has not been scheduled yet so entire duration.
            }
            //while there are tasks to schedule
            List<string> lastScheduledProcesses = new List<string>();
            while (tempTasks.Count > 0 && maxloops-- > 0)
            {
                // Add arriving Processes to start of ReadyList
                ReadyList = ReadyList+"|"; // Keep the old unscheduled processes at the highest priority
                // Add new incoming processes
                var Incoming = waitingTasks.Where(x => !lastScheduledProcesses.Exists(y => y == x.id) && x.start<current_slot).ToList(); // alte prozesse die noch zeitwollen und neu hinzugekommene
                // TODO: multiple entry times
                // Group the Incoming Processes by their arrival time
                var incomingGroups = Incoming.GroupBy(x => x.start).ToList();
                incomingGroups.Sort((x,y)=>(int)x.Key-(int)y.Key); // sort groups by arrival
                foreach (var group in incomingGroups) 
                {
                    foreach(Task t in group)
                    {
                        if (!ReadyList.Contains(t.id))
                        {
                            ReadyList = ReadyList + "," + t.id;// add a waiting task
                        }
                    }
                    if (group.First().start < current_slot) // if the time of the entered process is smaller than the current_slot, group
                    {
                        ReadyList += "|";
                    }
                }

                // Add older already scheduled processes
                //foreach (Task t in waitingTasks.Where(x => lastScheduledProcesses.Exists(y => y == x.id)))
                foreach (Task t in waitingTasks.Where(x => lastScheduledProcesses.Exists(y => y == x.id) || x.start==current_slot))
                {
                    if (!ReadyList.Contains(t.id))
                    {
                        ReadyList = ReadyList + "," + t.id;// add a waiting task
                    }
                }
                ReadyList = FilterReadyList(ReadyList);
                // Output Ready List
                ReadyLatexInfo += ReadyList + "\\Rightarrow ";

                // now schedule the tasks from the Readylist
                List<int> availablecpu = new List<int>(Enumerable.Range(0, (int)cores));// pick a cpu
                var currentScheduledProcesses = new List<string>();
                for (int cpu = 0; cpu < cores; cpu++)
                {      
                    if (ReadyList.Replace("|","").Length> 0) // if there are any tasks in ReadyList
                    {
                        ReadyList = FilterReadyList(ReadyList);

                        var templist = ReadyList.Split('|')[0].Split(',').ToList();
                        templist.Sort(StrCmpLogicalW);
                        string taskToSchedule = templist[0];// pick the Task on top of ReadyList to Schedule it
                        if (current_slot > 1)
                        {
                            var preferedProcesses = templist.Where(x => lastScheduledProcesses.Contains(x) && availablecpu.Exists(y => Schedule[y + 1][current_slot - 1] == x)).ToList();
                            if (preferedProcesses.Count() > 0 && optimizeSwap)
                            {
                                taskToSchedule = preferedProcesses.First();// use task which is already on the cpu given the chance
                            }
                        }

                        currentScheduledProcesses.Add(taskToSchedule);
                        ReadyList = ReadyList.Replace(taskToSchedule,"");// remove it from readylist
                        ReadyList = FilterReadyList(ReadyList);
                        Console.WriteLine(taskToSchedule + " needs " + remainingtime[taskToSchedule]);
                        waitingTasks.RemoveAll(x => x.id == taskToSchedule);// task has been scheduled, so no longer is waiting
                        // select the best cpu for this task
                        int cpupos = availablecpu.First();
                        if (optimizeSwap)
                        {
                            foreach (var s in availablecpu)
                            {
                                Utils.setCollumCount(ref Schedule, Math.Max((int)current_slot, (int)Schedule[0].Count()));
                                if (Schedule[s + 1][current_slot - 1] == taskToSchedule)
                                {
                                    cpupos = s;
                                    break;
                                }
                            }
                        }
                        availablecpu.Remove(cpupos);
                        ScheduleTask(cpupos, current_slot, (uint)Math.Min(remainingtime[taskToSchedule],timeFrameLength), taskToSchedule);
                        if (remainingtime[taskToSchedule] <= timeFrameLength)
                        {
                            // if the task is finished, remove it
                            var ct = tempTasks.Single((x) => (x.id == taskToSchedule));
                            tempTasks.RemoveAll((x) => (x.id == taskToSchedule));
                            ct.end = (uint)(current_slot + remainingtime[taskToSchedule]);
                            remainingtime[taskToSchedule] = 0;
                            finishedTasks.Add(ct);
                        }
                        else
                        {   // else reduce the remaining time
                            Console.WriteLine(taskToSchedule + " decremented to " + remainingtime[taskToSchedule]);
                            remainingtime[taskToSchedule] -= (int)timeFrameLength;
                        }
                    }
                }
                lastScheduledProcesses = currentScheduledProcesses;
                current_slot += (int)timeFrameLength;
                waitingTasks = new List<Task>(tempTasks.Where((task) => (task.start <= current_slot) && IsAvailable(task, (uint)current_slot)));
                ReadyLatexInfo += ReadyList + "\\\\";
            }
            fillTimeLatex();
            return ("Input\\\\" + TasksToLatex() + Utils.Ltx("\\\\RR "+(optimizeSwap?"(minimum Swaps)":"") + "\\\\") + Utils.StringArrToLatexMatrix(Schedule, "\\emptyset") + "\\\\ " + ReadyLatexInfo);
        }








        public string ScheduleSPN()
        {
            // 
            List<Task> tempTasks = new List<Task>(tasks);
            int current_slot = 1;
            var waitingTasks = new List<Task>(tempTasks.Where((task) => (task.start <= current_slot) && IsAvailable(task, (uint)current_slot)));
            while (tempTasks.Count > 0)
            {
                foreach (Task t in waitingTasks.OrderBy(A => A.duration))
                {
                    int cpu = nextAvailableCpu(current_slot);
                    int start_slot = nextAvailableSlot(cpu, current_slot);
                    ScheduleTask(cpu, start_slot, t.duration, t.id);
                    tempTasks.RemoveAll((x) => (x.id == t.id));
                    var ttemp = t;
                    ttemp.end = (uint)start_slot + t.duration;
                    finishedTasks.Add(ttemp);
                }
                current_slot = nextAvailableSlot(nextAvailableCpu(current_slot+1),current_slot+1);
                waitingTasks = new List<Task>(tempTasks.Where((task) => (task.start <= current_slot) && IsAvailable(task, (uint)current_slot)));
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
        bool IsAvailable(Task t,uint time)
        {
            // Is there no task this task depends upon which is not finished(in finished list)
            bool res=  !Dependancies.Exists(X => (X.Item2 == t.id && !finishedTasks.Exists(y=> y.id==X.Item1 && y.end <= time)));
            Console.WriteLine("t:" + t.id + " at:" + time + " can " + (res?"":"not")+ "be scheduled");
            return res;
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
        public int nextAvailableSlot(int core,int minTime)
        { 
            int pos = Schedule[0].Length;
            for (int i = Schedule[0].Length - 1; i >= minTime; i--)
            {
                if (Schedule[core+1][i] != null)
                {
                    break;
                }
                pos = i;
            }
            return pos;
        }
        public int nextAvailableCpu(int minTime)
        {
            int time = int.MaxValue;
            int core = 0;
            for(int i = 0; i < cores; i++)
            {
                if (nextAvailableSlot(i,minTime) < time)
                {
                    time = nextAvailableSlot(i,minTime);
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
