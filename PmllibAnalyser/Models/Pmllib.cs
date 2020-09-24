using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PmllibAnalyser
{
    public class Pmllib
    {
        public static string[] IGNORED_EXTENSIONS = new[]
        {
            ".png",
            ".jpeg",
            ".jpg",
            ".index",
            ".pdf",
            ".db",
            ".docx",
            ".rtf",
            ".txt",
            ".bak",
            ".old",
            ".bck",
            ".dll",
            ".xml",
            ".pdb",
            ".lic",
            ".exe",
            ".xlsx",
            ".test",
            ".ico",
            ".dat",
            ".sln",
            ".cs",
            ".csproj",
            ".pptx",
            ".resx",
            ".xaml",
            ".settings",
            ".suo",
            ".lock",
            ".ide",
        };

        public Pmllib()
        {
            Root = new PmllibFolder("PMLLIB");
            Root.Expanded = true;

            Files = new List<PmllibFile>();
        }

        public PmllibFolder Root { get; set; }
        
        public List<PmllibFile> Files { get; set; }
        
        public List<Uda> UsedUDAs
        {
            get
            {
                var udas = Files.SelectMany(x => x.UDAs).Distinct().Select(x => new Uda(x)).ToList();

                foreach (var uda in udas)
                {
                    uda.UsedBy = Files.Where(x => x.UDAs.Contains(uda.Name)).ToList();
                }

                return udas;
            }
        } 

        public List<string> ExistingExtensions => Files.Select(x => x.File.Extension).Where(x => !IGNORED_EXTENSIONS.Contains(x.ToLower())).Distinct().ToList();

        public void FindReferences()
        {
            foreach (var file in Files)
            {
                if (file.File.Extension != "")
                {
                    file.UsedBy = Files.Where(x => x.References
                        .Any(x => x.ToLower() == file.Name.ToLower().Replace(file.File.Extension.ToLower(), ""))).Select(x => x.Name).ToList();
                }
                else
                {
                    file.UsedBy = Files.Where(x => x.References
                        .Any(x => x.ToLower() == file.Name.ToLower())).Select(x => x.Name).ToList();
                }
            }
        }
    }
}
