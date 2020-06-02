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


        private ObservableCollection<GalleryItem> galleryItems = new ObservableCollection<GalleryItem>();
        public ObservableCollection<GalleryItem> GalleryItems
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

        public bool SelectedItemIsTheSameAs(string itemName)
        {
            Console.WriteLine(GetIdOf(this.SelectedItem) + itemName);
            return GetIdOf(this.SelectedItem).Equals(itemName);
        }

        public void SetSelectedItemById(string id)
        {
            if (string.IsNullOrEmpty(id)) {
                Console.WriteLine("ID is empty");
                SelectedItem = new GalleryItem();
            }
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
            if (HasNoMoreData(idx)) {
                SelectedItem.Progress = 100;
                return 0;
            }
            //SetProgressPercentage(idx);
            return SelectedItem.Data[idx];
        }

        private void SetProgressPercentage(int idx)
        {
                SelectedItem.Progress = (int)Math.Round((double)idx / SelectedItem.Data.Count * 100);
            
        }

        public bool HasNoMoreData(int idx)
        {
            return SelectedItem.Data != null && idx >= SelectedItem.Data.Count;
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
            return item.Name + "\n" + item.TimeOfRecording;
        }

    }


}
