﻿using System;
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

namespace ParallelImplementationResearcher
{
    /// <summary>
    /// Логика взаимодействия для ThreadListBox.xaml
    /// </summary>
    public partial class ThreadListBox : UserControl
    {
        private int _threadNumber = 0;
        public int ThreadNumber
        {
            get { return _threadNumber; }
            set 
            { 
                _threadNumber = value;
				CheckList.Children.Clear();
                for(int i = 0; i < _threadNumber; i++)
                {
					SetCheckBox(i);
                }
            }
        }

        public List<int> GetCheckedThreads
        {
            get
            {
                List<int> checkedThreads= new List<int>();
                foreach(var check in CheckList.Children)
                {
                    if (check is CheckBox) {
                        if ((check as CheckBox) is not null && (check as CheckBox).IsChecked == true)
                        {
                            checkedThreads.Add(int.Parse((check as CheckBox).Content.ToString()));
                        }
                    }
                }
                return checkedThreads;
            }
        }

        private void SetCheckBox(int index)
        {
            CheckBox cb = new CheckBox();
            cb.IsChecked = true;
            cb.Name = $"TCB_{index}";
            cb.Content = $"{index + 1}";
            cb.Width = 50;
            CheckList.Children.Add(cb);
        }

        public ThreadListBox()
        {
            InitializeComponent();
        }

        
    }
}
