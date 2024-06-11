using Testcontainers.RabbitMq;

namespace TwitPoster.EmailSender.IntegrationTests.Fixtures;

public static class RabbitMqContainerExtensions
{
    private record RabbitMqQueue(string Name, int Messages);
  
    public static async Task WaitForQueueToBeReady(this RabbitMqContainer rabbitMqContainer, string queueName, TimeSpan? timeout = null)
    {
        timeout ??= TimeSpan.FromSeconds(10);
        
        var tcs = new TaskCompletionSource();
        var cancellationTokenSource = new CancellationTokenSource();

        _ = Task.Run(async () =>
        {
            while (true)
            {
                var queues = rabbitMqContainer.ListQueuesAsync(cancellationTokenSource.Token).Result;
                var queue = queues.FirstOrDefault(x => x.Name == queueName);

                if (queue is not { Messages: 0 })
                {
                    await Task.Delay(100, cancellationTokenSource.Token);
                    continue;
                }

                tcs.SetResult();
                break;
            }
        
        }, cancellationTokenSource.Token);
        
        var result = await Task.WhenAny(tcs.Task, Task.Delay(timeout.Value, cancellationTokenSource.Token));
        if (result != tcs.Task)
        {
            await cancellationTokenSource.CancelAsync();
            throw new TimeoutException($"Queue {queueName} did not become ready in {timeout.Value.TotalSeconds} seconds");
        }
    }

    
    private static async Task<List<RabbitMqQueue>> ListQueuesAsync(this RabbitMqContainer rabbitMqContainer, CancellationToken ct = default)
    {
        IList<string> command = new List<string> { "rabbitmqctl", "list_queues" };

        var result = await rabbitMqContainer.ExecAsync(command, ct);

        if (result.ExitCode == 0)
        {
            return ParseQueueOutput(result.Stdout);
        }

        throw new Exception($"Command failed with exit code {result.ExitCode}: {result.Stderr}");
    }

    private static readonly char[] OutputSeparator = ['\t'];
    private static readonly char[] NewLineSeparator = ['\r', '\n'];

    private static List<RabbitMqQueue> ParseQueueOutput(string output)
    {
        var lines = output.Split(NewLineSeparator, StringSplitOptions.RemoveEmptyEntries).Skip(1); // Skip the header
        var queues = new List<RabbitMqQueue>();

        foreach (var line in lines)
        {
            var parts = line.Split(OutputSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2 && int.TryParse(parts[1], out var messages))
            {
                queues.Add(new RabbitMqQueue ( parts[0], messages ));
            }
        }

        return queues;
    }
}