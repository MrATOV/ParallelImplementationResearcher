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
	/// Логика взаимодействия для NumSlider.xaml
	/// </summary>
	public partial class NumSlider : UserControl
	{
		public double Value
		{
			get
			{
				return NumberSlider.Value;
			}
			set
			{
				if (value > MaxValue)
				{
					NumberSlider.Value = MaxValue;
				} else if (value < MinValue)
				{
					NumberSlider.Value = MinValue;
				}
				else
				{
					NumberSlider.Value = value;
				}
			}
		}
		public double MinValue
		{
			get
			{
				return NumberSlider.Minimum;
			}
			set
			{
				NumberSlider.Minimum = value;
			}
		}

		public double MaxValue
		{
			get 
			{ 
				return NumberSlider.Maximum;
			}
			set
			{
				NumberSlider.Maximum = value;
			}
		}
		private void SetValue()
		{
			NumValue.Text = string.Format("{0:f2}", NumberSlider.Value);
		}
		public NumSlider()
		{
			InitializeComponent();
			SetValue();
		}

		private void NumSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			SetValue();
		}
	}
}
