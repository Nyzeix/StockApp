using StockApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Services
{}
public interface IAuthDbService
{
    User? CurrentUser { get; }
    Task<bool> LoginAsync(string username, string password);
    void Logout();
    Task<bool> RegisterAsync(string username, string password);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> AddUser(string username, string password, Boolean isAdmin);
    Task<bool> DeleteUser(string username);
    List<User> LoadUsers();
}