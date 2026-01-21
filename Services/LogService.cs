using SQLite;
using StockApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Services
{
    public class LogService : ILogService
    {
        // Objet de la Bdd
        private SQLiteConnection? _connection;

        private readonly object _lock = new();

        private SQLiteConnection GetConnection()
        {
            if (_connection != null)
                return _connection;

            var dbName = "stockapp.db";
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, dbName);

            _connection = new SQLiteConnection(dbPath);
            return _connection;
        }

        public LogService() {
            // Initialisation de la Bdd
            var conn = GetConnection();
            conn.CreateTable<Log>();
        }


        public void LogInfo(string tag, string message)
        {
            Log(tag, message, level: 1);
        }

        public void LogWarning(string tag, string message)
        {
            Log(tag, message, level: 2);
        }
        
        public void LogError(string tag, string message, Exception ex)
        {
            Log(tag,
                $"{message} | Exception: {ex.Message} | StackTrace: {ex.StackTrace}",
                level: 3
            );
        }

        private void Log(string tag,string message, int level)
        {
            var log = new Log
            {
                message = $"[{tag}] - {message}",
                level = level,
                timestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
            lock (_lock)
            {
                    var conn = GetConnection();
                    conn.Insert(log);
            }
        }
    }
}
