using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using Npgsql;
using IsolationLevel = System.Data.IsolationLevel;

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
                
                using (var transaction = sqlConnection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    using (var sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = "SELECT 1";
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Transaction = transaction;

                        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} Executing the command for threadId: {taskId}");

                        sqlCommand.ExecuteNonQuery();

                        Thread.Sleep(6000);

                        transaction.Commit();

                        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} Committed for threadId: {taskId}");
                    }
                }
            }
        }

        private DbConnection NewDbConnection(string connectionString)
        {
            if (ConfiguredDbEnum == DbEnum.MsSql)
                return new SqlConnection(connectionString);
            else if (ConfiguredDbEnum == DbEnum.Postgres)
                return new NpgsqlConnection(connectionString);

            throw new Exception("Do not know the database");
        }

        private string ConnStrName => ConfiguredDbEnum + "DbConnection";

        private DbEnum ConfiguredDbEnum
        {
            get
            {
                if (ConfigurationManager.AppSettings["db"] == "Postgres")
                    return DbEnum.Postgres;
                else if (ConfigurationManager.AppSettings["db"] == "MsSql")
                    return DbEnum.MsSql;

                return DbEnum.Unknown;
            }
        } 
    }
}