using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbTransactionBehaviour
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Task> tasks = new List<Task>();
            int totalTaskNumber = int.Parse(args.Length == 0 ? "4" : args[0]);

            for (var i = 0; i < totalTaskNumber; ++i)
            {
                var threadId = i;

                Task tsk = new Task(() =>
                {
                    var worker = new Worker();

                    worker.Run(threadId);

                }, TaskCreationOptions.LongRunning);

                tasks.Add(tsk);
            }

            tasks.ForEach(t => t.Start());

            while (true)
            {
                try
                {
                    Task.WaitAll(tasks.ToArray());

                    // reaching here means that all tasks
                    // have completed without any problem
                    break;
                }
                catch (AggregateException ae)
                {
                    foreach (var innerException in ae.InnerExceptions)
                    {
                        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} EXCEPTION: with {innerException.Message}");
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
