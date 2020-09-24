using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PmllibAnalyser
{
    public class PmllibAnalyserService
    {
        private readonly string _path;

        public PmllibAnalyserService(string path)
        {
            _path = path.EndsWith("\\") ? path : path + "\\";
        }

        public Pmllib Load()
        {
            var pmllib = new Pmllib();

            var files = Directory.GetFiles(_path, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var parts = file.Replace(_path, "").Split("\\", StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 1)
                {
                    var item = new PmllibFile(file);
                    pmllib.Root.Files.Add(item);
                    pmllib.Files.Add(item);
                }
                else
                {
                    var currentFolder = pmllib.Root;
                    for (int i = 0; i < parts.Length; i++)
                    {
                        var foundFolder = currentFolder.Folders.SingleOrDefault(x => x.Name == parts[i]);
                        
                        if (foundFolder != null)
                        {
                            currentFolder = foundFolder;
                        }
                        else
                        {
                            if (i == parts.Length - 1)
                            {
                                var newFile = new PmllibFile(file);
                                currentFolder.AddFile(newFile);
                                pmllib.Files.Add(newFile);
                            }
                            else 
                            {
                                var newFolder = new PmllibFolder(parts[i]);
                                currentFolder.AddFolder(newFolder);
                                currentFolder = newFolder;
                            }
                        }
                    }
                }
            }

            pmllib.FindReferences();

            return pmllib;
        }
    }
}
