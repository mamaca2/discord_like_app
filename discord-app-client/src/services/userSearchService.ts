import { CONFIGS } from "../configs";

export interface UserSearchDto {
  id: string;
  userName: string;
  tag: string;
  image?: string;
}

export interface PagedList<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export async function searchUsers(
  token: string,
  query: string,
  pageNumber: number = 1,
  pageSize: number = 10
): Promise<PagedList<UserSearchDto>> {
  const params = new URLSearchParams({
    query,
    pageNumber: pageNumber.toString(),
    pageSize: pageSize.toString(),
  });

  const response = await fetch(
    `${CONFIGS.API_BASE_URL}${CONFIGS.ENDPOINTS.SEARCH}?${params}`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }
  );

  if (!response.ok) {
    throw new Error("Failed to search users");
  }

  return response.json();
}