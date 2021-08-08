using System;
using System.IO.Ports;

namespace sim800
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] ports = SerialPort.GetPortNames();
            Console.WriteLine("The following serial ports were found:");
            foreach(string port in ports)
            {
                Console.WriteLine(port);
            }
            var serial = new SerialPort(ports[0],115200);
            serial.ReadTimeout = 3000;
            serial.WriteTimeout = 3000;
            serial.Open();
            serial.Write("AT\r\n");
            var data = serial.ReadLine();
            Console.WriteLine(data);
            serial.WriteLine("AT+CCLK?\r\n");
            data = serial.ReadLine();
            Console.WriteLine(data);
            data = serial.ReadLine();
            Console.WriteLine(data);
            data = serial.ReadLine();
            Console.WriteLine(data);
        }
    }
}
