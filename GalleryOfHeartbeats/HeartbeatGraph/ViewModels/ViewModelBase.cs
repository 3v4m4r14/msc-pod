using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryOfHeartbeats.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public virtual void OnLoad()
        {
            Console.WriteLine("Default onload");
        }
        public virtual void OffLoad()
        {
            Console.WriteLine("Default offload");
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
