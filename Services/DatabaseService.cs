using Microsoft.Extensions.Logging.Abstractions;
using SQLite;
using StockApp.Models;
using StockApp.Services;
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
    // On ne touche plus jamais à _businessDb directement en dehors de GetConnectionAsync
    private SQLiteAsyncConnection _businessDb;
    private readonly ILogService _log;
    private Task _initTask; // Pour gérer l'initialisation unique

    public DatabaseService(ILogService logService)
    {
        _log = logService;
    }

    // C'est la CLÉ de la solution : Lazy Initialization
    private async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_initTask == null || _initTask.IsFaulted)
        {
            _initTask = InitBusinessDb();
        }

        await _initTask; // On attend que la DB soit prête
        return _businessDb;
    }

    private async Task InitBusinessDb()
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, DbList.Data);
        // Flags pour éviter les erreurs "Database Locked"
        var flags = SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;

        var connection = new SQLiteAsyncConnection(path, flags);

        await connection.CreateTableAsync<Product>();
        await connection.CreateTableAsync<Supplier>();
        await connection.CreateTableAsync<User>();
        _businessDb = connection;
    }

    // ----------------------------
    // --- Gestion des produits ---
    // ----------------------------
    public async Task<List<Product>> GetProductsAsync()
    {
        var db = await GetConnectionAsync();
        return await db.Table<Product>().ToListAsync();
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        try
        {
            var db = await GetConnectionAsync();
            return await db.GetAsync<Product>(id);
        }
        catch (Exception ex)
        {
            _log.LogError("DatabaseService", $"Error getting product ID: {id}", ex);
            return null;
        }
    }

    public async Task<bool> AddProductAsync(Product product)
    {
        try
        {
            var db = await GetConnectionAsync();
            _log.LogInfo("DatabaseService", $"Adding product: {product.Name}");
            await db.InsertAsync(product);
            return true;
        }
        catch (Exception ex)
        {
            _log.LogError("DatabaseService", "Error adding product", ex);
            return false;
        }
    }

    public async Task<bool> UpdateProductAsync(Product product)
    {
        try
        {
            var db = await GetConnectionAsync();
            await db.UpdateAsync(product);
            _log.LogInfo("DatabaseService", $"Updating product: {product.Name}");
            return true;
        }
        catch (Exception ex)
        {
            _log.LogError("DatabaseService", "Error updating product", ex);
            return false;
        }
    }

    public async Task<bool> DeleteProductAsync(Product product)
    {
        try
        {
            var db = await GetConnectionAsync();
            await db.DeleteAsync(product);
            _log.LogInfo("DatabaseService", $"Deleting product: {product.Name}");
            return true;
        }
        catch (Exception ex)
        {
            _log.LogError("DatabaseService", "Error deleting product", ex);
            return false;
        }
    }

    // --------------------------------
    // --- Gestion des fournisseurs ---
    // --------------------------------
    public async Task<List<Supplier>> GetSuppliersAsync()
    {
        var db = await GetConnectionAsync();
        return await db.Table<Supplier>().ToListAsync();
    }

    public async Task<bool> AddSupplierAsync(Supplier supplier)
    {
        try
        {
            var db = await GetConnectionAsync();
            await db.InsertAsync(supplier);
            _log.LogInfo("DatabaseService", $"Adding supplier: {supplier.Name}");
            return true;
        }
        catch (Exception ex)
        {
            _log.LogError("DatabaseService", "Error adding supplier", ex);
            return false;
        }
    }

    public async Task<bool> UpdateSupplierAsync(Supplier supplier)
    {
        try
        {
            var db = await GetConnectionAsync();
            await db.UpdateAsync(supplier);
            _log.LogInfo("DatabaseService", $"Updating supplier: {supplier.Name}");
            return true;
        }
        catch (Exception ex)
        {
            _log.LogError("DatabaseService", "Error updating supplier", ex);
            return false;
        }
    }

    public async Task<bool> DeleteSupplierAsync(Supplier supplier)
    {
        try
        {
            var db = await GetConnectionAsync();
            await db.DeleteAsync(supplier);
            _log.LogInfo("DatabaseService", $"Deleting supplier: {supplier.Name}");
            return true;
        }
        catch (Exception ex)
        {
            _log.LogError("DatabaseService", "Error deleting supplier", ex);
            return false;
        }
    }
}
