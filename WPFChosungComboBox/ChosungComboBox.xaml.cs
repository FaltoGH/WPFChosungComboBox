using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public string[] ItemsSource
        {
            get
            {
                return viewModel.ItemsSource;
            }
            set
            {
                viewModel.ItemsSource = value;
            }
        }
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
        public event EventHandler<object> WriteLine;
        public int MaxDropDownCount { get; set; } = 10;
        public ChosungComboBox()
        {
            InitializeComponent();
            
            CollectionViewSource cvs = (CollectionViewSource)Resources["cvs"];
            cvs.SortDescriptions.Add(new SortDescription("Length", ListSortDirection.Ascending));
        }
        public event EventHandler<string> SelectionChanged;


        private void PWriteLine(object x)
        {
            WriteLine?.Invoke(this, x);
        }


        private void comboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            PWriteLine("ComboBox.PreviewKeyDown");
            if (comboBox.IsKeyboardFocusWithin)
            {
                comboBox.IsDropDownOpen = true;
            }
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




        private string keyword;
        private string Keyword
        {
            get
            {
                return keyword;
            }
            set
            {
                if (keyword != value)
                {
                    keyword = value;
                }
            }
        }

        private void Filter()
        {
            string[] ret;

            try
            {
                string text = Keyword;
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

                    ret = scoreboard.OrderByDescending(x => x.Value).Take(MaxDropDownCount).Select(x => x.Key).ToArray();
                }
                else
                {
                    ret = null;
                }
            }
            catch(Exception ex)
            {
                FirstChanceException?.Invoke(this, new FirstChanceExceptionEventArgs(ex));
                ret = null;
            }

            Filtered = ret;
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

            Keyword = comboBox.PART_EditableTextBox.Text;
            Filter();
            FilteredSet = Filtered.ToHashSet();
            PListCollectionView.Refresh();
        }


        private string[] Filtered;
        private HashSet<string> FilteredSet;
        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            string item = e?.Item?.ToString();
            if (!string.IsNullOrWhiteSpace(item))
            {
                e.Accepted = FilteredSet?.Contains(item) == true;
            }
        }


        private ListCollectionView PListCollectionView
        {
            get
            {
                return (ListCollectionView)comboBox.ItemsSource;
            }
        }


        private void comboBox_PropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PWriteLine(
$"ComboBox.PropertyChanged: {e.Property} '{e.OldValue ?? "null"}' '{e.NewValue ?? "null"}'");
        }


        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PWriteLine($"ComboBox.SelectionChanged: SelectedItem: {comboBox.SelectedItem ?? "null"}");
            if(comboBox.SelectedItem != null)
            {
                SelectionChanged?.Invoke(sender, comboBox.SelectedItem.ToString());
            }
        }


        public object SelectedItem
        {
            get => comboBox.SelectedItem;
            set => comboBox.SelectedItem = value;
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            PWriteLine($"OnKeyDown: {e.Key}");
        }


        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            PWriteLine($"OnPreviewKeyDown: {e.Key}");
            if (e.Key == Key.Enter)
            {
                if (Filtered != null)
                {
                    if (Filtered.Length > 0)
                    {
                        string first = Filtered[0];
                        Text = first;
                        comboBox.IsDropDownOpen = false;
                        SelectionChanged?.Invoke(this, first);
                        e.Handled = true;
                    }
                }
            }
            comboBox.SelectedItem = null;
        }


        private void comboBox_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (comboBox.IsKeyboardFocusWithin)
            {
                comboBox.IsDropDownOpen = true;
            }
        }


        private void comboBox_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }


        private void comboBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }


        private void StackPanel2_PropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PWriteLine(
$"StackPanel.PropertyChanged: {e.Property} '{e.OldValue ?? "null"}' '{e.NewValue ?? "null"}'");
        }

    }
}
