using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WPFChosungComboBox
{
    internal class ComboBox2:ComboBox
    {
        internal event DependencyPropertyChangedEventHandler PropertyChanged;


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            PropertyChanged?.Invoke(this, e);
        }


        private TextBox part_EditableTextBox;
        internal TextBox PART_EditableTextBox
        {
            get
            {
                if (part_EditableTextBox == null)
                {
                    ControlTemplate ctrlt = Template;
                    if (ctrlt != null)
                    {
                        object element = ctrlt.FindName("PART_EditableTextBox", this);
                        if (element is TextBox textBox)
                        {
                            part_EditableTextBox = textBox;
                        }
                    }
                }

                return part_EditableTextBox;// possibly null.
            }
        }


        internal string Text2
        {
            get
            {
                if (part_EditableTextBox != null)
                {
                    return part_EditableTextBox.Text;
                }
                return Text;
            }
        }


    }
}
