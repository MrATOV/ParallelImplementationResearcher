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
		ResearchBase() : pb(new PluginBase){}
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

		void AddParameterValue(String^ values) {
			pin_ptr<const WCHAR> strValues = PtrToStringChars(values);
			pb->addParameterValue(strValues);
		}

		void AddThreadValue(int ThreadNumber) {
			pb->addThreadValue(ThreadNumber);
		}

		void SetResearchName(String^ researchName) {
			pin_ptr<const WCHAR> strResearchName = PtrToStringChars(researchName);
			pb->setResearchName(strResearchName);
		}

		void SetConfidenceIntervalOptions(size_t size, AlphaPercent alphaPercent, 
			IntervalTypeValue intervalTypeValue, CalculateValue calculateValue) {
			Alpha alpha = static_cast<Alpha>(alphaPercent);
			IntervalType intervalType = static_cast<IntervalType>(intervalTypeValue);
			CalcValue calcValue = static_cast<CalcValue>(calculateValue);
			pb->setConfidenceIntervalOptions(size, alpha, intervalType, calcValue);
		}

		void SetArrayData(String^ dataType, size_t size, int minValue, int maxValue) {
			pin_ptr<const WCHAR> strDataType = PtrToStringChars(dataType);
			pb->setArrayData(strDataType, size, minValue, maxValue);
		}

		void SetArrayData(String^ dataType, size_t size, bool isIncrease) {
			pin_ptr<const WCHAR> strDataType = PtrToStringChars(dataType);
			pb->setArrayData(strDataType, size, isIncrease);
		}

		void SetMatrixData(String^ dataType, size_t rowSize, size_t columnSize, int minValue, int maxValue) {
			pin_ptr<const WCHAR> strDataType = PtrToStringChars(dataType);
			pb->setMatrixData(strDataType, rowSize, columnSize, minValue, maxValue);
		}

		void SetMatrixData(String^ dataType, size_t rowSize, size_t columnSize, bool isIncrease) {
			pin_ptr<const WCHAR> strDataType = PtrToStringChars(dataType);
			pb->setMatrixData(strDataType, rowSize, columnSize, isIncrease);
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

		String^ GetFunctionInformation(int index, FunctionInformation functionString) {
			FunctionString fs = static_cast<FunctionString>(functionString);
			return gcnew String(pb->getFunctionInformation(index, fs));
		}
	};
}
