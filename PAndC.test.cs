using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using MySql.Server;
using System.Diagnostics;
using System.Linq;


class DbColumn
{
    public string name, type;
    public bool isNotNull;
    public DbColumn (string name, string type, bool isNotNull)
    {
        this.name = name;
        this.type = type;
        this.isNotNull = isNotNull;
    }

    public DbColumn(string name, string type)
    {
        this.name = name;
        this.type = type;
        isNotNull = false;
    }
}

namespace Torn
{
    [TestClass]
    public class PAndCTest
    {
        private static readonly string _testDatabaseName = "ng_system";
        private static MySqlServer CreateDatabase()
        {
            MySqlServer dbServer = MySqlServer.Instance;
            dbServer.StartServer();

            //Create a database and select it
            MySqlHelper.ExecuteNonQuery(dbServer.GetConnectionString(), string.Format("CREATE DATABASE {0};USE {0};", _testDatabaseName));
            return dbServer;
        }

        private static string CreateTable(MySqlServer dbServer, string tableName, DbColumn[] columns, string primaryKey)
        {
            string columnsQuery = columns.Aggregate("", (acc, column) => string.Format("{0} `{1}` {2} {3},", acc, column.name, column.type, column.isNotNull ? "NOT NULL" : ""));
            Console.WriteLine(columnsQuery);
            //Create a table
            MySqlHelper.ExecuteNonQuery(dbServer.GetConnectionString(_testDatabaseName), string.Format("CREATE TABLE {0} ({1} PRIMARY KEY (`{2}`)) ENGINE = MEMORY;", tableName, columnsQuery, primaryKey));
            return tableName;
        }

        [TestMethod]
        public void TimeSpanTest()
        {
            //Setting up and starting the server
            //This can also be done in a AssemblyInitialize method to speed up tests
            MySqlServer dbServer = CreateDatabase();
            string gameLog = CreateTable(
                dbServer,
                "ng_game_log", 
                new[]
                {
                    new DbColumn("Event_Type", "INT", true),
                    new DbColumn("Time_Logged", "DATETIME", true)
                },
                "Time_Logged"
            );

            string registry = CreateTable(
                dbServer,
                "ng_registry",
                new[]
                {
                    new DbColumn("Registry_ID", "INT", true),
                    new DbColumn("Int_Data_1", "INT", true)
                },
                "Registry_ID"
            );

            //Set Mock Current Time
            MySqlHelper.ExecuteNonQuery(dbServer.GetConnectionString(_testDatabaseName), "SET TIMESTAMP = UNIX_TIMESTAMP('2021-01-01T00:30:00')");

            //Insert data
            MySqlHelper.ExecuteNonQuery(dbServer.GetConnectionString(_testDatabaseName), string.Format("INSERT INTO {0} (`Registry_ID`,`Int_Data_1`) VALUES (0, 50)", registry));
            //Populate with a few records
            MySqlHelper.ExecuteNonQuery(dbServer.GetConnectionString(_testDatabaseName), string.Format("INSERT INTO {0} (`Event_Type`,`Time_Logged`) VALUES (0, '2021-01-01T00:00:00')", gameLog));
            MySqlHelper.ExecuteNonQuery(dbServer.GetConnectionString(_testDatabaseName), string.Format("INSERT INTO {0} (`Event_Type`,`Time_Logged`) VALUES (0, '2019-01-01T00:00:00')", gameLog));
            MySqlHelper.ExecuteNonQuery(dbServer.GetConnectionString(_testDatabaseName), string.Format("INSERT INTO {0} (`Event_Type`,`Time_Logged`) VALUES (0, '2020-01-01T00:00:00')", gameLog));



            PAndC pAndCServer = new PAndC(dbServer.GetConnectionString, _testDatabaseName);

            TimeSpan gameTimeElapsed = pAndCServer.GameTimeElapsed();

            TimeSpan expected = new TimeSpan(0, 30, 0);

            Assert.AreEqual(expected, gameTimeElapsed);

            //Shutdown server
            dbServer.ShutDown();
        }
    }
}