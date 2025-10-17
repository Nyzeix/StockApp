using StockApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Services
{}

public interface IAuthService
{
    User? CurrentUser { get; }
    Task<bool> RegisterAsync(string username, string password);
    Task<bool> LoginAsync(string username, string password);
    void Logout();
    Task<bool> UsernameExistsAsync(string username);
}