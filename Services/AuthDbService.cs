using SQLite;
using StockApp.Models;
using StockApp.Utils;

namespace StockApp.Services;

public class AuthDbService : IAuthDbService
{
    // Objet de la Bdd
    private SQLiteConnection? _connection;

    // Lock, pour éviter les problèmes de concurrence (multi-threading, appris grâce à mes expérience en système embarqué (Nicolas Soubigou))
    private readonly object _lock = new();

    public User? CurrentUser { get; private set; }


    /// <summary>
    /// Constructeur de la classe, qui initialise la base de données et crée un utilisateur admin par défaut si aucun utilisateur n'existe.
    /// </summary>
    public AuthDbService()
    {
        // Initialisation de la Bdd
        var conn = GetConnection();
        conn.CreateTable<User>();
        var UsersList = conn.Table<User>().ToList();
        if (UsersList.Count == 0 || !UsersList.Any(u => u.IsAdmin))
        {
            // Création d'un utilisateur admin par défaut si la table est vide
            var salt = Crypto.NewSalt();
            var hash = Crypto.HashPassword("admin", salt);
            var adminUser = new User
            {
                Username = "admin",
                PasswordHash = hash,
                Salt = salt,
                IsAdmin = true
            };
            conn.Insert(adminUser);
        }

    }


    /// <summary>
    /// Méthode pour obtenir la connexion à la base de données. Si la connexion existe déjà, elle est réutilisée, sinon elle est créée.
    /// </summary>
    /// <returns>SQLiteConnection</returns>
    private SQLiteConnection GetConnection()
    {
        if (_connection != null)
            return _connection;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, DbList.Data);

        _connection = new SQLiteConnection(dbPath);
        return _connection;
    }


    /// <summary>
    /// Méthode pour se connecter à l'application. Elle vérifie si l'utilisateur existe et si le mot de passe est correct. Si c'est le cas, elle stocke l'utilisateur connecté dans la propriété CurrentUser.
    /// </summary>
    /// <param name="username">Nom d'utilisateur</param>
    /// <param name="password">Mot de passe</param>
    /// <returns>Un booléen indiquant si la connexion a réussi ou non.</returns>
    public async Task<bool> LoginAsync(string username, string password)
    {
        return await Task.Run(() =>
        {
            lock (_lock)
            {
                try
                {
                    var conn = GetConnection();
                    var user = conn.Table<User>()
                                   .FirstOrDefault(u => u.Username == username);
                    // Vérification user existe
                    if (user == null)
                        return false;

                    var hash = Crypto.HashPassword(password, user.Salt);
                    if (hash == user.PasswordHash)
                    {
                        CurrentUser = user;
                        return true;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        });
    }


    /// <summary>
    /// Méthode pour se déconnecter de l'application. Elle réinitialise la propriété CurrentUser à null.
    /// </summary>
    public void Logout()
    {
        CurrentUser = null;
    }


    /// <summary>
    /// Méthode pour enregistrer un nouvel utilisateur. Elle vérifie si le nom d'utilisateur est déjà pris, et si ce n'est pas le cas, elle crée un nouvel utilisateur avec le mot de passe hashé et le sel, puis l'insère dans la base de données.
    /// </summary>
    /// <param name="username">Nom d'utilisateur</param>
    /// <param name="password">Mot de passe</param>
    /// <returns>Un booléen indiquant si l'enregistrement a réussi ou non.</returns>
    public async Task<bool> RegisterAsync(string username, string password)
    {
        return await Task.Run(() =>
        {
            lock (_lock)
            {
                try
                {
                    var conn = GetConnection();
                    var salt = Crypto.HashPassword(password, Crypto.NewSalt());
                    var hash = Crypto.HashPassword(password, salt);
                    var user = new User
                    {
                        Username = username,
                        PasswordHash = hash,
                        Salt = salt,
                        IsAdmin = false
                    };
                    conn.Insert(user);
                    

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        });
    }


    /// <summary>
    /// Méthode pour vérifier si un nom d'utilisateur existe déjà dans la base de données. Elle retourne true si le nom d'utilisateur est pris, et false s'il est disponible.
    /// </summary>
    /// <param name="username">Nom d'utilisateur à vérifier</param>
    /// <returns>Un booléen indiquant si le nom d'utilisateur est déjà utilisé ou non.</returns>
    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await Task.Run(() =>
        {

            lock (_lock)
            {
                try
                {
                    var conn = GetConnection();
                    // Si user existe après cette ligne, alors le nom d'utilisateur est pris
                    var user = conn.Table<User>()
                                   .FirstOrDefault(u => u.Username == username);
                    return user != null;
                }
                catch
                {
                    return false;
                } 
            }
        });
    }


    /// <summary>
    /// Méthode pour ajouter un nouvel utilisateur. Elle vérifie si le nom d'utilisateur est déjà pris, et si ce n'est pas le cas, elle crée un nouvel utilisateur avec le mot de passe hashé et le sel, puis l'insère dans la base de données.
    /// </summary>
    /// <param name="username">Nom d'utilisateur</param>
    /// <param name="password">Mot de passe</param>
    /// <param name="isAdmin">Indique si l'utilisateur est administrateur</param>
    /// <returns>Un booléen indiquant si l'ajout a réussi ou non.</returns>
    public async Task<bool> AddUser(string username, string password, bool isAdmin)
    {
        return await Task.Run(() =>
        {
            lock (_lock)
            {
                try
                {
                    var conn = GetConnection();
                    // Vérification si l'utilisateur existe déjà
                    var existingUser = conn.Table<User>()
                                           .FirstOrDefault(u => u.Username == username);
                    if (existingUser != null)
                        return false;
                    var salt = Crypto.NewSalt();
                    var hash = Crypto.HashPassword(password, salt);
                    var user = new User
                    {
                        Username = username,
                        PasswordHash = hash,
                        Salt = salt,
                        IsAdmin = isAdmin
                    };
                    conn.Insert(user);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        });
    }


    /// <summary>
    /// Méthode pour modifier un utilisateur existant. Elle vérifie si l'utilisateur existe, et si c'est le cas, elle met à jour les informations de l'utilisateur dans la base de données. Si le mot de passe a été modifié, il est hashé à nouveau avec un nouveau sel.
    /// Le nom d'utilisateur ne peut pour l'instant, pas être modifié.
    /// </summary>
    /// <param name="modifiedUser">Utilisateur modifié</param>
    /// <returns>Un booléen indiquant si la mise à jour a réussi ou non.</returns>
    public async Task<bool> UpdateUserAsync(User modifiedUser)
    {
        return await Task.Run(() =>
        {
            lock (_lock)
            {
                try
                {
                    var conn = GetConnection();
                    var existingUser = conn.Table<User>()
                                           .FirstOrDefault(u => u.Username == modifiedUser.Username);
                    if (existingUser == null)
                        return false;

                    existingUser.IsAdmin = modifiedUser.IsAdmin;
                    // Si le mot de passe a été modifié, on le hash à nouveau
                    if (!string.IsNullOrEmpty(modifiedUser.PasswordHash) && modifiedUser.PasswordHash != existingUser.PasswordHash)
                    {
                        var salt = Crypto.NewSalt();
                        var hash = Crypto.HashPassword(modifiedUser.PasswordHash, salt);
                        existingUser.PasswordHash = hash;
                        existingUser.Salt = salt;
                    }
                    conn.Update(existingUser);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        });
    }


    /// <summary>
    /// Méthode pour supprimer un utilisateur. Elle vérifie si l'utilisateur existe, et si c'est le cas, elle le supprime de la base de données.
    /// </summary>
    /// <param name="username">Nom d'utilisateur à supprimer</param>
    /// <returns>Un booléen indiquant si la suppression a réussi ou non.</returns>
    public Task<bool> DeleteUser(string username)
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                    var conn = GetConnection();
                    var user = conn.Table<User>()
                                   .FirstOrDefault(u => u.Username == username);
                    if (user == null)
                        return false;
                    conn.Delete(user);
                    return true;
            }
        });
    }


    /// <summary>
    /// Méthode pour charger la liste des utilisateurs depuis la base de données. Elle retourne une liste d'utilisateurs.
    /// </summary>
    /// <returns>La liste des utilisateurs enregistrés</returns>
    public List<User> LoadUsers()
    {
        return GetConnection().Table<User>().ToList();
    }

    /// <summary>
    /// Méthode asynchrone pour charger la liste des utilisateurs depuis la base de données. Elle retourne une liste d'utilisateurs.
    /// </summary>
    /// <returns>La liste des utilisateurs enregistrés</returns>
    public async Task<List<User>> GetUsersAsync()
    {
        return await Task.Run(() =>
        {
            lock (_lock)
            {
                return GetConnection().Table<User>().ToList();
            }
        });
    }
}
