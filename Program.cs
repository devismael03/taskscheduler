using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TaskSchedulerVersion2
{
    

    enum ProcessInterval
    {
        Once,EachDay,EachWeek,EachMonth,EachYear
    }
    [Serializable]
    class Task
    {
        public string Path { get; set; }

        public DateTime ExecuteTime { get; set; }
        public ProcessInterval Interval { get; set; }
        public Dictionary<int, string> intervalKeys = new Dictionary<int, string>() {
            {0,"Once"},
            {1,"EachDay" },
            {2,"EachWeek" },
            {3,"EachMonth" },
            {4,"EachYear" }
        };
        [NonSerialized]
        public Timer timer;
        public Task(string path, DateTime executeTime, ProcessInterval interval)
        {
         
            Path = path;
            ExecuteTime = executeTime;
            Interval = interval;
            StartTimer();
        }

        private void StartTimer()
        {
          
            ulong LeftTime = (ulong)(((ExecuteTime.Subtract(DateTime.Now)).TotalMilliseconds));
            
            timer = new Timer(LeftTime);
            timer.Start();
            timer.Elapsed += this.Execute;

        }
        private void Execute(object source, ElapsedEventArgs e)
        {

            try
            {
                Process.Start(Path);
            }catch(Exception f)
            {
                
                timer.Stop();

                Controller.currentRepo.Remove(this);
                Controller.WriteFile();

            }
            
            switch (intervalKeys[(int)Interval]){
                case "EachDay":
                    ExecuteTime = ExecuteTime.AddDays(1);
                    Console.WriteLine(ExecuteTime);
                    break;
                case "EachWeek":
                    ExecuteTime = ExecuteTime.AddDays(7);
                    Console.WriteLine(ExecuteTime);
                    break;
                case "EachMonth":
                    ExecuteTime = ExecuteTime.AddMonths(1);
                    Console.WriteLine(ExecuteTime);
                    break;
                case "EachYear":
                    ExecuteTime = ExecuteTime.AddYears(1);
                    Console.WriteLine(ExecuteTime);
                    break;
            }
            if (intervalKeys[(int)Interval] != "Once")
            {
                StartTimer();
            }
            else
            {
                timer.Stop();

                Controller.currentRepo.Remove(this);
                Controller.WriteFile();

            }


        }
    }
    class  Controller
    {
        public static List<Task> currentRepo;
       
        public Controller()
        {
            currentRepo = ReadFile();
          


        }
        private List<Task> ReadFile()
        {
            
            using (FileStream s = File.Open("db.txt", FileMode.OpenOrCreate))
            {
                
                if (new FileInfo("db.txt").Length== 0)
                {
                    
                    return new List<Task>();
                }
                else
                {
                    
                    BinaryFormatter f = new BinaryFormatter();
                    return (List<Task>)f.Deserialize(s);

                }
            }
        }
       public static void WriteFile()
        {
            using (FileStream s = File.Open("db.txt", FileMode.Open))
            {
                
                BinaryFormatter f = new BinaryFormatter();
                
                f.Serialize(s, currentRepo);
            }
        }
        private void AddTask()
        {
            Console.Clear();
            Console.WriteLine("Enter path to program");
            string path = Console.ReadLine();
            Console.WriteLine("Enter accurate date to execution");
            DateTime date = DateTime.Parse(Console.ReadLine());
            Console.WriteLine("Enter execution interval : 0 for Once,1 for each day,2 for each week,3 for each month,4 for each year");
            int Interval = Int32.Parse(Console.ReadLine());
            Task task = new Task(path, date, (ProcessInterval)Interval);
            currentRepo.Add(task);
            WriteFile();
            Console.Clear();
            Run();
        }
        private void DeleteTask()
        {
            
            Console.WriteLine("Enter the Id of task to delete (the number in list of all tasks) or '123456' to go back");
            int num = Int32.Parse(Console.ReadLine());
            if(num == 123456)
            {
                
                Console.Clear();
                Run();
            }
            currentRepo[num].timer.Stop();
            currentRepo.Remove(currentRepo[num]);
            WriteFile();
            Console.Clear();
            Run();
        }
        private void ListAllTasksD()
        {
            Console.Clear();
            for (int i = 0; i < currentRepo.Count; i++)
            {
                Console.WriteLine($"{i} ------------- {currentRepo[i].Path}  -------------{currentRepo[i].ExecuteTime} ");
            }
        }
        private void ListAllTasksN()
        {
            Console.Clear();
            for (int i = 0; i < currentRepo.Count; i++)
            {
                Console.WriteLine($"{i} ------------- {currentRepo[i].Path}  -------------{currentRepo[i].ExecuteTime} ");
            }
            Console.WriteLine("Type 'home' to go back");
            string option = String.Empty;
            while(!(option == "home"))
            {
                option = Console.ReadLine();
            }
            Console.Clear();
            Run();
        }

        public void Run()
        {
            Console.Title = "Task Scheduler v1.0 by Mammadzadeh";
            Console.WriteLine("Hello ! Welcome to task scheduler.");
            Console.WriteLine("- Type 'add' (without quotes) to add new task");
            Console.WriteLine("- Type 'delete'(without quotes) to delete task");
            Console.WriteLine("- Type 'list'(without quotes) to see all tasks");
            string option = Console.ReadLine();
            switch (option)
            {
                case "add":
                    AddTask();
                    break;
                case "delete":
                    ListAllTasksD();
                    Console.WriteLine("\n\n");
                    DeleteTask();
                    break;
                case "list":
                    ListAllTasksN();
                    break;
            }
            
        }
        
    }
    class Program
    {
        static void Main(string[] args)
        {
            Controller controller = new Controller();
            controller.Run();
        }
    }
}
