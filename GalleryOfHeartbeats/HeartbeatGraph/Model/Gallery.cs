using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryOfHeartbeats.Model
{
    class Gallery
    {
        public Gallery()
        {
            GalleryItems = new List<GalleryItem>();
        }

        public Gallery(GalleryStruct galleryStruct)
        {
            GalleryItems = galleryStruct.GalleryItems;
        }


        private List<GalleryItem> galleryItems { get; set; }
        public List<GalleryItem> GalleryItems
        {
            get
            {
                return galleryItems;
            }
            set
            {
                galleryItems = value;
                ChangeProperty("HeartbeatOptions");
            }
        }

        private GalleryItem SelectedItem = new GalleryItem();
        public string SelectedItemName
        {
            get
            {
                return SelectedItem.Name;
            }
            set
            {
                SetSelectedItemById(value);
                ChangeProperty("SelectedItemName");
            }
        }

        private void SetSelectedItemById(string id)
        {
            if (String.IsNullOrEmpty(id)) {
                Console.WriteLine("ID is empty");
                SelectedItem = new GalleryItem(); }
            else
            {
                foreach (GalleryItem item in galleryItems)
                {
                    if (GetIdOf(item).Equals(id))
                    {
                        SelectedItem = item;
                        break;
                    }
                }
            }
            
        }

        public void RemoveSelectedItem()
        {
            SelectedItem = new GalleryItem();
        }

        public ObservableCollection<string> GetItemsAsStrings()
        {
            ObservableCollection<string> itemsAsStrings = new ObservableCollection<string>();
            foreach (GalleryItem item in galleryItems)
            {
                itemsAsStrings.Add(GetIdOf(item));
            }
            return itemsAsStrings;
        }

        private static string GetIdOf(GalleryItem item)
        {
            return item.Name + " " + item.TimeOfRecording;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void ChangeProperty(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }


}
