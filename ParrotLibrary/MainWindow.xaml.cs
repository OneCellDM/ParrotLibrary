using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        bool Loadoc = false;
        private ObservableCollection<DataModel.ParrotItem> parrotItems = new ObservableCollection<DataModel.ParrotItem>();
        private ObservableCollection<DataModel.ParrotItem> parrotItemsViewColletion = new ObservableCollection<DataModel.ParrotItem>();
        private Thread LoadDataToListThread;
        private View.ArticleView ArticleView;
        private CollectionView view;
        private PropertyGroupDescription groupDescription = new PropertyGroupDescription("FN");
        private PropertyGroupDescription groupDescription2 = new PropertyGroupDescription("Gender");
        private PropertyGroupDescription groupDescription3 = new PropertyGroupDescription("DefaultGroup");

        public MainWindow()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            InitializeComponent();
            FrameData.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            this.ParrotListView.Items.Clear();

            this.ParrotListView.ItemsSource = parrotItemsViewColletion;
            view = (CollectionView)CollectionViewSource.GetDefaultView(ParrotListView.ItemsSource);
            view.CurrentChanging += View_CurrentChanging;
            view.CurrentChanged += View_CurrentChanged;

            var Awaiter = Load().GetAwaiter();
            Awaiter.OnCompleted(() =>
            {
                LoadinPanel.Visibility = Visibility.Collapsed;
                ParrotFromGenderButton.IsChecked = true;
            });
        }

        private void View_CurrentChanged(object sender, EventArgs e)
        {}
        public async Task Load()
        {
            await Task.Run(() =>
            {
                var infod = new DirectoryInfo("Data");
                var files = infod.GetFiles("*.json", SearchOption.AllDirectories);
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(() =>
                {
                    LoadingProgressBar.Maximum = files.Count();
                }));
                foreach (var file in files)
                {
                    try
                    {
                        var p = Newtonsoft.Json.JsonConvert.DeserializeObject<ParrotLibrary.DataModel.ParrotItem>(File.ReadAllText(file.FullName));
                        p.Path = file.DirectoryName;
                        p.Image = file.DirectoryName + "\\" + p.Image;
                        try
                        {
                            var imageDpi = new BitmapImage(new Uri(p.Image)).DpiX; ;
                            GC.Collect(0);
                            GC.Collect(1);
                            GC.Collect(2);
                            GC.Collect(3);
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                p.Image = file.DirectoryName + "\\" + p.Images[p.Images.Length - 1];
                            }
                            catch (Exception EX) { }
                        }
                        this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                        {   parrotItems.Add(p);
                            LoadingProgressBar.Value++;
                        }));
                    }
                    catch (Exception ex)
                    {}
                    Thread.Sleep(100);
                    
                }
                Loadoc = true;
            });
        }

        private void LoadingDataToView(IEnumerable<DataModel.ParrotItem> items, PropertyGroupDescription propertyGroupDescription)
        {
            if (LoadDataToListThread != null && LoadDataToListThread.IsAlive == true)
            {
               LoadDataToListThread.Abort();               
               LoadDataToListThread.Join();
            }            
            parrotItemsViewColletion.Clear();

            view.GroupDescriptions.Clear();

            if (propertyGroupDescription != null)
                view.GroupDescriptions.Add(propertyGroupDescription);
                LoadDataToListThread = new Thread(() =>
                {
                  try
                  {
                      while (Loadoc != true)
                      { Thread.Sleep(300); }

                      foreach (var item in items)
                      {
                          if (!parrotItemsViewColletion.Contains(item))
                          {
                              Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                              {
                                    parrotItemsViewColletion.Add(item);
                              }));
                          }
                          Thread.Sleep(120);
                      }
                  }
                  catch(Exception EX) {  }
             });
            LoadDataToListThread.Start();
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

            AboutAppButton.IsEnabled = true;
            AboutAppButton.IsChecked = false;

            FrameData.Visibility = Visibility.Collapsed;
            ParrotListView.Visibility = Visibility.Visible;

            if (ParrotListView.Items.Count > 0)
                ParrotListView.ScrollIntoView(ParrotListView.Items[0]);

            LoadingDataToView(parrotItems, groupDescription);
        }

        private void ParrotFromABC_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void ParrotFromGenderButton_Checked(object sender, RoutedEventArgs e)
        {
            ParrotFromGenderButton.IsEnabled = false;

            AboutAppButton.IsEnabled = true;
            AboutAppButton.IsChecked = false;

            ParrotFromABC.IsChecked = false;
            ParrotFromABC.IsEnabled = true;

            FrameData.Visibility = Visibility.Collapsed;
            ParrotListView.Visibility = Visibility.Visible;

            if (ParrotListView.Items.Count > 0)
                ParrotListView.ScrollIntoView(ParrotListView.Items[0]);

            LoadingDataToView(parrotItems, groupDescription2);
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

                ArticleView = new View.ArticleView(ParrotListView.SelectedItem as DataModel.ParrotItem);
                ArticleView.ClosingEvent += ArticleView_ClosingEvent;
                ArticleView.ImageSelectedEvent += ArticleView_ImageSelectedEvent;
                FrameData.Navigate(ArticleView.Content);
            }
        }

        private void AboutAppButton_Checked(object sender, RoutedEventArgs e)
        {
            ParrotFromABC.IsChecked = false;
            ParrotFromGenderButton.IsChecked = false;
            ParrotListView.Visibility = Visibility.Collapsed;
            FrameData.Navigate(new View.AboutApp().Content);
            FrameData.Visibility = Visibility.Visible;
            AboutAppButton.IsEnabled = false;
        }

        private void SearchTextbox_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
        }

        private void SearchTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                ParrotFromABC.IsChecked = false;
                ParrotFromGenderButton.IsChecked = false;
                
                List<DataModel.ParrotItem> searchItems = new List<DataModel.ParrotItem>();

                if (SearchTextbox.Text.Length > 0)
                {
                    foreach (var k in parrotItems)
                        if (k.Title.Contains(SearchTextbox.Text))
                            searchItems.Add(k);
                        

                    LoadingDataToView(searchItems,groupDescription3);
                }
                else LoadingDataToView(parrotItems, groupDescription3);
            }));
        }
    }
}