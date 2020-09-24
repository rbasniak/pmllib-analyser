using RbkUtilities.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PmllibAnalyser
{
    public class TreeNode : BaseViewModel
    {
        public string Title { get; set; }

        public ObservableCollection<TreeNode> Children { get; set; }

        public bool Expanded
        {
            get { return GetPropertyValue<bool>(); }
            set { SetPropertyValue(value); }
        }
    }
}
