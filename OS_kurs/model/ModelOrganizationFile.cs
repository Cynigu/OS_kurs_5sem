using Autofac;
using DialogServiceForWPF2;
using LibraryOrganizationFileSystem;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace OS_kurs
{
    public class ModelOrganizationFile
    {
        #region Fields
        private List<File> userDirectory = new List<File>(); // Список файлов 
        private List<string> fatArray = new List<string>(); // FAT 

        private Dictionary<string , List<string>> pathsFiles = new Dictionary<string, List<string>>(); // Полные пути файлов

        private List<string> findEof = new List<string>(); // Ошибка: потярянные кластеры
        private int countIntersectingClasters = 0; // Ошибка: количество пересекающихся кластеров

        #endregion

        #region Rpoperties
        public List<File> UserDirectory => userDirectory;
        public List<string> FatArray => fatArray;

        public Dictionary<string, List<string>> PathsFiles => pathsFiles;

        public List<string> FindEof => findEof; 
        public int CountIntersectingClasters  => countIntersectingClasters;

        #endregion

        #region Constructors
        public ModelOrganizationFile()
        {
            // Примерные данные
            userDirectory = new List<File>
            {
                new File("A", 2),
                new File("B", 14),
                new File("C", 22),
                new File("D", 8)
            };
            fatArray = new List<string>(){ "-", "", "3", "4", "5", "6", "eof" , "",
                "9", "26", "bad", "eof", "", "", "16", "bad", "26", "", "", "",
                "", "bad", "23", "26", "", "", "27", "28", "eof", "", "", "",
                "eof", "32", "35", "36", "37", "38", "eof", "", "", "", "", "bad",
                "", "", "", "", "", ""};

            /*{
                { 0, "-" }, { 1, "" }, { 2, "3" }, { 3, "4" }, { 4, "5" }, { 5, "6" }, { 6, "eof" } , { 7, "" },
               { 8, "9" },{ 9, "26" }, { 10, "bad" }, { 11, "eof" }, { 12, "" }, { 13, "" }, { 14, "16" }, { 15, "bad" }, { 16, "26" }, { 17, "" }, { 18, "" },
                { 19, "" }, { 20,"" }, { 21, "bad" }, { 22 ,"23" }, { 23, "26" }, { 24, "" }, { 25, "" }, { 26, "27" }, { 27, "28" }, { 28,"eof" },
                { 29, "" }, { 30, "" }, { 31, "" },
                { 32, "eof" }, { 33,"32" }, { 34,"35" }, { 35, "36" }, { 36, "37" }, { 37, "38" }, { 38, "eof" }, { 39,"" }, { 40,"" }, { 41, "" },
                { 42, "" }, { 43, "bad" },
                { 44,"" }, { 45, "" }, { 46, "" }, { 47, "" }, { 48, "" }, { 49, "" }
            };*/
        }
        public ModelOrganizationFile(List<File> ud, List<string> fat)
        {
            // Примерные данные
            userDirectory = ud;
            fatArray = fat;
        }
        #endregion

        #region Methods

        // возвращает false если нечего исправлять
        public bool Correct()
        {
            bool isCorrect = false;
            if (!FindErrors())
            {
                isCorrect = false;
                return isCorrect;
            }
            // Пересекающиеся кластеры
            if (countIntersectingClasters > 0)
            {
                isCorrect = true;
                OrganizationFileSystem.FixingErrorIntersectingClusters(ref fatArray);
            }
            // Потерянные кластеры
            if (findEof.Count > 0)
            {
                isCorrect = true;
                OrganizationFileSystem.FixingErrorLostClusters(ref userDirectory, ref fatArray, findEof);
            }

            pathsFiles = OrganizationFileSystem.GetFullUserArray(userDirectory, fatArray);
            return isCorrect;
        }

        // возращает true если ошибки найдены, иначе false
        public bool FindErrors()
        {
            bool isFindErrors = false;
            pathsFiles = OrganizationFileSystem.GetFullUserArray(userDirectory, fatArray);
            findEof = OrganizationFileSystem.FindLostClusters(pathsFiles, fatArray);
            countIntersectingClasters = OrganizationFileSystem.FindIntersectingClusters(fatArray);
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
