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

namespace ParallelImplementationResearcher
{
    /// <summary>
    /// Логика взаимодействия для NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
		private int _value = 0;
		private int _minValue = 0;
		private int _maxValue = 1;

		public int Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				textNumber.Text = _value.ToString();
			}
		}

		public int MinValue
		{
			get { return _minValue; }
			set { _minValue = value; }
		}

		public int MaxValue
		{
			get { return _maxValue; }
			set { _maxValue = value; }
		}
        public NumericUpDown()
        {
            InitializeComponent();
			if (_value > _maxValue)
			{
				textNumber.Text = _maxValue.ToString();
				_value = _maxValue;
			} 
			else if (_value < _minValue)
			{
				textNumber.Text = _minValue.ToString();
				_value = _minValue;
			} 
			else
			{
				textNumber.Text = _value.ToString();
			}
        }

		private void buttonUp_Click(object sender, RoutedEventArgs e)
		{
			if (_value < _maxValue)
			{
				Value++;
			}
		}

		private void buttonDown_Click(object sender, RoutedEventArgs e)
		{
			if (_value > _minValue)
			{
				Value--;
			}
		}

		private void textNumber_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (textNumber == null)
			{
				return;
			}
			if (!int.TryParse(textNumber.Text, out _value))
			{
				textNumber.Text = _value.ToString();
			}
			if (_value > _maxValue)
			{
				Value = _maxValue;
			} 
			else if (_value < _minValue)
			{
				Value = _minValue;
			}
		}
	}
}