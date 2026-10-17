// Route Handler acting as the middleman between the browser and the ASP.NET
// backend for login. The browser never talks to /api/auth/login on the
// backend directly, and the JWT never reaches client-side JavaScript — it is
// stored in an httpOnly cookie set right here on the server.

import { NextResponse } from "next/server";
import { cookies } from "next/headers";
import { apiFetch, ApiError, AUTH_COOKIE_NAME } from "@/lib/api";
import type { AuthResponseDto, LoginDto, SessionUser } from "@/lib/types";
import { SESSION_COOKIE_NAME } from "@/lib/session";

// COOKIE_SECURE lets local Docker runs over plain HTTP opt out of the Secure
// flag; unset, it falls back to NODE_ENV so production still defaults to on.
const isSecureCookie = process.env.COOKIE_SECURE
  ? process.env.COOKIE_SECURE === "true"
  : process.env.NODE_ENV === "production";

export async function POST(request: Request) {
  const body = (await request.json()) as Partial<LoginDto>;

  if (!body.email || !body.password) {
    return NextResponse.json(
      { message: "Email and password are required." },
      { status: 400 },
    );
  }

  try {
    const auth = await apiFetch<AuthResponseDto>("/api/auth/login", {
      method: "POST",
      body: JSON.stringify({
        email: body.email,
        password: body.password,
      } satisfies LoginDto),
    });

    const cookieStore = await cookies();
    cookieStore.set(AUTH_COOKIE_NAME, auth.token, {
      httpOnly: true,
      secure: isSecureCookie,
      sameSite: "lax",
      path: "/",
      expires: new Date(auth.expiresAt),
    });

    const sessionUser: SessionUser = {
      fullName: auth.fullName,
      email: auth.email,
      role: auth.role,
    };

    cookieStore.set(SESSION_COOKIE_NAME, JSON.stringify(sessionUser), {
      httpOnly: false, // readable by Server Components via next/headers, and could be read client-side too — contains no secret
      secure: isSecureCookie,
      sameSite: "lax",
      path: "/",
      expires: new Date(auth.expiresAt),
    });

    return NextResponse.json(sessionUser, { status: 200 });
  } catch (error) {
    if (error instanceof ApiError) {
      return NextResponse.json(
        { message: error.message || "Invalid email or password." },
        { status: error.status },
      );
    }

    return NextResponse.json(
      { message: "Unable to reach the authentication service." },
      { status: 502 },
    );
  }
}
