using ImageDivider.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageDivider
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel _viewModel;
        Point oldMousePosition = new Point();

        public MainWindow()
        {
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("FR-fr");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("FR-fr");
            InitializeComponent();
        }

        private void StackPanel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void StackPanel_DropImage(object sender, DragEventArgs e)
        {
            string[] dropObject = (string[])e.Data.GetData(DataFormats.FileDrop);
            List<string> files = new List<string>();
            string[] filters = new string[] { ".png", ".gif", ".jpg", ".jpeg", ".tif", ".tiff" };

            if (dropObject.Length == 1)
            {
                FileAttributes attr = File.GetAttributes(dropObject[0]);
                FileInfo fi = new FileInfo(dropObject[0]);
                if (filters.Contains(fi.Extension.ToLower()))
                {
                    Img.Source = new BitmapImage(new Uri(dropObject[0], UriKind.Absolute));
                    BitmapSource src = (BitmapSource)Img.Source;
                    _viewModel.OriginalImageWidth = src.PixelWidth;
                    _viewModel.OriginalImageHeight = src.PixelHeight;
                    _viewModel.IsImageDropped = true;
                    _viewModel.ScrollViewerWidth = Grd.ActualWidth - 10;
                    _viewModel.ScrollViewerHeight = Grd.RowDefinitions[1].ActualHeight - 10;
                    _viewModel.ChangeImageSize();
                    _viewModel.DrawGridCommand(ImgContainer);
                }
            }
        }

        private void ImgContainer_MouseMove(object sender, MouseEventArgs e)
        {
            Point newMousePosition = Mouse.GetPosition((Canvas)sender);
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (newMousePosition.Y < oldMousePosition.Y) ImgScrollViewer.ScrollToVerticalOffset(ImgScrollViewer.VerticalOffset + 1);
                if (newMousePosition.Y > oldMousePosition.Y) ImgScrollViewer.ScrollToVerticalOffset(ImgScrollViewer.VerticalOffset - 1);
                if (newMousePosition.X < oldMousePosition.X) ImgScrollViewer.ScrollToHorizontalOffset(ImgScrollViewer.HorizontalOffset + 1);
                if (newMousePosition.X > oldMousePosition.X) ImgScrollViewer.ScrollToHorizontalOffset(ImgScrollViewer.HorizontalOffset - 1);
            }
            else
            {
                oldMousePosition = newMousePosition;
            }
        }

        private void ImgContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.SizeAll;
        }

        private void ImgContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ImgContainer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel.IsImageDropped)
            {
                Point p = Mouse.GetPosition(ImgContainer);
                int XPos = _viewModel.XPositions.Where(x => (x.From <= p.X && x.To >= p.X)).Select(x => x.Number).Single();
                int YPos = _viewModel.YPositions.Where(x => (x.From <= p.Y && x.To >= p.Y)).Select(x => x.Number).Single();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("Colonne : {0}", XPos));
                sb.AppendLine(string.Format("Ligne : {0}", YPos));
                MessageBox.Show(sb.ToString());
            }
        }
    }
}
