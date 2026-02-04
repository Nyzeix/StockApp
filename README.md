# StockApp
# Note
Les commits sont générés automatiquement via l'IA Copilot de Visual Studio.  

# TODO  
## Ajouter les base de données pour:  
- Produits
- Fournisseurs
- Type de produits
- Type de produits par fournisseurs (probablement lié au précédent)

## Ajouter l'historique de produits fournis par le fournisseur X:
- Conservé les produits (Nom, quantité, prix...) fourni par fournisseurs (BDD à nom variable (ex: historic_items_{supplier_name}))
- Historique de prix sur tel produit + graph ?


# TODO:
- Rework de l'interface de la page fournisseur pour qu'elle se rapproche de la page de gestion des produits.
- Finaliser le ViewModel de la page fournisseur (manque les fonctionnalités inaccessibles avec le format de la page actuelle. Se référer à la page des produits).
- Ajouter dans la Bdd les fonctions de logs de variation de produits (prix?, quantité, etc...) pour un suivi propre.
- Ajouter une page d'historique des modifications de produits avec un graph pour visualiser les variations.