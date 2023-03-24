using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace mToolkitFrameworkExtensions
{
    public static class WpfEtx
    {
        public static void ReplaceWith(this FrameworkElement elementToReplace, FrameworkElement newControl)
        {
            newControl.Width = elementToReplace.Width;
            newControl.Height = elementToReplace.Height;
            newControl.Margin = elementToReplace.Margin;

            // get parent of control
            var parent = elementToReplace.Parent;

            if (parent is Panel)
            {
                var panel = (Panel)parent;

                for (var i = 0; i < panel.Children.Count; i++)
                {
                    if (panel.Children[i] == elementToReplace)
                    {
                        panel.Children.RemoveAt(i);
                        panel.Children.Insert(i, newControl);
                        break;
                    }
                }
            }
            else if (parent is Decorator)
            {
                ((Decorator)parent).Child = newControl;
            }
            else if (parent is ContentControl)
            {
                ((ContentControl)parent).Content = newControl;
            }
            else
            {
                if (Debugger.IsAttached) Debugger.Break();
                throw new NotImplementedException("Missing other possibilities to implement");
            }
        }
    }
}
