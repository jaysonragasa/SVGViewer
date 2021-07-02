using System.Linq;
using System.Windows;

namespace SVGViewer
{
    /// <summary>
    /// SVGImage - https://github.com/dotnetprojects/SVGImage
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.AllowDrop = true;
            this.Drop += MainWindow_Drop;
        }

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Any())
                {
                    var svg = new SVGFile(files.FirstOrDefault());
                    this.svgCanv.SetImage(svg.SVGImage);

                    lblDragFileHere.Visibility = Visibility.Hidden;
                }
            }
        }
    }
}
