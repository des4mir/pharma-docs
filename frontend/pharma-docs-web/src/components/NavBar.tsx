// Server Component. Reads the session cookie
// directly via getSession() since that function runs on the server.

import Link from "next/link";
import { getSession } from "@/lib/session";
import LogoutButton from "./LogoutButton";

export default async function NavBar() {
  const session = await getSession();

  return (
    <header className="border-b border-gray-200">
      <nav className="mx-auto flex max-w-5xl items-center justify-between px-8 py-4">
        <Link href="/" className="text-lg font-semibold">
          PharmaDocs
        </Link>

        {session ? (
          <div className="flex items-center gap-4 text-sm">
            <Link href="/products" className="text-gray-600 hover:text-gray-900">
              Products
            </Link>
            <span className="text-gray-400">|</span>
            <span className="text-gray-700">
              {session.fullName}{" "}
              <span className="text-gray-400">({session.role})</span>
            </span>
            <LogoutButton />
          </div>
        ) : (
          <Link
            href="/login"
            className="rounded bg-blue-600 px-4 py-1.5 text-sm font-medium text-white hover:bg-blue-700"
          >
            Sign in
          </Link>
        )}
      </nav>
    </header>
  );
}