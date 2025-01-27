using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace WPFChosungComboBox
{
    public partial class ChosungComboBox : UserControl
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
                }
            }
        }


        public string[] ItemsSource { get; set; }


        public string Text
        {
            get
            {
                return comboBox?.Text2;
            }
            set
            {
                // Set text three times to display it to the end user.
                comboBox.Text = value;
                comboBox.Text = value;
                comboBox.Text = value;
            }
        }
        

        public ChosungComboBox()
        {
            InitializeComponent();
        }


        public event EventHandler<object> WriteLine;


        private void PWriteLine(object x)
        {
            WriteLine?.Invoke(this, x);
        }


        private void comboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            PWriteLine("ComboBox.PreviewKeyDown");

            comboBox.IsDropDownOpen = true;
        }

        public event EventHandler<FirstChanceExceptionEventArgs> FirstChanceException;

        private int GetScore(string keyword, string pattern, string itemSource)
        {
            bool isMatch = ChosungHelper.IsMatch(itemSource, pattern);
            if (isMatch)
            {
                return int.MaxValue - itemSource.Length;
            }
            else
            {
                return 0;
            }
        }

        public int MaxDropDownCount { get; set; } = 10;

        private string keyword;

        private string[] GetFilteredItems()
        {
            try
            {
                string text = keyword;
                if (text != null)
                {
                    string pattern = ChosungHelper.GetPattern(text);
                    PWriteLine("pattern: " + pattern);

                    Dictionary<string, int> scoreboard = new Dictionary<string, int>();

                    foreach (var itemSource in ItemsSource)
                    {
                        int score = GetScore(text, pattern, itemSource);
                        if (score > 0)
                        {
                            scoreboard[itemSource] = score;
                        }
                    }


                    string[] ret = scoreboard.OrderByDescending(x => x.Value).Take(MaxDropDownCount).Select(x => x.Key).ToArray();

                    return ret;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                FirstChanceException?.Invoke(this, new FirstChanceExceptionEventArgs(ex));
                return null;
            }
        }

        private void Filter()
        {
            comboBox.ItemsSource = GetFilteredItems();
        }


        public bool Separate { get; set; } = true;


        public string Cue
        {
            get
            {
                return textBlock.Text;
            }
            set
            {
                textBlock.Text = value;
            }
        }


        private void PART_EditableTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Separate)
            {
                ChosungHelper.Separate(comboBox.PART_EditableTextBox);
            }
            
            PWriteLine($"PART_EditableTextBox.Text: {comboBox.PART_EditableTextBox.Text}");

            if (comboBox.IsDropDownOpen)
            {
                keyword = comboBox.PART_EditableTextBox.Text;
            }
            
            Filter();

        }


        private void comboBox_PropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PWriteLine(
$"ComboBox.PropertyChanged: {e.Property} '{e.OldValue ?? "null"}' '{e.NewValue ?? "null"}'");
        }


        private void comboBox_KeyDown(object sender, KeyEventArgs e)
        {
            PWriteLine("ComboBox.KeyDown");
        }

        public event SelectionChangedEventHandler SelectionChanged;

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PWriteLine($"ComboBox.SelectionChanged: SelectedItem: {comboBox.SelectedItem ?? "null"}");
            SelectionChanged?.Invoke(sender, e);
        }

        public object SelectedItem
        {
            get => comboBox.SelectedItem;
            set => comboBox.SelectedItem = value;
        }

        public event EventHandler EnterKeyDown;

        public virtual void OnEnterKeyDown(object sender, EventArgs e)
        {
            PWriteLine($"OnEnterKeyDown({sender},{e})");

            string[] filtered = GetFilteredItems();
            if (filtered?.Length > 0)
            {
                string first = filtered[0];
                Text = first;
                comboBox.SelectedItem = first;
            }

            EnterKeyDown?.Invoke(sender, e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            PWriteLine($"OnKeyDown {e.Key}");
        }

        private Key lastPreviewKeyDown;

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            PWriteLine($"OnPreviewKeyDown {e.Key}");
            lastPreviewKeyDown = e.Key;
        }

        private void comboBox_DropDownClosed(object sender, EventArgs e)
        {
            PWriteLine("ComboBox.DropDownClosed");

            if (comboBox.IsSelectionBoxHighlighted)
            {
                if (!comboBox.IsMouseCaptureWithin)
                {
                    if (!comboBox.IsMouseCaptured)
                    {
                        if (lastPreviewKeyDown == Key.Enter)
                        {
                            OnEnterKeyDown(sender, e);
                        }
                    }
                }
            }
        }


        private void comboBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Now PART_EditableTextBox is initialized.
            comboBox.PART_EditableTextBox.TextChanged += PART_EditableTextBox_TextChanged;
        }

    }
}
