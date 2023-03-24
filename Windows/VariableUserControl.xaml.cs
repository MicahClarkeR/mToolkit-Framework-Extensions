using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace mToolkitFrameworkExtensions.Windows
{
    /// <summary>
    /// Interaction logic for VariableUserControl.xaml
    /// </summary>
    public partial class VariableUserControl : UserControl
    {
        public static bool CreateWindow(ref string value, string? title = null, List<string>? options = null, bool? invertOptions = null)
        {
            VariableWindow chooser = new VariableWindow(value, title, options, invertOptions);
            chooser.ShowDialog();

            if (!chooser.Confirmed)
                return false;

            value = chooser.Value;
            return true;
        }

        public static VariableUserControl CreateControl(string? value = null, List<string>? options = null, bool? invertOptions = null)
        {
            VariableUserControl control = new VariableUserControl(value, options, invertOptions);

            if (value != null)
                control.textBox.Text = value;

            return control;
        }

        public bool Success = false;
        public bool InvertOptions;
        public string Value;
        public readonly Model CurrentModel;
        public RoutedEventHandler UserConfirmed;

        public VariableUserControl() : this(null, null, null)
        {
        }

        public VariableUserControl(string? value, List<string>? options, bool? invertOptions = null)
        {
            CurrentModel = new Model();
            InitializeComponent();
            Initalise(value, options, invertOptions);
        }

        public VariableUserControl(string value) : this(value, null, null)
        {

        }

        public void Initalise(string? value, List<string>? options, bool? invertOptions = null)
        {
            InvertOptions = invertOptions ?? false;

            if (options != null)
            {
                CurrentModel.Values = options;
            }
            else
                Success = true;

            Value = value ?? string.Empty;
            textBox.Text = value ?? string.Empty;
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            border.Visibility = System.Windows.Visibility.Collapsed;
        }

        private static int SelectedChild = -1;
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                var border = (resultStack.Parent as ScrollViewer).Parent as Border;
                border.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }

            if (e.Key == Key.Enter)
            {
                if (CurrentModel.Values.Count != 0)
                {
                    e.Handled = CurrentModel.Values.Contains(textBox.Text);

                    if (!e.Handled)
                        textBox.Focus();
                }

                UserConfirmed.Invoke(this, e);
                Success = true;
                OldValue = textBox.Text;
                Value = textBox.Text;
                var border = (resultStack.Parent as ScrollViewer).Parent as Border;
                border.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }

            if (!InvertOptions && resultStack.Children.Count != 0)
            {
                bool changed = false;
                if (e.Key == Key.Down)
                {
                    changed = true; SelectedChild++;
                }
                else if (e.Key == Key.Up)
                {
                    changed = true; SelectedChild--;
                }

                if (changed)
                {
                    if (SelectedChild >= 0 && SelectedChild < resultStack.Children.Count)
                        SetCursorTo(resultStack.Children[SelectedChild]);
                    else
                        SelectedChild = -1;
                    return;
                }
            }

            if (CurrentModel.Values.Count != 0)
            {
                bool found = false;
                var border = (resultStack.Parent as ScrollViewer).Parent as Border;
                var data = CurrentModel.Values;

                string query = (sender as TextBox).Text;

                if (!SearchQuery(query))
                {
                    resultStack.Children.Add(new TextBlock() { Text = (!InvertOptions) ? "No results found." : "Valid value."});
                }

                if(InvertOptions && !data.Contains(query))
                {
                    Success = true;
                    Value = textBox.Text;
                }
            }
            else
            {
                Success = true;
                Value = textBox.Text;
            }
        }

        private void SetCursorTo(UIElement block)
        {
            // Get the position of the element
            Point position = block.PointToScreen(new Point(0, 0));

            // Set the mouse position
            SetCursorPos((int)((int) position.X + block.RenderSize.Width / 2), (int)((int)position.Y + block.RenderSize.Height / 2));
        }

        private bool SearchQuery(string query)
        {
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            border.Visibility = System.Windows.Visibility.Collapsed;
            var data = CurrentModel.Values;
            bool found = false;

            if (data.Count != 0)
            {
                // Clear the list
                resultStack.Children.Clear();

                if (query.Length == 0)
                {
                    border.Visibility = System.Windows.Visibility.Visible;

                    if (InvertOptions && data.Count > 0)
                        addItem("Value cannot be:");

                    // Add the result
                    foreach (var obj in data)
                    {
                        addItem(obj);
                    }

                    found = true;
                }
                else
                {
                    border.Visibility = System.Windows.Visibility.Visible;

                    // Add the result
                    foreach (var obj in data)
                    {
                        if (obj.ToLower().StartsWith(query.ToLower()))
                        {
                            if(InvertOptions && resultStack.Children.Count == 0)
                                    addItem("Value cannot be:");

                            // The word starts with this... Autocomplete must work
                            addItem(obj);
                            found = true;
                        }
                    }
                }
            }

            return found;
        }

        private static string OldValue = "";
        private void addItem(string text)
        {
            TextBlock block = new TextBlock();

            // Add the text
            block.Text = text;

            // A little style...
            block.Margin = new Thickness(2, 3, 2, 3);
            block.Cursor = Cursors.Hand;

            // Mouse events
            block.MouseLeftButtonDown += (sender, e) =>
            {
                textBox.Text = (sender as TextBlock).Text;

                if (!InvertOptions)
                {
                    Success = true;
                    OldValue = textBox.Text;
                    Value = textBox.Text;
                }

                var border = (resultStack.Parent as ScrollViewer).Parent as Border;
                border.Visibility = System.Windows.Visibility.Collapsed;
            };

            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.PeachPuff;

                if (!InvertOptions)
                {
                    OldValue = textBox.Text;
                    textBox.Text = b.Text;
                    textBox.CaretIndex = textBox.Text.Length;
                    SelectedChild = resultStack.Children.IndexOf(b);
                }
            };

            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;

                if (!InvertOptions)
                {
                    textBox.Text = OldValue;
                    textBox.CaretIndex = textBox.Text.Length;
                    SelectedChild = -1;
                }
            };

            // Add to the panel
            resultStack.Children.Add(block);
        }


        public class Model
        {
            public List<string> Values = new List<string>();
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchQuery(textBox.Text);
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            border.Visibility = System.Windows.Visibility.Collapsed;
        }

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);
    }
}
