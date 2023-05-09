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
using System.Windows.Media.Animation;
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
		private long _value = 0;
		private long _minValue = long.MinValue;
		private long _maxValue = long.MaxValue;

		public long Value
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
		public ulong uValue
		{
			get
			{
				return (ulong)_value;
			}
		}

		public long MinValue
		{
			get { return _minValue; }
			set { _minValue = value; CheckingValues(value); }
		}

		public long MaxValue
		{
			get { return _maxValue; }
			set { _maxValue = value; CheckingValues(value); }
		}

		private void CheckingValues(long value)
		{
			if (value > _maxValue)
			{
				Value = _maxValue;
			} 
			else if (value < _minValue)
			{
				Value = _minValue;
			} 
			else
			{
				Value = value;
			}

		}

        public NumericUpDown()
        {
            InitializeComponent();
			
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
			if (!long.TryParse(textNumber.Text, out _value))
			{
				CheckingValues(_value);
			}
		}
	}
}