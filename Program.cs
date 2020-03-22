using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleWithWeb
{
    class Program
    {
        static void Main()
        {
            // Start web server
            var webProgram = new WebProgram(new string[] { }, listenPort: 8080);
            Task.Factory.StartNew(webProgram.RunSync, TaskCreationOptions.LongRunning);
            
            // Run the original main loop
            OriginalConsoleProjectMain(webProgram.BridgeInstance);
            
            webProgram.Shutdown();
        }

        // Originally the console project would have had only this as its Main
        // It will count up from zero and make each new result available for web viewing
        // at GET localhost:8080/latest_value
        static void OriginalConsoleProjectMain(BridgeService bridgeService)
        {
            var x = 0.0;
            var increment = 1.0;

            while (true)
            {
                x += increment;
                Console.WriteLine("The value is now {0}", x);
                
                // This is the new line added to
                bridgeService.SetLatestValue(x);
                
                Thread.Sleep(100);
            }
        }
    }
}