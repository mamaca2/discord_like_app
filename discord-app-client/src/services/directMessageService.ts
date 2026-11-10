import { CONFIGS } from "../configs";

export interface DirectMessageResponseDto{
    id: number,
    senderId: string,
    receiverId: string,
    content: string,
    sentAt: Date
}

export async function getConversation(
    token: string,
    friendId: string
): Promise<DirectMessageResponseDto[]>{
  const response = await fetch(`${CONFIGS.API_BASE_URL}${CONFIGS.ENDPOINTS.DIRECT_MESSAGES.GET_CONVERSATION(friendId)}`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
  });

  if(!response.ok){
    const errorText = await response.text();
    throw new Error(errorText || "Failed to get conversation with friend");
  }

  return response.json();
}