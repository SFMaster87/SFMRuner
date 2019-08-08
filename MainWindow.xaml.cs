using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;



namespace SFMRuner
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
       
    public partial class MainWindow : Window
    {           
        private List<FileInfo> _fileListFullPath = new List<FileInfo>();
        private SFMFileList sfm_obj;
        
        public MainWindow()
        {            

            sfm_obj = new SFMFileList();
            new SFMNotifyIconClass(this);
            new SFMHotKey(this);
            InitializeComponent();
            this.ShowActivated = true;
            //this.Hide();            
        }
                              
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListBox.Items.Clear();
            _fileListFullPath.Clear();

            string searchingLine = TextBox.Text;
            sfm_obj.SearchLineInGlobalFileList(searchingLine);

            foreach (var file in sfm_obj.listOfFound)
            {
                _fileListFullPath.Add(file);

                StackPanel sp = new StackPanel();
                

                Image img = new Image();
                img.Width = 40;
                img.Height = 40;
                Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(file.FullName);
                using (Bitmap bmp = icon.ToBitmap())
                {
                    var stream = new MemoryStream();
                    bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    img.Source = BitmapFrame.Create(stream);
                }
                
                sp = CreateItemListBox(file.Name, file.FullName, img, file.LastWriteTime);
                ListBox.Items.Add(sp);
            }

        }

        private StackPanel CreateItemListBox(string fileName, string fullFileName, Image icon, DateTime dateTimeWritefile)
        {
            StackPanel mainStackPanel = new StackPanel();
            StackPanel subStackPanel = new StackPanel();
            TextBlock textBlockFileName = new TextBlock();
            Label labelFilePath = new Label();
            Label labelFileDateTime = new Label();

            mainStackPanel.Orientation = Orientation.Horizontal;
            subStackPanel.Orientation = Orientation.Vertical;
            
            textBlockFileName.FontWeight = FontWeights.Bold;

            mainStackPanel.Margin = new Thickness(5);
            subStackPanel.Margin = new Thickness(5);

            labelFilePath.Padding = new Thickness(10,0,0,0);
            labelFileDateTime.Padding = new Thickness(10, 0, 0, 0);


            textBlockFileName.FontSize = 20;
            labelFilePath.FontSize = 14;
            labelFileDateTime.FontSize = 14;

            textBlockFileName.Text = fileName;
            labelFilePath.Content = "Путь: " + fullFileName;
            labelFileDateTime.Content = "Дата изменения: " + dateTimeWritefile;


            subStackPanel.Children.Add(textBlockFileName);
            subStackPanel.Children.Add(labelFilePath);
            subStackPanel.Children.Add(labelFileDateTime);

            mainStackPanel.Children.Add(icon);
            mainStackPanel.Children.Add(subStackPanel);
            

            return mainStackPanel;
        }

        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                ListBox.Focus();
            }
            else if (e.Key == Key.Down)
            {
                ListBox.Focus();
            }
            else if (e.Key == Key.Enter)
            {
                startApp();
            }
            else if (e.Key == Key.Escape)
            {
                this.Hide();
            }

        }

        private void startApp()
        {
            if (ListBox.SelectedIndex >= 0 && ListBox.SelectedIndex <= ListBox.Items.Count)
                Process.Start(_fileListFullPath.ElementAt(ListBox.SelectedIndex).FullName);
        }

        private void ListBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                startApp();
            }
            else if (e.Key == Key.Escape)
            {
                TextBox.Focus();
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Window_Activated(object sender, EventArgs e)
        {            
            this.TextBox.Focus();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            startApp();
        }
    }
}