using System;
using System.Collections.Generic;
using System.Text;
using VVVV.Webinterface.Utilities;

namespace VVVV.Nodes.HttpGUI
{
    public class GuiDataObject
    {

        public enum Position
        {
            Head = 0,
            Body = 1,
            JavaScript = 2,
        }



        private SortedList<string, string> mCssProperties = new SortedList<string, string>();
        private List<GuiDataObject> mGuiUpstreamList;
        private SortedList<string,string> mTransform = new SortedList<string,string>();
        private string mNodeId = "";
        private string mSliceId = "";
        private StringBuilder mHead;
        private StringBuilder mBody;
        private Tag mTag;
        private StringBuilder mJavaScript;



        public Tag Tag
        {
            get
            {
                return mTag;
            }
            set
            {
                mTag = value;
            }
        }



        public string Head
        {
            get
            {
                if (mHead != null)
                {
                    return mHead.ToString();
                }
                else
                {
                    return ""; 
                }
            }
        }

        public StringBuilder JavaScript
        {
            get
            {
                return mJavaScript;
            }
        }


        public string Body
        {
            get
            {
                if (mBody != null)
                {
                    return mBody.ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public string NodeId
        {
            get
            {
                return mNodeId;
            }
            set
            {
                mNodeId = value;
            }
        }

        public string SliceId
        {
            get
            {
                return mSliceId;
            }
            set
            {
                mSliceId = value;
            }
        }

        public SortedList<string, string> CssProperties
        {
            get
            {
                return mCssProperties;
            }
            set
            {
                mCssProperties = value;
            }
        }

        public List<GuiDataObject> GuiUpstreamList
        {
            get
            {
                return mGuiUpstreamList;
            }
            set
            {
                mGuiUpstreamList = value;
            }
        }

        public SortedList<string, string> Transform
        {
            get
            {
                return mTransform;
            }
            set
            {
                mTransform = value;
            }
        }



        public void AddString(string pContent, Position pPosition, bool Reset)
        {
            if (Reset)
            {
                mHead = new StringBuilder();
                mBody = new StringBuilder();
                mJavaScript = new StringBuilder();
            }

            if (pPosition == Position.Head)
            {
                mHead.Append(pContent);
            }
            else if(pPosition == Position.Body)
            {
                mBody.Append(pContent);
            }else if(pPosition == Position.JavaScript)
            {
                mJavaScript.Append(pContent);
            }
        }

    }

}
