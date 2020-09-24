using Ookii.Dialogs.Wpf;
using RbkUtilities.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PmllibAnalyser
{
    public class MainViewModel: BaseViewModel
    {
        private readonly PmllibAnalyserService _service;

        public MainViewModel()
        {
            Folders = new ObservableCollection<PmllibFolder>();
            LoadButtonEnabled = true;
            LoadingVisibility = Visibility.Hidden;
            TabsVisibility = Visibility.Hidden;
        }

        public MainViewModel(Pmllib pmllib)
        {
            Folders = new ObservableCollection<PmllibFolder> { pmllib.Root };
            Udas = new ObservableCollection<Uda>(pmllib.UsedUDAs.OrderBy(x => x.Name));
        }

        public string MigrationRate
        {
            get { return GetPropertyValue<string>(); }
            set { SetPropertyValue(value); }
        }

        public ObservableCollection<PmllibFolder> Folders
        {
            get { return GetPropertyValue<ObservableCollection<PmllibFolder>>(); }
            set { SetPropertyValue(value); }
        }

        public ObservableCollection<Uda> Udas
        {
            get { return GetPropertyValue<ObservableCollection<Uda>>(); }
            set { SetPropertyValue(value); }
        }

        public string PmllibPath
        {
            get { return GetPropertyValue<string>(); }
            set { SetPropertyValue(value); }
        }

        public string TotalFiles
        {
            get { return GetPropertyValue<string>(); }
            set { SetPropertyValue(value); }
        }

        public string TotalCodeFiles
        {
            get { return GetPropertyValue<string>(); }
            set { SetPropertyValue(value); }
        }

        public string MigratedFiles
        {
            get { return GetPropertyValue<string>(); }
            set { SetPropertyValue(value); }
        }

        public Visibility LoadingVisibility
        {
            get { return GetPropertyValue<Visibility>(); }
            set { SetPropertyValue(value); }
        }

        public Visibility TabsVisibility
        {
            get { return GetPropertyValue<Visibility>(); }
            set { SetPropertyValue(value); }
        }

        public bool LoadButtonEnabled
        {
            get { return GetPropertyValue<bool>(); }
            set { SetPropertyValue(value); }
        }

        public ICommand LoadFolderCommand => new RelayCommand(parameter =>
        {
            var ofd = new VistaFolderBrowserDialog();

            if (!String.IsNullOrEmpty(PmllibPath))
            {
                var path = Path.GetDirectoryName(PmllibPath);
                ofd.SelectedPath = Directory.Exists(path) ? path : null;
            }

            if (ofd.ShowDialog() == true)
            {
                PmllibPath = ofd.SelectedPath;

                var service = new PmllibAnalyserService(PmllibPath);

                Task.Factory.StartNew(() =>
                {
                    LoadButtonEnabled = false;
                    LoadingVisibility = Visibility.Visible;
                    TabsVisibility = Visibility.Hidden;

                    var pmllib = service.Load();
                    Folders = new ObservableCollection<PmllibFolder> { pmllib.Root };
                    Udas = new ObservableCollection<Uda>(pmllib.UsedUDAs);

                    var migrated = pmllib.Files.Where(x => x.IsMigrated).Count();
                    MigratedFiles = migrated.ToString();
                    TotalFiles = pmllib.Files.Count().ToString();
                    TotalCodeFiles = pmllib.Files.Where(x => Pmllib.IGNORED_EXTENSIONS.Any(e => e == x.File.Extension.ToLower()) || x.File.Extension == "").Count().ToString();
                    MigrationRate = ((double)migrated / pmllib.Files.Count() * 100.0).ToString("0.00");
                })
                .ContinueWith((t, _) =>
                {
                    LoadButtonEnabled = true;
                    LoadingVisibility = Visibility.Hidden;
                    TabsVisibility = Visibility.Visible;
                }
        , null,
        TaskScheduler.FromCurrentSynchronizationContext());
            }
        });
    }
}
