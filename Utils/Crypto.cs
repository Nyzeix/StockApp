using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Utils
{}

public static class Crypto
{
    public static string NewSalt(int bytes = 16)
    {
        var b = RandomNumberGenerator.GetBytes(bytes);
        return Convert.ToBase64String(b);
    }

    public static string Sha256(string text) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(text)));

    public static string HashPassword(string password, string salt) =>
        Sha256(salt + password);
}