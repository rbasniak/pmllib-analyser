using RbkUtilities.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PmllibAnalyser
{
    public class PmllibFile : BaseViewModel
    {
        private readonly FileInfo _file;
        private List<string> _references;
        private List<string> _udas;
        public PmllibFile(string filename)
        {
            _file = new FileInfo(filename);
        }

        public string Name => _file.Name;

        public string Filename => _file.FullName;
        
        public FileInfo File => _file;
        
        public string Type { get; set; }
        
        public PmllibFolder Parent { get; set; }

        public ObservableCollection<TreeNode> Children
        {
            get
            {
                var udas = new ObservableCollection<TreeNode>(UDAs.Select(x => new TreeNode { Title = x }).OrderBy(x => x.Title));
                var usedBy = new ObservableCollection<TreeNode>(UsedBy.Select(x => new TreeNode { Title = x }).OrderBy(x => x.Title));
                var uses = new ObservableCollection<TreeNode>(References.Select(x => new TreeNode { Title = x }).OrderBy(x => x.Title));

                var children = new ObservableCollection<TreeNode>();

                if (Pmllib.IGNORED_EXTENSIONS.Any(x => x == File.Extension.ToLower()))
                {
                    return null;
                }

                if (udas.Count > 0)
                {
                    children.Add(new TreeNode
                    {
                        Title = "UDAs/UDETs",
                        Children = udas
                    });
                }

                if (usedBy.Count > 0)
                {
                    children.Add(new TreeNode
                    {
                        Title = "Used by",
                        Children = usedBy
                    });
                };

                if (uses.Count > 0)
                {
                    children.Add(new TreeNode
                    {
                        Title = "Uses",
                        Children = uses
                    });
                };

                return children;
            }
        }

        public bool Expanded
        {
            get { return GetPropertyValue<bool>(); }
            set { SetPropertyValue(value); }
        }

        public bool IsMigrated
        {
            get
            {
                var parent = Parent;

                while (parent != null)
                {
                    if (parent.Name.StartsWith("ITR"))
                    {
                        return true;
                    }

                    if (parent.Parent == null)
                    {
                        return false;
                    }

                    parent = parent.Parent;
                }

                return false;
            }
        }

        public List<string> UDAs
        {
            get
            {
                if (_udas == null)
                {
                    ProcessFile();
                }

                return _udas;
            }
        }

        public List<string> UsedBy { get; set; }

        public List<string> References
        {
            get
            {
                if (_references == null)
                {
                    ProcessFile();
                }

                return _references;
            }
        } 

        public void ProcessFile()
        {
            if (Pmllib.IGNORED_EXTENSIONS.Contains(_file.Extension.ToLower()))
            {
                _udas = new List<string>();
                _references = new List<string>();
                return;
            }

            var results = new List<string>();
            var lines = System.IO.File.ReadAllLines(Filename);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim().StartsWith("$("))
                {
                    while (!lines[i].Contains("$)"))
                    {
                        i++;
                    }
                }
                else if (lines[i].Trim().ToLower().StartsWith("$p"))
                {

                }
                else if (lines[i].Trim().ToLower().StartsWith("define method"))
                {

                }
                else if (lines[i].Trim().ToLower().StartsWith("define object"))
                {

                }
                else if (lines[i].Trim().ToLower().StartsWith("define function"))
                {

                }
                else if (lines[i].Trim().ToLower().StartsWith("setup form"))
                {

                }
                else if (lines[i].Trim().ToLower().StartsWith("--"))
                {

                }
                else if (lines[i].Trim().ToLower().StartsWith("$*"))
                {

                }
                else
                {
                    var temp = lines[i].Split(new string[] { "$*" }, StringSplitOptions.RemoveEmptyEntries);

                    if (temp.Length > 1)
                    {
                        results.Add(temp[0]);
                    }
                    else
                    {
                        results.Add(lines[i]);
                    }
                }
            }

            var file = String.Join("\n", results);

            var regex = new Regex(@"!![a-z0-9]*", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var matches1 = regex.Matches(file).Select(x => x.Value.Replace("!!", "")).Where(x => x.Length > 1).Distinct().ToList();

            regex = new Regex(@"\bobject [a-z0-9]*", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var matches2 = regex.Matches(file).Select(x => x.Value.Substring(7, x.Value.Length - 7)).Where(x => x.Length > 1).Distinct().ToList();

            //regex = new Regex(@"\bis [a-z0-9]*", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            //var matches3 = regex.Matches(file).Select(x => x.Value.Substring(3, x.Value.Length - 3)).Where(x => x.Length > 1).Distinct().ToList();

            var references = matches1.ToList();
            references.AddRange(matches2);
            // references.AddRange(matches3);

            var ignored = new[] { "CE", "ALERT", "STRING", "REAL", "BOOLEAN", "FILE", "ARRAY" };

            references = references.Select(x => x.ToUpper()).Where(x => !ignored.Contains(x)).Distinct().ToList();

            _references = references;

            regex = new Regex(@"\B:[a-z0-9_-]*", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            var matches = regex.Matches(file).Select(x => x.Value.ToUpper()).Where(x => x.Length > 1).Where(x => !Int32.TryParse(x.Replace(":", ""), out _)).Distinct().ToList();

            _udas = matches;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
