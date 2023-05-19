#pragma once

#include "pluginbase.h"
#include <vcclr.h>

#pragma comment(lib, "ResearchKernel.lib")

using namespace System;
using namespace System::Collections::Generic;
using System::ComponentModel::DescriptionAttribute;

namespace ResearchKernelWrapper {

	ref class EnumDescriptionConverter : public System::ComponentModel::EnumConverter
	{
	private:
		System::Type^ _enumType;
	public:
		EnumDescriptionConverter(System::Type^ type) : System::ComponentModel::EnumConverter(type)
		{
			_enumType = type;
		}

		System::Object^ ConvertTo(System::ComponentModel::ITypeDescriptorContext^ context, System::Globalization::CultureInfo^ culture, System::Object^ value, System::Type^ destType)override
		{
			if (destType == String::typeid) {
				if (value != nullptr) {
					System::Reflection::FieldInfo^ fi = value->GetType()->GetField(value->ToString());
					if (fi != nullptr) {
						auto attributes = (array<DescriptionAttribute^>^)fi->GetCustomAttributes(DescriptionAttribute::typeid, false);
			
						return (attributes->Length > 0 && !String::IsNullOrEmpty(attributes[0]->Description)) ? attributes[0]->Description : value->ToString();
					}
				}
			}
			return __super::ConvertTo(context, culture, value, destType);
		}
	};

	public enum class FunctionInformation {
		Names,
		DataType,
		Description,
		Parameters,
		Title,
		Realisation,
		Developer,
		Version
	};

	[System::ComponentModel::TypeConverter(EnumDescriptionConverter::typeid)]
	public enum class AlphaPercent {
		[Description("90%")]
		percent_90 = 10,
		[Description("95%")]
		percent_95 = 5,
		[Description("99%")]
		percent_99 = 1
	};

	[System::ComponentModel::TypeConverter(EnumDescriptionConverter::typeid)]
	public enum class IntervalTypeValue {
		[Description("Среднеквадратическое отклонение")]
		CD,
		[Description("Коэффициент Стьюдента")]
		Student_Coefficient
	};

	[System::ComponentModel::TypeConverter(EnumDescriptionConverter::typeid)]
	public enum class CalculateValue {
		[Description("Среднее значение")]
		Mean,
		[Description("Медиана")]
		Median,
		[Description("Мода")]
		Mode
	};

	public ref class ResearchBase
	{
	private:
		PluginBase* pb;

	protected:
		!ResearchBase() {
			delete pb;
		}

	public:
		ResearchBase() : pb(new PluginBase) {}
		~ResearchBase() {
			delete pb;
		}

		void ExecuteFunction(int index) {
			int f;
			f = pb->executeFunction(index);
		}

		void AddPlugin(String^ pluginFileName) {
			pin_ptr<const WCHAR> strPluginFileName = PtrToStringChars(pluginFileName);
			pb->addPlugin(strPluginFileName);
		}

		void AddParameterValue(String^ caption, String^ values) {
			pin_ptr<const WCHAR> strCaption = PtrToStringChars(caption);
			pin_ptr<const WCHAR> strValues = PtrToStringChars(values);
			pb->addParameterValue(strCaption, strValues);
		}

		void AddThreadValue(int ThreadNumber) {
			pb->addThreadValue(ThreadNumber);
		}

		void AddThreadValues(List<int>^ ThreadNumbers) {
			for (int i = 0; i < ThreadNumbers->Count; i++) {
				pb->addThreadValue(ThreadNumbers[i]);
			}
		}

		void SetConfidenceIntervalOptions(size_t size, AlphaPercent alphaPercent,
			IntervalTypeValue intervalTypeValue, CalculateValue calculateValue) {
			Alpha alpha = static_cast<Alpha>(alphaPercent);
			IntervalType intervalType = static_cast<IntervalType>(intervalTypeValue);
			CalcValue calcValue = static_cast<CalcValue>(calculateValue);
			pb->setConfidenceIntervalOptions(size, alpha, intervalType, calcValue);
		}

		void AddResearchToDB(String^ researchName) {
			pin_ptr<const WCHAR> strResearchName = PtrToStringChars(researchName);
			pb->addResearchDB(strResearchName);
		}

		void AddResearchInformationToDB() {
			pb->addResearchInformationDB();
		}

		void SetArrayData(String^ dataType, size_t size, int minValue, int maxValue) {
			pin_ptr<const WCHAR> strDataType = PtrToStringChars(dataType);
			pb->setArrayData(strDataType, size, minValue, maxValue);
		}

		void SetArrayData(String^ dataType, size_t size, bool isIncrease) {
			pin_ptr<const WCHAR> strDataType = PtrToStringChars(dataType);
			pb->setArrayData(strDataType, size, isIncrease);
		}

		void SetImageData(String^ fileName) {
			pin_ptr<const WCHAR> strFileName = PtrToStringChars(fileName);
			pb->setImageData(strFileName);
		}

		generic <class T>
		array<T>^ GetArrayData(size_t index, size_t size) {
			array<T>^ arr = gcnew array<T>(size);
			pin_ptr<T> p = &arr[0];
			pb->getDefaultData(index, p);
			return arr;
		}

		generic <class T>
		array<T,2>^ GetMatrixData(size_t index, size_t rowSize, size_t columnSize) {
			array<T, 2>^ matr = gcnew array<T, 2>(rowSize, columnSize);
			pin_ptr<T> p = &matr[0,0];
			pb->getDefaultData(index, p);
			return matr;
		}

		array<unsigned char>^ GetImageData(size_t index, size_t size) {
			array<unsigned char>^ image = gcnew array<unsigned char>(size);
			pin_ptr<unsigned char> p = &image[0];
			pb->getDefaultData(index, p);
			return image;
		}
		
		String^ GetTextData(size_t index, size_t size) {
			array<WCHAR>^ text = gcnew array<WCHAR>(size);
			pin_ptr<WCHAR> p = &text[0];
			pb->getDefaultData(index, p);
			String^ str = gcnew String(text);
			return str;
		}

		void SaveProcessingData(bool needSave) {
			pb->needSaveProcessingData(needSave);
		}

		List<String^>^ GetProcessingData() {
			int number = pb->getProcessingDataCount();
			List<String^>^ processingData = gcnew List<String^>(number);
			String^ data;
			for (int i = 0; i < number; i++) {
				data = gcnew String(pb->getProcessingData(i));
				processingData->Add(data);
			}
			return processingData;
		}

		String^ GetProcessingValues(int index) {
			String^ values = gcnew String(pb->getProcessingValues(index));
			return values;
		}

		void SetMatrixData(String^ dataType, size_t rowSize, size_t columnSize, int minValue, int maxValue) {
			pin_ptr<const WCHAR> strDataType = PtrToStringChars(dataType);
			pb->setMatrixData(strDataType, rowSize, columnSize, minValue, maxValue);
		}

		void SetMatrixData(String^ dataType, size_t rowSize, size_t columnSize, bool isIncrease) {
			pin_ptr<const WCHAR> strDataType = PtrToStringChars(dataType);
			pb->setMatrixData(strDataType, rowSize, columnSize, isIncrease);
		}

		void SetTextData(String^ text, size_t length) {
			pin_ptr<const WCHAR> strText = PtrToStringChars(text);
			pb->setTextData(strText, text->Length);
		}
		
		void SetTextData(size_t wordCount, size_t minWordLength, size_t maxWordLength) {
			pb->setTextData(wordCount, minWordLength, maxWordLength);
		}

		property int GetFunctionCount {
			int get() {
				return pb->getFunctionCount();
			}
		}

		property List<String^>^ GetTitles {
			List<String^>^ get() {
				int number = GetFunctionCount;
				List<String^>^ titles = gcnew List<String^>(number);
				String^ title, ^ realisation, ^version;
				
				for (int i = 0; i < number; i++) {
					title = gcnew String(pb->getFunctionInformation(i, FunctionString::Title));
					realisation = gcnew String(pb->getFunctionInformation(i, FunctionString::Realisation));
					version = gcnew String(pb->getFunctionInformation(i, FunctionString::functionVersion));
					titles->Add(title + " " + realisation + " ver." + version);
				}
				return titles;
			}
		}

		property List<String^>^ GetResearchList {
			List<String^>^ get() {
				int number = pb->getResearchListCount();
				List<String^>^ researchList = gcnew List<String^>(number);
				String^ research;
				for (int i = 0; i < number; i++) {
					research = gcnew String(pb->getResearchListRow(i));
					researchList->Add(research);
				}
				return researchList;
			}
		}

		property int GetResearchListCount {
			int get() {
				return pb->getResearchListCount();
			}
		}

		List<String^>^ GetDataResearch(int index) {
			int number = pb->getDataResearchCount(index);
			List<String^>^ dataResearch = gcnew List<String^>(number);
			String^ data;
			for (int i = 0; i < number; i++) {
				data = gcnew String(pb->getDataResearchRow(i, index));
				dataResearch->Add(data);
			}
			return dataResearch;
		}

		List<String^>^ GetDataParameters(int index) {
			int number = pb->getDataParametersCount(index);
			List<String^>^ dataResearch = gcnew List<String^>(number);
			String^ data;
			for (int i = 0; i < number; i++) {
				data = gcnew String(pb->getDataParametersRow(i, index));
				dataResearch->Add(data);
			}
			return dataResearch;
		}

		List<String^>^ GetAlgorithmEvaluation(int index) {
			int number = pb->getAlgorithmEvaluationCount(index);
			List<String^>^ algorithmEvaluation = gcnew List<String^>(number);
			String^ algorithm;
			for (int i = 0; i < number; i++) {
				algorithm = gcnew String(pb->getAlgorithmEvaluationRow(i, index));
				algorithmEvaluation->Add(algorithm);
			}
			return algorithmEvaluation;
		}
		
		List<String^>^ GetCurrentDataResearch() {
			int number = pb->getCurrentDataResearchCount();
			List<String^>^ dataResearch = gcnew List<String^>(number);
			String^ data;
			for (int i = 0; i < number; i++) {
				data = gcnew String(pb->getCurrentDataResearchRow(i));
				dataResearch->Add(data);
			}
			return dataResearch;
		}

		List<String^>^ GetCurrentDataParameters(int index) {
			int number = pb->getCurrentDataParametersCount(index - 1);
			List<String^>^ dataResearch = gcnew List<String^>(number);
			String^ data;
			for (int i = 0; i < number; i++) {
				data = gcnew String(pb->getCurrentDataParametersRow(i, index - 1));
				dataResearch->Add(data);
			}
			return dataResearch;
		}

		List<String^>^ GetCurrentAlgorithmEvaluation(int index) {
			int number = pb->getCurrentAlgorithmEvaluationCount(index - 1);
			List<String^>^ algorithmEvaluation = gcnew List<String^>(number);
			String^ algorithm;
			for (int i = 0; i < number; i++) {
				algorithm = gcnew String(pb->getCurrentAlgorithmEvaluationRow(i, index - 1));
				algorithmEvaluation->Add(algorithm);
			}
			return algorithmEvaluation;
		}

		String^ GetFunctionInformation(int index, FunctionInformation functionString) {
			FunctionString fs = static_cast<FunctionString>(functionString);
			return gcnew String(pb->getFunctionInformation(index, fs));
		}

		String^ GetFunctionPluginFileName(int index) {
			String^ fileName = gcnew String(pb->getFunctionPluginFileName(index));
			return fileName;
		}
	};
}
