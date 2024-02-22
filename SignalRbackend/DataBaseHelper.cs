using System.Data.SQLite;

namespace SignalRbackend;

public static class DataBaseHelper
{
    private static string connectionstring = @"Data Source=..\..\..\Files\Test.db;Version=3;";


    public static void InitializeDatabase()
    {
        if (!File.Exists(@"..\..\..\Files\Test.db"))
        {
            SQLiteConnection.CreateFile(@"..\..\..\Files\Test.db");
        }

        using (SQLiteConnection connection = new SQLiteConnection(connectionstring))
        {
            connection.Open();

            // Create tables for your data
            string createUsersTableQuery = @"
                        CREATE TABLE IF NOT EXISTS users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserName TEXT NOT NULL,
                        Age INTEGER NOT NULL,
                        PassWord TEXT NOT NULL
                    );";

            // Create tables for your data
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

    public static void DeleteUser(string username)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionstring))
        {
            connection.Open();

            string inputstring = $@"
                DELETE FROM users WHERE UserName='{username}';";

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

    public static void PushChatRoom(string endpoint) // FIXA
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

    public static List<User> ReadAllUsers()
    {
        var userList = new List<User>();

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
                        userList.Add(new User(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetString(3)));
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
