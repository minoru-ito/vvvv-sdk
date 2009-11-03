using System;
using System.Collections.Generic;
using System.Text;
using VVVV.PluginInterfaces.V1;
using VVVV.Webinterface.Utilities;
using VVVV.Webinterface.jQuery;

namespace VVVV.Nodes.HttpGUI
{
    class Draggable : JQueryNode, IPlugin, IDisposable
    {
    	
    	#region field declaration 

        private bool FDisposed = false;

        private IEnumIn FAxisEnumInput;
		private IEnumIn FRevertEnumInput;

        private INodeIn FOnStopNodeInput;
        private IJQueryIO FUpstreamOnStopNodeInterface;
		protected JQueryNodeIOData FUpstreamOnStopNodeData = null;
		private JQueryExpression FUpstreamOnStopExpression;

		private IEnumIn FOnStopApplyTo;

		private INodeIn FOnStopSelectorNodeInput;
		private IHttpGUIIO FUpstreamOnStopSelectorNodeInterface;
		private List<string> FUpstreamOnStopSelectorNodeIds = null;

        private bool FOnStopNodeInputEventThisFrame;
		private bool FOnStopSelectorNodeInputEventThisFrame;
		
		protected bool FInputNodePinChangedThisFrame;
		protected bool FOnStopSelectorNodeChangedThisFrame;

        private JavaScriptGenericObject FDraggableArguments;

		private JQuery FOnStopHandlerBody;
		private JavaScriptAnonymousFunction FOnStopHandler;
		private MethodCall FBindOnStopHandler;


        #endregion field declaration


        #region constructor/destructor

        /// <summary>
        /// the nodes constructor
        /// nothing to declare for this node
        /// </summary>
        public Draggable()
        {
            FExpression = JQueryExpression.This();
            FDraggableArguments = new JavaScriptGenericObject();
            FExpression.ApplyMethodCall("draggable", FDraggableArguments);

			FOnStopHandlerBody = new JQuery();
			FOnStopHandler = new JavaScriptAnonymousFunction(FOnStopHandlerBody, "event", "ui");

			FBindOnStopHandler = new MethodCall("bind", "dragstop", FOnStopHandler);
			FBindOnStopHandler.DoInclude = false;

			FExpression.AddChainedOperation(FBindOnStopHandler);
        }

        /// <summary>
        /// Implementing IDisposable's Dispose method.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        /// 

            #region Dispose

            public void Dispose()
            {
                Dispose(true);
                // Take yourself off the Finalization queue
                // to prevent finalization code for this object
                // from executing a second time.
                GC.SuppressFinalize(this);
            }


            /// <summary>
            /// Dispose(bool disposing) executes in two distinct scenarios.
            /// If disposing equals true, the method has been called directly
            /// or indirectly by a user's code. Managed and unmanaged resources
            /// can be disposed.
            /// If disposing equals false, the method has been called by the
            /// runtime from inside the finalizer and you should not reference
            /// other objects. Only unmanaged resources can be disposed.
            /// </summary>
            /// <param name="disposing"></param>
            protected virtual void Dispose(bool disposing)
            {
                // Check to see if Dispose has already been called.
                if (FDisposed == false)
                {
                    if (disposing)
                    {
                        // Dispose managed resources.
                    }
                    // Release unmanaged resources. If disposing is false,
                    // only the following code is executed.
                    //mWebinterfaceSingelton.DeleteNode(mObserver);
                    FHost.Log(TLogType.Message, FPluginInfo.Name.ToString() + "(Http Gui) Node is being deleted");

                    // Note that this is not thread safe.
                    // Another thread could start disposing the object
                    // after the managed resources are disposed,
                    // but before the disposed flag is set to true.
                    // If thread safety is necessary, it must be
                    // implemented by the client.
                }

                FDisposed = true;
            }


            /// <summary>
            /// Use C# destructor syntax for finalization code.
            /// This destructor will run only if the Dispose method
            /// does not get called.
            /// It gives your base class the opportunity to finalize.
            /// Do not provide destructors in WebTypes derived from this class.
            /// </summary>
        ~Draggable()
            {
                // Do not re-create Dispose clean-up code here.
                // Calling Dispose(false) is optimal in terms of
                // readability and maintainability.
                Dispose(false);
            }

            #endregion dispose

        #endregion constructor/destructor


        #region Pugin Information

        public static IPluginInfo FPluginInfo;

        public static IPluginInfo PluginInfo
        {
            get
            {
                if (FPluginInfo == null)
                {
                    //fill out nodes info
                    //see: http://www.vvvv.org/tiki-index.php?page=Conventions.NodeAndPinNaming
                    FPluginInfo = new PluginInfo();

                    //the nodes main name: use CamelCaps and no spaces
                    FPluginInfo.Name = "Draggable";
                    //the nodes category: try to use an existing one
                    FPluginInfo.Category = "JQuery";
                    //the nodes version: optional. leave blank if not
                    //needed to distinguish two nodes of the same name and category
                    FPluginInfo.Version = "";

                    //the nodes author: your sign
                    FPluginInfo.Author = "iceberg";
                    //describe the nodes function
                    FPluginInfo.Help = "Node for adding the JQuery UI Draggable Interaction to an HTML DOM element";
                    //specify a comma separated list of tags that describe the node
                    FPluginInfo.Tags = "";

                    //give credits to thirdparty code used
                    FPluginInfo.Credits = "";
                    //any known problems?
                    FPluginInfo.Bugs = "";
                    //any known usage of the node that may cause troubles?
                    FPluginInfo.Warnings = "";



                    //leave below as is
                    System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
                    System.Diagnostics.StackFrame sf = st.GetFrame(0);
                    System.Reflection.MethodBase method = sf.GetMethod();
                    FPluginInfo.Namespace = method.DeclaringType.Namespace;
                    FPluginInfo.Class = method.DeclaringType.Name;
                    //leave above as is
                }

                return FPluginInfo;
            }
        }


        #endregion Plugin Information


        #region pin creation

        protected override void OnSetPluginHost()
        {
            // create required pins
            FHost.CreateEnumInput("Axis", TSliceMode.Single, TPinVisibility.True, out FAxisEnumInput);
            FHost.UpdateEnum("Draggable.Axis", "None", new string[] { "None", "x", "y" });
            FAxisEnumInput.SetSubType("Draggable.Axis");

			FHost.CreateEnumInput("Revert", TSliceMode.Single, TPinVisibility.True, out FRevertEnumInput);
			FHost.UpdateEnum("Draggable.Revert", "false", new string[] { "false", "true", "invalid", "valid"});
			FRevertEnumInput.SetSubType("Draggable.Revert");

            FHost.CreateNodeInput("OnStop", TSliceMode.Single, TPinVisibility.True, out FOnStopNodeInput);
            FOnStopNodeInput.SetSubType(new Guid[1] { JQueryIO.GUID }, JQueryIO.FriendlyName);

			FHost.CreateEnumInput("On Stop Apply To", TSliceMode.Single, TPinVisibility.True, out FOnStopApplyTo);
			FHost.UpdateEnum("Draggable.OnStopApplyTo", "Helper", new string[] { "Helper", "This", "Selector"});
			FOnStopApplyTo.SetSubType("Draggable.OnStopApplyTo");

			FHost.CreateNodeInput("On Stop Selector", TSliceMode.Single, TPinVisibility.True, out FOnStopSelectorNodeInput);
			FOnStopSelectorNodeInput.SetSubType(new Guid[1] { HttpGUIIO.GUID }, HttpGUIIO.FriendlyName);
        }

        #endregion pin creation


        #region Main Loop



		protected override void OnEvaluate(int SpreadMax, bool changedSpreadSize, string NodeId, List<string> SlideId, bool ReceivedNewString, List<string> ReceivedString)
		{
            bool newDataOnOnStopInputSlice = false;
			bool newDataOnOnStopSelectorInputSlice = false;

            if (FOnStopNodeInput.IsConnected && (FOnStopNodeInputEventThisFrame || FUpstreamOnStopNodeInterface.PinIsChanged))
            {
                newDataOnOnStopInputSlice = true;
                for (int i = 0; i < SpreadMax; i++)
                {
                    FUpstreamOnStopNodeData = FUpstreamOnStopNodeInterface.GetJQueryData(i);
					FUpstreamOnStopExpression = FUpstreamOnStopNodeData.BuildChain();
                }
            }

			if (FOnStopSelectorNodeInput.IsConnected && (FOnStopSelectorNodeInputEventThisFrame || FUpstreamOnStopSelectorNodeInterface.PinIsChanged()))
			{
				newDataOnOnStopSelectorInputSlice = true;
				for (int i = 0; i < SpreadMax; i++)
				{
					FUpstreamOnStopSelectorNodeIds = FUpstreamOnStopSelectorNodeInterface.GetAllNodeIds();
				}
			}

            FInputNodePinChangedThisFrame = FOnStopNodeInputEventThisFrame || newDataOnOnStopInputSlice;
			FOnStopSelectorNodeChangedThisFrame = FOnStopSelectorNodeInputEventThisFrame || newDataOnOnStopSelectorInputSlice;

            if (changedSpreadSize || DynamicPinsAreChanged())
			{
                for (int i = 0; i < SpreadMax; i++)
                {
                    #region axis
                    string axisSlice;

                    FAxisEnumInput.GetString(i, out axisSlice);
                    if (axisSlice == "None")
                    {
                        FDraggableArguments.Set("axis", false);
                    }
                    else
                    {
                        FDraggableArguments.Set("axis", axisSlice);
                    } 
                    #endregion

					#region revert
					string revertSlice;

					FRevertEnumInput.GetString(i, out revertSlice);
					
					switch (revertSlice)
					{
						case "false":
							FDraggableArguments.Set("revert", false);
							break;
						case "true":
							FDraggableArguments.Set("revert", true);
							break;
						default:
							FDraggableArguments.Set("revert", revertSlice);
							break;
					}
					#endregion


					#region onStop
					if (FUpstreamOnStopNodeData != null)
					{
						string onStopApplyToSlice;
						FOnStopApplyTo.GetString(i, out onStopApplyToSlice);

						FBindOnStopHandler.DoInclude = true;
						JQueryExpression handlerExpression;
						switch (onStopApplyToSlice)
						{
							case "This":
								handlerExpression = JQueryExpression.This();
								break;
							case "Helper":
								handlerExpression = JQueryExpression.Dollars(FOnStopHandler.Arguments[1].Member("helper"));
								break;
							case "Selector":
								handlerExpression = FUpstreamOnStopSelectorNodeIds != null ? JQueryExpression.Dollars(new CompoundSelector(FUpstreamOnStopSelectorNodeIds)) : JQueryExpression.This();
								break;
							default:
								handlerExpression = JQueryExpression.This();
								break;

						}
						FOnStopHandlerBody.Expression = handlerExpression.Chain(FUpstreamOnStopExpression);
					}
					else
					{
						FBindOnStopHandler.DoInclude = false;
					} 
					#endregion

                }
			}

			FOnStopNodeInputEventThisFrame = false;
		}

        #endregion Main Loop

        protected override bool DynamicPinsAreChanged()
		{
			return (FAxisEnumInput.PinIsChanged || FOnStopApplyTo.PinIsChanged || FRevertEnumInput.PinIsChanged || FInputNodePinChangedThisFrame || FOnStopSelectorNodeChangedThisFrame);
		}

        #region IPluginConnections Members

        public override void ConnectPin(IPluginIO pin)
        {
            base.ConnectPin(pin);
            
            //cache a reference to the upstream interface when the NodeInput pin is being connected
            if (pin == FOnStopNodeInput)
            {
                if (FOnStopNodeInput != null)
                {
                    INodeIOBase upstreamInterface;
                    FOnStopNodeInput.GetUpstreamInterface(out upstreamInterface);
                    FUpstreamOnStopNodeInterface = upstreamInterface as IJQueryIO;
                    FOnStopNodeInputEventThisFrame = true;
                }
            }
			else if (pin == FOnStopSelectorNodeInput)
			{
				if (FOnStopSelectorNodeInput != null)
				{
					INodeIOBase upstreamInterface;
					FOnStopSelectorNodeInput.GetUpstreamInterface(out upstreamInterface);
					FUpstreamOnStopSelectorNodeInterface = upstreamInterface as IHttpGUIIO;
					FOnStopSelectorNodeInputEventThisFrame = true;
				}
			}
        }

        public override void DisconnectPin(IPluginIO pin)
        {
            base.DisconnectPin(pin);
            
            //reset the cached reference to the upstream interface when the NodeInput is being disconnected
            if (pin == FOnStopNodeInput)
            {
                FUpstreamOnStopNodeInterface = null;
                FUpstreamOnStopNodeData = null;
                FOnStopSelectorNodeInputEventThisFrame = true;
            }
			else if (pin == FOnStopSelectorNodeInput)
			{
				FUpstreamOnStopSelectorNodeInterface = null;
				FUpstreamOnStopSelectorNodeIds = null;
				FOnStopSelectorNodeInputEventThisFrame = true;
			}
        }

        #endregion
	}
}