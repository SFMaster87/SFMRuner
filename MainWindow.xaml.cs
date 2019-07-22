using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
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
                sp.Orientation = System.Windows.Controls.Orientation.Horizontal;

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

                TextBlock tb = new TextBlock();
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.Text = file.Name;
                sp.Children.Add(img);
                sp.Children.Add(tb);

                ListBox.Items.Add(sp);
            }

        }

        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                //if(ListBox.SelectedIndex > 0)
                //    ListBox.SelectedIndex = ListBox.SelectedIndex - 1;    
                ListBox.Focus();
            }
            else if (e.Key == Key.Down)
            {
                //if (ListBox.SelectedIndex < ListBox.Items.Count)
                //    ListBox.SelectedIndex = ListBox.SelectedIndex + 1;                
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
            TextBox.Focus();
        }
    }
}