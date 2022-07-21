using System;
using System.IO.Ports;
using VirtualDesktopLib;
using DisplayManagerLib;
using AutoHotkey.Interop;

using System.Media;
using LibreHardwareMonitor.Hardware;

namespace StreamDeck
{
    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }

    class StreamDeck
    {
        public static void Main(string[] args)
        {
            var ahk = AutoHotkeyEngine.Instance;
            ahk.ExecRaw("#UseHook");
            ahk.ExecRaw("SetKeyDelay, 1, 1");

            int side = 0;
            int disp = 0;

            string[] ports = SerialPort.GetPortNames();
            Console.WriteLine("The following serial ports were found:");
            foreach (string port in ports)
            {
                Console.WriteLine(port);
            }

            var sp = new SerialPort("COM3", 19200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);

            sp.RtsEnable = true;
            sp.DtrEnable = true;

            sp.Open();
            var readData = "";

            char[] tab = new char[4]; // Too big
            int count = 0;


            //long startTimer = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            //Computer computer = new Computer
            //{
            //    IsCpuEnabled = true,
            //    IsGpuEnabled = true,
            //    IsMemoryEnabled = true,
            //    IsMotherboardEnabled = true,
            //    IsControllerEnabled = true,
            //    IsNetworkEnabled = true,
            //    IsStorageEnabled = true
            //};

            //computer.Open();
            //computer.Accept(new UpdateVisitor());

            //int hardwareIndex = -1;
            //int CPU_Index = -1;
            //int sensorIndex = -1;
            //int CPU_Sensor_Index = -1;


            //for (int i = 0; i < computer.Hardware.Count; ++i)
            //{
            //    if (computer.Hardware[i].Name == "AMD Ryzen 7 5800X")
            //    {
            //        CPU_Index = i;
            //    }
            //    if (computer.Hardware[i].Name == "NVIDIA GeForce RTX 3070 Ti")
            //    {
            //        hardwareIndex = i;
            //    }
            //}

            //for (int i = 0; i < computer.Hardware[CPU_Index].Sensors.Length; ++i)
            //{

            //    if (computer.Hardware[CPU_Index].Sensors[i].Name == "Core (Tctl/Tdie)")
            //    {
            //        CPU_Sensor_Index = i;
            //        break;
            //    }
            //}

            //for (int i = 0; i < computer.Hardware[hardwareIndex].Sensors.Length; ++i)
            //{
            //    if (computer.Hardware[hardwareIndex].Sensors[i].Name == "GPU Memory Junction")
            //    {
            //        sensorIndex = i;
            //        break;
            //    }
            //}

            //String vram, cpu, serial;

            //string json;
            //var web = new System.Net.WebClient();
            //var url = @"https://api.binance.com/api/v1/ticker/price?symbol=BTCUSDT";
            //String currentPrice = "";

            while (readData != "Stop received.\r")
            {
                //if ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - startTimer > 5000)
                //{
                //    computer.Accept(new UpdateVisitor());
                //    vram = ((float)computer.Hardware[hardwareIndex].Sensors[sensorIndex].Value).ToString();
                //    cpu = ((float)computer.Hardware[CPU_Index].Sensors[CPU_Sensor_Index].Value).ToString("0.0");
                //    serial = sp.BytesToRead.ToString();

                //    try
                //    {
                //        json = web.DownloadString(url);

                //        dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                //        currentPrice = (Convert.ToDecimal(obj.price)).ToString("0.0");
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine("Exception was triggered : " + e);
                //    }

                //    if (vram.Length == 2)
                //    {
                //        sp.WriteLine("VRAM : " + vram + "           " + "CPU : " + cpu + "          " + "Serial : " + serial + "          " + "BTC : " + currentPrice);
                //    }
                //    else if (vram.Length == 3)
                //    {
                //        sp.WriteLine("VRAM : " + vram + "          " + "CPU : " + cpu + "          " + "Serial : " + serial + "          " + "BTC : " + currentPrice);
                //    }
                //    startTimer = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                //}

                try
                {
                    if (sp.BytesToRead == 1)
                    {
                        sp.Read(tab, 0, 4);

                        switch (new string(tab)) // Ugly conversion
                        {
                            case "OPC4": // PGDN
                                Desktop.FromIndex(0).MakeVisible();
                                break;
                            case "OPC3": // 8
                                Console.WriteLine("Pasting...");
                                ahk.ExecRaw("SendEvent, %Clipboard%");
                                break;
                            case "OPC3\n":
                                Console.WriteLine("OPC3.");
                                break;
                            case "OPC1":
                                Console.WriteLine("OPC1.");
                                break;
                            case "OPC5":
                                for (int i = 0; i < 4; ++i)
                                {
                                    ahk.ExecRaw("SendEvent, 800");
                                    ahk.ExecRaw("SendEvent, {Enter}");
                                    ahk.ExecRaw("SendEvent, {Down}");
                                    ahk.ExecRaw("SendEvent, {Down}");
                                    ahk.ExecRaw("SendEvent, {Left}");
                                    ahk.ExecRaw("SendEvent, {Left}");
                                    ahk.ExecRaw("SendEvent, 1600");
                                    ahk.ExecRaw("SendEvent, {Enter}");

                                    ahk.ExecRaw("SendEvent, {Up}");
                                    ahk.ExecRaw("SendEvent, {Up}");
                                }
                                // Friday
                                ahk.ExecRaw("SendEvent, 800");
                                ahk.ExecRaw("SendEvent, {Enter}");
                                ahk.ExecRaw("SendEvent, {Down}");
                                ahk.ExecRaw("SendEvent, {Down}");
                                ahk.ExecRaw("SendEvent, {Left}");
                                ahk.ExecRaw("SendEvent, {Left}");
                                ahk.ExecRaw("SendEvent, 1600");
                                ahk.ExecRaw("SendEvent, {Enter}");

                                ahk.ExecRaw("SendEvent, {F9}");
                                ahk.ExecRaw("SendEvent, {Right}");
                                ahk.ExecRaw("SendEvent, {Right}");
                                ahk.ExecRaw("SendEvent, {Right}");
                                ahk.ExecRaw("SendEvent, {Down}");
                                ahk.ExecRaw("SendEvent, R");
                                ahk.ExecRaw("SendEvent, {Enter}");
                                ahk.ExecRaw("SendEvent, {F12}");
                                break;
                            case "OPC6": // 5
                                Desktop.FromIndex(1).MakeVisible();
                                // ahk.ExecRaw("SendEvent, {F5}");
                                break;
                            case "OPC7": // 7
                                Desktop.FromIndex(2).MakeVisible();
                                // ahk.ExecRaw("SendEvent, {F7}");
                                break;
                        }
                    }
                    else if (sp.BytesToRead > 4)
                    {
                        sp.Close();
                        sp.Open();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    DateTime now = DateTime.Now;
                    Console.WriteLine(now.ToString("F"));
                    Console.WriteLine("---");

                    if (e.Message == "The port is closed.")
                    {
                        try
                        {
                            sp.Open();
                        }
                        catch
                        {
                            Console.WriteLine("Could not reconnect.");
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        SystemSounds.Exclamation.Play();
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            sp.Close();
            // computer.Close();
        }
    }
}
