using System;

namespace sim800
{
    class Program
    {
        static void Main(string[] args)
        {
            var gsm = new Gsm();
            gsm.GetSerialPorts();
            gsm.ConnectToGsm();
            gsm.SendAt();
            var currentDatetime = gsm.GetDate();
            Console.WriteLine(currentDatetime.ToLongDateString());
        }
    }
}
