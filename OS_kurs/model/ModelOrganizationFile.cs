using Autofac;
using DialogServiceForWPF2;
using LibraryOrganizationFileSystem;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

namespace OS_kurs
{
    public class ModelOrganizationFile : ReactiveObject
    {
        #region Fields
        private ObservableCollection<File> userDirectory = new ObservableCollection<File>(); // Список файлов 
        private ObservableCollection<MyClaster> fatArray = new ObservableCollection<MyClaster>(); // FAT 

        private Dictionary<string , List<string>> pathsFiles = new Dictionary<string, List<string>>(); // Полные пути файлов

        private List<string> findEof = new List<string>(); // Ошибка: потярянные кластеры
        private int countIntersectingClasters = 0; // Ошибка: количество пересекающихся кластеров
        #endregion


        #region Rpoperties
        public ObservableCollection<File> UserDirectory { get => userDirectory; set => this.RaiseAndSetIfChanged(ref userDirectory, value); }
        public ObservableCollection<MyClaster> FatArray { get => fatArray; set => this.RaiseAndSetIfChanged(ref fatArray, value); }

        public Dictionary<string, List<string>> PathsFiles  => pathsFiles;

        public List<string> FindEof => findEof; 
        public int CountIntersectingClasters  => countIntersectingClasters;

        #endregion

        #region Constructors
        public ModelOrganizationFile()
        {
            // Примерные данные
            UserDirectory = new ObservableCollection<File>
            {
                new File("A", 2),
                new File("B", 14),
                new File("C", 22),
                new File("D", 8)
            };
            FatArray = new ObservableCollection<MyClaster>()
            {
                new MyClaster( 0, "-" ),new MyClaster(  1, "" ), new MyClaster( 2, "3" ), new MyClaster( 3, "4" ),new MyClaster( 4, "5" ),
                new MyClaster( 5, "6" ), new MyClaster( 6, "eof" ) ,new MyClaster(  7, "" ),
               new MyClaster( 8, "9" ),new MyClaster( 9, "26" ), new MyClaster( 10, "bad" ), new MyClaster( 11, "eof" ), new MyClaster( 12, "" ),
                new MyClaster( 13, "" ), new MyClaster( 14, "16" ), new MyClaster( 15, "bad" ), new MyClaster( 16, "26" ),new MyClaster( 17, "" ),
                new MyClaster( 18, "" ), new MyClaster( 19, "" ),new MyClaster(  20,"" ),new MyClaster(  21, "bad" ),new MyClaster(  22 ,"23" ),
                new MyClaster( 23, "26" ), new MyClaster( 24, "" ),new MyClaster(  25, "" ),new MyClaster( 26, "27" ),new MyClaster(  27, "28" ),
                new MyClaster( 28,"eof" ), new MyClaster( 29, "" ), new MyClaster( 30, "" ), new MyClaster( 31, "" ),
                new MyClaster( 32, "eof"), new MyClaster( 33,"32" ), new MyClaster( 34,"35" ), new MyClaster( 35, "36" ),
                new MyClaster( 36, "37" ), new MyClaster( 37, "38" ), new MyClaster(38, "eof" ), new MyClaster(39,"" ),new MyClaster( 40,"" ),
                new MyClaster( 41, "" ),new MyClaster( 42, "" ), new MyClaster( 43, "bad"),
                new MyClaster( 44,"" ), new MyClaster( 45, "" ), new MyClaster( 46, "" ), new MyClaster(47, "" ), new MyClaster( 48, "" ),
                new MyClaster( 49, "" )
            };
        }
        public ModelOrganizationFile(List<File> ud, List<string> fat)
        {
            // Примерные данные
            UserDirectory = new ObservableCollection<File>(ud);
            for (int i=0; i < fat.Count; i++)
            {
                FatArray.Add(new MyClaster(i, fat[i]));
            }
        }
        #endregion

        #region Methods

        // Измение пользовательской директорией
        public void DeleteFile(File file)
        {
            UserDirectory.Remove(file);
        }
        public void AddFile(string name, string claster)
        {
            int t;
            if (!int.TryParse(claster,out t))
                throw new ArgumentException();
            for (int i =0; i< UserDirectory.Count; i++)
            {
                if (UserDirectory[i].name == name)
                    throw new InvalidCastException();
            }
            UserDirectory.Add(new File(name, t));
        }
        public void ChangeFile(File file, string newName, string newClaster)
        {
            int t;
            if (!int.TryParse(newClaster, out t))
                throw new ArgumentException();
            if (t == 0)
                throw new ArgumentException();

            for (int i = 0; i < UserDirectory.Count; i++)
            {
                if (UserDirectory[i].name == newName)
                    throw new InvalidCastException();
            }
            //var f = from file1 in UserDirectory where file1.name == file.name select file1;
            //f.ToArray()[0] = new File(newName, t);
            int j = UserDirectory.IndexOf(file);
            UserDirectory[j] = new File(newName, t);

            //file.name = newName;
            //file.index = t;
        }

        // Изменение фэт
        public void DeleteRowFat(MyClaster claster)
        {
            int inx = claster.Index;
            if (inx == 0)
            {
                throw new ArgumentException();
            }
            
            FatArray.Remove(claster);
            for (int i=inx; i < FatArray.Count; i++)
            {
                fatArray[i].Index = i;
            }
            //DefaultDialogService.ShowMessage("!!!" + FatArray[key]);
        }
        public void ChangeRowFat(int key, string val)
        {
            int t;
            if (key == 0 && val!= "-")
            {
                throw new ArgumentException();
            }
            else if (key!=0 && val != ""  && val != "eof" 
                && val != "bad" && !int.TryParse(val, out t))
            {
                throw new ArgumentException();
            }
            else if (key !=0 && int.TryParse(val, out t) && t >= fatArray.Count)
            {
                throw new ArgumentException();
            }

            FatArray[key] = new MyClaster( key, val);
            //DefaultDialogService.ShowMessage("!!!" + FatArray[key]);
        }
        public void AddRowFat(string row)
        {
            int t;
            if (fatArray.Count == 0 && row != "-")
            {
                throw new ArgumentException();
            }
            else if (row != "" && row != "eof"
                && row != "bad" && !int.TryParse(row, out t))
            {
                throw new ArgumentException();
            }
            else if (int.TryParse(row, out t) && t > fatArray.Count)
            {
                throw new ArgumentException();
            }

            FatArray.Add( new MyClaster(fatArray.Count , row));
        }

        // возвращает false если нечего исправлять
        public bool Correct()
        {
            bool isCorrect = false;
            if (!FindErrors())
            {
                isCorrect = false;
                return isCorrect;
            }

            List<string> fat = new List<string>();
            for (int i = 0; i < fatArray.Count; i++)
            {
                fat.Add(fatArray[i].Claster);
            }

            List<File> ud = new List<File>(userDirectory);

            
            // Пересекающиеся кластеры
            if (countIntersectingClasters > 0)
            {
                isCorrect = true;
                OrganizationFileSystem.FixingErrorIntersectingClusters(ref fat);
            }

            // Потерянные кластеры
            if (findEof.Count > 0)
            {
                isCorrect = true;
                OrganizationFileSystem.FixingErrorLostClusters(ref ud, ref fat, findEof);
            }

            //UserDirectory = userDirectory;
            pathsFiles = OrganizationFileSystem.GetFullUserArray(ud, fat);

            UserDirectory = new ObservableCollection<File>(ud);

            for (int i=0; i< fatArray.Count; i++)
            {
                FatArray[i] = new MyClaster(i, fat[i]);
            }
            return isCorrect;
        }

        // возращает true если ошибки найдены, иначе false
        public bool FindErrors()
        {
            bool isFindErrors = false;
            List<string> fat = new List<string>();
            for (int i = 0; i < fatArray.Count; i++)
            {
                fat.Add(fatArray[i].Claster);
            }
            List<File> ud = new List<File>(userDirectory);
            pathsFiles = OrganizationFileSystem.GetFullUserArray(ud, fat);

            countIntersectingClasters = OrganizationFileSystem.FindIntersectingClusters(fat);
            findEof = OrganizationFileSystem.FindLostClusters(PathsFiles, fat);

            if (findEof.Count > 0)
            {
                isFindErrors = true;
            }
            if (countIntersectingClasters > 0)
            {
                isFindErrors = true;
            }
            return isFindErrors;
        }
        #endregion
    }
}
