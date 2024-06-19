using OpenHardwareMonitor.Hardware;
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
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;



namespace PowerStats
{
    public partial class Form1 : Form
    {
        private Computer computer;

        public Form1()
        {
            InitializeComponent();
            computer = new Computer();
            computer.CPUEnabled = true; // İsteğe bağlı, diğer donanım türlerini de etkinleştirebilirsiniz
            computer.GPUEnabled = true;
            computer.Open();

            timer1.Interval = 3000; // 3 saniye
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Start();

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_POWER_STATUS
        {
            public byte ACLineStatus;
            public byte BatteryFlag;
            public byte BatteryLifePercent;
            public byte Reserved1;
            public int BatteryLifeTime;
            public int BatteryFullLifeTime;
        }

        [DllImport("kernel32.dll")]
        public static extern bool GetSystemPowerStatus(out SYSTEM_POWER_STATUS lpSystemPowerStatus);

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateBatteryInfo();
            // Timer'ı başlat
            timer1.Interval = 3000; // 3 saniye
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Start();

        }

        private void UpdateBatteryInfo()
        {
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Battery");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection collection = searcher.Get();

            foreach (ManagementObject mo in collection)
            {
                label4.Text = ("Batarya Türü: " + mo["Chemistry"]);
            }

            SYSTEM_POWER_STATUS status;
            if (GetSystemPowerStatus(out status))
            {
                label1.Text = ("AC Durumu: " + (status.ACLineStatus == 1 ? "Şarj oluyor" : "Şarj Olmuyor"));
                byte batteryFlag = status.BatteryFlag == 0 ? (byte)1 : status.BatteryFlag;
                label2.Text = ("Batarya Durumu: " + batteryFlag);
                label3.Text = ("Batarya Şarj Yüzdesi: " + "%" + status.BatteryLifePercent);
            }
            else
            {
                label4.Text = ("Batarya durumu alınamadı.");
            }

            //foreach (var hardwareItem in computer.Hardware)
            //{
            //    hardwareItem.Update();
            //    if (hardwareItem.HardwareType == HardwareType.Battery)
            //    {
            //        foreach (var sensor in hardwareItem.Sensors)
            //        {
            //            if (sensor.SensorType == SensorType.Load)
            //            {
            //                label5.Text = "Pil Sağlığı: " + sensor.Value.ToString() + "%";
            //            }
            //        }
            //    }
            //}

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateBatteryInfo();
        }

    }
}
