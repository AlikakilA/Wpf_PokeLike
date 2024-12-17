using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.IdentityModel.Tokens;

namespace WpfCours.MVVM.ViewModel
{
    public class User : BaseVM
    {
        private string _nom;
        private string _email;
        private string _password;

        public string Nom
        {
            get { return _nom; }
            set
            {
                _nom = value;
                OnPropertyChanged("Nom");
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
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Pass");
            }
        }


    }

}

