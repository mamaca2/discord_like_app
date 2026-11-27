import { CONFIGS } from "../configs";

export interface ServerDto {
  id: number;
  name: string;
  image?: string;
  ownerId: string;
  memberCount: number;
}

export interface ServerChatDto{
  id: number,
  name: string,
  serverId: number
}

export interface ServerChatMessageDto {
  id: number;
  chatId: number;
  senderId: string;
  content: string;
  isMine: boolean;
}

export async function getUserServers(token: string): Promise<ServerDto[]> {
  const response = await fetch(`${CONFIGS.API_BASE_URL}${CONFIGS.ENDPOINTS.SERVERS.USER_SERVERS}`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
  });

  if (!response.ok) {
    const errorText = await response.text();
    console.error(`API Error (${response.status}):`, errorText);
    
    throw new Error(errorText || `Failed to load user servers (Status: ${response.status})`);
  }

  const data = await response.json();
  return data.value ?? data;
}

export async function createServer(
  token: string,
  name: string,
  image: File | null
): Promise<ServerDto> {
  const formData = new FormData();

  formData.append("Name", name);

  if (image) {
    formData.append("Image", image);
  }

  const response = await fetch(
    `${CONFIGS.API_BASE_URL}${CONFIGS.ENDPOINTS.SERVERS.CREATE}`,
    {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`
      },
      body: formData
    }
  );

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Failed to create server");
  }

  const data = await response.json();
  return data.value ?? data;
}

export async function createServerChat(
  token: string,
  serverId: number,
  name: string
): Promise<ServerChatDto> {
  const response = await fetch(
    `${CONFIGS.API_BASE_URL}${CONFIGS.ENDPOINTS.SERVERS.CREATE_CHAT(serverId)}`,
    {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json"
      },
      body: JSON.stringify({ name })
    }
  );

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Failed to create chat");
  }

  const data = await response.json();
  return data.value ?? data;
}

export async function getServerChats(
  token: string,
  serverId: number
): Promise<ServerChatDto[]> {
  const response = await fetch(
    `${CONFIGS.API_BASE_URL}${CONFIGS.ENDPOINTS.SERVERS.GET_CHATS(serverId)}`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`
      }
    }
  );

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Failed to get chats");
  }

  const data = await response.json();
  return data.value ?? data;
}

export async function getServerChatMessages(
  token: string,
  serverId: number,
  chatId: number,
  page: number,
  pageSize: number = 20
): Promise<ServerChatMessageDto[]> {
  const response = await fetch(
    `${CONFIGS.API_BASE_URL}${CONFIGS.ENDPOINTS.SERVERS.GET_CHAT_MESSAGES(serverId, chatId)}?page=${page}&pageSize=${pageSize}`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`
      }
    }
  );

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Failed to load messages");
  }

  const data = await response.json();
  return data.value ?? data;
}

export async function inviteFriendToServer(
  token: string,
  serverId: number,
  friendId: string
): Promise<void> {
  const response = await fetch(
    `${CONFIGS.API_BASE_URL}${CONFIGS.ENDPOINTS.SERVERS.INVITE(serverId, friendId)}`,
    {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        durationHours: 168,
        maxUses: 0,
      }),
    }
  );

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || `Invite failed (${response.status})`);
  }
}

export async function joinServer(
  token: string,
  code: string
): Promise<void> {
  const response = await fetch(
    `${CONFIGS.API_BASE_URL}${CONFIGS.ENDPOINTS.SERVERS.JOIN(code)}`,
    {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`
      }
    }
  );

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || `Failed to join server (${response.status})`);
  }
}
