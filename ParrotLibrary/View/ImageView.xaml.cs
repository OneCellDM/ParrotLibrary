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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ParrotLibrary.View
{
    /// <summary>
    /// Логика взаимодействия для ImageView.xaml
    /// </summary>
    public partial class ImageView : Page
    {
        public delegate void CloseImageView();

        public event CloseImageView CloseImageViewEvent;
        public ImageView()=>InitializeComponent();

        public ImageView(ImageSource imageSource) 
        {
            InitializeComponent();
            ImageData.Source = imageSource; 
        }   
        private void CloseButton_Click(object sender, RoutedEventArgs e)=>CloseImageViewEvent?.Invoke();


        
    }
}
