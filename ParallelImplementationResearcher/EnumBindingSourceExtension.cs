using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace ParallelImplementationResearcher
{
	public class EnumBindingSourceExtension : MarkupExtension
	{
		public Type EnumType { get; private set; }

		public EnumBindingSourceExtension(Type enumType)
		{
			EnumType = enumType;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Enum.GetValues(EnumType);
		}
	}
}
