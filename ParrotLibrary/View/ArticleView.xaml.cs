using System;
using System.Collections.Generic;
using System.Linq;
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


namespace ParrotLibrary.View
{
    /// <summary>
    /// Логика взаимодействия для ArticleView.xaml
    /// </summary>
    public partial class ArticleView : Page
    {

        public delegate void ArticleClosing();
        public event ArticleClosing ClosingEvent;
        public delegate void ImageSelected(ImageSource imageSource);
        public event ImageSelected ImageSelectedEvent;

        public ArticleView()=>InitializeComponent(); 
        public ArticleView(ParrotLibrary.DataModel.ParrotItem parrotItem)
        {
            InitializeComponent();
            DocView.Document = (FlowDocument)XamlReader.Parse(parrotItem.Data);
           
            for (int i = 0; i < parrotItem.Images?.Length; i++)
            {
                try
                {
                    ImagesView.Items.Add(new Image()
                    {
                        Width = 150,
                        Height = 200,

                        Stretch = Stretch.Uniform,
                        Source = new BitmapImage(new Uri(parrotItem.Path + "\\" + parrotItem.Images[i])) 
                        { DecodePixelWidth=150, DecodePixelHeight = 200 }
                    }
                    ); 
                }
                catch(Exception ex)
                {

                };

            }

            try
            {
                ImagesView.Items.Add(new Image()
                {
                    Width = 150,
                    Height = 200,
                    Stretch = Stretch.Uniform,
                    Source = new BitmapImage(new Uri(parrotItem.Image)) { DecodePixelWidth = 150, DecodePixelHeight = 200 }
                });
            }
            catch (Exception ex)
            {
                
            }
            try
            {
                Stretch st=new Stretch();
               
                 st = Stretch.UniformToFill;
               
               
                TitlePanel.Background = new ImageBrush()
                {
                    ImageSource = (ImagesView.Items[ImagesView.Items.Count-1] as Image).Source,
                          
                    Stretch=st,                 
                };
                
            }
            catch(Exception ex) {  }
            TitleTextBlock.Text = parrotItem.Title;
           
        }
        public ArticleView(String Data)
        {
            InitializeComponent();
            DocView.Document = (FlowDocument)XamlReader.Parse(Data);      
        }

        private void ImagesView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {        
            if(ImagesView.SelectedIndex > -1) 
                ImageSelectedEvent?.Invoke( (ImagesView.SelectedItem as Image)?.Source );
        }
              
        private void CloseArticleButton_Click(object sender, RoutedEventArgs e)=> ClosingEvent?.Invoke();
        
    }
}
