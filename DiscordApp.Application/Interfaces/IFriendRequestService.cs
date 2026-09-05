using DiscordApp.Application.DTOs.FriendRequestDTO;
using DiscordApp.Application.DTOs.FriendRequestDTOs;
using DiscordApp.Application.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordApp.Application.contracts;

public interface IFriendRequestService
{
    Task<Result<FriendRequestDto>> SendAsync(string senderId, CreateFriendRequestDto dto);
    Task<Result> AcceptAsync(int requestId, string userId);
    Task<Result> DeclineAsync(int requestId, string userId);
    Task<Result<List<FriendRequestDto>>> GetIncomingPendingRequestsAsync(string userId);
    Task<Result<List<FriendRequestDto>>> GetOutgoingPendingRequestsAsync(string userId);
}
