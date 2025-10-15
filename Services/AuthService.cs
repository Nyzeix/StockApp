using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using StockApp.Models;
using StockApp.Utils;

namespace StockApp.Services;

public class AuthService : IAuthService
{
    private const string PREFS_KEY = "users_json_v1";
    private readonly object _lock = new();

    public User? CurrentUser { get; private set; }

    private List<User> LoadUsers()
    {
        var json = Preferences.Get(PREFS_KEY, "");
        if (string.IsNullOrWhiteSpace(json)) return new List<User>();
        try { return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>(); }
        catch { return new List<User>(); }
    }

    private void SaveUsers(List<User> users)
    {
        var json = JsonSerializer.Serialize(users);
        Preferences.Set(PREFS_KEY, json);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await Task.Run(() =>
        {
            var users = LoadUsers();
            return users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        });
    }

    public async Task<bool> RegisterAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        return await Task.Run(() =>
        {
            lock (_lock)
            {
                var users = LoadUsers();
                if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                    return false;

                var salt = Crypto.NewSalt();
                var hash = Crypto.HashPassword(password, salt);

                users.Add(new User { Username = username.Trim(), PasswordHash = hash, Salt = salt });
                SaveUsers(users);
                return true;
            }
        });
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        return await Task.Run(() =>
        {
            var users = LoadUsers();
            var u = users.FirstOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (u is null) return false;

            var hash = Crypto.HashPassword(password, u.Salt);
            if (hash != u.PasswordHash) return false;

            CurrentUser = u;
            return true;
        });
    }

    public void Logout() => CurrentUser = null;
}