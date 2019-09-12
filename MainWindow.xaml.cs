using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GlobalHotKey;
using System.Windows.Media.Effects;


namespace SFMRuner
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        // Create the hotkey manager.
        private HotKeyManager hotKeyManager = new HotKeyManager();

        private List<FileInfo> _fileListFullPath = new List<FileInfo>();
        //private SFMFileList sfm_obj;
        private SFMSearch _searchObj;

        private GlobalFileList globalFileList;

        public MainWindow()
        {            
            InitializeComponent();

            //_searchObj = new SFMSearch(this);
            
            new SFMNotifyIconClass(this);
            
            this.ShowActivated = true;
            
            var hotKey = hotKeyManager.Register(Key.Space, /*ModifierKeys.Control |*/ ModifierKeys.Alt);
            hotKeyManager.KeyPressed += HotKeyManagerPressed;

        }

        private void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.HotKey.Key == Key.Space)
            {
                this.Show();
                this.Activate();
            }
                
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListBox.Items.Clear();
            //_searchObj.SearchLine = TextBox.Text;
            //_searchObj.RunSearch(TextBox.Text);
            //List<FileInfo> per = globalFileList.GetGlobalFileList();
            globalFileList.SearchLineInGlobalFileList(TextBox.Text);
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
            {
                StackPanel spMain = (StackPanel)ListBox.SelectedItem as StackPanel;
                StackPanel spSub = (StackPanel)spMain.Children[1] as StackPanel;
                Label label = (Label)spSub.Children[1] as Label;
                string filePath = label.Content.ToString().Substring(6);
                if (File.Exists(filePath))
                {
                    Process.Start(filePath);
                }                
            }                
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
            this.TextBox.Text = "";
            this.TextBox.Focus();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            startApp();
        }

        private void MainWindow1_Closed(object sender, EventArgs e)
        {
            // Dispose the hotkey manager.
            hotKeyManager.Dispose();
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            DropShadowEffect dropShadow = new DropShadowEffect();
            img.Effect = dropShadow;
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            img.Effect = null;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void MainWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void MainWindow1_Initialized(object sender, EventArgs e)
        {
            globalFileList = new GlobalFileList(this);
            while (true)
            {
                if (globalFileList.GetStatusInitGlobalFileList() == true)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
        }
    }
}