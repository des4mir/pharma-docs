// Route Handler that ends the session. The JWT cookie is httpOnly, so
// client-side JavaScript cannot delete it directly — only server code
// (a Route Handler or Server Function) can clear it via a Set-Cookie header.

import { NextResponse } from "next/server";
import { cookies } from "next/headers";
import { AUTH_COOKIE_NAME } from "@/lib/api";

export async function POST() {
  const cookieStore = await cookies();
  cookieStore.delete(AUTH_COOKIE_NAME);

  return NextResponse.json({ success: true }, { status: 200 });
}
