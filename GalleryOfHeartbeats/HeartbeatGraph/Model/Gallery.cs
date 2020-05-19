using System;
using System.Collections.Generic;
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

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void ChangeProperty(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }


}
