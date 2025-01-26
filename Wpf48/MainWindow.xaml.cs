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

namespace Wpf48
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly IReadOnlyList<string> allJmcodes;

        public MainWindow()
        {
            InitializeComponent();
            allJmcodes = GetAllJmcodes();
            ccb.ItemsSource = allJmcodes.ToArray();
        }

        private static List<string> GetAllJmcodes()
        {
            var ret = new List<string>()
            {
                "삼성전자", "3S", "HDC현대산업개발", "맘스터치", "상신전자", "LG생활건강", "데브시스터즈", "동화약품", "하나마이크론"
            };

            const int N = 2500;

            for(int i = 0; i < N; i++)
            {
                ret.Add(Guid.NewGuid().ToString("N"));
            }

            return ret;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ccb.Text = "asdfasdf 1";
        }


        private void ccb_EnterKeyDown(object sender, EventArgs e)
        {
            Console.WriteLine("CComboBox.EnterKeyDown: " + ccb.Text);
        }
    }
}
