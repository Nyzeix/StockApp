using Microsoft.Extensions.Logging.Abstractions;
using SQLite;
using StockApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Services
{ }
public class DatabaseService : IDatabaseService
{
    private SQLiteAsyncConnection _businessDb; // Produits, Fournisseurs, Users
    private SQLiteAsyncConnection _logsDb;     // Logs

    public DatabaseService()
    {
        _ = InitBusinessDb();
        _ = InitLogsDb();
    }

    // Initialisation de la base Métier
    private async Task<bool> InitBusinessDb()
    {
        if (_businessDb != null) return false;
        var path = Path.Combine(FileSystem.AppDataDirectory, "StockData.db");
        _businessDb = new SQLiteAsyncConnection(path);

        await _businessDb.CreateTableAsync<Product>();
        await _businessDb.CreateTableAsync<Supplier>();
        await _businessDb.CreateTableAsync<User>();
        return true;
    }

    // Initialisation de la base Logs
    private async Task<bool> InitLogsDb()
    {
        if (_logsDb != null) return false;
        var path = Path.Combine(FileSystem.AppDataDirectory, "AppLogs.db");
        _logsDb = new SQLiteAsyncConnection(path);

        await _logsDb.CreateTableAsync<Log>();
        return true;
    }

    // ----------------------------
    // --- Gestion des produits ---
    // ----------------------------
    public async Task<List<Product>> GetProductsAsync()
    {
        await InitBusinessDb();
        return await _businessDb.Table<Product>().ToListAsync();
    }

    public Task<Product> GetProductByIdAsync(int id)
    {
        try
        {
            Product foundProduct = _businessDb.GetAsync<Product>(id).Result;
            return Task.FromResult(foundProduct);
        }
        catch (Exception ex)
        {
            LogError("DatabaseService", $"Error getting product with ID: {id}", ex);
            return null;
        }
    }

    public Task<bool> AddProductAsync(Product product)
    {
        try
        {
            _businessDb.InsertAsync(product);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            LogError("DatabaseService", "Error when adding product to database", ex);
            return Task.FromResult(false);
        }
    }

    public Task<bool> UpdateProductAsync(Product product)
    {
        try
        {
            _businessDb.UpdateAsync(product);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            LogError("DatabaseService", "Error when updating product", ex);
            return Task.FromResult(false);
        }
    }

    public Task<bool> DeleteProductAsync(Product product)
    {
        try
        {
            _businessDb.DeleteAsync(product);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            LogError("DatabaseService", "Error deleting product", ex);
            return Task.FromResult(false);
        }
    }


    // --------------------------------
    // --- Gestion des fournisseurs ---
    // --------------------------------
    public Task<List<Supplier>> GetSuppliersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddSupplierAsync(Supplier supplier)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateSupplierAsync(Supplier supplier)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteSupplierAsync(Supplier supplier)
    {
        throw new NotImplementedException();
    }


    // -----------------------
    // --- Méthodes de Log ---
    // -----------------------
    public void LogInfo(string tag, string message)
    {
        _ = AddLogAsync(tag, message, level: 1);
    }

    public void LogWarning(string tag, string message)
    {
        _ = AddLogAsync(tag, message, level: 2);
    }

    public void LogError(string tag, string message, Exception ex)
    {
        _ = AddLogAsync(tag,
            $"{message} | Exception: {ex.Message} | StackTrace: {ex.StackTrace}",
            level: 3
        );
    }

    private async Task AddLogAsync(string tag, string message, int level)
    {
        await InitLogsDb();
        await _logsDb.InsertAsync(
            new Log {
                message = $"[{tag}] - {message}",
                level = level,
                timestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()});
    }

        
}