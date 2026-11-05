import type { FriendRequestDto } from "../../services/friendRequestService";

interface FriendRequestsProps {
  incomingRequests: FriendRequestDto[];
  outgoingRequests: FriendRequestDto[];
}

export function FriendRequests({
  incomingRequests,
  outgoingRequests,
}: FriendRequestsProps) {
  return (
    <div>
      <h2>Incoming Requests</h2>

      {incomingRequests.map((request) => (
        <div key={request.id}>
          {request.senderUserName}
        </div>
      ))}

      <h2>Outgoing Requests</h2>

      {outgoingRequests.map((request) => (
        <div key={request.id}>
          {request.receiverUserName}
        </div>
      ))}
    </div>
  );
}