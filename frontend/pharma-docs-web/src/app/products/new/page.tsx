import { redirect } from "next/navigation";
import ProductForm from "@/components/ProductForm";
import { isRegAffairsOfficer } from "@/lib/session";
import { createProductAction } from "../actions";

export default async function NewProductPage() {
  // Render-time gating is not a security boundary on its own (the Server
  // Action independently re-checks the role, and so does the backend), but
  // it avoids showing a write form to a Viewer who navigates here directly.
  if (!(await isRegAffairsOfficer())) {
    redirect("/products");
  }

  return (
    <main className="mx-auto max-w-2xl p-8">
      <h1 className="mb-6 text-2xl font-semibold">New Product</h1>
      <ProductForm
        mode="create"
        action={createProductAction}
        submitLabel="Create Product"
        cancelHref="/products"
      />
    </main>
  );
}