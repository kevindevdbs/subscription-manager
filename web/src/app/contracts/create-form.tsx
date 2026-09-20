"use client";

import { useActionState, useEffect, useRef } from "react";
import type { ActionState } from "@/components/form";
import { Field, FormErrors, FormSelect, SubmitButton } from "@/components/form";
import type { Customer, Plan } from "@/lib/types";
import { formatMoney } from "@/lib/format";
import { createContract } from "./actions";

export function CreateContractForm({
  customers,
  plans,
}: {
  customers: Customer[];
  plans: Plan[];
}) {
  const [state, action] = useActionState<ActionState, FormData>(
    createContract,
    null,
  );
  const formRef = useRef<HTMLFormElement>(null);

  useEffect(() => {
    if (state && "ok" in state) {
      formRef.current?.reset();
    }
  }, [state]);

  const today = new Date().toISOString().slice(0, 10);

  return (
    <form ref={formRef} action={action} className="grid gap-4 sm:grid-cols-3">
      <FormSelect
        label="Cliente"
        name="customerId"
        options={customers.map((c) => ({ value: c.id, label: c.name }))}
      />
      <FormSelect
        label="Plano"
        name="planId"
        options={plans
          .filter((p) => p.isActive)
          .map((p) => ({
            value: p.id,
            label: `${p.name} — ${formatMoney(p.monthlyPrice)}`,
          }))}
      />
      <Field label="Início" name="startDate" type="date" defaultValue={today} />
      <div className="sm:col-span-3">
        <FormErrors state={state} />
      </div>
      <div className="sm:col-span-3">
        <SubmitButton>Criar contrato</SubmitButton>
      </div>
    </form>
  );
}
