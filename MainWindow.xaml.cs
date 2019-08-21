using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GlobalHotKey;


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
        
        public MainWindow()
        {            
            InitializeComponent();

            //sfm_obj = new SFMFileList();
            _searchObj = new SFMSearch(this);
            
            new SFMNotifyIconClass(this);
            
            this.ShowActivated = true;
            //this.Hide();            

            // Register Ctrl+Alt+F5 hotkey. Save this variable somewhere for the further unregistering.
            var hotKey = hotKeyManager.Register(Key.Space, /*ModifierKeys.Control |*/ ModifierKeys.Alt);
            // Handle hotkey presses.
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
            _searchObj.SearchLine = TextBox.Text;
            _searchObj.RunSearch(TextBox.Text);
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
    }
}