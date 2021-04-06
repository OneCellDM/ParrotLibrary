
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JsonImagesImput
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ArticleDataModel currentItem;
       
        ArticleDataModel CurrentArticleData
        {
            get => currentItem; 
            set {
                currentItem = value;
                var p = XamlReader.Parse(currentItem.Data.Replace("\"", "'"));
                FlowDocument flowDocument = p as FlowDocument;
                DocView.Document = flowDocument;
            }
        }
        ObservableCollection<ArticleDataModel> obc = new ObservableCollection<ArticleDataModel>();
        public MainWindow()
        {
            InitializeComponent();
            JsonFilesListview.ItemsSource = obc;
            LoadingData();
        }
        
        private void JsonFilesListview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(JsonFilesListview.SelectedIndex!=-1)
                CurrentArticleData = JsonFilesListview.SelectedItem as ArticleDataModel;         
        }

        public void LoadingData()
        {
            var infod = new DirectoryInfo("data");
            var files = infod.GetFiles("*.json", SearchOption.AllDirectories);
          
            Task.Run(() =>
            {
                foreach (var file in files)
                {
                    try
                    {

                        try
                        {
                            var p = Newtonsoft.Json.JsonConvert.DeserializeObject<ArticleDataModel>(File.ReadAllText(file.FullName));
                            p.Path = file.DirectoryName;
                            // p.Image = file.DirectoryName + "\\" + p.Image;
                            //p.Image = "https://shopdinelive.net/wp-content/uploads/2018/06/andrew-pons-57133-unsplash.jpg";
                            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                            {
                                try
                                {
                                    obc.Add(p);
                                }
                                catch (Exception EX)
                                {
                                    Debug.WriteLine(EX);
                                }
                            }));
                        }
                        catch (Exception EX)
                        {
                            Debug.WriteLine(EX);
                        }


                    }
                    catch (Exception EX)
                    {
                        Debug.WriteLine("Error reading: " + file.Name);
                    }
                }


            });
        }

        private void MainImageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            /*
            if (currentItem != null)
            {
                string s = "<Image Width = '600' Source='" + MainImageTextBox.Text + "'></Image>";
                if (CurrentArticleData.Data.Contains("<Image Width = '600' ></Image>"))   
                    currentItem.Data = currentItem.Data.Replace("<Image Width = '600' ></Image>", s);
                
                else
                {
                    string parse = CurrentArticleData.Data.Substring(

                        CurrentArticleData.Data.IndexOf("<Image"),
                        (CurrentArticleData.Data.IndexOf("</Image>") - CurrentArticleData.Data.IndexOf("<Image")) + 8
                    ) ;
                    currentItem.Data= currentItem.Data.Replace(parse, s);
                    Debug.WriteLine(s);
                    
                }
                CurrentArticleData = currentItem;
            }
            */
        }
        string GenName()
        {
            Random random = new Random();
            int Isnumber = 0;
            string Rname = "";
            for (int i = 0; i < 25; i++)
            {
                Isnumber = random.Next(2);
                if (Isnumber == 0)
                    Rname += (char)random.Next(97, 122);
                else Rname += (char)random.Next(48, 57);
            }
            return Rname;
        }
        public DirectoryInfo CreateDir(string Name)
        {
            var dir = new DirectoryInfo(Name);
            if (!dir.Exists)
                dir.Create();

            if (!new DirectoryInfo(Name + "\\Img").Exists)
                new DirectoryInfo(Name + "\\Img").Create();

            return dir;
        }

        public string DownloadFile(DirectoryInfo dir, string url)
        {
            
            FileInfo file;
            string name = "Img\\" + GenName();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            file = new FileInfo(dir.FullName + "\\" + name);
            if (file.Exists)
            {
                return DownloadFile(dir, url);
            }
            new WebClient().DownloadFile(url, dir.FullName + "\\" + name);
            return name;

        }

        public string[] GetUrlsFromPhotoUrlTextbox()
        {
            string[] urls = PhotoUrlTextbox.Text.Split('\n');
            return urls;
        }

        private void PhotoUrlTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PhotoListview.Items.Clear();
            var urls= GetUrlsFromPhotoUrlTextbox();
            if (urls.Length > 0)
            {
                foreach (var photourl in urls)
                {
                    try
                    {
                        Image image = new Image();
                        image.Source = new BitmapImage(new Uri(photourl));
                        image.Width = 80;
                        image.Height = 80;
                        image.Stretch = Stretch.UniformToFill;
                        PhotoListview.Items.Add(image);
                    }
                    catch (Exception EX)
                    {

                    }
                }
            }
        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            List<string> downloadedurls = new List<string>();
            var dir=   CreateDir(CurrentArticleData.Title);
            var photosurls = GetUrlsFromPhotoUrlTextbox();
            var mainphotourl = MainImageTextBox.Text;
            try
            {
                var mainphotoname = DownloadFile(dir,mainphotourl);
                CurrentArticleData.Data = CurrentArticleData.Data.Replace(mainphotourl, "Data\\" + CurrentArticleData.Title + "\\" + mainphotoname);
                CurrentArticleData.Image =  mainphotoname;

            }
            catch(Exception EX)
            {

            }
            foreach (var k in photosurls)
            {
                try
                {
                    downloadedurls.Add(DownloadFile(dir, k));
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            CurrentArticleData.Images = downloadedurls.ToArray();
            var json = JsonConvert.SerializeObject(CurrentArticleData);
            File.WriteAllText(CurrentArticleData.Title+"\\"+currentItem.Title+".json", json);


            MessageBox.Show("Работа завершена");
          
            

        }
    }
}
