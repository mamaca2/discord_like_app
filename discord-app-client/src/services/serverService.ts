import { CONFIGS } from "../configs";

export interface ServerDto {
  id: number;
  name: string;
  image?: string;
  ownerId: string;
  memberCount: number;
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