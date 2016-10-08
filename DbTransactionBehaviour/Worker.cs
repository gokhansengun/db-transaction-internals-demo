﻿using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
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