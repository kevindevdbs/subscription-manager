"use server";

import { revalidatePath } from "next/cache";
import { apiSend } from "@/lib/api";
import type { ActionState } from "@/components/form";

export async function generateInvoices(
  _prev: ActionState,
  formData: FormData,
): Promise<ActionState> {
  const month = String(formData.get("referenceMonth") ?? "");
  if (!month) {
    return { errors: ["Informe a competência."] };
  }

  const result = await apiSend("/api/invoices/generate", "POST", {
    referenceMonth: `${month}-01`,
  });

  if (!result.ok) {
    return { errors: result.errors };
  }

  revalidatePath("/invoices");
  return { ok: true };
}

async function transition(path: string): Promise<ActionState> {
  const result = await apiSend(path, "PATCH");
  if (!result.ok) {
    return { errors: result.errors };
  }
  revalidatePath("/invoices");
  return { ok: true };
}

export async function markOverdue(): Promise<ActionState> {
  const result = await apiSend("/api/invoices/mark-overdue", "POST");
  if (!result.ok) {
    return { errors: result.errors };
  }
  revalidatePath("/invoices");
  return { ok: true };
}

export async function payInvoice(_prev: ActionState, formData: FormData) {
  return transition(`/api/invoices/${formData.get("id")}/pay`);
}

export async function refundInvoice(_prev: ActionState, formData: FormData) {
  return transition(`/api/invoices/${formData.get("id")}/refund`);
}

export async function cancelInvoice(_prev: ActionState, formData: FormData) {
  return transition(`/api/invoices/${formData.get("id")}/cancel`);
}
