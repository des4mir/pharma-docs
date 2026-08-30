import { redirect } from "next/navigation";
import ProductForm from "@/components/ProductForm";
import { getProductById } from "@/lib/products";
import { isRegAffairsOfficer } from "@/lib/session";
import { updateProductAction } from "../../actions";

interface EditProductPageProps {
  params: Promise<{ id: string }>;
}

export default async function EditProductPage({
  params,
}: EditProductPageProps) {
  const { id } = await params;

  if (!(await isRegAffairsOfficer())) {
    redirect(`/products/${id}`);
  }

  const product = await getProductById(id);
  const boundAction = updateProductAction.bind(null, id);

  return (
    <main className="mx-auto max-w-2xl p-8">
      <h1 className="mb-6 text-2xl font-semibold">Edit Product</h1>
      <ProductForm
        mode="edit"
        action={boundAction}
        initialValues={{
          name: product.name,
          din: product.din ?? "",
          npn: product.npn ?? "",
          medicinalIngredient: product.medicinalIngredient,
          manufacturer: product.manufacturer,
          dosageForm: product.dosageForm,
          routeOfAdministration: product.routeOfAdministration,
          therapeuticCategory: product.therapeuticCategory,
        }}
        submitLabel="Save Changes"
        cancelHref={`/products/${id}`}
      />
    </main>
  );
}