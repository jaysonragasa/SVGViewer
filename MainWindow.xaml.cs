using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SVGViewer
{
    /// <summary>
    /// SVGImage - https://github.com/dotnetprojects/SVGImage
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<SVGFile> Files { get; set; } = new ObservableCollection<SVGFile>();

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            this.AllowDrop = true;
            this.Drop += MainWindow_Drop;

            this.lvFiles.SelectionChanged += LvFiles_SelectionChanged;
        }

        private void LvFiles_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var svg = lvFiles.SelectedItem as SVGFile;
            if (svg != null)
            {
                this.svgCanv.SetImage(svg.SVGImage);
                lblDragFileHere.Visibility = Visibility.Hidden;
            }
        }

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Any())
                {
                    this.Files.Clear();
                    
                    foreach (var file in files)
                    {
                        // make sure it is SVG
                        if(Path.GetExtension(file).ToLower() == ".svg")
                        {
                            var svg = new SVGFile(file);
                            this.Files.Add(svg);
                        }
                    }

                    if (this.Files.Any())
                    {
                        this.svgCanv.SetImage(this.Files.FirstOrDefault().SVGImage);
                        lblDragFileHere.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        void NotifyUI([CallerMemberName] string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }
}
