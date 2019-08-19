using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Image = System.Windows.Controls.Image;

namespace SFMRuner
{

    class SFMSearch
    {
        private List<string> _pathsList = new List<string>(); //лист путей для поиска
        //private List<FileInfo> _globalFileList = new List<FileInfo>(); //лист путей для поиска
        public Queue<FileInfo> globalListOfFound = new Queue<FileInfo>(); //лист найденных
        private MainWindow _mainWindowHandler;


        private string _searchLine;
        public string SearchLine { set => _searchLine = value;}

        private Thread[] massSearchThread;
        private object listBoxLock = new object();

        public SFMSearch(MainWindow mainWindowHandler)
        {
            this._mainWindowHandler = mainWindowHandler;
            AddDefaultFolders();    //добавляем стандартные пути            
            CreateThreads();        //создаем потоки для поиска            

        }

        public void RunSearch(string searchLine)
        {
            _searchLine = searchLine;
            RunThreads();           //запускаем потоки

        }

        private void CreateThreads()
        {
            massSearchThread = new Thread[_pathsList.Count];
            for (int i = 0; i < massSearchThread.Length; i++)
            {
                massSearchThread[i] = new Thread(new ParameterizedThreadStart(SearchFilesAndFolders));
                //massSearchThread[i].SetApartmentState(ApartmentState.STA);
            }
        }

        private void RunThreads()
        {
            for (int i = 0; i < _pathsList.Count; i++)
            {                
                massSearchThread[i].Start(_pathsList[i]);
            }
        }

        //  точка входа потоков поиска
        private void SearchFilesAndFolders(object searchPath)
        {
            string path = (string)searchPath;
            Queue<string> qDirs = new Queue<string>(); //очередь для папок и подпапок
            List<FileInfo> fileList = new List<FileInfo>(); //лист путей для поиска

            qDirs.Enqueue(path);
            do
            {
                DirectoryInfo dir = new DirectoryInfo(qDirs.Dequeue());

                if (dir.Exists)
                {
                    try
                    {
                        foreach (var itemFile in dir.GetFiles())
                        {
                            if (itemFile.Extension == ".exe" || itemFile.Extension == ".lnk")
                            {
                                fileList.Add(itemFile);
                            }
                        }
                        foreach (var itemDir in dir.GetDirectories())
                        {
                            qDirs.Enqueue(itemDir.FullName);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                }
            }
            while (qDirs.Count > 0);

            SearchLineInFileList(fileList);
        }

        private void SearchLineInFileList(List<FileInfo> fileList)
        {
            string sl = @"\w*" + _searchLine + @"\w*";
            Regex regex = new Regex(sl, RegexOptions.IgnoreCase);
            foreach (var file in fileList)
            {
                MatchCollection matches = regex.Matches(file.Name);
                if (matches.Count > 0)
                {
                    Action action = () => _mainWindowHandler.ListBox.Items.Add(CreateBlockForListBox(file));
                    _mainWindowHandler.Dispatcher.BeginInvoke(action);
                }
            }
        }

        private StackPanel CreateBlockForListBox(FileInfo file)
        {
            StackPanel mainStackPanel = new StackPanel();

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

            StackPanel subStackPanel = new StackPanel();
            TextBlock textBlockFileName = new TextBlock();
            Label labelFilePath = new Label();
            Label labelFileDateTime = new Label();

            mainStackPanel.Orientation = Orientation.Horizontal;
            subStackPanel.Orientation = Orientation.Vertical;

            textBlockFileName.FontWeight = FontWeights.Bold;

            mainStackPanel.Margin = new Thickness(5);
            subStackPanel.Margin = new Thickness(5);

            labelFilePath.Padding = new Thickness(10, 0, 0, 0);
            labelFileDateTime.Padding = new Thickness(10, 0, 0, 0);

            textBlockFileName.FontSize = 20;
            labelFilePath.FontSize = 14;
            labelFileDateTime.FontSize = 14;

            textBlockFileName.Text = file.Name;
            labelFilePath.Content = "Путь: " + file.FullName;
            labelFileDateTime.Content = "Дата изменения: " + file.LastWriteTime;

            subStackPanel.Children.Add(textBlockFileName);
            subStackPanel.Children.Add(labelFilePath);
            subStackPanel.Children.Add(labelFileDateTime);

            mainStackPanel.Children.Add(img);
            mainStackPanel.Children.Add(subStackPanel);

            return mainStackPanel;
            
        }
        
        private void AddDefaultFolders()
        {
            _pathsList.Add(System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory));
            _pathsList.Add(System.Environment.GetFolderPath(System.Environment.SpecialFolder.StartMenu));

        }
    }
}
