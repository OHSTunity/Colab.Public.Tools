using Starcounter;
using Starcounter.Advanced;
using System.Linq;
using System;
using System.Collections.Generic;



namespace Colab.Public
{
    [MenuItem_json]
    partial class MenuItem : Json
    {
        protected override void OnData()
        {
        }

        public Type PageType;

        public Boolean HasChildrenCB
        {
            get { return Children.Count() > 0; }
        }

        void Handle(Input.Morph input)
        {
            Master.SendCommand(ColabCommand.MORPH_URL, Url);
        }

        /// <summary>
        /// Level is internal to avoid programming bugs
        /// creating a infinity loopback
        /// </summary>
        private int _level = 0;

        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }
    }
}
