﻿using System;
using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Hosting.Pins.Input
{
	public class GenericInputPin<T> : DiffPin<T>, IPinUpdater
	{
		protected INodeIn FNodeIn;
		protected IGenericIO FUpstreamInterface;
		protected IGenericIO FDefaultInterface;
		
		public GenericInputPin(IPluginHost host, InputAttribute attribute)
			: base(host, attribute)
		{
			FDefaultInterface = new GenericIO();
			
			host.CreateNodeInput(attribute.Name, (TSliceMode)attribute.SliceMode, (TPinVisibility)attribute.Visibility, out FNodeIn);
			FNodeIn.SetSubType(new Guid[] { typeof(T).GUID }, typeof(T).FullName);
			
			FUpstreamInterface = FDefaultInterface;
			
			base.InitializeInternalPin(FNodeIn);
		}
		
		public override void Connect(IPin otherPin)
		{
			INodeIOBase usI;
			FNodeIn.GetUpstreamInterface(out usI);
			FUpstreamInterface = usI as IGenericIO;
		}
		
		public override void Disconnect(IPin otherPin)
		{
			FUpstreamInterface = FDefaultInterface;
		}
		
		protected override bool IsInternalPinChanged
		{
			get
			{
				return FNodeIn.PinIsChanged;
			}
		}
		
		unsafe protected override void DoUpdate()
		{
			SliceCount = FNodeIn.SliceCount;
			
			for (int i = 0; i < FSliceCount; i++)
			{
				int usS;
				if (FUpstreamInterface != null)
				{
					FNodeIn.GetUpsreamSlice(i, out usS);
					FBuffer[i] = (T) FUpstreamInterface.GetSlice(usS);
				}
				else
				{
					FBuffer[i] = default(T);
				}
			}
		}
	}
}
