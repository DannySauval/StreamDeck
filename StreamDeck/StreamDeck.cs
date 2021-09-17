using System;
using System.IO.Ports;
using VirtualDesktopLib;
using DisplayManagerLib;
using AutoHotkey.Interop;

using System.Media;

namespace StreamDeck
{
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

            var sp = new SerialPort(ports[0], 19200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);

            sp.RtsEnable = true;
            sp.DtrEnable = true;

            sp.Open();
            var readData = "";

            char[] tab = new char[5]; // Too big

            while (readData != "Stop received.\r")
            {
                try
                {
                    //readData = sp.ReadLine();
                    if (sp.BytesToRead == 5)
                    {
                        sp.Read(tab, 0, 5);
                        // Console.WriteLine(tab);

                        switch (new string(tab)) // Ugly conversion
                        {
                            case "OPC1\n":
                                if (side == 0)
                                {
                                    side = 1;
                                    if (Desktop.FromDesktop(Desktop.FromIndex(Desktop.Count - 1)) != 0)
                                    {
                                        Desktop.Current.Right.MakeVisible();
                                        Console.WriteLine("Switching to right virtual desktop");
                                    }
                                    else
                                    {
                                        SystemSounds.Exclamation.Play();
                                        Console.WriteLine("No right virtual desktop to switch to.");
                                    }
                                }
                                else if (side == 1)
                                {
                                    side = 0;
                                    if (Desktop.FromDesktop(Desktop.FromIndex(Desktop.Count - 1)) != 0)
                                    {
                                        Desktop.Current.Left.MakeVisible();
                                        Console.WriteLine("Switching to left virtual desktop");
                                    }
                                    else
                                    {
                                        SystemSounds.Exclamation.Play();
                                        Console.WriteLine("No left virtual desktop to switch to.");
                                    }
                                }
                                break;
                            case "OPC0\n":
                                /*if (disp != 1)
                                {
                                    disp = 1;
                                    DisplayManager.SetDisplayMode(DisplayManager.DisplayMode.External);
                                }
                                else
                                {
                                    disp = 0;
                                    DisplayManager.SetDisplayMode(DisplayManager.DisplayMode.Extend);
                                }*/
                                Console.WriteLine("Pasting...");
                                ahk.ExecRaw("SendEvent, %Clipboard%");
                                break;
                            case "OPC2\n":
                                break;
                                if (disp != 2)
                                {
                                    disp = 2;
                                    DisplayManager.SetDisplayMode(DisplayManager.DisplayMode.Internal);
                                }
                                else
                                {
                                    disp = 0;
                                    DisplayManager.SetDisplayMode(DisplayManager.DisplayMode.Extend);
                                }
                        }
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
                        SystemSounds.Exclamation.Play();
                }
                System.Threading.Thread.Sleep(1);
            }
            sp.Close();
        }
    }
}
