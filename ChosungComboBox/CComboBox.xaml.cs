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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChosungComboBox
{
    public partial class CComboBox : UserControl
    {

        private bool isDropDownOpen;
        public bool IsDropDownOpen
        {
            get => isDropDownOpen;
            set
            {
                if (isDropDownOpen != value)
                {
                    isDropDownOpen = value;
                    OnIsDropDownOpenChanged();
                }
            }
        }

        public virtual void OnIsDropDownOpenChanged()
        {
        }

        public string[] ItemsSource { get; set; }


        public string Text
        {
            get
            {
                return comboBox.Text;
            }
            set
            {
                // Set text three times to display it to the end user.
                comboBox.Text = value;
                comboBox.Text = value;
                comboBox.Text = value;
            }
        }


        public CComboBox()
        {
            InitializeComponent();
        }


        private void comboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (log)
            {
                Console.WriteLine("ComboBox.PreviewKeyDown");
            }

            comboBox.IsDropDownOpen = true;
        }

        private string[] GetFilteredItems()
        {
            string text = comboBox.Text;
            string pattern = ChosungHelper.GetPattern(text);
            string[] filtered = ItemsSource.Where(x => ChosungHelper.IsMatch(x, pattern)).Take(10).ToArray();
            return filtered;
        }

        private void Filter()
        {
            comboBox.ItemsSource = GetFilteredItems();
        }


        private bool textChangedFlag;


        private void comboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textChangedFlag)
            {
                return;
            }

            textChangedFlag = true;

            textChangedFlag = false;
        }


        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            log = false;
        }

        private bool log;

        private void comboBox_PropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (log)
            {
                Console.WriteLine(
$"{DateTime.Now:ss.fff} ComboBox.PropertyChanged {e.Property} '{e.OldValue ?? "null"}' '{e.NewValue ?? "null"}'");
            }

            if(e.Property == ComboBox.TextProperty)
            {
                Filter();
            }
            else if(e.Property == ComboBox.IsDropDownOpenProperty)
            {
                if (e.OldValue.Equals(true))
                {
                    if (e.NewValue.Equals(false))
                    {
                        
                    }
                }
            }
        }

        private void comboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (log)
            {
                Console.WriteLine("ComboBox.KeyDown");
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (log)
            {
                Console.WriteLine("ComboBox.SelectionChanged");
            }
        }

        public event EventHandler EnterKeyDown;

        public virtual void OnPossibleEnterKeyDown(object sender, EventArgs e)
        {
            string[] filtered = GetFilteredItems();
            if (filtered.Length > 0)
            {
                string first = filtered[0];
                Text = first;
            }

            EnterKeyDown?.Invoke(sender, e);
        }

        private void comboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBox.IsSelectionBoxHighlighted)
            {
                if (!comboBox.IsMouseCaptureWithin)
                {
                    if (!comboBox.IsMouseCaptured)
                    {
                        OnPossibleEnterKeyDown(sender, e);
                        
                    }
                }
            }
        }

    }
}
