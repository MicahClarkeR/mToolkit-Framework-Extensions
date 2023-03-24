using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static mToolkitFrameworkExtensions.Windows.VariableUserControl;

namespace mToolkitFrameworkExtensions.Windows
{
    /// <summary>
    /// Interaction logic for VariableWindow.xaml
    /// </summary>
    public partial class VariableWindow : Window
    {
        public bool Success
        {
            get
            {
                return Input.Success;
            }
        }
        public string Value
        {
            get { return Input.Value; }
        }

        public bool Confirmed { get; private set; } = false;

        public VariableWindow(string? value = null, string? title = null, List<string>? options = null, bool? invertOptions = null)
        {
            InitializeComponent();
            Input.Initalise(value, options, invertOptions);
            Input.textBox.Focus();
            Input.UserConfirmed += Button_Click;

            Label.Content = (!string.IsNullOrEmpty(title)) ? title : Label.Content;

            if (invertOptions == true)
                Label.Content = $"{Label.Content} (Unique)";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Input.Success)
            {
                Confirmed = true;
                e.Handled = true;
                Close();
            }

        }
    }
}
