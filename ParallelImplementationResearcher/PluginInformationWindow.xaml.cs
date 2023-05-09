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
using System.Windows.Shapes;

namespace ParallelImplementationResearcher
{
	/// <summary>
	/// Логика взаимодействия для PluginInformationWindow.xaml
	/// </summary>
	public partial class PluginInformationWindow : Window
	{
		public PluginInformationWindow(List<PluginInformation> pluginInformationTable)
		{
			InitializeComponent();
			PluginInformationTable.ItemsSource= pluginInformationTable;
		}
	}
}
