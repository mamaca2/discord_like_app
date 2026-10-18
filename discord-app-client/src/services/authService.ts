import { CONFIGS } from "../configs";

const { API_BASE_URL, ENDPOINTS } = CONFIGS;

async function request(path: string, body: unknown): Promise<string> {
  const res = await fetch(`${API_BASE_URL}${path}`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });

  const text = await res.text();

  if (!res.ok) {
    let message = text;
    try {
      const json = JSON.parse(text);
      message = json.message ?? json.title ?? json.detail ?? text;
    } catch {
      /* not JSON, keep raw text */
    }
    throw new Error(message || "Request failed");
  }

  return text;
}

export const startRegistration = async (email: string, password: string) => {
  await request(ENDPOINTS.AUTH.START_REGISTRATION, { email, password });
};

export const verifyCode = async (email: string, code: string) => {
  await request(ENDPOINTS.AUTH.VERIFY_CODE, { email, code });
};

export const completeRegistration = async (email: string, username: string) => {
  await request(ENDPOINTS.AUTH.COMPLETE_REGISTRATION, {
    email,
    username,
    role: "User",
  });
};

export const loginUser = async (creds: { email: string; password: string }): Promise<string> => {
  const text = await request(ENDPOINTS.AUTH.LOGIN, creds);
  try {
    const parsed = JSON.parse(text);
    return typeof parsed === "string" ? parsed : parsed.token ?? text;
  } catch {
    return text;
  }
};