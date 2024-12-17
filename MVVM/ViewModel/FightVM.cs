using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfCours.Model;

namespace WpfCours.MVVM.ViewModel
{
    class FightVM : BaseVM
    {
        public string SelectedPokemonName { get; set; }
    
        public ObservableCollection<Spell> MyPokemonAttacks { get; set; }

        public IRelayCommand<Spell> AttackCommand { get; }

        public Monster MyPokemon { get; set; }
        public Monster OpponentPokemon { get; set; }

        public int MyPokemonHealth { get; set; }
        public int OpponentHealth { get; set; }
        public int MyPokemonMaxHealth { get; set; }
        public int OpponentMaxHealth { get; set; }



        public FightVM()
        {

            MyPokemon = SharedData.SelectedMonster;
            MyPokemonAttacks = new ObservableCollection<Spell>(SharedData.SelectedMonsterAttacks);
            MyPokemonMaxHealth = MyPokemon.Health;

            MyPokemonHealth = MyPokemon.Health;

           
            LoadRandomOpponent();

            AttackCommand = new RelayCommand<Spell>(ExecuteAttack);
        }

        private void LoadRandomOpponent()
        {
            using (var context = new ExerciceMonsterContext())
            {
                var random = new Random();
                var allMonsters = context.Monsters.Where(m => m.Id != MyPokemon.Id).ToList();
                if (allMonsters.Count > 0)
                {
                    OpponentPokemon = allMonsters[random.Next(allMonsters.Count)];
                    OpponentHealth = OpponentPokemon.Health;
                    OpponentMaxHealth = OpponentPokemon.Health; // PV totaux

                    OnPropertyChanged(nameof(OpponentPokemon));
                    OnPropertyChanged(nameof(OpponentHealth));
                    OnPropertyChanged(nameof(OpponentMaxHealth));
                }
            }
        }

        private void ExecuteAttack(Spell spell)
        {
            if (spell == null) return;

            
            OpponentHealth -= spell.Damage;
            OnPropertyChanged(nameof(OpponentHealth));


            if (OpponentHealth <= 0)
            {
                
                var result = MessageBox.Show(
                    $"{OpponentPokemon.Name} est K.O. ! Vous avez gagné !\nVoulez-vous continuer à jouer ?",
                    "Victoire !",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                   
                    LoadRandomOpponent();
                    MessageBox.Show("Un nouvel adversaire apparaît !");
                }
                else
                {
             
                    MainWindowVM.OnRequestVMChange?.Invoke(new MonstersViewVM());
                }

                return;
            }

        
            var random = new Random();
            int counterAttackDamage = random.Next(10, 20);
            MyPokemonHealth -= counterAttackDamage;
            OnPropertyChanged(nameof(MyPokemonHealth));

            if (MyPokemonHealth <= 0)
            {
                MessageBox.Show($"{MyPokemon.Name} est K.O. ! Vous avez perdu !");
                MainWindowVM.OnRequestVMChange?.Invoke(new MonstersViewVM());
            }
        }


    }
    public static class SharedData
    {
        public static Monster SelectedMonster { get; set; }
        public static ObservableCollection<Spell> SelectedMonsterAttacks { get; set; } = new ObservableCollection<Spell>();
    }
}
