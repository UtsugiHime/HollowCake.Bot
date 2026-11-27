namespace HollowCake.Bot.Core.Exceptions;

public abstract class BotException(string message, Exception? inner = null) : Exception(message, inner)
{
    protected abstract string DefaultUserMessage { get; }

    public string UserMessage
    {
        get => field ?? DefaultUserMessage;
        internal set;
    }
}