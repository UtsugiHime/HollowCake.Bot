using NetCord;

namespace HollowCake.Bot.NetCord.Exceptions;

public class InteractionErrorOptions
{
    public string Title { get; set; } = "Error";
    
    public string UnexpectedErrorMessage { get; set; } = "An unexpected error occurred. Please try again later.";
    public string ExecutionFailedMessage { get; set; } = "Unable to complete this interaction. Please try again later.";
}