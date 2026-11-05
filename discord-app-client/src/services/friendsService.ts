import { CONFIGS } from "../configs";

export type UserStatus = "Online" | "Offline";

export interface FriendDto {
  id: string;
  username: string;
  tag: string;
  image?: string;
  status: UserStatus;
}

export interface AddFriendDto {
  username: string;
  tag: string;
}

export async function getUserFriends(token: string): Promise<FriendDto[]> {
  const response = await fetch(`${CONFIGS.API_BASE_URL}${CONFIGS.ENDPOINTS.FRIENDS.BASE}`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
  });

  if (!response.ok) {
    const errorText = await response.text();
    console.error(`API Error (${response.status}):`, errorText);
    
    throw new Error(errorText || `Failed to load user friends (Status: ${response.status})`);
  }

  const data = await response.json();
  return data.value ?? data;
}

export async function respondToFriendRequest(
  token: string,
  requestId: string,
  accept: boolean
): Promise<void> {
  const response = await fetch(
    `${CONFIGS.API_BASE_URL}${CONFIGS.ENDPOINTS.FRIEND_REQUESTS}/${requestId}/${accept ? "accept" : "decline"}`,
    {
      method: "PUT",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    }
  );

  if (!response.ok) {
    const errorText = await response.text();
    console.error(`API Error (${response.status}):`, errorText);

    throw new Error(errorText || `Failed to update request (Status: ${response.status})`);
  }
}