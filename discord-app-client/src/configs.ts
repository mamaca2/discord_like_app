
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
      BASE: '/servers',
      JOIN: (code: string) => `/servers/join/${code}`,
    },
    FRIENDS: {
      BASE: '/friends',
      REQUESTS: '/friends/requests',
    },
  },

  STORAGE_KEYS: {
    TOKEN: 'discord_app_token',
  },
} as const;