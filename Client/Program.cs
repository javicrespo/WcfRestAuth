using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using WcfHttpAuth.Wsse;

namespace Client
{
    class Program
    {
        static void Main()
        {
            //wsse
            Console.WriteLine("\nWsse");

            try
            {
                var response = WebRequest.Create("http://localhost:2391/WsseService.svc/HelloWorld")
                    .WithWsseToken("user", "password")
                    .GetResponse();

                using (var readStream = new StreamReader(response.GetResponseStream()))
                {
                    var buffer = new Char[1024];
                    int count;
                    do
                    {
                        count = readStream.Read(buffer, 0, buffer.Length);
                        Console.Write(buffer.Take(count).ToArray());

                    } while (count != 0);
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine("Error!:"+ ex.Message);
            }

            //basic
            Console.WriteLine("\n\nBasic");

            var request = WebRequest.Create("http://localhost:2391/BasicService.svc/HelloWorld");
            request.Credentials = new NetworkCredential("user", "password");
            try
            {
                using (var readStream = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    var buffer = new Char[1024];
                    int count;
                    do
                    {
                        count = readStream.Read(buffer, 0, buffer.Length);
                        Console.Write(buffer.Take(count).ToArray());

                    } while (count != 0);
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine("Error!:" + ex.Message);
            }
            Console.WriteLine("\n\nPress a key to close.");
            Console.Read();
        }
        
    }
}
