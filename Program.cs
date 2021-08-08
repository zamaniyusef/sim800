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
            foreach(string port in ports){
                Console.WriteLine(port);
            }
            Console.ReadLine();
        }
    }
}
