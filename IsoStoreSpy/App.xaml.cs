using IsoStoreSpy.Plugins;
using IsoStoreSpy.ViewModels;

using System.Windows;

namespace IsoStoreSpy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            IsoStoreSpyViewModel = new IsoStoreSpyViewModel();

            IsoStoreSpyViewModel.Plugins.Add(new PreviewText());
            IsoStoreSpyViewModel.Plugins.Add(new PreviewImage());
            IsoStoreSpyViewModel.Plugins.Add(new PreviewDatabase());
        }

        /// <summary>
        /// ViewModel
        /// </summary>

        public static IsoStoreSpyViewModel IsoStoreSpyViewModel
        {
            get;
            private set;
        }

        /// <summary>
        /// Affichage d'un MessageBox
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <param name="icon"></param>
        /// <returns></returns>

        public static MessageBoxResult ShowMessageBox(string message, MessageBoxButton buttons, MessageBoxImage icon)
        {
            return MessageBox.Show(message, "IsoStoreSpy", buttons, icon);
        }

        /// <summary>
        /// Affichage d'une question
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>

        public static MessageBoxResult ShowQuestion(string message)
        {
            return ShowMessageBox(message, MessageBoxButton.OKCancel, MessageBoxImage.Question);
        }

        /// <summary>
        /// Erreur
        /// </summary>
        /// <param name="message"></param>

        public static void ShowError(string message)
        {
            ShowMessageBox(message, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
