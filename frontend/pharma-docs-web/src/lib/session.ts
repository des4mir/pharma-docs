// Reads the non-sensitive "session hint" cookie (fullName, email, role) that
// the login route sets alongside the httpOnly JWT cookie. This cookie is NOT
// httpOnly — it exists purely so Server Components can decide what UI to
// render (e.g. show/hide the "New Product" button). It carries no secret
// and grants no access by itself; the backend independently re-checks the
// real JWT's role claim on every request via [Authorize(Roles = "...")].

import { cookies } from "next/headers";
import type { SessionUser } from "./types";

export const SESSION_COOKIE_NAME = "pharmadocs_session";

export async function getSession(): Promise<SessionUser | null> {
  const cookieStore = await cookies();
  const raw = cookieStore.get(SESSION_COOKIE_NAME)?.value;
  if (!raw) return null;

  try {
    return JSON.parse(raw) as SessionUser;
  } catch {
    return null;
  }
}

export async function isRegAffairsOfficer(): Promise<boolean> {
  const session = await getSession();
  return session?.role === "RegAffairsOfficer";
}