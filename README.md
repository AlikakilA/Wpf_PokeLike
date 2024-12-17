#### PROBLEMES
Depuis quelques jours j'ai un problème git qui nécessite que je réinstalle tout mon PC (git fork bomb). Je n'ai pas voulus le faire avant de finir le projet.
Donc je n'ai pas pu push, il manque les dossiers bin et obj, nécessaires au lancement du projet.




Description de l'application
Cette application est un simulateur de combat Pokémon développé en WPF (Windows Presentation Foundation) avec le pattern MVVM. Les fonctionnalités principales incluent :

Sélection d'un Pokémon :

L'utilisateur peut choisir un Pokémon parmi une liste affichée.
Les informations détaillées du Pokémon sélectionné (nom, PV, attaques) s'affichent.
Combat Pokémon :

Une fois le Pokémon sélectionné, un combat est simulé contre un adversaire aléatoire.
Chaque Pokémon possède des attaques avec des dégâts spécifiques récupérés depuis l'API PokeAPI.
Interface utilisateur :

Affichage dynamique des PV via des barres de vie.
Boutons pour lancer des attaques.
Possibilité de poursuivre le combat avec un nouvel adversaire ou de retourner au menu de sélection.
Intégration API :

L'application utilise PokeAPI pour récupérer les informations des Pokémon (nom, image, attaques et dégâts).
Base de données locale :

Les données des Pokémon et attaques sont stockées dans une base de données locale SQL Server.
Les données sont prises depuis l'API Pokemon.
L'initialisation de la base de données se fait via un bouton.

Prérequis
1. Environnement de développement
Visual Studio (Version 2019 ou plus récente)
.NET 6.0 ou supérieur
2. Modules et Packages
Les packages suivants doivent être installés dans le projet WPF. Vous pouvez utiliser NuGet Package Manager pour les ajouter :

Microsoft.EntityFrameworkCore.SqlServer

Utilisé pour la connexion à la base de données SQL Server.
bash
Copier le code
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Newtonsoft.Json

Utilisé pour désérialiser les réponses JSON de l'API PokeAPI.
bash
Copier le code
Install-Package Newtonsoft.Json
CommunityToolkit.Mvvm

Utilisé pour gérer le pattern MVVM (commandes, notifications de propriété, etc.).
bash
Copier le code
Install-Package CommunityToolkit.Mvvm
Microsoft.Data.SqlClient

Bibliothèque pour la connexion SQL Server.
bash
Copier le code
Install-Package Microsoft.Data.SqlClient

Instructions d'installation
1. Cloner le projet
Clonez ce dépôt Git sur votre machine locale :

bash
Copier le code
git clone https://github.com/votre-repository/pokemon-combat-simulator.git
2. Configuration de la base de données
SQL Server LocalDB ou SQL Server Express : Assurez-vous qu'un serveur SQL Server local est installé.

Configurez votre chaîne de connexion dans la page de settings et importer des pokemons de l'api.

csharp
Copier le code
optionsBuilder.UseSqlServer("Server=VOTRE_SERVEUR;Database=ExerciceMonster;Trusted_Connection=True;TrustServerCertificate=True;");
Initialisation de la base de données :

Lancez l'application.
Cliquez sur le bouton "Initialiser la base de données" pour créer les tables nécessaires.
