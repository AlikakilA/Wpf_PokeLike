using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
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
using System.Net.Http;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WpfCours.Model;
using System.Linq;
using System.Windows;

namespace WpfCours.MVVM.ViewModel
{


    public class SettingsViewVM : BaseVM
    {

        private readonly PokemonService _pokemonService;

        public SettingsViewVM()
        {
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
    }
  
}