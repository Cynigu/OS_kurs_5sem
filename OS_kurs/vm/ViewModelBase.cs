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
        private ModelOrganizationFile userdirectory = new ModelOrganizationFile();

        // Методы
        private readonly Action<object> _startCorrect; // Начать программу по исправлению ошибок файловой системы
        private readonly Action<object> _findErrors; // Поиск ошибок в организации файловой системы
        private readonly Action<object> _openInstruction; // Открыть инструкцию
        private readonly Action<object> _openHelp; // Открыть справку
        private readonly Action<object> _openFile; // Открыть файл
        private readonly Action<object> _saveFile; // Сохранить в файл

        // Комманды
        private ICommand startCorrectionCommand;
        private ICommand findErrorsCommand;
        private ICommand openHelpCommand;
        private ICommand openInstructionCommand;
        private ICommand openFileCommand;
        private ICommand saveFileCommand;

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

        public string StrErrors { get => strErrors; set => this.RaiseAndSetIfChanged(ref strErrors, value); }
        public string StrCorrectData { get => strCorrectData; set => this.RaiseAndSetIfChanged(ref strCorrectData, value); }

        

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
        }



        #region Methods

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
                    userdirectory = new ModelOrganizationFile(ud, fat);
                }
            }
               
        }

        private void StartCorrect()
        {
            StrCorrectData = "Исходные цепочки кластеров:\n";
            StrCorrectData += ConvertToStringPathsFile();
            StrCorrectData += "Исходный fat: \n";
            StrCorrectData += ConvertToStringFAT();
            StrCorrectData += "Исправлено " + userdirectory.UserDirectory.Count + " файла(ов)\n";
            bool isCorrect = userdirectory.Correct();
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
            bool isFind = userdirectory.FindErrors();
            StrErrors = "Проверено " + userdirectory.UserDirectory.Count + " файла(ов)\n";
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
            if (userdirectory.FindEof.Count > 0)
            {
                text += "Ошибка: Найдено " + userdirectory.FindEof.Count + " потерянных цепочек кластеров\n";
            }
            if (userdirectory.CountIntersectingClasters > 0)
            {
                text += "Ошибка: Найдено " + userdirectory.CountIntersectingClasters + " перескающихся кластеров\n";
            }
            return text;
        }
        private string ConvertToStringFAT() 
        {
            string text = "";
            for(int i=0; i< userdirectory.FatArray.Count; i++)
            {
                text += i + ": " + userdirectory.FatArray[i] + " ;\n";
            }
            return text;
        }
        private string ConvertToStringPathsFile()
        {
            string text = "";
            foreach (KeyValuePair<string, List<string>> k in userdirectory.PathsFiles)
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
            string help = "Справка:\n";
            var info = _conteiner.Resolve<InformationWindow>(new NamedParameter("p1", help));
            info.Show();
        }
        #endregion
    }
}
