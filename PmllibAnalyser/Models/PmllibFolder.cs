using RbkUtilities.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PmllibAnalyser
{
    public class PmllibFolder: BaseViewModel
    {
        public PmllibFolder(string name)
        {
            Name = name;
            Folders = new ObservableCollection<PmllibFolder>();
            Files = new ObservableCollection<PmllibFile>();
        }
        
        public string Name { get; set; }
        
        public bool IsMigrated
        {
            get
            {
                var parent = this;

                while (parent != null)
                {
                    if (parent.Name.StartsWith("ITR"))
                    {
                        return true;
                    }

                    parent = parent.Parent;
                }

                return false;
            }
        }
        
        public PmllibFolder Parent { get; set; }
        
        public ObservableCollection<PmllibFolder> Folders { get; set; }
        
        public ObservableCollection<PmllibFile> Files { get; set; }
        
        public ObservableCollection<object> Children 
        {
            get
            {
                var results = new ObservableCollection<object>();

                foreach (var item in Folders.OrderBy(x => x.Name))
                {
                    results.Add(item);
                }

                foreach (var item in Files.OrderBy(x => x.Name))
                {
                    if (item.Name.ToLower() != "thumbs.db")
                    {
                        results.Add(item);
                    }
                }

                return results;
            }
        }
        
        public bool Expanded
        {
            get { return GetPropertyValue<bool>(); }
            set { SetPropertyValue(value); }
        }

        public void AddFolder(PmllibFolder folder)
        {
            folder.Parent = this;

            Folders.Add(folder);
        }

        public void AddFile(PmllibFile file)
        {
            file.Parent = this;

            Files.Add(file);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
