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

        /// <summary>
        /// Constructeur de la classe LogService.
        /// Il initialise la connexion à la base de données des logs et crée la table des logs si elle n'existe pas déjà.
        /// </summary>
        public LogService() {
            // Initialisation de la Bdd
            var conn = GetConnection();
            conn.CreateTable<Log>();
        }


        /// <summary>
        /// Méthode pour obtenir la connexion à la base de données des logs.
        /// Si la connexion n'existe pas, elle est créée. Sinon, la connexion existante est retournée.
        /// </summary>
        /// <returns>SQLiteConnection</returns>
        private SQLiteConnection GetConnection()
        {
            if (_connection != null)
                return _connection;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, DbList.AppLogs);

            _connection = new SQLiteConnection(dbPath);
            return _connection;
        }


        /// <summary>
        /// Méthode pour enregistrer un message d'information dans la base de données des logs. 
        /// Elle prend en paramètre un tag pour identifier la source du message, et le message lui-même.
        /// </summary>
        /// <param name="tag">TAG, prédéfini dans chaque classe qui utilise le service de log</param>
        /// <param name="message">Message à enregistrer</param>
        public void LogInfo(string tag, string message)
        {
            Log(tag, message, level: 1);
        }


        /// <summary>
        /// Méthode pour enregistrer un message d'avertissement dans la base de données des logs.
        /// Elle prend en paramètre un tag pour identifier la source du message, et le message lui-même.
        /// </summary>
        /// <param name="tag">TAG, prédéfini dans chaque classe qui utilise le service de log</param>
        /// <param name="message">Message à enregistrer</param>
        public void LogWarning(string tag, string message)
        {
            Log(tag, message, level: 2);
        }
        

        /// <summary>
        /// Méthode pour enregistrer un message d'erreur dans la base de données des logs.
        /// Elle prend en paramètre un tag pour identifier la source du message, le message lui-même,
        /// et une exception associée à l'erreur.
        /// Le message enregistré inclut les détails de l'exception, tels que le message d'erreur et la trace de la pile.
        /// </summary>
        /// <param name="tag">TAG, prédéfini dans chaque classe qui utilise le service de log</param>
        /// <param name="message">Message à enregistrer</param>
        /// <param name="ex">Exception associée à l'erreur</param>
        public void LogError(string tag, string message, Exception ex)
        {
            Log(tag,
                $"{message} | Exception: {ex.Message} | StackTrace: {ex.StackTrace}",
                level: 3
            );
        }

        /// <summary>
        /// Méthode privée pour enregistrer un message de log dans la base de données.
        /// Elle prend en paramètre:
        /// un tag pour identifier la source du message, 
        /// le message lui-même, 
        /// le niveau de gravité du message (1 pour info, 2 pour avertissement, 3 pour erreur).
        /// Le message est formaté pour inclure le tag.
        /// </summary>
        /// <param name="tag">TAG, prédéfini dans chaque classe qui utilise le service de log</param>
        /// <param name="message">Message à enregistrer</param>
        /// <param name="level">Niveau de gravité du message (1 pour info, 2 pour avertissement, 3 pour erreur)</param>
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
