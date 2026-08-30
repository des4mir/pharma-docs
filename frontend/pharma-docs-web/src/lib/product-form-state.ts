// Shared client/server state shape for the Product create/edit form.
// This lives outside any "use server" file because Server Action modules may
// only export async functions — a plain object export like
// initialProductFormState would fail the build (see
// https://nextjs.org/docs/messages/invalid-use-server-value).

export interface ProductFormState {
  errors: string[];
  values: Record<string, string>;
}

export const initialProductFormState: ProductFormState = {
  errors: [],
  values: {},
};
