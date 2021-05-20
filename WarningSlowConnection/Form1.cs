using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Net.NetworkInformation;

namespace WarningSlowConnection
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            // move it out of the visible area (should be invisible but just to not bother anyone)
            this.Location = new Point(int.MinValue, int.MinValue);
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Hide();
            List<string> ignoreNames = Properties.Settings.Default.IgnoreAdapterNames.Cast<string>().Select(s => s?.Trim()).ToList();

            // get all adapters (only connected and not ignored interfaces)
            IEnumerable<NetworkInterface> adapters = NetworkInterface.GetAllNetworkInterfaces().Where(w => w.OperationalStatus == OperationalStatus.Up && !ignoreNames.Any(a => a.Equals(w.Name?.Trim(), StringComparison.OrdinalIgnoreCase)));

            // filter all adapters which are too slow
            List<NetworkInterface> tooSlow = adapters.Where(w => (w.Speed / 1000 / 1000) < Properties.Settings.Default.MinConnectionSpeedMb).ToList();

            if(tooSlow.Count > 0)
            {
                // open adapter overview and print some information
                System.Diagnostics.Process.Start("ncpa.cpl");
                MessageBox.Show($"Following connection speeds are too low:{Environment.NewLine}{Environment.NewLine}- {string.Join($"{Environment.NewLine}- ", tooSlow.Select(s => s.Name))}", $"Interfaces with too slow connection speeds found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            this.Close();
        }
    }
}
