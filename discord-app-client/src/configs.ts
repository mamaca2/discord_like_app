export const CONFIGS = {
  API_BASE_URL: import.meta.env.VITE_API_BASE_URL || "/api",
  
  ENDPOINTS: {
    AUTH: {
      LOGIN: '/auth/login',
      START_REGISTRATION: '/auth/start-registration',
      VERIFY_CODE: '/auth/verify-code',
      COMPLETE_REGISTRATION: '/auth/complete-registration',
    },
    SERVERS: {
      USER_SERVERS: '/servers/user-servers',
      JOIN: (code: string) => `/servers/join/${code}`,
      CREATE: '/servers',
      CREATE_CHAT: (serverId: number) => `/servers/${serverId}/create-chat`,
      GET_CHATS: (serverId: number) => `/servers/${serverId}/get-chats`,
      GET_CHAT_MESSAGES: (serverId: number, chatId: number) =>
        `/servers/${serverId}/chats/${chatId}/messages`,
      INVITE: (serverId: number, friendId: string) =>
        `/servers/${serverId}/invite-friend/${encodeURIComponent(friendId)}`
    },
    FRIENDS: {
      BASE: '/friends'
    },
    FRIEND_REQUESTS: {
      SEND: '/friend-requests/send',
      INCOMING: '/friend-requests/incoming',
      OUTGOING: '/friend-requests/outgoing',
      ACCEPT: (requestId: number) =>
        `/friend-requests/${requestId}/accept`,
      DECLINE: (requestId: number) =>
        `/friend-requests/${requestId}/decline`,
    },
    DIRECT_MESSAGES: {
      GET_CONVERSATION: (friendId: string) =>
        `/direct-messages/get-conversation/${friendId}`,
    },

    SEARCH: '/users/search',
  },

  STORAGE_KEYS: {
    TOKEN: 'discord_app_token',
  },
} as const;