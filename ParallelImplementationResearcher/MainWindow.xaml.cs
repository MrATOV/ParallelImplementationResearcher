using Microsoft.Win32;
using ResearchKernelWrapper;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media.Imaging;

namespace ParallelImplementationResearcher
{
	public enum PlotType
	{
		Time,
		Acceleration,
		Efficiency,
		Cost
	}

	public partial class MainWindow : Window
	{
		ResearchBase _researchBase;
		
		string[]? dataInfo;
		List<ulong> _arraySize;

		ScottPlot.Plottable.BarPlot? timePlot;
		ScottPlot.Plottable.BarPlot? accelerationPlot;
		ScottPlot.Plottable.BarPlot? efficiencyPlot;
		ScottPlot.Plottable.BarPlot? costPlot;

		private void AddPlugin(string fileName)
		{

			_researchBase.AddPlugin(fileName);
			List<string> functions = _researchBase.GetTitles;
			lb_AlgorithmList.ItemsSource = functions;
		}

		public MainWindow()
		{
			InitializeComponent();
			cb_ArrayGenerateType.SelectedIndex = 0;
			cb_MatrixGenerateType.SelectedIndex = 0;
			lb_threads.ThreadNumber = 15;
			_researchBase = new ResearchBase();
			_arraySize = new List<ulong>();
			
			BarPlot.Configuration.Pan = false;
			BarPlot.Configuration.Zoom = false;
			BarPlot.Configuration.ScrollWheelZoom = false;
			BarPlot.Configuration.MiddleClickDragZoom = false;
		}
		private void FillDataResearchTable(int researchIndex)
		{
			List<string> dataResearch;
			if (cb_ResearchList.SelectedItem.ToString().Contains("<...>"))
			{
				dataResearch = _researchBase.GetCurrentDataResearch();

			}
			else
			{
				dataResearch = _researchBase.GetDataResearch(researchIndex);
			}
			var dataResearchInformation = new List<DataResearchInformation>();
			foreach (var data in dataResearch)
			{
				List<string> dataRow = data.Split("%%").ToList();
				DataResearchInformation dri = new DataResearchInformation();
				dri.ID = dataRow[0];
				dri.AlgorithmName = dataRow[2];
				dri.TypeImplementation = dataRow[3];
				dri.Data = dataRow[4];
				dataResearchInformation.Add(dri);
			}
			DataResearchTable.ItemsSource = dataResearchInformation;
		}

		private void ShowPlotType(PlotType plotType)
		{
			timePlot.IsVisible = false;
			accelerationPlot.IsVisible = false;
			efficiencyPlot.IsVisible = false;
			costPlot.IsVisible = false;
			BarPlot.Plot.XLabel("Количество потоков");
			switch (plotType)
			{
				case PlotType.Time:
					timePlot.IsVisible = true; 
					BarPlot.Plot.Title("Время");
					BarPlot.Plot.YLabel("Время, мс");
					break;
				case PlotType.Acceleration:
					accelerationPlot.IsVisible = true;
					BarPlot.Plot.Title("Коэффициент ускорения");
					BarPlot.Plot.YLabel("Значение ускорения");
					break;
				case PlotType.Efficiency:
					efficiencyPlot.IsVisible = true;
					BarPlot.Plot.Title("Коэффициент эффективности");
					BarPlot.Plot.YLabel("Значение эффективности");
					break;
				case PlotType.Cost:
					costPlot.IsVisible = true;
					BarPlot.Plot.Title("Коэффициент стоимости");
					BarPlot.Plot.YLabel("Значение стоимости");
					break;
			}
			BarPlot.Plot.AxisAuto();
			BarPlot.Refresh();
		}

		private void FillDataParameters(int dataIndex)
		{
			cb_ParameterCaptionList.Items.Clear();
			List<string> dataParameters;
			if (cb_ResearchList.SelectedItem.ToString().Contains("<...>"))
			{
				dataParameters = _researchBase.GetCurrentDataParameters(dataIndex);
			}
			else
			{
				dataParameters = _researchBase.GetDataParameters(dataIndex);

			}
			foreach (var data in dataParameters)
			{
				string dataRow = data.Split("%%")[2];
				cb_ParameterCaptionList.Items.Add(dataRow);
			}
			if (cb_ParameterCaptionList.Items.Count > 0) cb_ParameterCaptionList.SelectedIndex = 0;
		}

		private void FillAlgorithmEvaluation(int algorithmIndex)
		{
			List<string> algorithmEvaluation;
			if (cb_ResearchList.SelectedItem.ToString().Contains("<...>"))
			{
				algorithmEvaluation = _researchBase.GetCurrentAlgorithmEvaluation(algorithmIndex);
			}else
			{
				algorithmEvaluation = _researchBase.GetAlgorithmEvaluation(algorithmIndex);

			}
			var algorithmEvaluationInformation = new List<AlgorithmEvaluationInformation>();
			foreach (var algorithm in algorithmEvaluation)
			{
				List<string> algorithmRow = algorithm.Split("%%").ToList();
				AlgorithmEvaluationInformation aei = new AlgorithmEvaluationInformation();
				aei.ID = algorithmRow[0];
				aei.ThreadNumber = algorithmRow[2];
				aei.Time = algorithmRow[3];
				aei.Acceleration = algorithmRow[4];
				aei.Efficiency = algorithmRow[5];
				aei.Cost = algorithmRow[6];
				algorithmEvaluationInformation.Add(aei);
			}

			AlgorithmEvaluationGraph(algorithmEvaluationInformation);
			AlgorithmEvaluationTable.ItemsSource = algorithmEvaluationInformation;
		}

		private void AlgorithmEvaluationGraph(List<AlgorithmEvaluationInformation> algorithmEvaluationInformation)
		{
			double[] threads = new double[algorithmEvaluationInformation.Count];
			string[] threadsLabels = new string[algorithmEvaluationInformation.Count];
			double[] times = new double[algorithmEvaluationInformation.Count]; 
			double[] accelerations = new double[algorithmEvaluationInformation.Count]; 
			double[] efficiencies = new double[algorithmEvaluationInformation.Count]; 
			double[] costs = new double[algorithmEvaluationInformation.Count]; 
			for(int i = 0; i < algorithmEvaluationInformation.Count; i++)
			{
				threads[i] = double.Parse(algorithmEvaluationInformation[i].ThreadNumber.Replace('.',',')) - 1;
				threadsLabels[i] = algorithmEvaluationInformation[i].ThreadNumber;
				times[i] = double.Parse(algorithmEvaluationInformation[i].Time.Replace('.', ','));
				accelerations[i] = double.Parse(algorithmEvaluationInformation[i].Acceleration.Replace('.', ','));
				efficiencies[i] = double.Parse(algorithmEvaluationInformation[i].Efficiency.Replace('.', ','));
				costs[i] = double.Parse(algorithmEvaluationInformation[i].Cost.Replace('.', ','));
			}

			BarPlot.Plot.XTicks(threadsLabels);

			timePlot = BarPlot.Plot.AddBar(times, threads);
			timePlot.BarWidth = 0.8;
			timePlot.ShowValuesAboveBars= true;
			timePlot.Label = "Время";
			timePlot.IsVisible = false;

			accelerations[0] = 0;
			accelerationPlot = BarPlot.Plot.AddBar(accelerations, threads);
			accelerationPlot.BarWidth = 0.8;
			accelerationPlot.ShowValuesAboveBars = true;
			accelerationPlot.Label = "Коэффициент ускорения";
			accelerationPlot.IsVisible = false;

			efficiencies[0] = 0;
			efficiencyPlot = BarPlot.Plot.AddBar(efficiencies, threads);
			efficiencyPlot.BarWidth = 0.8;
			efficiencyPlot.ShowValuesAboveBars = true;
			efficiencyPlot.Label = "Коэффициент эффективности";
			efficiencyPlot.IsVisible= false;

			costPlot = BarPlot.Plot.AddBar(costs, threads);
			costPlot.BarWidth = 0.8;
			costPlot.ShowValuesAboveBars = true;
			costPlot.Label = "Коэффициент стоимости";
			costPlot.IsVisible = false;


			rb_Time.IsChecked = true;
		}

		List<string>? researchList;
		private void UpdateResearchList()
		{
			if (_researchBase.GetResearchListCount > 0)
			{
				researchList = new List<string>();
				researchList = (from research in _researchBase.GetResearchList
											 select research.Replace("%%", " ")).ToList();

				cb_ResearchList.ItemsSource = researchList;
				//cb_ResearchList.SelectedIndex = 0;
			}
		}

		private void MenuLoadPlugin_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = "Плагины (.dll)|*.dll";

			bool? result = dialog.ShowDialog();
			string fileName;

			if (result == true)
			{
				fileName = dialog.FileName;
				AddPlugin(fileName);
				lb_AlgorithmList.SelectedIndex = 0;
			}
		}

		private void HideDataGroupBox()
		{
			gb_Array.Visibility = Visibility.Collapsed;
			gb_Matrix.Visibility = Visibility.Collapsed;
			gb_Text.Visibility = Visibility.Collapsed;
			gb_Image.Visibility = Visibility.Collapsed;
		}

		private string GetDataTypeName(List<string> dataTypeList)
		{
			if (dataTypeList.Count > 0)
			{
				string dataTypeName = "";
				HideDataGroupBox();
				switch (dataTypeList[0])
				{
					case "Array":
						dataTypeName += "Массив";
						gb_Array.Visibility = Visibility.Visible;
						break;
					case "Matrix":
						dataTypeName += "Матрица";
						gb_Matrix.Visibility = Visibility.Visible;
						break;
					case "Text":
						dataTypeName += "Текст";
						gb_Text.Visibility = Visibility.Visible;
						break;
					case "Image":
						dataTypeName += "Изображение";
						gb_Image.Visibility = Visibility.Visible;
						break;
					default:
						dataTypeName += "Неизвестно";
						break;
				}
				if (dataTypeList.Count == 2)
				{
					switch (dataTypeList[1])
					{
						case "int":
							dataTypeName += " целых чисел (4 Б)";
							break;
						case "long":
							dataTypeName += " целых чисел (8 Б)";
							break;
						case "float":
							dataTypeName += " вещественных чисел (4 Б)";
							break;
						case "double":
							dataTypeName += " вещественных чисел (8 Б)";
							break;
					}
				}
				return dataTypeName;
			}
			return "";
		}

		private void lb_AlgorithmList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string dataType = _researchBase.GetFunctionInformation(lb_AlgorithmList.SelectedIndex, FunctionInformation.DataType);
			List<string> dataTypeList = dataType.Split(" ").ToList();
			l_DataType.Content = $"Тип данных: {GetDataTypeName(dataTypeList)}";
			TabInputData.Items.Clear();
			dataInfo = _researchBase.GetFunctionInformation(lb_AlgorithmList.SelectedIndex, FunctionInformation.DataType).Split(" ");
			AddParameterObject();
		}

		private object AddProcessingArray(string arrayStr)
		{
			TextBlock tb = new TextBlock();
			tb.Text = arrayStr;
			tb.TextWrapping = TextWrapping.Wrap;

			ScrollViewer sv = new ScrollViewer();
			sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
			sv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
			sv.Content = tb;

			TabItem ti = new TabItem();
			//ti.Header = $"Массив {TabProcessingData.Items.Count + 1}";
			ti.Content = sv;
			return ti;
		}

		private void ShowProcessingData()
		{
			var processingData = _researchBase.GetProcessingData();
			cb_ProcessingDataList.ItemsSource = processingData;
		}

		private string JoinRowMatrix<T>(string splitter, T[,] matrix, int index)
		{
			string matrixStr = "";
			for(int i = 0; i < matrix.GetLength(1); i++)
			{
				matrixStr += matrix[index, i].ToString() + splitter;
			}
			return matrixStr;
		}

		private string JoinMatrix<T>(string splitter, T[,] matrix)
		{
			string matrixStr = "";
			for (int i = 0; i < matrix.GetLength(0); i++)
			{
				for (int j = 0; j < matrix.GetLength(1); i++)
				{
					matrixStr += matrix[i, j].ToString() + splitter;
				}
			}
			return matrixStr;
		}

		private void b_Execute_Click(object sender, RoutedEventArgs e)
		{
			_researchBase.SetConfidenceIntervalOptions(nud_ExecuteIterationNumber.uValue,
				(AlphaPercent)cb_TrustValue.SelectedValue,
				(IntervalTypeValue)cb_IntervalCoefficient.SelectedValue,
				(CalculateValue)cb_IntervalValue.SelectedValue);
			List<int> threadNumbers = lb_threads.GetCheckedThreads;
			_researchBase.AddThreadValues(threadNumbers);
			_researchBase.SaveProcessingData(cb_ShowProcessingData.IsChecked.Value);
			_researchBase.ExecuteFunction(lb_AlgorithmList.SelectedIndex);
			if (cb_ShowProcessingData.IsChecked.Value)
			{
				ShowProcessingData();
			}
			AddNewResearch();

			MessageBox.Show("Успешно");
		}

		private void AddNewResearch()
		{
			researchList.Add("<...>");
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			cb_IntervalCoefficient.SelectedIndex = cb_IntervalCoefficient.Items.Count > 0 ? 0 : -1;
			cb_IntervalValue.SelectedIndex = cb_IntervalValue.Items.Count > 0 ? 0 : -1;
			cb_TrustValue.SelectedIndex = cb_TrustValue.Items.Count > 0 ? 0 : -1;
			UpdateResearchList();
		}

		private void ClearParameters()
		{
			for (int i = p_Parameters.Children.Count - 1; i >= 0; i--)
			{
				if (p_Parameters.Children[i] is not Label)
				{
					p_Parameters.Children.RemoveAt(i);
				}
			}
		}

		private void AddParameterObject()
		{
			ClearParameters();
			string parametersStr = _researchBase.GetFunctionInformation(lb_AlgorithmList.SelectedIndex,
				FunctionInformation.Parameters);
			if (!string.IsNullOrEmpty(parametersStr))
			{
				List<string> parameters = parametersStr.Split(";").ToList();
				for (int i = 0; i < parameters.Count; i++)
				{
					if (parameters[i].Contains("bool"))
					{
						CheckBox cb = new CheckBox();
						cb.IsChecked = true;
						cb.Name = $"Param_{i}_CB";
						cb.Margin = new Thickness(0, 12, 0, 0);
						string title = Regex.Match(parameters[i], @"\((.*)\)").Groups[1].Value;
						cb.Content = string.IsNullOrEmpty(title) ? $"Неизвестно" : title;
						p_Parameters.Children.Add(cb);
						continue;
					}
					if (parameters[i].Contains("int"))
					{
						TextBlock tb = new TextBlock();
						tb.Name = $"Param_{i}_TB";
						string title = Regex.Match(parameters[i], @"\((.*)\)").Groups[1].Value;
						tb.Text = string.IsNullOrEmpty(title) ? $"Неизвестно" : title;
						tb.TextWrapping = TextWrapping.Wrap;
						tb.Margin = new Thickness(0, 12, 0, 0);
						p_Parameters.Children.Add(tb);

						NumericUpDown nud = new NumericUpDown();
						nud.Name = $"Param_{i}_NUD";
						string interval = Regex.Match(parameters[i], @"\[(.*)\]").Groups[1].Value;
						var intervalValues = interval.Split(",").ToList();
						nud.MaxValue = long.Parse(intervalValues[1]);
						nud.MinValue = long.Parse(intervalValues[0]);
						p_Parameters.Children.Add(nud);
						continue;
					}
					if (parameters[i].Contains("double"))
					{
						TextBlock tb = new TextBlock();
						tb.Name = $"Param_{i}_TB";
						string title = Regex.Match(parameters[i], @"\((.*)\)").Groups[1].Value;
						tb.Text = string.IsNullOrEmpty(title) ? $"Неизвестно" : title;
						tb.TextWrapping = TextWrapping.Wrap;
						tb.Margin = new Thickness(0, 12, 0, 0);
						p_Parameters.Children.Add(tb);

						NumSlider s = new NumSlider();
						s.Name = $"Param_{i}_S";
						string interval = Regex.Match(parameters[i], @"\[(.*)\]").Groups[1].Value;
						var intervalValues = interval.Split(",").ToList();
						s.MaxValue = double.Parse(intervalValues[1]);
						s.MinValue = double.Parse(intervalValues[0]);
						p_Parameters.Children.Add(s);
						continue;
					}
					if (parameters[i].Contains("enumeration"))
					{
						TextBlock tb = new TextBlock();
						tb.Name = $"Param_{i}_TB";
						string title = Regex.Match(parameters[i], @"\((.*)\)").Groups[1].Value;
						tb.Text = string.IsNullOrEmpty(title) ? $"Неизвестно" : title;
						tb.TextWrapping = TextWrapping.Wrap;
						tb.Margin = new Thickness(0, 12, 0, 0);
						p_Parameters.Children.Add(tb);

						ComboBox cbx = new ComboBox();
						cbx.Name = $"Param_{i}_CBx";
						string valueStr = Regex.Match(parameters[i], @"\[(.*)\]").Groups[1].Value;
						valueStr = valueStr.Replace(", ", ",");
						var values = valueStr.Split(",").ToList();
						cbx.ItemsSource = values;
						cbx.SelectedIndex = values.Count > 0 ? 0 : -1;
						p_Parameters.Children.Add(cbx);
						continue;
					}
					if (parameters[i].Contains("text"))
					{
						TextBlock tb = new TextBlock();
						tb.Name = $"Param_{i}_TB";
						string title = Regex.Match(parameters[i], @"\((.*)\)").Groups[1].Value;
						tb.Text = string.IsNullOrEmpty(title) ? $"Неизвестно" : title;
						tb.TextWrapping = TextWrapping.Wrap;
						tb.Margin = new Thickness(0, 12, 0, 0);
						p_Parameters.Children.Add(tb);

						TextBox tbx = new TextBox();
						tbx.Name = $"Param_{i}_TBx";
						p_Parameters.Children.Add(tbx);
					}
				}
			}
		}

		private string SetParameterValues()
		{
			string parameterValues = "";
			string parameterCaption = "";
			foreach (var parameter in p_Parameters.Children)
			{
				if (parameter is TextBlock)
				{
					parameterCaption += ((TextBlock)parameter).Text + ": ";
					continue;
				}
				if (parameter is CheckBox)
				{
					parameterValues += ((CheckBox)parameter).IsChecked == true ? "1 " : "0 ";
					parameterCaption += ((CheckBox)parameter).Content.ToString() + (((CheckBox)parameter).IsChecked == true ? ": Да; " : ": Нет; ");
					continue;
				}
				if (parameter is NumericUpDown)
				{
					parameterValues += ((NumericUpDown)parameter).Value.ToString() + " ";
					parameterCaption += ((NumericUpDown)parameter).Value.ToString() + "; ";
					continue;
				}
				if (parameter is NumSlider)
				{
					parameterValues += ((NumSlider)parameter).Value.ToString() + " ";
					parameterCaption += ((NumSlider)parameter).Value.ToString() + "; ";
					continue;
				}
				if (parameter is ComboBox)
				{
					parameterValues += ((ComboBox)parameter).SelectedIndex.ToString() + " ";
					parameterCaption += ((ComboBox)parameter).SelectedIndex.ToString() + "; ";
					continue;
				}
				if (parameter is TextBox)
				{
					parameterValues += ((TextBox)parameter).Text + " ";
					parameterCaption += ((TextBox)parameter).Text + "; ";
				}
			}
			_researchBase.AddParameterValue(parameterCaption, parameterValues);
			return parameterCaption;
		}

		private void cb_ShowProcessingData_Click(object sender, RoutedEventArgs e)
		{
			if (cb_ShowProcessingData.IsChecked.HasValue && cb_ShowProcessingData.IsChecked.Value)
			{
				TabProcessingData.Visibility = Visibility.Visible;
			}
			else
			{
				TabProcessingData.Visibility = Visibility.Collapsed;
			}
		}

		private void MenuItem_Click_1(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void cb_ArrayGenerateType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cb_ArrayGenerateType.SelectedIndex == 1)
			{
				cb_Array_Sequence.Visibility = Visibility.Visible;
				p_Array_Random.Visibility = Visibility.Collapsed;
			}
			else
			{
				cb_Array_Sequence.Visibility = Visibility.Collapsed;
				p_Array_Random.Visibility = Visibility.Visible;
			}
		}

		private void cb_MatrixGenerateType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cb_MatrixGenerateType.SelectedIndex == 1)
			{
				cb_Matrix_Sequence.Visibility = Visibility.Visible;
				p_Matrix_Random.Visibility = Visibility.Collapsed;
			}
			else
			{
				cb_Matrix_Sequence.Visibility = Visibility.Collapsed;
				p_Matrix_Random.Visibility = Visibility.Visible;
			}
		}

		private void b_generateMatrix_Click(object sender, RoutedEventArgs e)
		{
			string dataType = _researchBase.GetFunctionInformation(lb_AlgorithmList.SelectedIndex, FunctionInformation.DataType)
				.Split(" ")[1];
			if (cb_MatrixGenerateType.SelectedIndex == 1)
			{
				_researchBase.SetMatrixData(dataType, nud_MatrixRowSize.uValue, nud_MatrixColumnSize.uValue, cb_Matrix_Sequence.IsChecked.Value);
			}
			else
			{
				_researchBase.SetMatrixData(dataType, nud_MatrixRowSize.uValue, nud_MatrixColumnSize.uValue, 
					(int)nud_MinMatrix.Value, (int)nud_MaxMatrix.Value);
			}
			if (dataType.Contains("int")) TabInputData.Items.Add(AddNewInputMatrixTabItem(JoinMatrix(", ",
				_researchBase.GetMatrixData<int>((ulong)TabInputData.Items.Count, nud_MatrixRowSize.uValue, nud_MatrixColumnSize.uValue))));
			if (dataType.Contains("long")) TabInputData.Items.Add(AddNewInputMatrixTabItem(JoinMatrix(", ",
				_researchBase.GetMatrixData<long>((ulong)TabInputData.Items.Count, nud_MatrixRowSize.uValue, nud_MatrixColumnSize.uValue))));
			if (dataType.Contains("float")) TabInputData.Items.Add(AddNewInputMatrixTabItem(JoinMatrix(", ",
				_researchBase.GetMatrixData<float>((ulong)TabInputData.Items.Count, nud_MatrixRowSize.uValue, nud_MatrixColumnSize.uValue))));
			if (dataType.Contains("double")) TabInputData.Items.Add(AddNewInputMatrixTabItem(JoinMatrix(", ",
				_researchBase.GetMatrixData<double>((ulong)TabInputData.Items.Count, nud_MatrixRowSize.uValue, nud_MatrixColumnSize.uValue))));
			TabInputData.SelectedIndex = TabInputData.Items.Count - 1;
			_arraySize.Add(nud_ArraySize.uValue);
		}

		private object AddNewInputArrayTabItem(string arrayInput)
		{
			TextBlock tb = new TextBlock();
			tb.Text = arrayInput;
			tb.TextWrapping = TextWrapping.Wrap;

			ScrollViewer sv = new ScrollViewer();
			sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
			sv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
			sv.Content = tb;

			TabItem ti = new TabItem();
			ti.Header = $"Массив {TabInputData.Items.Count + 1}";
			ti.Content = sv;
			return ti;
		}
		
		private object AddNewInputTextTabItem(string textInput)
		{
			TextBlock tb = new TextBlock();
			tb.Text = textInput;
			tb.TextWrapping = TextWrapping.Wrap;

			ScrollViewer sv = new ScrollViewer();
			sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
			sv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
			sv.Content = tb;

			TabItem ti = new TabItem();
			ti.Header = $"Текст {TabInputData.Items.Count + 1}";
			ti.Content = sv;
			return ti;
		}
		private object AddNewInputMatrixTabItem(string matrixInput)
		{
			TextBlock tb = new TextBlock();
			tb.Text = matrixInput;
			tb.TextWrapping = TextWrapping.Wrap;

			ScrollViewer sv = new ScrollViewer();
			sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
			sv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
			sv.Content = tb;

			TabItem ti = new TabItem();
			ti.Header = $"Матрица {TabInputData.Items.Count + 1}";
			ti.Content = sv;
			return ti;
		}

		private void b_GenerateArray_Click(object sender, RoutedEventArgs e)
		{
			string dataType = _researchBase.GetFunctionInformation(lb_AlgorithmList.SelectedIndex, FunctionInformation.DataType)
				.Split(" ")[1];
			if (cb_ArrayGenerateType.SelectedIndex == 1)
			{
				_researchBase.SetArrayData(dataType, nud_ArraySize.uValue, cb_Array_Sequence.IsChecked.Value);
			}
			else
			{
				_researchBase.SetArrayData(dataType, nud_ArraySize.uValue, (int)nud_MinArray.Value, (int)nud_MaxArray.Value);
			}
			if (dataType.Contains("int")) TabInputData.Items.Add(AddNewInputArrayTabItem(string.Join(", ",
				_researchBase.GetArrayData<int>((ulong)TabInputData.Items.Count, nud_ArraySize.uValue))));
			if (dataType.Contains("long")) TabInputData.Items.Add(AddNewInputArrayTabItem(string.Join(", ",
				_researchBase.GetArrayData<long>((ulong)TabInputData.Items.Count, nud_ArraySize.uValue))));
			if (dataType.Contains("float")) TabInputData.Items.Add(AddNewInputArrayTabItem(string.Join(", ",
				_researchBase.GetArrayData<float>((ulong)TabInputData.Items.Count, nud_ArraySize.uValue))));
			if (dataType.Contains("double")) TabInputData.Items.Add(AddNewInputArrayTabItem(string.Join(", ",
				_researchBase.GetArrayData<double>((ulong)TabInputData.Items.Count, nud_ArraySize.uValue))));
			TabInputData.SelectedIndex = TabInputData.Items.Count - 1;
			_arraySize.Add(nud_ArraySize.uValue);
		}

		private void b_GetArrayFromFile_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = "Файлы с массивом (.txt)|*.txt";

			bool? result = dialog.ShowDialog();

			if (result == true)
			{
				/*fileName = dialog.FileName;
				AddPlugin(fileName);
				lb_AlgorithmList.SelectedIndex = 0;*/
			}
		}

		private void b_GetMatrixFromFile_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = "Файлы с матрицей (.txt)|*.txt";

			bool? result = dialog.ShowDialog();

			if (result == true)
			{
				/*fileName = dialog.FileName;
				AddPlugin(fileName);
				lb_AlgorithmList.SelectedIndex = 0;*/
			}
		}



		private void MenuOpenPluginListItem_Click(object sender, RoutedEventArgs e)
		{
			if (lb_AlgorithmList.Items.Count > 0)
			{
				List<PluginInformation> pluginInformationTable = new List<PluginInformation>();
				for (int i = 0; i < _researchBase.GetFunctionCount; i++)
				{
					PluginInformation pluginInformation = new PluginInformation();
					pluginInformation.ID = (i + 1).ToString();
					pluginInformation.NamePluginFile = _researchBase.GetFunctionPluginFileName(i);
					pluginInformation.NameFunction = _researchBase.GetFunctionInformation(i, FunctionInformation.Title);
					pluginInformation.FunctionDescription = _researchBase.GetFunctionInformation(i, FunctionInformation.Description);
					pluginInformation.FunctionRealisation = _researchBase.GetFunctionInformation(i, FunctionInformation.Realisation);
					pluginInformation.Developer = _researchBase.GetFunctionInformation(i, FunctionInformation.Developer);
					pluginInformation.Version = _researchBase.GetFunctionInformation(i, FunctionInformation.Version);
					pluginInformationTable.Add(pluginInformation);
				}
				PluginInformationWindow window = new PluginInformationWindow(pluginInformationTable);
				window.ShowDialog();
			}
			else
			{
				MessageBox.Show("Отсутствуют подключенные плагины.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void DataResearchTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if ((DataResearchTable.SelectedItem as DataResearchInformation) != null)
			{
				FillDataParameters(int.Parse((DataResearchTable.SelectedItem as DataResearchInformation).ID));
			}
		}

		private void RadioButton_Checked(object sender, RoutedEventArgs e)
		{
			if (timePlot != null)
			{
				RadioButton radio = sender as RadioButton;
				switch (radio.Content.ToString())
				{
					case "Время":
							ShowPlotType(PlotType.Time);
							break;
					case "Коэффициент ускорения":
							ShowPlotType(PlotType.Acceleration);
							break;
					case "Коэффициент эффективности":
							ShowPlotType(PlotType.Efficiency);
							break;
					case "Коэффициент стоимости":
							ShowPlotType(PlotType.Cost);
							break;
				
				}
			}
		}

		private void b_AddParameterSet_Click(object sender, RoutedEventArgs e)
		{
			if (p_Parameters.Children.Count > 1)
			{
				lb_ParametersList.Items.Add(SetParameterValues());
			}
			else
			{
				MessageBox.Show("Отсутствуют объекты для настройки параметров", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
			}

		}

		private void cb_parameters_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			int tableIndex = int.Parse((DataResearchTable.SelectedItem as DataResearchInformation).ID);
			var dataResearch = _researchBase.GetDataParameters(tableIndex);
			int algorithmIndex = int.Parse(dataResearch[cb_ParameterCaptionList.SelectedIndex].Split("%%")[0]);
			FillAlgorithmEvaluation(algorithmIndex);
		}

		private void cb_ResearchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cb_ResearchList.SelectedItem != null)
			{
				if (cb_ResearchList.SelectedItem.ToString().Contains("<...>"))
				{
					FillDataResearchTable(0);
					SaveResearchPanel.Visibility = Visibility.Visible;
				}
				else
				{
					SaveResearchPanel.Visibility = Visibility.Collapsed;
					FillDataResearchTable(int.Parse(cb_ResearchList.SelectedItem.ToString().Split(" ")[0]));
				}

			}
		}

		private void b_addNewResearch_Click(object sender, RoutedEventArgs e)
		{
			string researchName = tb_ResearchName.Text;
			_researchBase.AddResearchToDB(researchName);
			_researchBase.AddResearchInformationToDB();
			UpdateResearchList();
			MessageBox.Show("Успешно");
		}

		private object AddNewInputImageTabItem(byte[] imageInput)
		{
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(imageInput, 0, imageInput.Length);

			BitmapImage imgsource = new BitmapImage();
			imgsource.BeginInit();
			imgsource.StreamSource = memoryStream;
			imgsource.EndInit();
			Image image = new Image();
			image.Source = imgsource;

			ScrollViewer sv = new ScrollViewer();
			sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
			sv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
			sv.Content = image;

			TabItem ti = new TabItem();
			ti.Header = $"Изображение {TabInputData.Items.Count + 1}";
			ti.Content = sv;
			return ti;
		}

		private void b_LoadImage_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = "Изображение (.bmp)|*.bmp";

			bool? result = dialog.ShowDialog();

			if (result == true)
			{
				string fileName = dialog.FileName;
				FileStream filestream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
				_researchBase.SetImageData(fileName);
				var bitmapImage = _researchBase.GetImageData(0, (ulong)filestream.Length);
				TabInputData.Items.Add(AddNewInputImageTabItem(bitmapImage));
			}
		}

		private void b_GenerateRandomText_Click(object sender, RoutedEventArgs e)
		{
			_researchBase.SetTextData(nud_WordCount.uValue, nud_MinTextCount.uValue, nud_MaxTextCount.uValue);

		}

		private void b_ChangeThreadNumbers_Click(object sender, RoutedEventArgs e)
		{
			lb_threads.ThreadNumber = (int)nud_ThreadNumbersListCount.Value;
		}

		private void FillProcessingValues(int index)
		{
			string processingValues;
			processingValues = _researchBase.GetProcessingValues(index);

			TextBlock tb = new TextBlock();
			tb.TextWrapping = TextWrapping.Wrap;
			tb.Text = processingValues.Replace("%%", ", ");
			sv_ProcessingData.Content = tb;
		}

		private void cb_ProcessingDataList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			int algorithmIndex = int.Parse(cb_ProcessingDataList.Items[cb_ProcessingDataList.SelectedIndex].ToString().Split(" ")[0]);
			FillProcessingValues(algorithmIndex);
		}
	}
}
