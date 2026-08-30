// The [id] folder name is a dynamic route segment

import { notFound } from "next/navigation";
import Link from "next/link";
import { getProductById } from "@/lib/products";
import { ApiError } from "@/lib/api";
import { isRegAffairsOfficer } from "@/lib/session";

interface ProductDetailPageProps {
  params: Promise<{ id: string }>;
}

export default async function ProductDetailPage({
  params,
}: ProductDetailPageProps) {
  const { id } = await params;

  let product;
  try {
    product = await getProductById(id);
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) {
      notFound(); // renders Next.js's built-in 404 page
    }
    throw error;
  }

  const canEdit = await isRegAffairsOfficer();

  return (
    <main className="mx-auto max-w-2xl p-8">
      <div className="mb-2 flex items-center justify-between">
        <h1 className="text-2xl font-semibold">{product.name}</h1>
        {canEdit && (
          <Link
            href={`/products/${product.id}/edit`}
            className="rounded border border-gray-300 px-3 py-1.5 text-sm font-medium text-gray-700 hover:bg-gray-50"
          >
            Edit
          </Link>
        )}
      </div>
      <dl className="mt-6 grid grid-cols-[160px_1fr] gap-y-3 text-sm">
        <dt className="text-gray-500">DIN</dt>
        <dd>{product.din}</dd>

        <dt className="text-gray-500">NPN</dt>
        <dd>{product.npn}</dd>

        <dt className="text-gray-500">Medicinal Ingredient</dt>
        <dd>{product.medicinalIngredient}</dd>

        <dt className="text-gray-500">Manufacturer</dt>
        <dd>{product.manufacturer}</dd>

        <dt className="text-gray-500">Dosage Form</dt>
        <dd>{product.dosageForm}</dd>

        <dt className="text-gray-500">Route of Administration</dt>
        <dd>{product.routeOfAdministration}</dd>

        <dt className="text-gray-500">Therapeutic Category</dt>
        <dd>{product.therapeuticCategory}</dd>

        <dt className="text-gray-500">Created By</dt>
        <dd>{product.createdByName}</dd>
      </dl>
    </main>
  );
}