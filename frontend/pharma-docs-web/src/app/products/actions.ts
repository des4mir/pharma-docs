"use server";

import { redirect } from "next/navigation";
import { createProduct, updateProduct } from "@/lib/products";
import { ApiError } from "@/lib/api";
import { getSession } from "@/lib/session";
import type { ProductFormState } from "@/lib/product-form-state";
import type { CreateProductDto, Product, UpdateProductDto } from "@/lib/types";

function textField(formData: FormData, key: string): string {
  return String(formData.get(key) ?? "");
}

function formValues(formData: FormData): Record<string, string> {
  return Object.fromEntries(formData.entries()) as Record<string, string>;
}

const FORBIDDEN_STATE_MESSAGE =
  "You do not have permission to perform this action.";

export async function createProductAction(
  _prevState: ProductFormState,
  formData: FormData,
): Promise<ProductFormState> {
  // Defense in depth: the "New Product" link is hidden from Viewers in the
  // UI, but a Server Action is a POST endpoint reachable by anyone who can
  // send the request, regardless of what rendered. The backend independently
  // re-checks the role on every request; this check only fails fast and
  // avoids a confusing round trip for a request we already know will be
  // rejected.
  const session = await getSession();
  if (session?.role !== "RegAffairsOfficer") {
    return { errors: [FORBIDDEN_STATE_MESSAGE], values: formValues(formData) };
  }

  const dto: CreateProductDto = {
    name: textField(formData, "name"),
    din: textField(formData, "din") || null,
    npn: textField(formData, "npn") || null,
    medicinalIngredient: textField(formData, "medicinalIngredient"),
    manufacturer: textField(formData, "manufacturer"),
    dosageForm: textField(formData, "dosageForm"),
    routeOfAdministration: textField(formData, "routeOfAdministration"),
    therapeuticCategory: textField(formData, "therapeuticCategory"),
  };

  let product: Product;
  try {
    product = await createProduct(dto);
  } catch (error) {
    return toFormState(error, formData);
  }

  // redirect() throws internally and must be called outside try/catch —
  // otherwise the catch block above would swallow it and the redirect
  // would silently never happen.
  redirect(`/products/${product.id}`);
}

export async function updateProductAction(
  productId: string,
  _prevState: ProductFormState,
  formData: FormData,
): Promise<ProductFormState> {
  const session = await getSession();
  if (session?.role !== "RegAffairsOfficer") {
    return { errors: [FORBIDDEN_STATE_MESSAGE], values: formValues(formData) };
  }

  const dto: UpdateProductDto = {
    name: textField(formData, "name"),
    medicinalIngredient: textField(formData, "medicinalIngredient"),
    manufacturer: textField(formData, "manufacturer"),
    dosageForm: textField(formData, "dosageForm"),
    routeOfAdministration: textField(formData, "routeOfAdministration"),
    therapeuticCategory: textField(formData, "therapeuticCategory"),
  };

  try {
    await updateProduct(productId, dto);
  } catch (error) {
    return toFormState(error, formData);
  }

  redirect(`/products/${productId}`);
}

function toFormState(error: unknown, formData: FormData): ProductFormState {
  const values = formValues(formData);
  if (error instanceof ApiError) {
    return {
      errors: error.errors.length > 0 ? error.errors : [error.message],
      values,
    };
  }
  return {
    errors: ["Unable to reach the server. Please try again."],
    values,
  };
}
