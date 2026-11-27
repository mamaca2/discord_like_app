using DiscordApp.Domain.Common;

namespace DiscordApp.Domain.Constants;

public static class DirectMessageErrors
{
    public static readonly Error EmptyMessage = new(
        "DirectMessage.EmptyMessage",
        "Message cannot be empty.");

    public static readonly Error SenderNotFound = new(
        "DirectMessage.SenderNotFound",
        "Sender not found.");

    public static readonly Error ReceiverNotFound = new(
        "DirectMessage.ReceiverNotFound",
        "Receiver not found.");
}
