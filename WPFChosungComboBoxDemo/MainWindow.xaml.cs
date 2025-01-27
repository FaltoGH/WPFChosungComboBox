using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
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
using WPFChosungComboBox;

namespace WPFChosungComboBoxDemo
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly StringDictionary code2name;
        private readonly StringDictionary name2code;


        public MainWindow()
        {
            InitializeComponent();

            Assembly assembly = typeof(WPFChosungComboBox.ChosungComboBox).Assembly;
            Title += " " + assembly.GetName().Version.ToString();

            string text = File.ReadAllText("codelist.txt");
            string[] tokens = text.Split(';');

            code2name = new StringDictionary();
            name2code = new StringDictionary();

            for(int i = 0; i < tokens.Length; i += 2)
            {
                code2name[tokens[i]] = tokens[i + 1];
                name2code[tokens[i + 1]] = tokens[i];
            }

            List<string> jmnames = new List<string>();

            foreach(var value in code2name.Values)
            {
                jmnames.Add(value.ToString());
            }

            ccb.ItemsSource = jmnames.ToArray();
        }


        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                string text = tb.Text;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    string name = code2name[text];
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        ccb.Text = null;
                    }
                    else
                    {
                        ccb.Text = name;
                    }
                }
            }
        }


        private void ccb_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            Console.WriteLine(e.Exception);
        }


        private void ccb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string jmname = ccb.SelectedItem?.ToString();
            if (!string.IsNullOrWhiteSpace(jmname))
            {
                string jmcode = name2code[jmname];
                if (!string.IsNullOrWhiteSpace(jmcode))
                {
                    tb.Text = jmcode;
                }
            }
        }

        private void ccb_WriteLine(object sender, object e)
        {
            Console.WriteLine($"{DateTime.Now:mm:ss.fff} {e}");
        }

    }
}
