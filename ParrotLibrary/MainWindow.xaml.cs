using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ParrotLibrary
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<DataModel.ParrotItem> parrotItems = new ObservableCollection<DataModel.ParrotItem>();

        private View.ArticleView ArticleView;

        CollectionView view;
        PropertyGroupDescription groupDescription = new PropertyGroupDescription("FN");
        PropertyGroupDescription groupDescription2 = new PropertyGroupDescription("Gender");
    
        public MainWindow()
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            InitializeComponent();
            this.ParrotListView.Items.Clear();

            this.ParrotListView.ItemsSource = parrotItems;
            view = (CollectionView)CollectionViewSource.GetDefaultView(ParrotListView.ItemsSource);
            view.CurrentChanging += View_CurrentChanging;
            view.CurrentChanged += View_CurrentChanged;
            ParrotFromGenderButton.IsChecked = true;
            Load();

        }

        private void View_CurrentChanged(object sender, EventArgs e)
        {
           
        }

        public  async  void Load()
        {
                await Task.Run(() =>
                {
                    
                    var infod = new DirectoryInfo("Data");
                    var files = infod.GetFiles("*.json", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {                           
                                var p = Newtonsoft.Json.JsonConvert.DeserializeObject<ParrotLibrary.DataModel.ParrotItem>(File.ReadAllText(file.FullName));
                                p.Path = file.DirectoryName;
                                p.Image = file.DirectoryName + "\\" + p.Image;
                            try
                            {
                               var imageDpi= new BitmapImage(new Uri(p.Image)).DpiX; ;
                               
                                GC.Collect(0);
                                GC.Collect(1);
                                GC.Collect(2);
                                GC.Collect(3);
                            }
                            catch(Exception ex)
                            {

                                try
                                {
                                    p.Image = file.DirectoryName + "\\" + p.Images[p.Images.Length - 1];
                                }
                                catch(Exception EX) { }
                            }

                                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(() =>
                                {
                                    this.Dispatcher.Thread.Priority = System.Threading.ThreadPriority.BelowNormal;
                                    parrotItems.Add(p);
                                }));
                            
                        }
                        catch(Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                   

                });
        }






        private void ArticleView_ImageSelectedEvent(System.Windows.Media.ImageSource imageSource)
        {
            View.ImageView imageView = new View.ImageView(imageSource);
            FrameData.Navigate(imageView.Content);
            imageView.CloseImageViewEvent += ImageView_CloseImageViewEvent;
        }

        private void ImageView_CloseImageViewEvent()
        {

            FrameData.Navigate(ArticleView.Content);
        }

        private void ArticleView_ClosingEvent()
        {
            FrameData.Visibility = Visibility.Collapsed;
            ParrotListView.Visibility = Visibility.Visible;
        }
        private void ParrotFromABC_Checked(object sender, RoutedEventArgs e)
        {
            ParrotFromGenderButton.IsChecked = false;
            ParrotFromGenderButton.IsEnabled = true;
            ParrotFromABC.IsEnabled = true;
            if (ParrotListView.Items.Count > 0)
                ParrotListView.ScrollIntoView(ParrotListView.Items[0]);

            Task.Run(() =>
            {
              
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                 {
                     view.GroupDescriptions.Clear();                    
                    view.GroupDescriptions.Add(groupDescription);
                    

                 })) ;

            });
            
           

        }

        private void ParrotFromABC_Unchecked(object sender, RoutedEventArgs e)
        {

        }
        private void ParrotFromGenderButton_Checked(object sender, RoutedEventArgs e)
        {
           
            ParrotFromGenderButton.IsEnabled = false;
            ParrotFromABC.IsChecked = false;
            ParrotFromABC.IsEnabled = true;
            if (ParrotListView.Items.Count > 0)
                ParrotListView.ScrollIntoView(ParrotListView.Items[0]);

            Task.Run(() =>
            {

                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                {
                    
                    view.GroupDescriptions.Clear();
                    view.GroupDescriptions.Add(groupDescription2);
                }));
            });

        }

        private void View_CurrentChanging(object sender, System.ComponentModel.CurrentChangingEventArgs e)
        {
        }

        private void ParrotListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ParrotListView.SelectedIndex > -1)
            {
                ParrotListView.Visibility = Visibility.Collapsed;
                FrameData.Visibility = Visibility.Visible;
                FrameData.NavigationUIVisibility = NavigationUIVisibility.Hidden;

               
                ArticleView = new View.ArticleView(ParrotListView.SelectedItem as DataModel.ParrotItem);
                ArticleView.ClosingEvent += ArticleView_ClosingEvent;
                ArticleView.ImageSelectedEvent += ArticleView_ImageSelectedEvent;
                FrameData.Navigate(ArticleView.Content);                
            }
        }

       
    }
}