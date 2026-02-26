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
{
    public class DatabaseService : IDatabaseService
    {
        // On ne touche plus jamais à _businessDb directement en dehors de GetConnectionAsync
        private SQLiteAsyncConnection? _businessDb;
        private readonly ILogService _log;
        private Task? _initTask; // Pour gérer l'initialisation unique


        /// <summary>
        /// Constructeur de la classe DatabaseService. 
        /// Il prend en paramètre un service de log pour enregistrer les opérations effectuées sur la base de données.
        /// L'initialisation de la base de données est différée jusqu'à ce qu'une opération nécessitant une connexion soit effectuée,
        /// grâce à l'utilisation d'une tâche d'initialisation (_initTask) qui garantit que la base de données est initialisée une seule 
        /// fois et que toutes les opérations attendent que l'initialisation soit terminée avant d'accéder à la base de données.
        /// </summary>
        /// <param name="logService"></param>
        public DatabaseService(ILogService logService)
        {
            _log = logService;
        }

        
        /// <summary>
        /// Méthode pour obtenir une connexion à la base de données.
        /// Si la base de données n'est pas encore initialisée, elle l'initialise en appelant InitBusinessDb().
        /// Toutes les opérations qui nécessitent une connexion à la base de données doivent passer par cette méthode
        /// pour garantir que la base de données est prête avant d'être utilisée.
        /// </summary>
        /// <returns>SQLiteAsyncConnection</returns>
        /// <exception cref="Exception"></exception>
        private async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            if (_initTask == null || _initTask.IsFaulted)
            {
                _initTask = InitBusinessDb();
            }

            await _initTask; // On attend que la DB soit prête
            return _businessDb ?? throw new Exception("Database initialization failed.");
        }


        /// <summary>
        /// Méthode pour initialiser la base de données.
        /// Elle crée une connexion à la base de données SQLite, crée les tables nécessaires (Product, Supplier, User)
        /// si elles n'existent pas déjà, et stocke la connexion dans la variable _businessDb.
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// Méthode pour récupérer la liste des produits depuis la base de données.
        /// Elle retourne une liste de produits.
        /// </summary>
        /// <returns>Liste de produits</returns>
        public async Task<List<Product>> GetProductsAsync()
        {
            var db = await GetConnectionAsync();
            return await db.Table<Product>().ToListAsync();
        }


        /// <summary>
        /// Méthode pour récupérer un produit spécifique par son ID depuis la base de données.
        /// Elle retourne le produit correspondant à l'ID fourni, ou null si aucun produit n'est trouvé.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Un produit ou null si non trouvé</returns>
        public async Task<Product?> GetProductByIdAsync(int id)
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


        /// <summary>
        /// Méthode pour ajouter un nouveau produit à la base de données. 
        /// Elle prend en paramètre un objet Product, l'insère dans la base de données.
        /// </summary>
        /// <param name="product">Nouveau produit à ajouter</param>
        /// <returns>Booléen indiquant si l'ajout a réussi</returns>
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


        /// <summary>
        /// Méthode pour mettre à jour un produit existant dans la base de données.
        /// Elle prend en paramètre un objet Product modifié,
        /// met à jour les informations du produit dans la base de données.
        /// </summary>
        /// <param name="product">Produit modifié à mettre à jour</param>
        /// <returns>Booléen indiquant si la mise à jour a réussi</returns>
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

        /// <summary>
        /// Méthode pour supprimer un produit de la base de données.
        /// Elle prend en paramètre un objet Product à supprimer,
        /// le supprime de la base de données.
        /// </summary>
        /// <param name="product"></param>
        /// <returns>Booléen indiquant si la suppression a réussi</returns>
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


        /// <summary>
        /// Méthode pour récupérer la liste des fournisseurs depuis la base de données.
        /// </summary>
        /// <returns>Liste des fournisseurs</returns>
        public async Task<List<Supplier>> GetSuppliersAsync()
        {
            var db = await GetConnectionAsync();
            return await db.Table<Supplier>().ToListAsync();
        }


        /// <summary>
        /// Méthode pour ajouter un nouveau fournisseur à la base de données.
        /// Elle prend en paramètre un objet Supplier, l'insère dans la base de données.
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns>Booléen indiquant si l'ajout a réussi</returns>
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


        /// <summary>
        /// Méthode pour mettre à jour un fournisseur existant dans la base de données.
        /// Elle prend en paramètre un objet Supplier modifié,
        /// met à jour les informations du fournisseur dans la base de données.
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns>Booléen indiquant si la mise à jour a réussi</returns>
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


        /// <summary>
        /// Méthode pour supprimer un fournisseur de la base de données.
        /// Elle prend en paramètre un objet Supplier à supprimer,
        /// le supprime de la base de données.
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns>Booléen indiquant si la suppression a réussi</returns>
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
}