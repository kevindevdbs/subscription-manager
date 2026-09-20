"use client";

import { useFormStatus } from "react-dom";
import type { ReactNode } from "react";

export type ActionState = { errors: string[] } | { ok: true } | null;

export function Field({
  label,
  name,
  type = "text",
  required = true,
  defaultValue,
  placeholder,
  step,
}: {
  label: string;
  name: string;
  type?: string;
  required?: boolean;
  defaultValue?: string;
  placeholder?: string;
  step?: string;
}) {
  return (
    <label className="block">
      <span className="mb-1.5 block text-sm text-muted">{label}</span>
      <input
        name={name}
        type={type}
        required={required}
        defaultValue={defaultValue}
        placeholder={placeholder}
        step={step}
        className="w-full rounded-md border border-border bg-surface-2 px-3 py-2 text-sm outline-none focus:border-accent"
      />
    </label>
  );
}

export function SelectField({
  label,
  name,
  options,
  required = true,
}: {
  label: string;
  name: string;
  options: { value: string; label: string }[];
  required?: boolean;
}) {
  return (
    <label className="block">
      <span className="mb-1.5 block text-sm text-muted">{label}</span>
      <select
        name={name}
        required={required}
        defaultValue=""
        className="w-full rounded-md border border-border bg-surface-2 px-3 py-2 text-sm outline-none focus:border-accent"
      >
        <option value="" disabled>
          Selecione…
        </option>
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
    </label>
  );
}

export function SubmitButton({ children }: { children: ReactNode }) {
  const { pending } = useFormStatus();

  return (
    <button
      type="submit"
      disabled={pending}
      className="rounded-md border border-accent bg-accent px-4 py-2 text-sm font-semibold text-background transition-opacity hover:opacity-90 disabled:opacity-50"
    >
      {pending ? "Enviando…" : children}
    </button>
  );
}

export function FormErrors({ state }: { state: ActionState }) {
  if (!state || !("errors" in state) || state.errors.length === 0) {
    return null;
  }

  return (
    <ul className="rounded-md border border-red-500/30 bg-red-500/10 px-4 py-3 text-sm text-red-400">
      {state.errors.map((error) => (
        <li key={error}>{error}</li>
      ))}
    </ul>
  );
}
