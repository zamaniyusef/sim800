using System;
using System.IO.Ports;
using System.Globalization;
using System.Threading;

namespace sim800
{
    public class Gsm
    {
        private const int BUADRATE = 115200;
        private const int READ_TIMEOUT = 3000;
        private const int WRITE_TIMEOUT = 3000;
        private SerialPort Serial { get; set; }
        private string[] Ports { get; set; }

        public Gsm() { }

        public void ConnectToGsm()
        {
            Serial = new SerialPort(Ports[0], BUADRATE);
            Serial.ReadTimeout = READ_TIMEOUT;
            Serial.WriteTimeout = WRITE_TIMEOUT;
            Serial.Open();
        }

        public void GetSerialPorts()
        {
            Ports = SerialPort.GetPortNames();
            Console.WriteLine("The following serial ports were found:");
            foreach (string port in Ports)
            {
                Console.WriteLine(port);
            }
        }

        public void SendAt()
        {
            Console.WriteLine("Send At Command To Sim800");
            Serial.Write("AT\r\n");
            var data = Serial.ReadLine();
            Console.WriteLine($"Respose From Sim800 is {data}");
        }

        public DateTime GetDate()
        {
            Serial.WriteLine("AT+CCLK?\r\n");
            string data = Serial.ReadLine();
            if (data.Contains("OK"))
            {
                data = Serial.ReadLine();
                data = Serial.ReadLine();
                data = data.Split('"')[1];
                return DateTime.ParseExact(data,
                "yy/MM/dd,HH:mm:ss+ff",
                CultureInfo.InvariantCulture);
            }
            return new DateTime();
        }

        public int GetBalance()
        {
            Serial.WriteLine("AT+CUSD=1\r\n");
            string data = Serial.ReadLine();
            data = Serial.ReadLine();
            if (data.Contains("OK"))
            {
                Serial.WriteLine("AT+CUSD=1,\"*140*11#\"\r\n");
                Thread.Sleep(8000);
                data = Serial.ReadExisting();
                int pFrom = data.IndexOf("Your balance is ") + "Your balance is ".Length;
                int pTo = data.LastIndexOf(" Rial");
                data = data.Substring(pFrom, pTo - pFrom);
                return int.Parse(data);
            }
            return 0;
        }
    }
}