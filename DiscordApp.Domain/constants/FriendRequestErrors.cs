using DiscordApp.Domain.Common;

namespace DiscordApp.Domain.Constants;

public static class FriendRequestErrors
{
    public static readonly Error RequestNotFound = new(
        "FriendRequest.NotFound",
        "Friend request not found.");

    public static readonly Error RequestNotFoundOrUnauthorized = new(
        "FriendRequest.NotFoundOrUnauthorized",
        "Friend request not found or you are not authorized to accept it.");

    public static readonly Error NotPending = new(
        "FriendRequest.NotPending",
        "Only pending friend requests can be accepted.");

    public static readonly Error NotAuthorizedToDecline = new(
        "FriendRequest.NotAuthorizedToDecline",
        "You are not authorized to decline this friend request.");

    public static readonly Error UserNotFound = new(
        "FriendRequest.UserNotFound",
        "User not found.");

    public static readonly Error CannotRequestYourself = new(
        "FriendRequest.CannotRequestYourself",
        "You cannot send a friend request to yourself.");

    public static readonly Error ReceiverNotFound = new(
        "FriendRequest.ReceiverNotFound",
        "Receiver not found.");

    public static readonly Error SenderNotFound = new(
        "FriendRequest.SenderNotFound",
        "Sender not found.");

    public static readonly Error AlreadyFriends = new(
        "FriendRequest.AlreadyFriends",
        "You are already friends with this user.");

    public static readonly Error AlreadySent = new(
        "FriendRequest.AlreadySent",
        "A pending friend request has already been sent to this user.");

    public static readonly Error IncomingRequestExists = new(
        "FriendRequest.IncomingRequestExists",
        "This user has already sent you a friend request. Accept their request instead.");
}
