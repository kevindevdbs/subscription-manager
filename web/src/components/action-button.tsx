"use client";

import { useActionState } from "react";
import { useFormStatus } from "react-dom";
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

  const styles =
    tone === "danger"
      ? "border-red-500/40 text-red-400 hover:bg-red-500/10"
      : "border-border text-foreground hover:border-accent hover:text-accent";

  return (
    <button
      type="submit"
      disabled={pending}
      onClick={(event) => {
        if (confirmMessage && !window.confirm(confirmMessage)) {
          event.preventDefault();
        }
      }}
      className={`rounded-md border px-3 py-1.5 text-xs font-medium transition-colors disabled:opacity-50 ${styles}`}
    >
      {pending ? "…" : label}
    </button>
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
        <p className="mt-1 text-xs text-red-400">{state.errors.join(" ")}</p>
      )}
    </form>
  );
}
