using Autofac;
using CommandsService2;
using DialogServiceForWPF2;
using OS_kurs.view;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OS_kurs
{
    class MyConteiner
    {
        public static ContainerBuilder ContainerMain()
        {
            var builderBase = new ContainerBuilder();
            builderBase.Register((c, p) => new RelayCommand(p.Named<Action<object>>("p1"))).AsSelf().As<ICommand>();
            builderBase.Register((c, p) => new AsyncCommand(p.Named<Func<Task>>("p1"))).AsSelf().As<IAsyncCommand>();
            builderBase.Register((c, p) => new InformationWindow(p.Named<string>("p1"))).AsSelf();
            return builderBase;
        }
    }
}
