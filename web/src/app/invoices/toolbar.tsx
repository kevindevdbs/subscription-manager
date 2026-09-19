"use client";

import { useActionState } from "react";
import type { ActionState } from "@/components/form";
import { FormErrors, SubmitButton } from "@/components/form";
import { ActionButton } from "@/components/action-button";
import { generateInvoices, markOverdue } from "./actions";

export function InvoiceToolbar() {
  const [state, action] = useActionState<ActionState, FormData>(
    generateInvoices,
    null,
  );

  const thisMonth = new Date().toISOString().slice(0, 7);

  return (
    <div className="grid gap-4 sm:grid-cols-2">
      <form action={action} className="flex flex-wrap items-end gap-3">
        <label className="block">
          <span className="mb-1.5 block text-sm text-muted">Competência</span>
          <input
            name="referenceMonth"
            type="month"
            required
            defaultValue={thisMonth}
            className="rounded-md border border-border bg-surface-2 px-3 py-2 text-sm outline-none focus:border-accent"
          />
        </label>
        <SubmitButton>Gerar faturas</SubmitButton>
      </form>

      <div className="flex items-end">
        <ActionButton
          action={markOverdue}
          id=""
          label="Marcar vencidas agora"
          confirmMessage="Marcar como vencidas todas as faturas pendentes com vencimento passado?"
        />
      </div>

      <div className="sm:col-span-2">
        <FormErrors state={state} />
      </div>
    </div>
  );
}
