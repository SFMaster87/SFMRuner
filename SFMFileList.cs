using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace SFMRuner
{
    /// <summary>
    /// Класс представляющий список найденных файлов и осуществляющий поиск этих файлов
    /// </summary>
    class SFMFileList
    {
        private List<string> _pathsList = new List<string>(); //лист путей для поиска
        private List<FileInfo> _globalFileList = new List<FileInfo>(); //лист путей для поиска
        public List<FileInfo> listOfFound = new List<FileInfo>(); //лист найденных

        private delegate void SearchFilesFunction();

        public SFMFileList()
        {
            //загружаем стандартные пути для поиска 
            AddDefaultFolders();
            //загружаем пути для поиска
            ReadPathsList();
            //MakeGlobalFilesList(SearchOnlyFiles);
            MakeGlobalFilesList(SearchDirectoriesAndFiles);
        }

        private void AddDefaultFolders()
        {
            _pathsList.Add(System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory));            
            _pathsList.Add(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonDesktopDirectory));
            
            _pathsList.Add(System.Environment.GetFolderPath(System.Environment.SpecialFolder.StartMenu));
            _pathsList.Add(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonStartMenu));
            
        }


        /*читаем из FilePaths.txt пути для поиска файлов*/
        private void ReadPathsList()
        {
            //read file "FilePaths.txt"
            using (StreamReader sr = new StreamReader("FilePaths.txt", System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    _pathsList.Add(line);
                }
            }
        }

        private void MakeGlobalFilesList(Action searchStrategy)
        {
            searchStrategy();
        }

        private void SearchOnlyFiles()
        {
            foreach (var path in _pathsList)
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                if (dir.Exists)
                {
                    foreach (var itemDir in dir.GetFiles())
                    {
                        if (itemDir.Extension == ".exe" || itemDir.Extension == ".lnk")
                        {
                            _globalFileList.Add(itemDir);
                        }
                    }
                }
            }
        }

        private void SearchDirectoriesAndFiles()
        {            
            Queue<string> qDirs = new Queue<string>();            
            foreach (var path in _pathsList)
            {
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
                                    _globalFileList.Add(itemFile);
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
        }

        //поиск подстроки в глобальном листе файлов
        public void SearchLineInGlobalFileList(string line)
        {
            listOfFound.Clear();
            string sl = @"\w*" + line + @"\w*";
            Regex regex = new Regex(sl, RegexOptions.IgnoreCase);
            foreach (var item in _globalFileList)
            {
                MatchCollection matches = regex.Matches(item.Name);
                if (matches.Count > 0)
                {
                    listOfFound.Add(item);
                }

            }
        }
    }
}
