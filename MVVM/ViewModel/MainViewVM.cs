using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfCours.Model;
using WpfCours.MVVM.View;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Reflection.Metadata;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Net.Http;

namespace WpfCours.MVVM.ViewModel
{
    public class MainViewVM : BaseVM
    {
        private string _nom;
        private string _email;
        private string _motDePasse;

        public string Nom
        {
            get { return _nom; }
            set
            {
                _nom = value;
                OnPropertyChanged("Name");
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged("Email");
            }
        }

        public string Pass
        {
            get { return _motDePasse; }
            set
            {
                _motDePasse = value;
                OnPropertyChanged("Pass");
            }
        }

       
        public ICommand RequestChangeViewCommand { get; set; }
        public ICommand BttnSignClick { get; set; }
        public ICommand BttnLoginClick { get; set; }


        public MainViewVM()
        {
            RequestChangeViewCommand = new RelayCommand(HandleRequestChangeViewCommand);
            BttnLoginClick = new RelayCommand(HandleButtonLoginCommand);
            BttnSignClick = new RelayCommand(HandleBttnSignClick);
            ConnectionString = "Server=LAPTOP-S0SM52DR\\SQLEXPRESS;Database=ExerciceMonster;Trusted_Connection=True;TrustServerCertificate=True;";

         
            InitializeDbCommand = new RelayCommand(InitializeDatabase);

            _pokemonService = new PokemonService();
            ImportPokemonCommand = new AsyncRelayCommand(ImportPokemonsAsync);
        }
        public IAsyncRelayCommand ImportPokemonCommand { get; }

        private async Task ImportPokemonsAsync()
        {
            try
            {
                await _pokemonService.AddPokemonsToDatabaseAsync(50);
                MessageBox.Show("50 Pokémon ont été ajoutés à la base de données avec succès !");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }
        private readonly PokemonService _pokemonService;

        public void HandleRequestChangeViewCommand()
        {

            MainWindowVM.OnRequestVMChange?.Invoke(new GameVM());
        }

        

        public void HandleBttnSignClick()
        {
            if (!string.IsNullOrEmpty(Nom) && !string.IsNullOrEmpty(Pass))
            {
                try
                {
                    using (var context = new ExerciceMonsterContext())
                    {
                    
                        var existingUser = context.Logins.FirstOrDefault(u => u.Username == Nom);
                        if (existingUser != null)
                        {
                            MessageBox.Show("Email already registered!");
                            return;
                        }

                        var hashedPassword = HashPassword(Pass);

                   
                        var newUser = new Login
                        {
                            Username = Nom,
                            PasswordHash = hashedPassword
                        };

                    
                        context.Logins.Add(newUser);
                        context.SaveChanges();

                        MessageBox.Show($"Register confirmed! Welcome {Nom}!");
                        HandleRequestChangeViewCommand();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("All fields must be filled!");
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public void HandleButtonLoginCommand()
        {
            if (!string.IsNullOrEmpty(Nom) && !string.IsNullOrEmpty(Pass))
            {
                try
                {
                    using (var context = new ExerciceMonsterContext())
                    {
                        
                        var hashedPassword = HashPassword(Pass);

                       
                        var user = context.Logins.FirstOrDefault(u =>
                            u.Username == Nom && u.PasswordHash == hashedPassword);

                        if (user != null)
                        {
                          
                            HandleRequestChangeViewCommand();
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Both username and password fields must be filled!");
            }
        }


        private string _connectionString;

        public string ConnectionString
        {
            get => _connectionString;
            set
            {
                _connectionString = value;
                OnPropertyChanged(nameof(ConnectionString));
            }
        }

        public ICommand InitializeDbCommand { get; set; }



        private void InitializeDatabase()
        {
            try
            {
                if (string.IsNullOrEmpty(ConnectionString))
                {
                    MessageBox.Show("Veuillez entrer une URL de connexion valide !");
                    return;
                }

               
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    MessageBox.Show("Connexion réussie à la base de données !");
                }

            
                if (ExecuteDatabaseInitialization() ==1)
                {
                    MessageBox.Show("Base de données initialisée avec succès !");

                };

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }

        private int ExecuteDatabaseInitialization()
        {
            try
            {
                string script = @"
                CREATE DATABASE ExerciceMonster;
                
                -- Utilisation de la base de données
                USE ExerciceMonster;
                
                -- Table Login
                CREATE TABLE Login (
                 ID INT PRIMARY KEY IDENTITY(1,1),
                 Username NVARCHAR(50) NOT NULL,
                 PasswordHash NVARCHAR(255) NOT NULL
                );
                -- Table Player
                CREATE TABLE Player (
                 ID INT PRIMARY KEY IDENTITY(1,1),
                 Name NVARCHAR(50) NOT NULL,
                 LoginID INT,
                 FOREIGN KEY (LoginID) REFERENCES Login(ID)
                );
                -- Table Monster
                CREATE TABLE Monster (
                 ID INT PRIMARY KEY IDENTITY(1,1),
                 Name NVARCHAR(50) NOT NULL,
                 Health INT NOT NULL,
                 ImageUrl NVARCHAR(255) NULL
                );
                -- Table Spell
                CREATE TABLE Spell (
                 ID INT PRIMARY KEY IDENTITY(1,1),
                 Name NVARCHAR(50) NOT NULL,
                 Damage INT NOT NULL,
                 Description NVARCHAR(MAX)
                );
                -- Table PlayerMonster (relation Player <-> Monster)
                CREATE TABLE PlayerMonster (
                 PlayerID INT NOT NULL,
                 MonsterID INT NOT NULL,
                 PRIMARY KEY (PlayerID, MonsterID),
                 FOREIGN KEY (PlayerID) REFERENCES Player(ID),
                 FOREIGN KEY (MonsterID) REFERENCES Monster(ID)
                );
                -- Table MonsterSpell (relation Monster <-> Spell)
                CREATE TABLE MonsterSpell (
                 MonsterID INT NOT NULL,
                 SpellID INT NOT NULL,
                 PRIMARY KEY (MonsterID, SpellID),
                 FOREIGN KEY (MonsterID) REFERENCES Monster(ID),
                 FOREIGN KEY (SpellID) REFERENCES Spell(ID)
                );
            ";

                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(script, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'initialisation de la base de données : {ex.Message}");
                return 0;
            }



        }
    }

    public class PokemonApiResponse
    {
        public string Name { get; set; }

        [JsonProperty("sprites")] public Sprites Sprites { get; set; }

        [JsonProperty("stats")] public List<StatWrapper> Stats { get; set; }
        public List<MoveWrapper> Moves { get; set; }


    }

    public class Sprites
    {
        [JsonProperty("front_default")] public string FrontDefault { get; set; }
    }

    public class StatWrapper
    {
        [JsonProperty("base_stat")] public int BaseStat { get; set; }

        [JsonProperty("stat")] public Stat Stat { get; set; }
    }

    public class Stat
    {
        public string Name { get; set; }
    }

    public class PokemonService
    {
        private readonly HttpClient _httpClient;

        public PokemonService()
        {
            _httpClient = new HttpClient();
        }

        // Récupérer et transformer les Pokémon
        public async Task<List<Monster>> FetchPokemonsAsync(int count)
        {
            var monsters = new List<Monster>();

            for (int i = 1; i <= count; i++)
            {
                try
                {
                    // Récupérer les données de base du Pokémon
                    var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{i}");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<PokemonApiResponse>(json);

                        // Extraire les informations
                        var hpStat = data.Stats.FirstOrDefault(s => s.Stat.Name == "hp")?.BaseStat ?? 50;

                        var monster = new Monster
                        {
                            Name = data.Name,
                            Health = hpStat,
                            ImageUrl = data.Sprites?.FrontDefault,
                            Spells = new List<Spell>() // Liste pour stocker les attaques
                        };

                        // Ajouter les 4 premières attaques
                        foreach (var move in data.Moves.Take(4))
                        {
                            var spell = new Spell
                            {
                                Name = move.Move.Name,
                                Damage = new Random().Next(10, 50), // Dégâts aléatoires
                                Description = $"Attaque {move.Move.Name}"
                            };

                            monster.Spells.Add(spell);
                        }

                        monsters.Add(monster);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du traitement du Pokémon {i}: {ex.Message}");
                }
            }

            return monsters;
        }

        // Ajouter les Pokémon à la base de données
        public async Task AddPokemonsToDatabaseAsync(int count)
        {
            var monsters = await FetchPokemonsAsync(count);

            using (var context = new ExerciceMonsterContext())
            {
                foreach (var monster in monsters)
                {
                    // Ajouter les attaques (spells) si elles n'existent pas déjà
                    foreach (var spell in monster.Spells)
                    {
                        var existingSpell = context.Spells.FirstOrDefault(s => s.Name == spell.Name);
                        if (existingSpell == null)
                        {
                            context.Spells.Add(spell);
                        }
                        else
                        {
                            spell.Id = existingSpell.Id; // Utiliser l'ID existant
                        }
                    }

                    // Ajouter le monstre avec ses attaques
                    context.Monsters.Add(monster);
                }

                context.SaveChanges();
            }
        }
    }
}
