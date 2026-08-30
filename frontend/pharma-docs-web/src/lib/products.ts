// Server-only functions for the Products endpoints. Each one calls apiFetch,
// which attaches the JWT from the httpOnly cookie automatically.

import { apiFetch } from "./api";
import type { Product, CreateProductDto, UpdateProductDto } from "./types";

export function getProducts(): Promise<Product[]> {
  return apiFetch<Product[]>("/api/products");
}

export function getProductById(id: string): Promise<Product> {
  return apiFetch<Product>(`/api/products/${id}`);
}

export function createProduct(dto: CreateProductDto): Promise<Product> {
  return apiFetch<Product>("/api/products", {
    method: "POST",
    body: JSON.stringify(dto),
  });
}

export function updateProduct(
  id: string,
  dto: UpdateProductDto,
): Promise<Product> {
  return apiFetch<Product>(`/api/products/${id}`, {
    method: "PUT",
    body: JSON.stringify(dto),
  });
}

export function archiveProduct(id: string): Promise<void> {
  return apiFetch<void>(`/api/products/${id}/archive`, { method: "PATCH" });
}
