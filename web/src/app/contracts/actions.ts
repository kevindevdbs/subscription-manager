"use server";

import { revalidatePath } from "next/cache";
import { apiSend } from "@/lib/api";
import type { ActionState } from "@/components/form";

export async function createContract(
  _prev: ActionState,
  formData: FormData,
): Promise<ActionState> {
  const result = await apiSend("/api/contracts", "POST", {
    customerId: String(formData.get("customerId") ?? ""),
    planId: String(formData.get("planId") ?? ""),
    startDate: String(formData.get("startDate") ?? ""),
  });

  if (!result.ok) {
    return { errors: result.errors };
  }

  revalidatePath("/contracts");
  return { ok: true };
}

async function transition(path: string): Promise<ActionState> {
  const result = await apiSend(path, "PATCH");
  if (!result.ok) {
    return { errors: result.errors };
  }
  revalidatePath("/contracts");
  return { ok: true };
}

export async function suspendContract(_prev: ActionState, formData: FormData) {
  return transition(`/api/contracts/${formData.get("id")}/suspend`);
}

export async function reactivateContract(_prev: ActionState, formData: FormData) {
  return transition(`/api/contracts/${formData.get("id")}/reactivate`);
}

export async function cancelContract(_prev: ActionState, formData: FormData) {
  return transition(`/api/contracts/${formData.get("id")}/cancel`);
}
