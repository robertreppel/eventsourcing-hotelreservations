using System;
using Ada.Hotel.Reservations;
using Cqrs.Services;
using Nancy.Hosting.Self;
namespace Ada.Hotel.Api
{
    class Program
    {
        static void Main(string[] args)
        {
            var directBus = DirectBus.Instance;
            Bootstrap.WireUp(directBus, directBus);
         
            var url = "http://127.0.0.1:9000";
            using (var host = new NancyHost(new Uri(url)))
            {
                host.Start();
                Console.WriteLine("Nancy Server listening on  {0}", url);
                Console.ReadLine();
            }
        }

    }
}
