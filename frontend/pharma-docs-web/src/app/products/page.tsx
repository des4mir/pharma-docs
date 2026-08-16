// Server Component: runs entirely on the server, fetches data before the
// page is sent to the browser. No "use client" needed since there's no
// interactivity here yet (just a list + a conditional link).

import Link from "next/link";
import { getProducts } from "@/lib/products";
import { isRegAffairsOfficer } from "@/lib/session";

export default async function ProductsPage() {
  const [products, canEdit] = await Promise.all([
    getProducts(),
    isRegAffairsOfficer(),
  ]);

  return (
    <main className="mx-auto max-w-5xl p-8">
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Products</h1>
        {canEdit && (
          <Link
            href="/products/new"
            className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
          >
            New Product
          </Link>
        )}
      </div>

      {products.length === 0 ? (
        <p className="text-sm text-gray-600">No products yet.</p>
      ) : (
        <table className="w-full border-collapse text-sm">
          <thead>
            <tr className="border-b border-gray-300 text-left">
              <th className="py-2 pr-4">Name</th>
              <th className="py-2 pr-4">DIN</th>
              <th className="py-2 pr-4">Manufacturer</th>
              <th className="py-2 pr-4">Created By</th>
            </tr>
          </thead>
          <tbody>
            {products.map((product) => (
              <tr key={product.id} className="border-b border-gray-100">
                <td className="py-2 pr-4">
                  <Link
                    href={`/products/${product.id}`}
                    className="text-blue-600 hover:underline"
                  >
                    {product.name}
                  </Link>
                </td>
                <td className="py-2 pr-4">{product.din}</td>
                <td className="py-2 pr-4">{product.manufacturer}</td>
                <td className="py-2 pr-4">{product.createdByName}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </main>
  );
}