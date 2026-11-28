using NetCord;
using NetCord.Rest;

namespace HollowCake.Bot.NetCord.Responses;

public sealed class ResponseService(RestClient rest)
{
    // Raw methods
    public async Task RespondAsync(Interaction interaction, InteractionMessageProperties message)
        => await interaction.SendResponseAsync(InteractionCallback.Message(message));

    public async Task DeferAsync(Interaction interaction, bool ephemeral = false)
        => await interaction.SendResponseAsync(
            InteractionCallback.DeferredMessage(ephemeral ? MessageFlags.Ephemeral : null));

    public async Task DeferUpdateAsync(ComponentInteraction interaction)
        => await interaction.SendResponseAsync(InteractionCallback.DeferredModifyMessage);
    
    public Task<RestMessage> ModifyResponseAsync(Interaction interaction, Action<MessageOptions> modify)
        => rest.ModifyInteractionResponseAsync(interaction.ApplicationId, interaction.Token, modify);
    
    public Task<RestMessage> FollowupAsync(Interaction interaction, InteractionMessageProperties message)
        => rest.SendInteractionFollowupMessageAsync(interaction.ApplicationId, interaction.Token, message);
    
    public Task<RestMessage> GetResponseAsync(Interaction interaction)
        => rest.GetInteractionResponseAsync(interaction.ApplicationId, interaction.Token);
    
    public Task DeleteResponseAsync(Interaction interaction)
        => rest.DeleteInteractionResponseAsync(interaction.ApplicationId, interaction.Token);

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
}