using Autofac;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DialogServiceForWPF2;
using System.Windows.Input;
using OS_kurs.view;

namespace OS_kurs
{
    public class ViewModelBase : ReactiveObject
    {
        #region Fields
        protected readonly IContainer _conteiner = MyConteiner.ContainerMain().Build(); // Контейнер
        private string strErrors = "";
        private string strCorrectData = "";
        private string chageFatStr = "";
        private string nameFile = "";
        private string clasterFile = "";

        private ModelOrganizationFile userdirectory = new ModelOrganizationFile();
        private LibraryOrganizationFileSystem.File selectedFile;
        private MyClaster selectedClaster = new MyClaster();

        // Методы
        private readonly Action<object> _startCorrect; // Начать программу по исправлению ошибок файловой системы
        private readonly Action<object> _findErrors; // Поиск ошибок в организации файловой системы
        private readonly Action<object> _openInstruction; // Открыть инструкцию
        private readonly Action<object> _openHelp; // Открыть справку
        private readonly Action<object> _openFile; // Открыть файл
        private readonly Action<object> _saveFile; // Сохранить в файл
        private readonly Action<object> _addRowFat; // добавить строку в fat
        private readonly Action<object> _changeRowFat; // изменить строку в fat
        private readonly Action<object> _deleteRowFat; // изменить строку в fat

        private readonly Action<object> _addFile; // добавить строку в fat
        private readonly Action<object> _changeFile; // изменить строку в fat
        private readonly Action<object> _deleteFile; // изменить строку в fat

        // Комманды
        private ICommand startCorrectionCommand;
        private ICommand findErrorsCommand;
        private ICommand openHelpCommand;
        private ICommand openInstructionCommand;
        private ICommand openFileCommand;
        private ICommand saveFileCommand;
        private ICommand addRowFatCommand;
        private ICommand changeRowFatCommand;
        private ICommand deleteRowFatCommand;
        private ICommand addFileCommand;
        private ICommand changeFileCommand;
        private ICommand deleteFileCommand;
        #endregion

        #region Rpoperties
        // Комманды
        public ICommand StartCorrectionCommand => startCorrectionCommand ??
                    (startCorrectionCommand = _conteiner.Resolve<ICommand>(new NamedParameter("p1", _startCorrect)));
        public ICommand FindErrorsCommand => findErrorsCommand ??
                      (findErrorsCommand = _conteiner.Resolve<ICommand>(new NamedParameter("p1", _findErrors)));
       
       
        public ICommand OpenHelpCommand => openHelpCommand ??
                      (openHelpCommand = _conteiner.Resolve<ICommand>(new NamedParameter("p1", _openHelp)));
        public ICommand OpenInstructionCommand => openInstructionCommand ??
                      (openInstructionCommand = _conteiner.Resolve<ICommand>(new NamedParameter("p1", _openInstruction)));


        public ICommand OpenFileCommand => openFileCommand ??
                      (openFileCommand = _conteiner.Resolve<ICommand>(new NamedParameter("p1", _openFile)));
        public ICommand SaveFileCommand => saveFileCommand ??
                      (saveFileCommand = _conteiner.Resolve<ICommand>(new NamedParameter("p1", _saveFile)));

        public ICommand AddRowFatCommand => addRowFatCommand ??
                      (addRowFatCommand = _conteiner.Resolve<ICommand>(new NamedParameter("p1", _addRowFat)));
        public ICommand ChangeRowFatCommand => changeRowFatCommand ??
                      (changeRowFatCommand = _conteiner.Resolve<ICommand>(new NamedParameter("p1", _changeRowFat)));
        public ICommand DeleteRowFatCommand => deleteRowFatCommand ??
                      (deleteRowFatCommand = _conteiner.Resolve<ICommand>(new NamedParameter("p1", _deleteRowFat)));

        public ICommand AddFileCommand => addFileCommand ??
                      (addFileCommand = _conteiner.Resolve<ICommand>(new NamedParameter("p1", _addFile)));
        public ICommand ChangeFileCommand => changeFileCommand ??
                      (changeFileCommand = _conteiner.Resolve<ICommand>(new NamedParameter("p1", _changeFile)));
        public ICommand DeleteFileCommand => deleteFileCommand ??
                      (deleteFileCommand = _conteiner.Resolve<ICommand>(new NamedParameter("p1", _deleteFile)));

        public string StrErrors { get => strErrors; set => this.RaiseAndSetIfChanged(ref strErrors, value); }
        public string StrCorrectData { get => strCorrectData; set => this.RaiseAndSetIfChanged(ref strCorrectData, value); }
        public LibraryOrganizationFileSystem.File SelectedFile { get => selectedFile; set => this.RaiseAndSetIfChanged(ref selectedFile, value); }
        public MyClaster SelectedClaster 
        { 
            get => selectedClaster; 
            set {
                //ChangeFatStr = value.Claster; 
                this.RaiseAndSetIfChanged(ref selectedClaster, value); 
            } 
        }
        public ModelOrganizationFile UserDirectory { get => userdirectory; set => this.RaiseAndSetIfChanged(ref userdirectory, value); }
        public string ChangeFatStr { get => chageFatStr; set => this.RaiseAndSetIfChanged(ref chageFatStr, value); }
        public string NameFile { get => nameFile; set => this.RaiseAndSetIfChanged(ref nameFile, value); }
        public string ClasterFile { get => clasterFile; set => this.RaiseAndSetIfChanged(ref clasterFile, value); }
        

        #endregion

        public ViewModelBase()
        {
            // Комманды
            _startCorrect = obj => { StartCorrect(); };
            _findErrors = obj => { FindErrors(); };
            _openHelp = obj => { OpenHelp(); };
            _openInstruction = obj => { OpenInstruction(); };
            _openFile = obj => { OpenFile(); };
            _saveFile = obj => { SaveFile(); };

            _addRowFat = obj => { AddRowFat(); };
            _changeRowFat = obj => { ChangeRowFat(); };
            _deleteRowFat = obj => { DeleteRowFat(); };

            _addFile = obj => { AddFile(); };
            _deleteFile = obj => { DeleteFile(); };
            _changeFile = obj => { ChangeNameFile(); };
        }

        #region Methods

        private void DeleteFile()
        {
            UserDirectory.DeleteFile(SelectedFile);
        }

        private void AddFile()
        {
            try
            {
                UserDirectory.AddFile(NameFile, ClasterFile);
            }
            catch (ArgumentException)
            {
                DefaultDialogService.ShowMessage("Кластер должен быть числом");
            }
            catch (InvalidCastException)
            {
                DefaultDialogService.ShowMessage("Такое имя файла уже существует");
            }
        }
        private void ChangeNameFile()
        {
            try
            {
                UserDirectory.ChangeNameFile(SelectedFile,NameFile);
            }
            catch (ArgumentException)
            {
                DefaultDialogService.ShowMessage("Кластер должен быть числом");
            }
            catch (InvalidCastException)
            {
                DefaultDialogService.ShowMessage("Такое имя файла уже существует");
            }
        }

        private void DeleteRowFat()
        {
            try
            {
                UserDirectory.DeleteRowFat(SelectedClaster);
            }
            catch (ArgumentException)
            {
                DefaultDialogService.ShowMessage("Неверно введны данные");
            }
        }

        private void ChangeRowFat()
        {
            try
            {
                UserDirectory.ChangeRowFat(SelectedClaster.Index, ChangeFatStr);
            }
            catch(ArgumentException)
            {
                DefaultDialogService.ShowMessage("Неверно введны данные");
            }
        }

        private void AddRowFat()
        {
            try
            {
                UserDirectory.AddRowFat(chageFatStr);
            }
            catch (ArgumentException)
            {
                DefaultDialogService.ShowMessage("Неверно введны данные");
            }
        }

        private void SaveFile()
        {
            string filename;
            if (DefaultDialogService.SaveFileDialog("txt files (*.txt)|*.txt", out filename) == true)
            {
                using (StreamWriter sw = new StreamWriter(filename, false, System.Text.Encoding.Default))
                {
                    if (StrCorrectData == "" || StrCorrectData == null)
                    {
                        sw.Write(StrErrors);
                        return;
                    }
                    sw.Write(StrCorrectData);
                }
            }
        }

        private void OpenFile()
        {
            string filename;
            if (DefaultDialogService.OpenFileDialog("txt files (*.txt)|*.txt", out filename) == true)
            {
                
                using (StreamReader sr = new StreamReader(filename, System.Text.Encoding.Default))
                {
                    int countFile;
                    string line = sr.ReadLine();

                    // В первой строке кол-во файлов
                    string[] t1 = line.Trim().Split();
                    if (t1.Length > 1)
                    {
                        DefaultDialogService.ShowMessage("В первой строке должно быть количество файлов \nв пользовательской директории");
                        return;
                    }
                    else if (!int.TryParse(t1[0], out countFile))
                    {
                        DefaultDialogService.ShowMessage("В первой строке должно быть количество файлов \nв пользовательской директории");
                        return;
                    }

                    // В след countFile строках должно быть "имя файла" "кластер" через пробел
                    List<LibraryOrganizationFileSystem.File> ud = new List<LibraryOrganizationFileSystem.File>();
                    while (countFile!=0)
                    {
                        line = sr.ReadLine();
                        string[] t2 = line.Trim().Split();
                        if (t2.Length > 2)
                        {
                            DefaultDialogService.ShowMessage("Неверно введены данные пользовательской директории");
                            return;
                        }
                        int t;
                        if (!int.TryParse(t2[1], out t))
                        {
                            DefaultDialogService.ShowMessage("Неверно введены данные пользовательской директории");
                            return;
                        }
                        ud.Add(new LibraryOrganizationFileSystem.File(t2[0], t));
                        countFile--;
                    }
                    List<string> fat = new List<string>();
                    // В следующих строках массив FAT, на одну строку 1 символ, 0 страка всегда "-", "" заменяется "+"
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        string[] t2 = line.Trim().Split();
                        if (t2.Length > 1)
                        {
                            DefaultDialogService.ShowMessage("Неверно введен FAT");
                            return;
                        }
                        int t;
                        if (t2[0] != "+" && t2[0] != "-" && t2[0] != "eof" && t2[0] != "bad" && !int.TryParse(t2[0], out t))
                        {
                            DefaultDialogService.ShowMessage("Неверно введен FAT");
                            return;
                        }
                        if (t2[0] == "+")
                        {
                            t2[0] = "";
                        }
                        fat.Add(t2[0]);
                    }
                    UserDirectory = new ModelOrganizationFile(ud, fat);
                }
            }
               
        }

        private void StartCorrect()
        {
            StrCorrectData = "Исходные цепочки кластеров:\n";
            StrCorrectData += ConvertToStringPathsFile();
            StrCorrectData += "Исходный fat: \n";
            StrCorrectData += ConvertToStringFAT();
            StrCorrectData += "Исправлено " + UserDirectory.UserDirectory.Count + " файла(ов)\n";
            bool isCorrect = UserDirectory.Correct();
            if (!isCorrect)
            {
                StrCorrectData += "Исправлять нечего\n";
                return;
            }
            StrCorrectData += ConvertToStringErrors();

            StrCorrectData += "Получившиеся цепочки кластеров:\n";
            StrCorrectData += ConvertToStringPathsFile();
            StrCorrectData += "Получившийся fat: \n";
            StrCorrectData += ConvertToStringFAT();
        }

        private void FindErrors()
        {
            bool isFind = UserDirectory.FindErrors();
            StrErrors = "Проверено " + UserDirectory.UserDirectory.Count + " файла(ов)\n";
            StrErrors += "Цепочки кластеров:\n";
            StrErrors += ConvertToStringPathsFile();
            StrErrors += "Исходный fat: \n";
            StrErrors += ConvertToStringFAT();
            if (!isFind)
            {
                StrErrors += "Ошибки не найдены\n";
                return;
            }
            StrErrors += ConvertToStringErrors();
        }

        private string ConvertToStringErrors() 
        {
            string text = "";
            if (UserDirectory.FindEof.Count > 0)
            {
                text += "Ошибка: Найдено " + UserDirectory.FindEof.Count + " потерянных цепочек кластеров\n";
            }
            if (UserDirectory.CountIntersectingClasters > 0)
            {
                text += "Ошибка: Найдено " + UserDirectory.CountIntersectingClasters + " перескающихся кластеров\n";
            }
            return text;
        }
        private string ConvertToStringFAT() 
        {
            string text = "";
            for(int i=0; i< UserDirectory.FatArray.Count; i++)
            {
                text += i + ": " + UserDirectory.FatArray[i].Claster + " ;\n";
            }
            return text;
        }
        private string ConvertToStringPathsFile()
        {
            string text = "";
            foreach (KeyValuePair<string, List<string>> k in UserDirectory.PathsFiles)
            {
                text+= k.Key + ": ";
                for (int i = 0; i < k.Value.Count; i++)
                {
                    text += k.Value[i] + " ";
                }
                text+= "\n";
            }
            return text;
        }

        private void OpenInstruction()
        {
            string help = "Инструкция:\n";
            var info = _conteiner.Resolve<InformationWindow>(new NamedParameter("p1", help));
            info.Show();
        }

        private void OpenHelp()
        {
            string help = "Справка:\n" +
                "Курсовая работа по дисциплине \"Операционные системы\"\n" +
                "Работу выполнила студентка 494 группой 3 курса: Тюлькина Ирина Павловна\n" +
                "По теме: \"\"";
            var info = _conteiner.Resolve<InformationWindow>(new NamedParameter("p1", help));
            info.Show();
        }
        #endregion
    }
}
