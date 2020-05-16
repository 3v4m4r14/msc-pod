using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryOfHeartbeats.Model
{
    class Gallery
    {
        public List<GalleryItem> GalleryItems { get; set; }

        public Gallery()
        {
            GalleryItems = new List<GalleryItem>();
        }

        public override string ToString()
        {
            string asString = "";
            foreach(GalleryItem item in GalleryItems)
            {
                asString += item.ToString() + "\n";
            }
            return asString;
        }
    }
}
