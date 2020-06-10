﻿// https://stackoverflow.com/questions/15526289/show-tick-image-when-item-is-selected-in-wpf-listbox

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GalleryOfHeartbeats
{
    public class BoolToVisibleOrHidden : IValueConverter
    {
        public BoolToVisibleOrHidden() { }
        public bool Collapse { get; set; }
        public bool Reverse { get; set; }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool bValue = (bool)value;
            if (bValue != Reverse)
            {
                return Visibility.Visible;
            }
            else
            {
                if (Collapse)
                    return Visibility.Collapsed;
                else
                    return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            if (visibility == Visibility.Visible)
                return !Reverse;
            else
                return Reverse;
        }
    }
}
