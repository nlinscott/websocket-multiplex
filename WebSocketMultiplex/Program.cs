using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketMultiplex
{
    class Program
    {
        static void Main(string[] args)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Task running = Task.Run(() =>
                 {
                     using (IWebHost host = WebHost.CreateDefaultBuilder()
                         .UseStartup<Startup>()
                         .UseKestrel()
                         .ConfigureKestrel(opts => opts.ListenLocalhost(9000))
                         .Build())
                     {
                         host.RunAsync(cts.Token).Wait();
                     }
                 });

                System.Console.WriteLine("Running. Press any key to exit");

                System.Console.ReadKey();

                cts.Cancel();

                running.Wait();
            }
        }
    }
}
