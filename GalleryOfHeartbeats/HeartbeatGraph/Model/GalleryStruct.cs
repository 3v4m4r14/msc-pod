// Gallery of Heartbeats
// Author: Eva Maria Veitmaa
// Date: 2020

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryOfHeartbeats.Model
{
    class GalleryStruct
    {
        public List<GalleryItem> GalleryItems { get; set; }

        public GalleryStruct()
        {
            GalleryItems = new List<GalleryItem>();
        }

        public GalleryStruct(Gallery gallery)
        {
            GalleryItems = gallery.GalleryItems;
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
