// Optimistic auth guard. Next.js 16 renamed Middleware to Proxy — this file
// runs before every matched request and redirects to /login when the
// httpOnly session cookie is missing. It only checks for cookie presence
// (optimistic check); the backend is the source of truth for actual
// authorization on every request.

import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";
import { AUTH_COOKIE_NAME } from "@/lib/api";

const PUBLIC_ROUTES = ["/login"];

export default function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl;
  const isPublicRoute = PUBLIC_ROUTES.includes(pathname);
  const hasSession = request.cookies.has(AUTH_COOKIE_NAME);

  if (!isPublicRoute && !hasSession) {
    const loginUrl = new URL("/login", request.url);
    loginUrl.searchParams.set("from", pathname);
    return NextResponse.redirect(loginUrl);
  }

  if (isPublicRoute && hasSession) {
    return NextResponse.redirect(new URL("/", request.url));
  }

  return NextResponse.next();
}

export const config = {
  // Run on every route except the Next.js internals, static assets, and the
  // auth API routes (the login Route Handler must be reachable while logged out).
  matcher: ["/((?!api|_next/static|_next/image|favicon.ico).*)"],
};
