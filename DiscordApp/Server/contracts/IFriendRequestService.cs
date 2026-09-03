using DiscordApp.DTOs.FriendRequestDTO;
using DiscordApp.DTOs.FriendRequestDTOs;
using DiscordApp.Server.Results;

namespace DiscordApp.contracts;

public interface IFriendRequestService
{
    Task<Result<FriendRequestDto>> SendAsync(string senderId, CreateFriendRequestDto dto);
    Task<Result> AcceptAsync(int requestId, string userId);
    Task<Result> DeclineAsync(int requestId, string userId);
    Task<Result<List<FriendRequestDto>>> GetIncomingPendingRequestsAsync(string userId);
    Task<Result<List<FriendRequestDto>>> GetOutgoingPendingRequestsAsync(string userId);
}
