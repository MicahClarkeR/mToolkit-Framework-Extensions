using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace mToolkitFrameworkExtensions
{
    public static class ContextMenuExt
    {
        public static MenuItem CreateMenuItem(ItemsControl control, string header, RoutedEventHandler? callback = null)
        {
            MenuItem item = CreateMenuItem(header, callback);

            control.Items.Add(item);

            return item;
        }

        public static MenuItem CreateMenuItem(string header, RoutedEventHandler? callback = null)
        {
            MenuItem item = new MenuItem
            {
                Header = header
            };

            if (callback != null)
                item.Click += callback;

            return item;
        }
    }
}
