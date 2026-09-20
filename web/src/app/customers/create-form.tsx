"use client";

import { useActionState, useEffect, useRef } from "react";
import type { ActionState } from "@/components/form";
import { Field, FormErrors, SubmitButton } from "@/components/form";
import { createCustomer } from "./actions";

export function CreateCustomerForm() {
  const [state, action] = useActionState<ActionState, FormData>(
    createCustomer,
    null,
  );
  const formRef = useRef<HTMLFormElement>(null);

  useEffect(() => {
    if (state && "ok" in state) {
      formRef.current?.reset();
    }
  }, [state]);

  return (
    <form ref={formRef} action={action} className="grid gap-4 sm:grid-cols-3">
      <Field label="Nome" name="name" placeholder="Maria Silva" />
      <Field label="E-mail" name="email" type="email" placeholder="maria@exemplo.com" />
      <Field label="Documento (CPF/CNPJ)" name="document" placeholder="12345678901" />
      <div className="sm:col-span-3">
        <FormErrors state={state} />
      </div>
      <div className="sm:col-span-3">
        <SubmitButton>Cadastrar cliente</SubmitButton>
      </div>
    </form>
  );
}
