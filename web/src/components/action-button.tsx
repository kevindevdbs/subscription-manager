"use client";

import { useActionState } from "react";
import { useFormStatus } from "react-dom";
import { Button } from "@/components/ui/button";
import type { ActionState } from "./form";

type ServerAction = (
  prev: ActionState,
  formData: FormData,
) => Promise<ActionState>;

function Inner({
  label,
  tone,
  confirmMessage,
}: {
  label: string;
  tone: "default" | "danger";
  confirmMessage?: string;
}) {
  const { pending } = useFormStatus();

  return (
    <Button
      type="submit"
      size="sm"
      variant={tone === "danger" ? "destructive" : "outline"}
      disabled={pending}
      onClick={(event) => {
        if (confirmMessage && !window.confirm(confirmMessage)) {
          event.preventDefault();
        }
      }}
    >
      {pending ? "…" : label}
    </Button>
  );
}

export function ActionButton({
  action,
  id,
  label,
  tone = "default",
  confirmMessage,
}: {
  action: ServerAction;
  id: string;
  label: string;
  tone?: "default" | "danger";
  confirmMessage?: string;
}) {
  const [state, formAction] = useActionState<ActionState, FormData>(
    action,
    null,
  );

  return (
    <form action={formAction} className="inline-block">
      <input type="hidden" name="id" value={id} />
      <Inner label={label} tone={tone} confirmMessage={confirmMessage} />
      {state && "errors" in state && state.errors.length > 0 && (
        <p className="mt-1 text-xs text-destructive">{state.errors.join(" ")}</p>
      )}
    </form>
  );
}
