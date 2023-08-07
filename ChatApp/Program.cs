using ChatApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using (ServiceProvider serviceProvider = new ServiceCollection()
     .AddLogging(log =>
     {
         log.AddConsole();
     })
     .AddSingleton<ChatClient>()
     .BuildServiceProvider())
{
    Console.WriteLine("Enter your name");

    string name = Console.ReadLine();

    ChatClient client = serviceProvider.GetRequiredService<ChatClient>();

    Task t = Task.Run(async () =>
    {
        await client.Connect("ws://localhost:9000", new ChatApp.Shared.Model.UserModel { Name = name });
    });
    t.Wait();
    using (CancellationTokenSource tokenSource = new CancellationTokenSource())
    {
        Task listenting = client.ListenForMessages(tokenSource, (str) =>
        {
            Console.WriteLine(str.Substring(2));
        });

        string input = string.Empty;
        while(input != "exit")
        {
            input = Console.ReadLine();

            if(!string.IsNullOrEmpty(input))
            {
                await client.SendMessage(input);
            }
        }

        tokenSource.Cancel();
        await listenting;
    }
}