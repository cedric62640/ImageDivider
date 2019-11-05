using Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageDivider.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region attributes
        private double imageWidth = 0;
        private double imageHeight = 0;
        private double originalImageWidth = 0;
        private double originalImageHeight = 0;
        private double scrollViewerWidth = 0;
        private double scrollViewerHeight = 0;
        private int numberOfDivider = 8;
        private double zoomValue = 0;
        private double dividerStrokeThickness = 2;
        private bool isImageDropped = false;
        private Color dividerColor = Color.FromRgb(0, 0, 0);
        private bool keepRatio = true;
        private ICommand drawGrid;
        private ICommand changeImageSizeAndDrawGrid;
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        #endregion

        #region properties
        /// <summary>
        /// liste des épaisseurs du quadrillage
        /// </summary>
        public List<double> DividerStrokesThickness { get; set; }

        public List<Position> XPositions { get; set; }

        public List<Position> YPositions { get; set; }

        /// <summary>
        /// Largeur de l'image originale
        /// </summary>
        public double OriginalImageWidth
        {
            get { return originalImageWidth; }
            set
            {
                originalImageWidth = value;
                OnPropertyChanged("OriginalImageWidth");
            }
        }

        /// <summary>
        /// Hauteur de l'image originale
        /// </summary>
        public double OriginalImageHeight
        {
            get { return originalImageHeight; }
            set
            {
                originalImageHeight = value;
                OnPropertyChanged("OriginalImageHeight");
            }
        }

        /// <summary>
        /// Largeur du scroller
        /// </summary>
        public double ScrollViewerWidth
        {
            get { return scrollViewerWidth; }
            set
            {
                scrollViewerWidth = value;
                OnPropertyChanged("ScrollViewerWidth");
            }
        }

        /// <summary>
        /// Hauteur du scroller
        /// </summary>
        public double ScrollViewerHeight
        {
            get { return scrollViewerHeight; }
            set
            {
                scrollViewerHeight = value;
                OnPropertyChanged("ScrollViewerHeight");
            }
        }

        /// <summary>
        /// Largeur de l'image affichée
        /// </summary>
        public double ImageWidth
        {
            get { return imageWidth; }
            set
            {
                imageWidth = value;
                OnPropertyChanged("ImageWidth");
            }
        }

        /// <summary>
        /// Hauteur de l'image affichée
        /// </summary>
        public double ImageHeight
        {
            get { return imageHeight; }
            set
            {
                imageHeight = value;
                OnPropertyChanged("ImageHeight");
            }
        }

        /// <summary>
        /// Nombre de division du quadrillage
        /// </summary>
        public int NumberOfDivider
        {
            get { return numberOfDivider; }
            set
            {
                numberOfDivider = value;
                OnPropertyChanged("NumberOfDivider");
            }
        }

        /// <summary>
        /// Valeur du zoom
        /// </summary>
        public double ZoomValue
        {
            get { return zoomValue; }
            set
            {
                zoomValue = value;
                OnPropertyChanged("ZoomValue");
            }
        }

        /// <summary>
        /// Image déposée ou non
        /// </summary>
        public bool IsImageDropped
        {
            get { return isImageDropped; }
            set
            {
                isImageDropped = value;
                OnPropertyChanged("IsImageDropped");
            }
        }

        /// <summary>
        /// Couleur du quadrillage
        /// </summary>
        public Color DividerColor
        {
            get { return dividerColor; }
            set
            {
                dividerColor = value;
                OnPropertyChanged("DividerColor");
            }
        }

        /// <summary>
        /// Epaisseur de la ligne du quadrillage
        /// </summary>
        public double DividerStrokeThickness
        {
            get { return dividerStrokeThickness; }
            set
            {
                dividerStrokeThickness = value;
                OnPropertyChanged("DividerStrokeThickness");
            }
        }

        /// <summary>
        /// Conserve le ratio pour le quadrillage
        /// </summary>
        public bool KeepRatio
        {
            get { return keepRatio; }
            set
            {
                keepRatio = value;
                OnPropertyChanged("KeepRatio");
            }
        }


        #endregion

        #region commands
        /// <summary>
        /// Commande de traçage du quadrillage
        /// </summary>
        public ICommand DrawGrid
        {
            get
            {
                return drawGrid ?? (drawGrid = new RelayCommand(
                   x =>
                   {
                       DrawGridCommand(x);
                   })
                );
            }
        }

        public ICommand ChangeImageSizeAndDrawGrid
        {
            get
            {
                return changeImageSizeAndDrawGrid ?? (changeImageSizeAndDrawGrid = new RelayCommand(
                   x =>
                   {
                       ChangeImageSizeAndDrawGridCommand(x);
                   })
                );
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public MainWindowViewModel()
        {
            DividerStrokesThickness = new List<double>() { 1, 2, 3 };
        }
        #endregion

        #region publics methods
        /// <summary>
        /// Redimensionnement de l'image
        /// </summary>
        public void ChangeImageSize()
        {
            ImageHeight = OriginalImageHeight + (OriginalImageHeight * ZoomValue / 100);
            ImageWidth = OriginalImageWidth + (OriginalImageWidth * ZoomValue / 100);
            //if (zoomValue <= -50) DividerStrokeThickness = 1;
            //if (zoomValue > -50) DividerStrokeThickness = 2;
        }

        /// <summary>
        /// Traçage du quadrillage
        /// </summary>
        /// <param name="o">Objet sur lequel appliquer le traçage du quadrillage</param>
        public void DrawGridCommand(object o)
        {
            if (o != null)
            {
                Canvas canvas = o as Canvas;

                List<UIElement> elementsToRemove = new List<UIElement>();
                foreach (UIElement element in canvas.Children)
                {
                    Type t = element.GetType();
                    if (t.Name == "Line") elementsToRemove.Add(element);
                }

                foreach (UIElement lineElement in elementsToRemove)
                {
                    canvas.Children.Remove(lineElement);
                }

                double widthInterval = Math.Round(ImageWidth / NumberOfDivider);
                double heightInterval = Math.Round(ImageHeight / NumberOfDivider);

                if (KeepRatio)
                {
                    if (widthInterval > heightInterval)
                    {
                        heightInterval = widthInterval;
                    }
                    else if (heightInterval > widthInterval)
                    {
                        widthInterval = heightInterval;
                    }
                }

                double interval = widthInterval;

                XPositions = new List<Position>();
                YPositions = new List<Position>();
                double lastXPosition = 0;
                double lastYPosition = 0;

                //lignes verticales
                for (int i = 1; i <= NumberOfDivider; i++)
                {
                    Line line = new Line();
                    line.Stroke = new SolidColorBrush(DividerColor);

                    line.X1 = interval;
                    line.X2 = interval;
                    line.Y1 = 0;
                    line.Y2 = ImageHeight;

                    XPositions.Add(new Position() { From = lastXPosition, To = interval, Number = i });
                    lastXPosition = interval;

                    line.StrokeThickness = DividerStrokeThickness;
                    canvas.Children.Add(line);
                    interval += widthInterval;

                    if (interval > ImageWidth)
                    {
                        XPositions.Add(new Position() { From = lastXPosition, To = ImageWidth, Number = i + 1 });
                        lastXPosition = interval;
                        break;
                    }
                }

                //lignes horizontales
                interval = heightInterval;
                for (int i = 1; i <= NumberOfDivider; i++)
                {
                    Line line = new Line();
                    line.Stroke = new SolidColorBrush(DividerColor);

                    line.X1 = 0;
                    line.X2 = ImageWidth;
                    line.Y1 = interval;
                    line.Y2 = interval;

                    YPositions.Add(new Position() { From = lastYPosition, To = interval, Number = i });
                    lastYPosition = interval;

                    line.StrokeThickness = DividerStrokeThickness;
                    canvas.Children.Add(line);
                    interval += heightInterval;
                    if (interval > ImageHeight)
                    {
                        YPositions.Add(new Position() { From = lastYPosition, To = ImageHeight, Number = i + 1 });
                        lastYPosition = interval;
                        break;
                    }
                }
            }
        }
        #endregion

        #region privates methods
        /// <summary>
        /// Redimensionnement de l'image et traçage du quadrillage
        /// </summary>
        /// <param name="o">Objet sur lequel appliquer le traçage du quadrillage</param>
        private void ChangeImageSizeAndDrawGridCommand(object o)
        {
            ChangeImageSize();
            DrawGridCommand(o);
        }
        #endregion

        #region structs
        public struct Position
        {
            public int Number { get; set; }
            public double From { get; set; }
            public double To { get; set; }
        }
        #endregion
    }
}
