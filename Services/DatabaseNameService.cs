namespace StockApp.Services
{
    /// <summary>
    /// Classe statique contenant les noms des bases de données utilisées dans l'application. Cela permet d'éviter les erreurs de frappe et de centraliser la gestion des noms de bases de données.
    /// </summary>
    public static class DbList
    {
        public const string Data = "Data.db";
        public const string AppLogs = "AppLogs.db";
    }
}