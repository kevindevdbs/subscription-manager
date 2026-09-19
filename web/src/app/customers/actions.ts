"use server";

import { revalidatePath } from "next/cache";
import { apiSend } from "@/lib/api";
import type { ActionState } from "@/components/form";

export async function createCustomer(
  _prev: ActionState,
  formData: FormData,
): Promise<ActionState> {
  const result = await apiSend("/api/customers", "POST", {
    name: String(formData.get("name") ?? "").trim(),
    email: String(formData.get("email") ?? "").trim(),
    document: String(formData.get("document") ?? "").trim(),
  });

  if (!result.ok) {
    return { errors: result.errors };
  }

  revalidatePath("/customers");
  return { ok: true };
}
