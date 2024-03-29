﻿using System.Data.SQLite;

namespace SignalRbackend;

public static class DataBaseHelper
{
    private static string connectionstring = @"Data Source=..\..\..\Files\Test.db;Version=3;";

    // Viktigt: Om det blir sökvägsfel finns 2 sätt att lösa det:
    // 1: Öppna EXEN från bin/debug/net8.0 eller
    // 2: Ta bort '..\..\..\' från sökvägen här och nedan så funkar det från visual studio
    // Beror på att serverns sökväg utgår från NET8-mappen från exen och från projektets grundmapp från VS



    public static void InitializeDatabase()
    {

        // Viktigt: samma som ovan gäller här
        if (!File.Exists(@"..\..\..\Files\Test.db"))
        {
            // Viktigt: Och här
            SQLiteConnection.CreateFile(@"..\..\..\Files\Test.db");
        }

        using (SQLiteConnection connection = new SQLiteConnection(connectionstring))
        {
            connection.Open();

            string createUsersTableQuery = @"
                        CREATE TABLE IF NOT EXISTS users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserName TEXT NOT NULL,
                        Age INTEGER NOT NULL,
                        PassWord TEXT NOT NULL
                    );";

            string createChatRoomsTableQuery = @"
                        CREATE TABLE IF NOT EXISTS chatrooms (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        RoomName TEXT NOT NULL
                    );";

            string createBlackListTableQuery = @"
                        CREATE TABLE IF NOT EXISTS blacklist (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        IP TEXT NOT NULL
                    );";

            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = createUsersTableQuery;
                command.ExecuteNonQuery();
                command.CommandText = createChatRoomsTableQuery;
                command.ExecuteNonQuery();
                command.CommandText = createBlackListTableQuery;
                command.ExecuteNonQuery();
            }
        }
    }

    public static void DeleteUser(string username, string age, string password)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionstring))
        {
            connection.Open();

            string inputstring = $@"
                DELETE FROM users WHERE UserName='{username}'
                AND Age='{age}'
                AND PassWord='{password}';";
                

            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = inputstring;
                command.ExecuteNonQuery();
            }
        }
    }

    public static void DeleteFromBlacklist(string IPaddress)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionstring))
        {
            connection.Open();

            string inputstring = $@"
                DELETE FROM blacklist WHERE IP='{IPaddress}';";

            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = inputstring;
                command.ExecuteNonQuery();
            }
        }
    }

    public static void DeleteChatroom(string endpoint)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionstring))
        {
            connection.Open();

            string inputstring = $@"
                DELETE FROM chatrooms WHERE RoomName='{endpoint}';";

            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = inputstring;
                command.ExecuteNonQuery();
            }
        }
    }

    public static void PushUser(User userInfo)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionstring))
        {
            connection.Open();

            string inputstring = $@"                
                INSERT INTO users (UserName, Age, PassWord)
                VALUES ('{userInfo.UserName}', '{userInfo.Age}', '{userInfo.PassWord}');";

            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = inputstring;
                command.ExecuteNonQuery();
            }
        }
    }

    public static void PushBlacklist(string IPaddress)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionstring))
        {
            connection.Open();

            string inputstring = $@"
                INSERT INTO blacklist (IP)
                VALUES ('{IPaddress}');";

            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = inputstring;
                command.ExecuteNonQuery();
            }
        }
    }

    public static void PushChatRoom(string endpoint) // FIXME:
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionstring))
        {
            connection.Open();

            string inputstring = $@"
                INSERT INTO chatrooms (RoomName)
                VALUES ('{endpoint}');";

            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = inputstring;
                command.ExecuteNonQuery();
            }
        }
    }

    public static Dictionary<string, User> ReadAllUsers()
    {
        Dictionary<string, User> userList = new Dictionary<string, User>();

        using (SQLiteConnection connection = new SQLiteConnection(connectionstring))
        {
            connection.Open();
            
            string inputstring = $@"
                SELECT * FROM users;";
            
            using (SQLiteCommand command = new SQLiteCommand(inputstring, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int ID = reader.GetInt32(0);
                        string username = reader.GetString(1);
                        int age = reader.GetInt32(2);
                        string password = reader.GetString(3);

                        userList.TryAdd(username, new User(ID, username, age, password));
                    }
                }
            }
        }

        return userList;
    }

    public static List<string> ReadAllBlackList()
    {
        var blackList = new List<string>();

        using (SQLiteConnection connection = new SQLiteConnection(connectionstring))
        {
            connection.Open();

            string inputstring = $@"
                SELECT * FROM blacklist;";

            using (SQLiteCommand command = new SQLiteCommand(inputstring, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        blackList.Add(reader.GetString(1));
                    }
                }
            }
        }

        return blackList;
    }

    public static List<string> ReadAllChatRooms()
    {
        var roomList = new List<string>();

        using (SQLiteConnection connection = new SQLiteConnection(connectionstring))
        {
            connection.Open();

            string inputstring = $@"
                SELECT * FROM chatrooms;";

            using (SQLiteCommand command = new SQLiteCommand(inputstring, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roomList.Add(reader.GetString(1));
                    }
                }
            }
        }

        return roomList;
    }
}
