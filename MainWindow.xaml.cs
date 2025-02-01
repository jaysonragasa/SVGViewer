using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

        private async void MainWindow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Any())
                {
                    this.Files.Clear();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
						progressBar.IsIndeterminate = true;
						progressBar.Maximum = files.Length;
					});
                    
                    List<SVGFile> svgs = new List<SVGFile>();

					//foreach (var file in files)
					for (int i = 0; i < files.Length; i++)
                    {
                        // make sure it is SVG
                        if (Path.GetExtension(files[i]).ToLower() == ".svg")
                        {
                            var svg = new SVGFile(files[i]);
                            svgs.Add(svg);
                        }

                        //await Application.Current.Dispatcher.InvokeAsync(async () =>
                        //{
                        //    progressBar.Value = i + 1;
                        //    await Task.Delay(1);
                        //});

                        //await Task.Delay(1);
                    }

                    foreach (var file in svgs)
                        this.Files.Add(file);

					await Application.Current.Dispatcher.InvokeAsync(async () =>
					{
						if (svgs.Any())
						{
							this.svgCanv.SetImage(svgs.FirstOrDefault().SVGImage);
							progressMessage.Visibility = Visibility.Hidden;
						}

						progressBar.IsIndeterminate = false;
						await Task.Delay(1);
					});

                    svgs.Clear();
                    svgs = null;
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

        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.svgCanv.Visibility = Visibility.Visible;

            string tosearch = txtSearch.Text;
            if (string.IsNullOrEmpty(tosearch))
            {
                this.svgCanv.Visibility = Visibility.Hidden;
				return;
            }

            var itemtosel = this.Files.Where(x => x.Filename.ToLower().Contains(tosearch)).FirstOrDefault();

            if (itemtosel is null)
            {
				this.svgCanv.Visibility = Visibility.Hidden;
				return;
            };

            lvFiles.SelectedItem = itemtosel;
            lvFiles.ScrollIntoView(itemtosel);
            lvFiles.SelectedValue = itemtosel;
        }
    }
}