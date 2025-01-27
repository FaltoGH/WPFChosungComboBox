using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPFChosungComboBox
{
    internal class ChosungViewModel:Notifier
    {

        private string[] itemsSource;
        public string[] ItemsSource
        {
            get => itemsSource;
            set
            {
                if(itemsSource != value)
                {
                    itemsSource = value;
                    OnPropertyChanged();
                }
            }
        }

    }
}
