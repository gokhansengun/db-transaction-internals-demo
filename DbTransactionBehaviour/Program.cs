using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace DbTransactionBehaviour
{
    class Worker
    {
        public void Run(int taskId)
        {
            var connStr = ConfigurationManager.ConnectionStrings[ConnStrName].ToString();
            using (var sqlConnection = NewDbConnection(connStr))
            {
                sqlConnection.Open();

                var transaction = sqlConnection.BeginTransaction(IsolationLevel.ReadCommitted);

                var rand = new Random();
                var theName = rand.Next(100000, 1000000).ToString();
                var theId = rand.Next(10, 100);

                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = $"INSERT INTO \"Employee\"(name, id) VALUES('{theName}',{theId})";
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.Transaction = transaction;

                    Console.WriteLine($"Executing the command for index: {taskId}");

                    sqlCommand.ExecuteNonQuery();

                    Thread.Sleep(6000);

                    transaction.Commit();

                    Console.WriteLine($"Committed for index: {taskId}");
                }
            }
        }

        private DbConnection NewDbConnection(string connectionString)
        {
            if (ConfiguredDb == Db.MsSql)
                return new SqlConnection(connectionString);
            else if (ConfiguredDb == Db.Postgres)
                return new NpgsqlConnection(connectionString);

            throw new Exception("Do not know the database");
        }

        private string ConnStrName => ConfiguredDb + "DbConnection";

        private Db ConfiguredDb
        {
            get
            {
                if (ConfigurationManager.AppSettings["db"] == "Postgres")
                    return Db.Postgres;
                else if (ConfigurationManager.AppSettings["db"] == "MsSql")
                    return Db.MsSql;

                return Db.Unknown;
            }
        } 
    }

    enum Db
    {
        Unknown = 0,
        MsSql = 1,
        Postgres = 2
    }

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
