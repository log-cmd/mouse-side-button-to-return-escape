using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace mouse_side_button_to_return_escape
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            new Thread(() => Func(System.Windows.Forms.Keys.Escape, ev_escape)) { IsBackground = true }.Start();
            new Thread(() => Func(System.Windows.Forms.Keys.Return, ev_return)) { IsBackground = true }.Start();

            hook = new MouseHook();

            hook.OnX1Down += () => ev_escape.Set();
            hook.OnX2Down += () => ev_return.Set();

            hook.Hook();
        }

        MouseHook hook;

        AutoResetEvent ev_return = new AutoResetEvent(false);
        AutoResetEvent ev_escape = new AutoResetEvent(false);

        void Func(System.Windows.Forms.Keys key, AutoResetEvent ev)
        {
            while (true)
            {
                ev.WaitOne();
                SI.Send(key, true);
                Thread.Sleep(50);
                SI.Send(key, false);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            hook.UnHook();
        }
    }
}
