using Microsoft.Extensions.Logging.Abstractions;
using SQLite;
using StockApp.Models;

namespace StockApp.Services
{ }
public interface IDatabaseService
{
    //Task<bool> InitBusinessDb();

    // Initialisation de la base Logs
    //Task<bool> InitLogsDb();


    // Gestion des produits
    public Task<List<Product>> GetProductsAsync();
    public Task<Product> GetProductByIdAsync(int id);
    public Task<bool> AddProductAsync(Product product);
    public Task<bool> UpdateProductAsync(Product product);
    public Task<bool> DeleteProductAsync(Product product);

    // Gestion des fournisseurs
    public Task<List<Supplier>> GetSuppliersAsync();
    public Task<bool> AddSupplierAsync(Supplier supplier);
    public Task<bool> UpdateSupplierAsync(Supplier supplier);
    public Task<bool> DeleteSupplierAsync(Supplier supplier);

}