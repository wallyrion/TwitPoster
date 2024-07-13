namespace TwitPoster.Chat.IntegrationTests;

public static class TaskExtensions
{
    public static async Task<T> WaitForResult<T>(this TaskCompletionSource<T> source, TimeSpan timeout = default)
    {
        if (timeout == default)
        {
            timeout = TimeSpan.FromSeconds(5);
        }

        var task = source.Task;
        var result = await Task.WhenAny(task, Task.Delay(timeout));
        if (result == task)
        {
            return task.Result;
        }

        throw new TimeoutException();
    }
}