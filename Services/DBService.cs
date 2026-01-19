using SQLite;
using StockApp.Models;

namespace StockApp.Services;

public static class TestDBService
{
    private static SQLiteConnection? _connection;

    private static SQLiteConnection GetConnection()
    {
        if (_connection != null)
            return _connection;

        var dbName = "stockapp.db";
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, dbName);

        _connection = new SQLiteConnection(dbPath);
        return _connection;
    }

    public static bool TestLoginSimple(string username, string password)
    {
        try
        {
            var conn = GetConnection();
            var user = conn.Table<User>()
                           .FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

            return user != null;
        }
        catch
        {
            return false;
        }
    }
}
