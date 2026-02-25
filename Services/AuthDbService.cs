using SQLite;
using StockApp.Models;

namespace StockApp.Services;

public class AuthDbService : IAuthDbService
{
    // Objet de la Bdd
    private SQLiteConnection? _connection;

    // Lock, pour éviter les problèmes de concurrence (multi-threading, appris grâce à mes expérience en système embarqué (Nicolas Soubigou))
    private readonly object _lock = new();

    public User? CurrentUser { get; private set; }

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

    private SQLiteConnection GetConnection()
    {
        if (_connection != null)
            return _connection;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, DbList.Data);

        _connection = new SQLiteConnection(dbPath);
        return _connection;
    }

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

    public void Logout()
    {
        CurrentUser = null;
    }

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

    public List<User> LoadUsers()
    {
        return GetConnection().Table<User>().ToList();
    }

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
