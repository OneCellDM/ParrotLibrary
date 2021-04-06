using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ParrotLibraryArticleGenerator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
        private void DataTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataTextBox.Text.Length > 0)
            {
                try
                {               
                    var p = XamlReader.Parse(DataTextBox.Text.Replace("\"", "'"));
                    FlowDocument flowDocument = p as FlowDocument;
                    DocView.Document = flowDocument;
  
                    UpdatePreviewImages();
                }
                catch (Exception EX) { }
            }
        }
        void WriteData(DirectoryInfo dir,int Family,int Gender,string Title, string Data)
        {
            ArticleDataModel articleDataModel = new ArticleDataModel();
            articleDataModel.Data = Data;
            articleDataModel.Image = DownloadMainImage(dir);

            articleDataModel.Title = Title;
            articleDataModel.Family = Family;
            articleDataModel.Gender = Gender;

            articleDataModel.Images = GetSavedImages(dir);
            string output = JsonConvert.SerializeObject(articleDataModel);

            var file=new FileInfo(dir.FullName+"\\Data.json");
            StreamWriter sw = new StreamWriter(file.OpenWrite());
            sw.Write(output);
            sw.Close();
        }

        public string[] GetSavedImages(DirectoryInfo dir)
        {
            var files = new DirectoryInfo(dir.FullName + "\\Img").GetFiles();
            string[] ArrayFiles = new string[files.Length];
            for (int i = 0; i < ArrayFiles.Length; i++)
                ArrayFiles[i] = "Img\\" + files[i].Name;
            return ArrayFiles;
        }
        public string DownloadMainImage(DirectoryInfo dir)
        {

           return  DownloadFile(dir, ImageUrlBox.Text);
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
               if(file.Exists)
               {
                    return DownloadFile(dir, url);
               }
            new WebClient().DownloadFile(url, dir.FullName + "\\"+name);
            return name;
          
        }
        public string[] GetSubImages()
        {
            string[] s = new string[0];
            try
            {
                s = ImagesTextBox.Text.Split('\n');
            }
            catch(Exception ex) { }
            return s;
        }

        public List<String> ParseImagesFromPage(String PageData)
        {
            List<string> ImageUrls = new List<string>();
            
            var f = DataTextBox.Text.Split('<');
            for (int i = 0; i < f.Length; i++)
            {
                var d = f[i];
                if (d.Contains("Image") & d.Contains("Source"))
                {
                    try
                    {

                        ImageUrls.Add(d.Substring(d.LastIndexOf("Source=")).Split('=')[1].Replace(">", "").Replace("'", "").Replace("\"", ""));
  
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
            return ImageUrls;
        }

        public void UpdatePreviewImages()
        {
            ImagesView.Items.Clear();
            try
            {
               foreach(var k in ParseImagesFromPage(DataTextBox.Text))
                {
                    ImagesView.Items.Add(new Image()
                    {
                        Width = 150,

                        Height = 200,
                        Stretch = Stretch.UniformToFill,
                        Source = new BitmapImage(new Uri(k))
                    }
                    ) ;
                }
                foreach(var k in  GetSubImages())
                    {
                    ImagesView.Items.Add(new Image()
                    {
                        Width = 150,

                        Height = 200,
                        Stretch = Stretch.UniformToFill,
                        Source = new BitmapImage(new Uri(k))
                    }
            );
                }
            }
            catch(Exception ex)
            {

            }
        }
        private void ImagesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ImagesTextBox.Text.Length > 0)
            {
                UpdatePreviewImages();
            }
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {

            var t = DataTextBox.Text.Replace("\"", "'");
            var dir = CreateDir(TitleTextbox.Text);
         
           

            foreach (var urld in ParseImagesFromPage(DataTextBox.Text))
            {
                try
                {

                    t.Replace(urld, DownloadFile(dir, urld));
                }
                catch(Exception ex)
                {
                    
                }
            }
            foreach (var urld in GetSubImages())
            {
                try
                {

                    DownloadFile(dir, urld);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(urld);
                    Debug.WriteLine(ex.Message);
                }
            }

            try
            {
                WriteData(dir, SubFamilyCombox.SelectedIndex, GenderCombox.SelectedIndex, TitleTextbox.Text, t);

                dir.MoveTo(@"C:\Users\dan08\source\repos\ParrotLibrary\ParrotLibrary\bin\Debug\Data\" + TitleTextbox.Text);
            }
            catch(Exception EX)
            {

            }
            
            
        }
    }
}