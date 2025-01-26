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
                if (PART_EditableTextBox != null)
                {
                    return PART_EditableTextBox.Text;
                }
                else if(comboBox != null)
                {
                    return comboBox.Text;
                }
                else
                {
                    return null;
                }
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
            Log = false;
            InitializeComponent();
        }

        private void WriteLine(object x)
        {
            if (Log)
            {
                Console.WriteLine($"{DateTime.Now:mm:ss.fff} {x}");
            }
        }


        private void comboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Log)
            {
                WriteLine("ComboBox.PreviewKeyDown");
            }

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

        private string[] GetFilteredItems()
        {
            try
            {
                string text = Text;

                string pattern = ChosungHelper.GetPattern(text);

                Dictionary<string, int> scoreboard = new Dictionary<string, int>();

                foreach (var itemSource in ItemsSource)
                {
                    int score = GetScore(text, pattern, itemSource);
                    if (score > 0)
                    {
                        scoreboard[itemSource] = score;
                    }
                }


                string[] ret = scoreboard.OrderByDescending(x => x.Value).Take(MaxDropDownCount).Select(x=>x.Key).ToArray();

                return ret;
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
            
        }

        internal bool Log;

        private TextBox PART_EditableTextBox;

        private TextBox TryGetPART_EditableTextBox()
        {
            // https://stackoverflow.com/a/52633838/14367566

            if (PART_EditableTextBox == null)
            {
                if (comboBox != null)
                {
                    ControlTemplate ctrlt = comboBox.Template;
                    if (ctrlt != null)
                    {
                        object element = ctrlt.FindName("PART_EditableTextBox", comboBox);
                        if (element is TextBox textBox)
                        {
                            PART_EditableTextBox = textBox;
                        }
                    }
                }
            }

            return PART_EditableTextBox;
        }

        private bool textChangedEventHandlerAdded;
        private void AddTextChangedEventHandler()
        {
            if (!textChangedEventHandlerAdded)
            {
                textChangedEventHandlerAdded = true;
                PART_EditableTextBox.TextChanged += PART_EditableTextBox_TextChanged;
            }
        }

        public bool Separate{get; set; } = true;

        private void PART_EditableTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Separate)
            {
                ChosungHelper.Separate(PART_EditableTextBox);
            }
            
            WriteLine($"PART_EditableTextBox.Text: {PART_EditableTextBox.Text}");
            Filter();
            comboBox.SelectedIndex = -1;
        }

        private void comboBox_PropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            WriteLine(
$"ComboBox.PropertyChanged: {e.Property} '{e.OldValue ?? "null"}' '{e.NewValue ?? "null"}'");

            if (e.Property == ComboBox.TextProperty)
            {
                if (!textChangedEventHandlerAdded)
                {
                    Filter();
                }
            }
            else if (e.Property == ComboBox.IsDropDownOpenProperty)
            {
                if (e.OldValue.Equals(true))
                {
                    if (e.NewValue.Equals(false))
                    {
                    }
                }
            }
            else if (e.Property == ComboBox.ActualWidthProperty)
            {
                TextBox tb = TryGetPART_EditableTextBox();
                if (tb != null)
                {
                    AddTextChangedEventHandler();
                }
            }


        }

        private void comboBox_KeyDown(object sender, KeyEventArgs e)
        {
            WriteLine("ComboBox.KeyDown");
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WriteLine("ComboBox.SelectionChanged");
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
