using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace DbTransactionBehaviour
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Task> tasks = new List<Task>();
            int totalTaskNumber = int.Parse(args.Length == 0 ? "20" : args[0]);

            for (var i = 0; i < totalTaskNumber; ++i)
            {
                var index = i;

                Task tsk = new Task(() =>
                {
                    var worker = new Worker();

                    worker.Run(index);

                }, TaskCreationOptions.LongRunning);

                tasks.Add(tsk);
            }

            tasks.ForEach(t => t.Start());

            while (true)
            {
                try
                {
                    Task.WaitAll(tasks.ToArray());
                }
                catch (AggregateException ae)
                {
                    foreach (var innerException in ae.InnerExceptions)
                    {
                        Console.WriteLine($"EXCEPTION: {innerException.Message}");
                    }
                    
                    var newTaskList = new List<Task>();

                    foreach (var task in tasks)
                    {
                        if (!task.IsCompleted)
                            newTaskList.Add(task);
                    }

                    if (newTaskList.Count == 0)
                        break;

                    tasks = newTaskList;
                }
            }
        }
    }
}
