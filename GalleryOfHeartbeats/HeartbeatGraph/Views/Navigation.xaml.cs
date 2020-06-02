using GalleryOfHeartbeats.Models;
using GalleryOfHeartbeats.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GalleryOfHeartbeats.View
{
    /// <summary>
    /// Interaction logic for Navigation.xaml
    /// </summary>
    public partial class Navigation : Window
    {
        private NavigationViewModel NavigationViewModel = new NavigationViewModel();

        public Navigation()
        {
            InitializeComponent();
            DataContext = NavigationViewModel;
        }

        private void ButtonCloseApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void RecordingView_Clicked(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                NavigationViewModel.ChangeViewModel(ViewTypes.RECORDING);
            }
        }

        private void GalleryView_Clicked(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                NavigationViewModel.ChangeViewModel(ViewTypes.GALLERY);
            }
        }

        private void SettingsView_Clicked(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                NavigationViewModel.ChangeViewModel(ViewTypes.SETTINGS);
            }
        }

        private void AboutView_Clicked(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                NavigationViewModel.ChangeViewModel(ViewTypes.ABOUT);
            }
        }
    }
}
