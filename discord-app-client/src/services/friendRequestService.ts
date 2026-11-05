import { CONFIGS } from "../configs";

export interface FriendRequestDto {
  id: number;
  senderId: string;
  senderUserName: string;
  senderTag: string;
  senderImage?: string | null;

  receiverId: string;
  receiverUserName: string;
  receiverTag: string;
  receiverImage?: string | null;

  sentAt: string;
}

export async function sendFriendRequest(
  token: string,
  receiverId: string
): Promise<FriendRequestDto> {
  const response = await fetch(
    `${CONFIGS.API_BASE_URL}${CONFIGS.ENDPOINTS.FRIEND_REQUESTS.SEND}`,
    {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ receiverId }),
    }
  );

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Failed to send friend request");
  }

  return response.json();
}

export async function getIncomingFriendRequests(
  token: string
): Promise<FriendRequestDto[]> {
  const response = await fetch(
    `${CONFIGS.API_BASE_URL}${CONFIGS.ENDPOINTS.FRIEND_REQUESTS.INCOMING}`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }
  );

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Failed to get incoming friend requests");
  }

  return response.json();
}

export async function getOutgoingFriendRequests(
  token: string
): Promise<FriendRequestDto[]> {
  const response = await fetch(
    `${CONFIGS.API_BASE_URL}${CONFIGS.ENDPOINTS.FRIEND_REQUESTS.OUTGOING}`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }
  );

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Failed to get outgoing friend requests");
  }

  return response.json();
}