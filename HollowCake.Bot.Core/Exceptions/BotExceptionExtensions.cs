namespace HollowCake.Bot.Core.Exceptions;

public static class BotExceptionExtensions
{
    public static TException WithUserMessage<TException>(this TException exception, string message)
        where TException : BotException
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        exception.UserMessage = message;
        return exception;
    }
}