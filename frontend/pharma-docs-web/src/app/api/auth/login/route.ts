// Route Handler acting as the middleman between the browser and the ASP.NET
// backend for login. The browser never talks to /api/auth/login on the
// backend directly, and the JWT never reaches client-side JavaScript — it is
// stored in an httpOnly cookie set right here on the server.

import { NextResponse } from "next/server";
import { cookies } from "next/headers";
import { apiFetch, ApiError, AUTH_COOKIE_NAME } from "@/lib/api";
import type { AuthResponseDto, LoginDto, SessionUser } from "@/lib/types";

export async function POST(request: Request) {
  const body = (await request.json()) as Partial<LoginDto>;

  if (!body.email || !body.password) {
    return NextResponse.json(
      { message: "Email and password are required." },
      { status: 400 }
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
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      path: "/",
      expires: new Date(auth.expiresAt),
    });

    const sessionUser: SessionUser = {
      fullName: auth.fullName,
      email: auth.email,
      role: auth.role,
    };

    return NextResponse.json(sessionUser, { status: 200 });
  } catch (error) {
    if (error instanceof ApiError) {
      return NextResponse.json(
        { message: error.message || "Invalid email or password." },
        { status: error.status }
      );
    }

    return NextResponse.json(
      { message: "Unable to reach the authentication service." },
      { status: 502 }
    );
  }
}
