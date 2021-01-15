using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using NotifIcon.Properties;
using SysTrayControls;
//using SysTrayControls.Properties;

namespace bb_SysTray
{
    public class Lamp
    {
        public bool isAvailable { get; set; }
        public bool state { get; set; }

        public bool Toggle()
        {
            state = !state;
            return state;
        }
    }


    class SysTrayIcon : IDisposable
    {
        private NotifyIcon ico { get; }
        public static Lamp lamp { get; set; }

        public SysTrayIcon()
        {
            ico = new NotifyIcon();
            ico.ContextMenu = new _ContextMenu();
            lamp = new Lamp();

            var _lamp_state = (int) getDeviceState();
            if (_lamp_state == 3)
            {
                lamp.isAvailable = false;
                lamp.state = false;
            }else{
            lamp.state = _lamp_state == 1;
            }
        }

        public enum Status
        {
            On=1,
            Off=0,
            Unknown=-1
        }
        public Status getDeviceState()
        {
            String url = @"http://192.168.86.236/status";

            var response = hitEndpoint(url);

            if(response == "true")
                return Status.On;
            if (response == "false")
                return Status.Off;
            return Status.Unknown;
        }
        public void setDeviceState(bool state)
        {
            string html = string.Empty;
            List<string> urls = new List<string>
            {
                @"http://192.168.86.236", // direct to iot device
                @"http://192.168.86.21:3000/dev" // server endpoint to iot device
            };
            string url = urls[0] + (lamp.state ? "/on" : "/off");

            var response = hitEndpoint(url);

            Console.WriteLine($"Response: {response}\nLamp state: {lamp.state}");


        }

        private String hitEndpoint(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            var webResponse = request.GetResponse();
            var webStream = webResponse.GetResponseStream();
            var responseReader = new StreamReader(webStream);
            var response = responseReader.ReadToEnd();
            
            responseReader.Close();
            return response;
        }

        public void Display()
        {
            ico.Icon = setIcon();
            ico.MouseClick += new MouseEventHandler(ico_MouseClick);
            ico.Text = "System tray utility to toggle office lamp state.";
            ico.Visible = true;

            // Attach a context menu.
            ico.ContextMenuStrip =
                new _ContextMenu().Create(); // ToDo: look into using resources to store global values.
        }

        private void ico_MouseClick(object sender, MouseEventArgs mouse)
        {
            if (mouse.Button == MouseButtons.Right)
            {
                ico.ContextMenuStrip.Show();
            }
            else if (mouse.Button == MouseButtons.Left)
            {
                lamp.Toggle();
            }

            setDeviceState(lamp.state);
            ico.Icon = setIcon();
        }

        private Icon setIcon()
        {
            var icon = lamp.state
                ? "C:\\Users\\Mr.BiggyBear\\source\\repos\\InANutshell_8\\SysTrayControls\\Resources\\lamp_on.ico"
                : "C:\\Users\\Mr.BiggyBear\\source\\repos\\InANutshell_8\\SysTrayControls\\Resources\\lamp_off.ico";

            return  new Icon(icon);
        }
        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SysTrayIcon()
        {
            Dispose(false);
        }

        public class _ContextMenu : ContextMenu
        {
            private bool isAboutOpen = false;

            public _ContextMenu()
            {
            }

            public ContextMenuStrip Create()
            {
                // Add the default menu options.
                ContextMenuStrip menu = new ContextMenuStrip();
                ToolStripMenuItem item;
                ToolStripSeparator sep;

                // Windows Explorer.
                item = new ToolStripMenuItem();
                item.Text = "Toggle";
                item.Click += new EventHandler(ToggleClick);
                item.Image = lamp.state ? Resources.lamp_on : Resources.lamp_off;
                menu.Items.Add(item);

                // About.
                item = new ToolStripMenuItem();
                item.Text = "About";
                item.Click += new EventHandler(AboutClick);
                //item.Image = Resources.About;
                menu.Items.Add(item);

                // Separator.
                sep = new ToolStripSeparator();
                menu.Items.Add(sep);

                // Exit.
                item = new ToolStripMenuItem();
                item.Text = "Exit";
                item.Click += new System.EventHandler(ExitClick);
                //item.Image = Resources.Exit;
                menu.Items.Add(item);

                return menu;
            }

            private void ToggleClick(object sender, EventArgs mouse)
            {
                lamp.Toggle();
            }

            private void AboutClick(object sender, EventArgs mouse)
            {
                //if (!isAboutLoaded)
                //{
                //    isAboutLoaded = true;
                //    new AboutBox().ShowDialog();
                //    isAboutLoaded = false;
                //}
            }
            private void ExitClick(object sender, EventArgs mouse)
            {
                // Quit without further ado.
                Application.Exit();
            }
        }
    }
}
