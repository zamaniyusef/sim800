using System;
using System.IO.Ports;
using System.Globalization;
using System.Threading;
using System.Text;
using System.Linq;

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
            Thread.Sleep(1000);
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
                Thread.Sleep(10000);
                data = Serial.ReadExisting();
                int pFrom = data.IndexOf("Your balance is ") + "Your balance is ".Length;
                int pTo = data.LastIndexOf(" Rial");
                data = data.Substring(pFrom, pTo - pFrom);
                return int.Parse(data);
            }
            return 0;
        }

        public void SendSms()
        {
            Serial.WriteLine("AT+CMGF=1\r");
            Thread.Sleep(1000);
            Serial.Write("AT+CMGS=\"09372024722\"\r\n");
            Thread.Sleep(1000);
            Serial.Write("This is text message\x1A");
            Thread.Sleep(1000);
            var data = Serial.ReadLine();
            Console.WriteLine($"Respose From Sim800 is {data}");
            Console.WriteLine("Message Sent");
        }

        public void SendPersianSms()
        {
            Serial.WriteLine("AT+CSCS=?\r\n");
            Thread.Sleep(1000);
            var data = Serial.ReadExisting();
            Console.WriteLine($"Respose From Sim800 is {data}");
            Serial.WriteLine("AT+CSCS=\"HEX\"\r\n");
            Thread.Sleep(1000);
            Serial.WriteLine("AT+CSMP=49,167,0,8\r\n");
            Thread.Sleep(1000);
            Serial.Write("AT+CMGS=\"09133084832\"\r\n");
            Thread.Sleep(1000);
            var test = "سلام\nبه شما تبریک می‌گویم این اولین پیامک فارسی ماست.\nMr Porkabirian".Select(t => $"{Convert.ToUInt16(t):X4}").ToArray();
            var message = "";
            for (int i = 0; i < test.Length; i++)
            {
                message += test[i];
            }
            Console.WriteLine(message);
            Serial.Write(message + "\x1A");
            Thread.Sleep(2000);
            Console.WriteLine("Sent Message");
        }

        public void Close()
        {
            Serial.Close();
        }
    }
}