using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfCours.MVVM.ViewModel
{
    public class MainWindowVM : BaseVM
    {

        // Event => On User request to change ViewModel & view
        static public  Action<BaseVM> OnRequestVMChange;

        #region Commands
        #endregion
        #region Variables

        /// <summary>
        /// this the current view model where the user stand
        /// </summary>
        private BaseVM _currentVM;
        public BaseVM CurrentVM
        {
            get => _currentVM;
            set
            {
                //come from CommunotyToolsKit => Notify binding variables to display
                // informations in the view
                SetProperty(ref _currentVM, value);
                OnPropertyChanged(nameof(CurrentVM));
            }
        }


     
        #endregion

        // Constructor of the mainWindowVM
        // Called on instanciate
        public MainWindowVM()
        {
            // Subscribe to HandleRequestViewChange
            // => Call the function when event is Invoke.
            MainWindowVM.OnRequestVMChange += HandleRequestViewChange;

            //Invoke the event with the newVM instancied
            MainWindowVM.OnRequestVMChange?.Invoke(new MainViewVM());

        }

        /// <summary>
        /// Called when Event Invoke (OnRequestVMChange)
        /// </summary>
        /// <param name="a_VMToChange"></param>
        public void HandleRequestViewChange(BaseVM a_VMToChange)
        {
            //Notify currentVM will be hide
            CurrentVM?.OnHideVM();

            // Assign the new VM
            CurrentVM = a_VMToChange;

            //Notify currentVM will be shown
            CurrentVM?.OnShowVM();
        }
      
    }
}
