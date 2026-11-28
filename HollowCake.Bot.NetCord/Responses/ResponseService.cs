using NetCord;
using NetCord.Rest;

namespace HollowCake.Bot.NetCord.Responses;

public sealed class ResponseService(RestClient rest)
{
    private readonly InteractionResponseStateTracker _tracker = new();
    
    internal InteractionResponseState GetState(Interaction interaction) 
        => _tracker.GetState(interaction);
    
    // Raw methods
    public async Task RespondAsync(Interaction interaction, InteractionMessageProperties message)
    {
        await interaction.SendResponseAsync(InteractionCallback.Message(message));
        _tracker.SetState(interaction, InteractionResponseState.Acknowledged);
    }

    public async Task DeferAsync(Interaction interaction, bool ephemeral = false)
    {
        await interaction.SendResponseAsync(
            InteractionCallback.DeferredMessage(ephemeral ? MessageFlags.Ephemeral : null));
        _tracker.SetState(interaction, InteractionResponseState.DeferredMessage);
    }

    public async Task DeferUpdateAsync(ComponentInteraction interaction)
    {
        await interaction.SendResponseAsync(InteractionCallback.DeferredModifyMessage);
        _tracker.SetState(interaction, InteractionResponseState.DeferredUpdate);
    }

    public async Task<RestMessage> ModifyResponseAsync(Interaction interaction, Action<MessageOptions> modify)
    {
        var message = await rest.ModifyInteractionResponseAsync(interaction.ApplicationId, interaction.Token, modify);
        CompleteDeferral(interaction);
        return message;
    }

    public async Task<RestMessage> FollowupAsync(Interaction interaction, InteractionMessageProperties message)
    {
        var response = await rest.SendInteractionFollowupMessageAsync(
            interaction.ApplicationId, interaction.Token, message);
        
        if (_tracker.GetState(interaction) is InteractionResponseState.DeferredMessage)
            CompleteDeferral(interaction);
        
        return response;
    }

    public Task<RestMessage> GetResponseAsync(Interaction interaction)
        => rest.GetInteractionResponseAsync(interaction.ApplicationId, interaction.Token);
    
    public async Task DeleteResponseAsync(Interaction interaction)
    {
        await rest.DeleteInteractionResponseAsync(interaction.ApplicationId, interaction.Token);
        CompleteDeferral(interaction);
    }

    // Convenience methods
    public Task RespondAsync(Interaction interaction, EmbedProperties embed, bool ephemeral = false)
        => RespondAsync(interaction, new InteractionMessageProperties()
            .AddEmbeds(embed)
            .WithFlags(ephemeral ? MessageFlags.Ephemeral : null));

    public Task RespondAsync(Interaction interaction, string content, bool ephemeral = false)
        => RespondAsync(interaction, new InteractionMessageProperties()
            .WithContent(content)
            .WithFlags(ephemeral ? MessageFlags.Ephemeral : null));

    public Task RespondAsync(Interaction interaction, IEnumerable<AttachmentProperties> attachments,
        bool ephemeral = false)
        => RespondAsync(interaction, new InteractionMessageProperties()
            .AddAttachments(attachments)
            .WithFlags(ephemeral ? MessageFlags.Ephemeral : null));

    public Task RespondAsync(Interaction interaction, EmbedProperties embed, IEnumerable<AttachmentProperties> attachments,
        bool ephemeral = false)
        => RespondAsync(interaction, new InteractionMessageProperties()
            .AddEmbeds(embed)
            .AddAttachments(attachments)
            .WithFlags(ephemeral ? MessageFlags.Ephemeral : null));

    public Task RespondAsync(Interaction interaction, string content, IEnumerable<AttachmentProperties> attachments,
        bool ephemeral = false)
        => RespondAsync(interaction, new InteractionMessageProperties()
            .WithContent(content)
            .AddAttachments(attachments)
            .WithFlags(ephemeral ? MessageFlags.Ephemeral : null));

    public Task<RestMessage> ModifyResponseAsync(Interaction interaction, EmbedProperties embed,
        IEnumerable<AttachmentProperties>? attachments = null)
        => ModifyResponseAsync(interaction, message =>
            {
                message.AddEmbeds(embed);
                if (attachments is not null) message.AddAttachments(attachments);
            });

    public Task<RestMessage> ModifyResponseAsync(Interaction interaction, string content,
        IEnumerable<AttachmentProperties>? attachments = null)
        => ModifyResponseAsync(interaction, message =>
            {
                message.WithContent(content);
                if (attachments is not null) message.AddAttachments(attachments);
            });
    
    public Task<RestMessage> ModifyResponseAsync(Interaction interaction, IEnumerable<AttachmentProperties> attachments) 
        => ModifyResponseAsync(interaction, message => message.AddAttachments(attachments));

    public Task<RestMessage> FollowupAsync(Interaction interaction, EmbedProperties embed, bool ephemeral = false,
        IEnumerable<AttachmentProperties>? attachments = null)
        => FollowupAsync(interaction, new InteractionMessageProperties()
            .AddEmbeds(embed)
            .WithFlags(ephemeral ? MessageFlags.Ephemeral : null)
            .AddAttachments(attachments ?? []));

    public Task<RestMessage> FollowupAsync(Interaction interaction, string content, bool ephemeral = false,
        IEnumerable<AttachmentProperties>? attachments = null)
        => FollowupAsync(interaction, new InteractionMessageProperties()
            .WithContent(content)
            .WithFlags(ephemeral ? MessageFlags.Ephemeral : null)
            .AddAttachments(attachments ?? []));

    public Task<RestMessage> FollowupAsync(Interaction interaction, IEnumerable<AttachmentProperties> attachments,
        bool ephemeral = false) 
        => FollowupAsync(interaction, new InteractionMessageProperties()
            .AddAttachments(attachments)
            .WithFlags(ephemeral ? MessageFlags.Ephemeral : null));
    
    public Task<RestMessage> GetFollowupAsync(Interaction interaction, ulong messageId)
        => rest.GetInteractionFollowupMessageAsync(interaction.ApplicationId, interaction.Token, messageId);
    
    public Task<RestMessage> ModifyFollowupAsync(Interaction interaction, ulong messageId, Action<MessageOptions> modify)
        => rest.ModifyInteractionFollowupMessageAsync(interaction.ApplicationId, interaction.Token, messageId, modify);
    
    public Task DeleteFollowupAsync(Interaction interaction, ulong messageId)
        => rest.DeleteInteractionFollowupMessageAsync(interaction.ApplicationId, interaction.Token, messageId);
    
    // Helpers
    private void CompleteDeferral(Interaction interaction)
    {
        var state = _tracker.GetState(interaction);
        if (state is InteractionResponseState.DeferredMessage or InteractionResponseState.DeferredUpdate)
            _tracker.SetState(interaction, InteractionResponseState.Acknowledged);
    }
}