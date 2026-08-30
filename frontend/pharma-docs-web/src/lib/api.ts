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
  errors: string[];

  constructor(status: number, message: string, errors: string[] = []) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.errors = errors;
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
  options: ApiFetchOptions = {},
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
    const { message, errors } = await parseErrorResponse(response);
    throw new ApiError(response.status, message, errors);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

// The backend returns two different error body shapes depending on failure
// type: `{ errors: string[] }` for FluentValidation failures (e.g. the
// DIN/NPN exclusivity rule), and `{ message: string }` for single business
// rule violations (e.g. duplicate DIN/NPN). Both must be captured so callers
// (e.g. Server Actions) can surface the exact validation messages to the user
// instead of a generic HTTP status text.
async function parseErrorResponse(
  response: Response,
): Promise<{ message: string; errors: string[] }> {
  try {
    const body = (await response.json()) as {
      message?: string;
      errors?: string[];
    };
    const errors = Array.isArray(body?.errors) ? body.errors : [];
    const message = body?.message ?? errors[0] ?? response.statusText;
    return { message, errors };
  } catch {
    return { message: response.statusText, errors: [] };
  }
}
