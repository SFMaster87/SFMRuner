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
using Image = System.Windows.Controls.Image;

namespace SFMRuner
{
    class GlobalFileList
    {
        private Queue<string> pathsQueue = new Queue<string>(); //очередь путей для поиска
        private Thread[] massiveThreads;
        private List<FileInfo> globalFileList = new List<FileInfo>();
        private MainWindow mainWindowHandler;

        public GlobalFileList(MainWindow mainWindowHandler)
        {
            this.mainWindowHandler = mainWindowHandler;

            AddDefaultFolders();    //добавляем стандартные пути
            CreateThreads();
            RunThreads();           //запускаем потоки

        }

        public void refreshGlobalFileList() //обновить список файлов
        {
            globalFileList.Clear();
            CreateThreads();
            RunThreads();           //запускаем потоки
        }

        public List<FileInfo> GetGlobalFileList() //получить список файлов
        {
            return globalFileList;
        }

        public bool GetStatusInitGlobalFileList()
        {            
            for (int i = 0; i < massiveThreads.Length; i++)
            {
                if (massiveThreads[i].IsAlive)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        
        private void AddDefaultFolders() //добавить пути по умолчанию
        {
            pathsQueue.Enqueue(System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory));
            pathsQueue.Enqueue(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonDesktopDirectory));
            pathsQueue.Enqueue(System.Environment.GetFolderPath(System.Environment.SpecialFolder.StartMenu));
            pathsQueue.Enqueue(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonStartMenu));

        }

        private void CreateThreads()
        {
            massiveThreads = new Thread[pathsQueue.Count];
            for (int i = 0; i < massiveThreads.Length; i++)
            {
                massiveThreads[i] = new Thread(new ParameterizedThreadStart(SearchFilesAndFolders));
            }
        }

        private void RunThreads()
        {
            for (int i = 0; i < pathsQueue.Count; i++)
            {
                massiveThreads[i].Start(pathsQueue.Dequeue());
            }
        }

        private void SearchFilesAndFolders(object searchPath)  //формируем глобальный лист
        {
            string path = (string)searchPath;
            Queue<string> qDirs = new Queue<string>(); //очередь для папок и подпапок            

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
                                globalFileList.Add(itemFile);
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
        }

        public void SearchLineInGlobalFileList(string searchLine)
        {
            string sl = @"\w*" + searchLine + @"\w*";
            Regex regex = new Regex(sl, RegexOptions.IgnoreCase);
            foreach (var file in globalFileList)
            {
                MatchCollection matches = regex.Matches(file.Name);
                if (matches.Count > 0)
                {
                    Action action = () => mainWindowHandler.ListBox.Items.Add(CreateBlockForListBox(file));
                    mainWindowHandler.Dispatcher.BeginInvoke(action);
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
            subStackPanel.Name = "subStackPanel";
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
            labelFilePath.Name = "filePathLabel";
            labelFilePath.Content = "Путь: " + file.FullName;
            labelFileDateTime.Content = "Дата изменения: " + file.LastWriteTime;

            subStackPanel.Children.Add(textBlockFileName);
            subStackPanel.Children.Add(labelFilePath);
            subStackPanel.Children.Add(labelFileDateTime);

            mainStackPanel.Children.Add(img);
            mainStackPanel.Children.Add(subStackPanel);

            return mainStackPanel;

        }
    }
}
