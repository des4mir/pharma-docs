// Typed API client for talking to the PharmaDocs ASP.NET backend.
// Server-only: reads the JWT from the httpOnly cookie and attaches it as a
// Bearer token. Client Components must never import this directly — call
// through a Next.js Route Handler instead.

import { cookies } from "next/headers";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL;

// Name of the httpOnly cookie that stores the backend JWT.
export const AUTH_COOKIE_NAME = "pharmadocs_token";

export class ApiError extends Error {
  status: number;

  constructor(status: number, message: string) {
    super(message);
    this.name = "ApiError";
    this.status = status;
  }
}

interface ApiFetchOptions extends RequestInit {
  // Explicit token override, used by the login Route Handler before any
  // cookie exists yet. Everywhere else the token is read from the cookie.
  token?: string;
}

/**
 * Calls the PharmaDocs backend API with the JWT attached from the httpOnly
 * session cookie (or an explicit override). Throws ApiError on non-2xx
 * responses.
 */
export async function apiFetch<T>(
  path: string,
  options: ApiFetchOptions = {}
): Promise<T> {
  if (!API_BASE_URL) {
    throw new Error("NEXT_PUBLIC_API_URL is not configured");
  }

  const { token: tokenOverride, headers, ...rest } = options;
  const token = tokenOverride ?? (await cookies()).get(AUTH_COOKIE_NAME)?.value;

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...rest,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...headers,
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new ApiError(response.status, await extractErrorMessage(response));
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

async function extractErrorMessage(response: Response): Promise<string> {
  try {
    const body = (await response.json()) as { message?: string };
    return body?.message ?? response.statusText;
  } catch {
    return response.statusText;
  }
}
