﻿using System;
using System.Runtime.InteropServices;
using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Hosting.Pins.Config
{
    [ComVisible(false)]
	public class DynamicEnumConfigPin : ConfigPin<EnumEntry>
	{
		protected IEnumConfig FEnumConfigPin;
		protected string FEnumName;
		
		public DynamicEnumConfigPin(IPluginHost host, ConfigAttribute attribute)
			: base(host, attribute)
		{
			FEnumName = attribute.EnumName;
			
			host.CreateEnumConfig(attribute.Name, (TSliceMode)attribute.SliceMode, (TPinVisibility)attribute.Visibility, out FEnumConfigPin);
			FEnumConfigPin.SetSubType(FEnumName);
			
			base.Initialize(FEnumConfigPin);
		}
		
		public override EnumEntry this[int index]
		{
			get
			{
				int ord;
				FEnumConfigPin.GetOrd(index, out ord);

				return new EnumEntry(FEnumName, ord);
			}
			set
			{
				FEnumConfigPin.SetOrd(index, value.Index);
			}
		}
	}
}
