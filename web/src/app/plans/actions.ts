"use server";

import { revalidatePath } from "next/cache";
import { apiSend } from "@/lib/api";
import type { ActionState } from "@/components/form";

export async function createPlan(
  _prev: ActionState,
  formData: FormData,
): Promise<ActionState> {
  const price = Number(
    String(formData.get("monthlyPrice") ?? "").replace(",", "."),
  );

  const result = await apiSend("/api/plans", "POST", {
    name: String(formData.get("name") ?? "").trim(),
    monthlyPrice: Number.isFinite(price) ? price : 0,
  });

  if (!result.ok) {
    return { errors: result.errors };
  }

  revalidatePath("/plans");
  return { ok: true };
}

export async function deactivatePlan(
  _prev: ActionState,
  formData: FormData,
): Promise<ActionState> {
  const id = String(formData.get("id") ?? "");
  const result = await apiSend(`/api/plans/${id}/deactivate`, "PATCH");

  if (!result.ok) {
    return { errors: result.errors };
  }

  revalidatePath("/plans");
  return { ok: true };
}
