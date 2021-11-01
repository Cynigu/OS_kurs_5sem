using Autofac;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DialogServiceForWPF2;
namespace OS_kurs
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            var builderBase = new ContainerBuilder();
            builderBase.RegisterType<ViewModelBase>().AsSelf();
            var containerBase = builderBase.Build();
            var viewmodelBase = containerBase.Resolve<ViewModelBase>();
            var viewBase = new MainWindow { DataContext = viewmodelBase };
            viewBase.Show();
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            DefaultDialogService.ShowMessage("Ошибка\n" + e.Exception.StackTrace + " " + "Исключение: "
                + e.Exception.GetType().ToString() + " " + e.Exception.Message);

            e.Handled = true;
        }
    }
}
