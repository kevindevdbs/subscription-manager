"use client";

import { useActionState, useEffect, useRef } from "react";
import type { ActionState } from "@/components/form";
import { Field, FormErrors, SubmitButton } from "@/components/form";
import { createPlan } from "./actions";

export function CreatePlanForm() {
  const [state, action] = useActionState<ActionState, FormData>(createPlan, null);
  const formRef = useRef<HTMLFormElement>(null);

  useEffect(() => {
    if (state && "ok" in state) {
      formRef.current?.reset();
    }
  }, [state]);

  return (
    <form ref={formRef} action={action} className="grid gap-4 sm:grid-cols-3">
      <div className="sm:col-span-2">
        <Field label="Nome do plano" name="name" placeholder="Premium" />
      </div>
      <Field
        label="Mensalidade (R$)"
        name="monthlyPrice"
        type="number"
        step="0.01"
        placeholder="149.90"
      />
      <div className="sm:col-span-3">
        <FormErrors state={state} />
      </div>
      <div className="sm:col-span-3">
        <SubmitButton>Cadastrar plano</SubmitButton>
      </div>
    </form>
  );
}
