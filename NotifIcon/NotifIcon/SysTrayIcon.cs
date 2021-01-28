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
    public enum Status
    {
        On = 1,
        Off = 0,
        Unknown = -1
    }

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

    class Util
    {
        // networking communication
        public static Status getDeviceState()
        {
            String url = @"http://192.168.86.222:8888";

            var response = HitEndpoint(url);

            if (response == "true")
            {
                Console.WriteLine($"Response: {response}\nLamp state: {response}");
                return Status.On;
            }

            if (response == "false")
            {
                Console.WriteLine($"Response: {response}\nLamp state: {response}");
                return Status.Off;
            }

            return Status.Unknown;
        }
        public static void setDeviceState(bool state)
        {
            string html = string.Empty;
            List<string> urls = new List<string>
            {
                @"http://192.168.86.222:8888", // direct to iot device
                @"http://192.168.86.21:3000/dev" // server endpoint to iot device
            };
            string url = urls[0] + (state ? "/on" : "/off");

            var response = HitEndpoint(url);

            //if (response.Length > 1)
            //{
            //}

            Console.WriteLine($"Response: {response}\nLamp state: {response}");
        }
        public static String HitEndpoint(string url)
        {
            try
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
            catch (System.Net.WebException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception)
            {
                Console.WriteLine("Something went wrong here.");
            }

            return "";
        }
    }


    class SysTrayIcon : IDisposable
    {
        private NotifyIcon ico { get; }
        public static Lamp lamp { get; set; }
        public SysTrayIcon()
        {
            lamp = new Lamp();
            var _lamp_state = (int) (Util.getDeviceState());
            if (_lamp_state == -1)
            {
                Console.WriteLine("Remote device is offline...");
                lamp.isAvailable = false;
                lamp.state = false;
            }
            else
            {
                lamp.state = _lamp_state == 1;
                lamp.isAvailable = true;
            }


            ico = new NotifyIcon();
            setIcon();
            ico.ContextMenuStrip = CreateContext();
        }
        private void ToggleState()
        {
            Util.setDeviceState(lamp.state);
            ico.Icon = setIcon();
        }
        private void ToggleState(object sender, EventArgs e)
        {
            ToggleState();
        }
        // set icon
        private Icon setIcon()
        {
            var icon = lamp.state
                ? "../../Resources/lamp_on.ico"
                : "../../Resources/lamp_off.ico";

            return new Icon(icon);
        }
        // icon actions
        private void IconRightclick(object sender, MouseEventArgs mouse)
        {
            var position = new Point(System.Windows.Forms.Control.MousePosition.X,
                System.Windows.Forms.Control.MousePosition.X);

            if (mouse.Button == MouseButtons.Right &&
                !ico.ContextMenuStrip.Visible)
            {

                // new Point(20, 20) + mouse.Location;
                ico.ContextMenuStrip.Show(position);
            }
            //else
            //{
            //    ico.ContextMenuStrip.Hide();
            //}
        }
        private void IconDoubleClick(object sender, MouseEventArgs mouse)
        {
            if (mouse.Button == MouseButtons.Left)
            {
                ToggleState();
            }
        }
        // create context menu
        public ContextMenuStrip CreateContext()
        {
            // Add the default menu options.
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem item;
            ToolStripSeparator sep;

            // toggle
            item = new ToolStripMenuItem();
            item.Text = "Toggle";
            item.Click += new EventHandler(ToggleState);
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
        }// context actions
        private void AboutClick(object sender, EventArgs mouse)
        {

            MessageBox.Show(
                "Welcome!",
                "About",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

        }
        private void ExitClick(object sender, EventArgs mouse)
        {
            // Quit without further ado.
            Application.Exit();
        }
        // update
        public void UpdateDisplay()
        {
            ico.Icon = setIcon();
            ico.MouseClick += new MouseEventHandler(IconRightclick);
            ico.MouseDoubleClick += new MouseEventHandler(IconDoubleClick);
            ico.Text = "System tray utility to toggle office lamp state.";
            ico.Visible = true;

            // Attach a context menu.
//            ico.ContextMenuStrip = CreateContext(); // ToDo: look into using resources to store global values.
        }
        // clean-up
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
    }
}
