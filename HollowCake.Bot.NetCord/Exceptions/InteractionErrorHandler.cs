using global::NetCord;
using HollowCake.Bot.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using NetCord.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;

namespace HollowCake.Bot.NetCord.Exceptions;

public sealed class InteractionErrorHandler(InteractionErrorOptions? options = null) : 
    IApplicationCommandResultHandler<ApplicationCommandContext>, 
    IComponentInteractionResultHandler<ComponentInteractionContext>
{
    private const int AlreadyAcknowledged = 40060;
    private const int UnknownMessage = 10008;
    
    private readonly InteractionErrorOptions _options = options ?? new InteractionErrorOptions();

    public ValueTask HandleResultAsync(
        IExecutionResult result, 
        ApplicationCommandContext context, 
        GatewayClient? client,
        ILogger logger, 
        IServiceProvider services) 
        => HandleAsync(result, context.Interaction, logger, client, services);

    public ValueTask HandleResultAsync(
        IExecutionResult result, 
        ComponentInteractionContext context, 
        GatewayClient? client,
        ILogger logger, 
        IServiceProvider services) 
        => HandleAsync(result, context.Interaction, logger, client, services);

    private async ValueTask HandleAsync(
        IExecutionResult result,
        Interaction interaction,
        ILogger logger,
        GatewayClient? client,
        IServiceProvider services)
    {
        if (result is not IFailResult failure)
            return;

        var message = ResolveMessage(failure, logger);

        var embed = new EmbedProperties()
            .WithTitle(_options.Title)
            .WithDescription(message)
            .WithColor(_options.Color);

        try
        {
            var rest = client?.Rest ?? services.GetRequiredService<RestClient>();
            await RespondAsync(interaction, embed, rest);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to respond to interaction {InteractionId}.", interaction.Id);
        }
    }

    private string ResolveMessage(IFailResult failure, ILogger logger)
    {
        if (failure is IExceptionResult exceptionResult)
        {
            var exception = exceptionResult.Exception;

            if (exception is BotException botException)
            {
                logger.Log(
                    botException is BotExternalServiceException ? LogLevel.Error : LogLevel.Warning,
                    botException,
                    "{ExceptionMessage}",
                    botException.Message);
                
                return botException.UserMessage;
            }
            logger.LogError(exception, "Unexpected interaction error.");
            return _options.UnexpectedErrorMessage;
        }
        logger.LogWarning("Interaction execution failed: {FailureMessage}", failure.Message);
        return _options.ExecutionFailedMessage;
    }

    private static async Task RespondAsync(Interaction interaction, EmbedProperties embed, RestClient rest)
    {
        var message = new InteractionMessageProperties()
            .WithEmbeds([embed])
            .WithFlags(MessageFlags.Ephemeral);

        try
        {
            await interaction.SendResponseAsync(InteractionCallback.Message(message));
            return;
        }
        catch (RestException exception)when (exception.Error?.Code == AlreadyAcknowledged)
        {
            // Interaction already acknowledged.
        }

        RestMessage? original = null;

        try
        {
            original = await rest.GetInteractionResponseAsync(interaction.ApplicationId, interaction.Token);
        }
        catch (RestException exception)when (exception.Error?.Code == UnknownMessage)
        {
            // No original response exists.
        }

        if (original is not null && original.Flags.HasFlag(MessageFlags.Loading))
        {
            await rest.ModifyInteractionResponseAsync(
                interaction.ApplicationId,
                interaction.Token,
                response =>
                {
                    response.WithEmbeds([embed]);
                });

            return;
        }

        await rest.SendInteractionFollowupMessageAsync(
            interaction.ApplicationId,
            interaction.Token,
            message);
    }
}