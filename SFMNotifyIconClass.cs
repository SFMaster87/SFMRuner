using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    /// <summary>
    /// Класс представляющий иконку в tray
    /// </summary>

    sealed class SFMNotifyIconClass
    {
        private System.Windows.Forms.NotifyIcon _notifyIcon;    //object iconTray
        private MainWindow _mainWindow;                         // object mainWindow

        public SFMNotifyIconClass(MainWindow mainWindow)
        {
            this._mainWindow = mainWindow;

            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(NotifyIcon_Click);
            _notifyIcon.Icon = new System.Drawing.Icon("notify.ico");
            _notifyIcon.Visible = true;

            CreateNotifyContextMenu();            
        }

        //click по иконке в tray
        private void NotifyIcon_Click(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _mainWindow.Show();
                _mainWindow.Activate();
            }
        }

        //создание контекстного меню в tray
        private void CreateNotifyContextMenu()
        {
            System.Windows.Forms.ContextMenu notifyIconContextMenu = new System.Windows.Forms.ContextMenu();

            notifyIconContextMenu.MenuItems.Add("Exit", new EventHandler(exitItem_ContextMenu_Click));

            _notifyIcon.ContextMenu = notifyIconContextMenu;
        }
        
        private void exitItem_ContextMenu_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
