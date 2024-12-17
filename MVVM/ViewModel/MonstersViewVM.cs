using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using WpfCours.Model;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using WpfCours.MVVM.View;

namespace WpfCours.MVVM.ViewModel
{
    public class MonstersViewVM : BaseVM
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public ObservableCollection<Monster> Monsters { get; set; }

        private Monster _selectedMonster;
        public Monster SelectedMonster
        {
            get => _selectedMonster;
            set
            {
                if (_selectedMonster != value)
                {
                    _selectedMonster = value;
                    OnPropertyChanged(nameof(SelectedMonster));
                    LoadAttacksAsync(_selectedMonster.Id);
                }
            }
        }
        private ObservableCollection<Spell> _attacks;

        public ObservableCollection<Spell> Attacks
        {
            get => _attacks;
            set
            {
                _attacks = value;
                OnPropertyChanged(nameof(Attacks));
            }
        }
    

        public ICommand SelectMonsterCommand { get; }
        public IRelayCommand SpecialActionCommand { get; }

        public MonstersViewVM()
        {
            LoadMonsters();
            SelectMonsterCommand = new RelayCommand<Monster>(HandleSelectMonster);
            Attacks = new ObservableCollection<Spell>();

            SpecialActionCommand = new RelayCommand(ExecuteSpecialAction);
        }

        private void ExecuteSpecialAction()
        {
            if (SelectedMonster != null)
            {
                SharedData.SelectedMonster = SelectedMonster;

                MainWindowVM.OnRequestVMChange?.Invoke(new FightVM());
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un Pokémon !");
            }
        }

       
        private void LoadMonsters()
        {
            using (var context = new ExerciceMonsterContext())
            {
                Monsters = new ObservableCollection<Monster>(context.Monsters.ToList());
            }
        }

        private void HandleSelectMonster(Monster monster)
        {
            SelectedMonster = monster;
        }

        private async void LoadAttacksAsync(int pokemonId)
        {
            Attacks.Clear();
            SharedData.SelectedMonsterAttacks.Clear();

            try
            {
                var response = await _httpClient.GetStringAsync($"https://pokeapi.co/api/v2/pokemon/{pokemonId}");
                var data = JsonConvert.DeserializeObject<PokemonApiResponse>(response);

                var moves = data.Moves.Take(4);

                foreach (var move in moves)
                {
                    var attackUrl = move.Move.Url;

                    var attackResponse = await _httpClient.GetStringAsync(attackUrl);
                    var attackDetails = JsonConvert.DeserializeObject<MoveInfo>(attackResponse);

                 
                    var spell = new Spell
                    {
                        Name = move.Move.Name,
                        Damage = attackDetails.Power ?? 0,
                    };

                    Attacks.Add(spell);
                    SharedData.SelectedMonsterAttacks.Add(spell); 
                }
            }
            catch
            {
                var errorSpell = new Spell { Name = "Erreur", Damage = 0 };
                Attacks.Add(errorSpell);
                SharedData.SelectedMonsterAttacks.Add(errorSpell);
            }
        }




    }



    // Classes pour désérialiser les données de l'API

    public class MoveDetail
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class MoveWrapper
    {
        [JsonProperty("move")]
        public MoveDetail Move { get; set; }
    }

 

    public class MoveInfo
    {
        [JsonProperty("power")]
        public int? Power { get; set; }
    }


    public class SelectedBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Brushes.LightGreen : Brushes.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class AttackInfo
    {
        public string Name { get; set; }
        public int Power { get; set; }
    }
}
