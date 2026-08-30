"use client";

import { useActionState } from "react";
import type { InputHTMLAttributes } from "react";
import Link from "next/link";
import type { ProductFormState } from "@/lib/product-form-state";
import { initialProductFormState } from "@/lib/product-form-state";

interface ProductFormProps {
  mode: "create" | "edit";
  action: (
    state: ProductFormState,
    formData: FormData
  ) => Promise<ProductFormState>;
  initialValues?: Record<string, string>;
  submitLabel: string;
  cancelHref: string;
}

export default function ProductForm({
  mode,
  action,
  initialValues = {},
  submitLabel,
  cancelHref,
}: ProductFormProps) {
  const [state, formAction] = useActionState(action, initialProductFormState);
  const values = { ...initialValues, ...state.values };

  return (
    <form action={formAction} className="space-y-4">
      {state.errors.length > 0 && (
        <ul className="rounded bg-red-50 p-3 text-sm text-red-700">
          {state.errors.map((error, i) => (
            <li key={i}>{error}</li>
          ))}
        </ul>
      )}

      <Field name="name" label="Name" defaultValue={values.name} required />

      {mode === "create" ? (
        <>
          <Field
            name="din"
            label="DIN"
            defaultValue={values.din}
            pattern="\d{8}"
            inputMode="numeric"
            maxLength={8}
            title="8-digit Drug Identification Number"
          />
          <Field
            name="npn"
            label="NPN"
            defaultValue={values.npn}
            pattern="\d{8}"
            inputMode="numeric"
            maxLength={8}
            title="8-digit Natural Product Number"
          />
          <p className="text-xs text-gray-500">
            Provide exactly one identifier: an 8-digit DIN for a regulated
            drug, or an 8-digit NPN for a natural health product.
          </p>
        </>
      ) : (
        <div className="rounded border border-gray-200 bg-gray-50 p-3">
          <p className="text-sm text-gray-700">
            <span className="font-medium">Identifier:</span>{" "}
            {values.din
              ? `DIN ${values.din}`
              : values.npn
                ? `NPN ${values.npn}`
                : "Not set"}
          </p>
          <p className="mt-1 text-xs text-gray-500">
            A product&apos;s DIN/NPN cannot be changed after creation. To
            correct an identifier, archive this product and create a new one.
          </p>
        </div>
      )}

      <Field
        name="medicinalIngredient"
        label="Medicinal Ingredient"
        defaultValue={values.medicinalIngredient}
        required
      />
      <Field
        name="manufacturer"
        label="Manufacturer"
        defaultValue={values.manufacturer}
        required
      />
      <Field
        name="dosageForm"
        label="Dosage Form"
        defaultValue={values.dosageForm}
        required
      />
      <Field
        name="routeOfAdministration"
        label="Route of Administration"
        defaultValue={values.routeOfAdministration}
        required
      />
      <Field
        name="therapeuticCategory"
        label="Therapeutic Category"
        defaultValue={values.therapeuticCategory}
        required
      />

      <div className="flex items-center gap-3">
        <button
          type="submit"
          className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
        >
          {submitLabel}
        </button>
        <Link
          href={cancelHref}
          className="text-sm font-medium text-gray-600 hover:underline"
        >
          Cancel
        </Link>
      </div>
    </form>
  );
}

function Field({
  name,
  label,
  defaultValue,
  required = false,
  pattern,
  inputMode,
  maxLength,
  title,
}: {
  name: string;
  label: string;
  defaultValue?: string;
  required?: boolean;
  pattern?: string;
  inputMode?: InputHTMLAttributes<HTMLInputElement>["inputMode"];
  maxLength?: number;
  title?: string;
}) {
  return (
    <div>
      <label htmlFor={name} className="block text-sm font-medium text-gray-700">
        {label}
      </label>
      <input
        id={name}
        name={name}
        defaultValue={defaultValue}
        required={required}
        pattern={pattern}
        inputMode={inputMode}
        maxLength={maxLength}
        title={title}
        className="mt-1 w-full rounded border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none"
      />
    </div>
  );
}