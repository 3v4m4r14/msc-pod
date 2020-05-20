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
        }

        public Gallery(GalleryStruct galleryStruct)
        {
            GalleryItems = galleryStruct.GalleryItems;
        }


        private List<GalleryItem> galleryItems = new List<GalleryItem>();
        public List<GalleryItem> GalleryItems
        {
            get
            {
                return galleryItems;
            }
            private set
            {
                galleryItems = value;
            }
        }

        public GalleryItem SelectedItem = new GalleryItem();
        public string SelectedItemName
        {
            get
            {
                return SelectedItem.Name;
            }
            set
            {
                SetSelectedItemById(value);
            }
        }

        public void SetSelectedItemById(string id)
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
                        SelectedItemName = SelectedItem.Name;
                        break;
                    }
                }
            }         
        }

        public int GetSelectedItemDataValAt(int idx)
        {
            if (idx >= SelectedItem.Data.Count) { return 0; }
            return SelectedItem.Data[idx];
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

    }


}
