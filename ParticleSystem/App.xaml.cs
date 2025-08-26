using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ParticleSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
    }
}

//static class program
//{
//    [STAThread]
//    static void Main()
//    {
//        var app = new Application();
//        app.ShutdownMode = ShutdownMode.OnMainWindowClose;
//        var wnd = new particle_window(width: 1280, height: 720, particle_count: 100_000);
//        app.Run(wnd);
//    }
//}