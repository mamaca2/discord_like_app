export const CONFIGS = {
  API_BASE_URL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5187/api',
  
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
    },
    FRIENDS: {
      BASE: '/friends'
    },
    FRIEND_REQUESTS: {
      SEND: '/friend-requests/send',
      INCOMING: '/friend-requests/incoming',
      OUTGOING: '/friend-requests/outgoing',
    },

    SEARCH: '/users/search',
  },

  STORAGE_KEYS: {
    TOKEN: 'discord_app_token',
  },
} as const;