"use client";

import { useState, type ReactNode } from "react";
import { useFormStatus } from "react-dom";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

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
    <div className="grid gap-1.5">
      <Label htmlFor={name}>{label}</Label>
      <Input
        id={name}
        name={name}
        type={type}
        required={required}
        defaultValue={defaultValue}
        placeholder={placeholder}
        step={step}
      />
    </div>
  );
}

// Sentinela para a opção "todos": o Base UI, como o Radix, não aceita item com
// value vazio, mas o filtro precisa mandar "" na query para dizer "sem filtro".
const ALL = "__all__";

// Espelha o valor do Select do shadcn num input hidden, para o FormData (Server
// Action) e o form GET nativo lerem o valor sem depender do Base UI.
export function FormSelect({
  label,
  name,
  options,
  placeholder = "Selecione…",
  defaultValue = "",
  includeAll,
}: {
  label: string;
  name: string;
  options: { value: string; label: string }[];
  placeholder?: string;
  defaultValue?: string;
  includeAll?: string;
}) {
  const initial =
    defaultValue === "" && includeAll !== undefined ? ALL : defaultValue;
  const [value, setValue] = useState(initial);
  const submitted = value === ALL ? "" : value;

  // O Base UI, ao contrário do Radix, usa este mapa para o gatilho exibir o
  // rótulo do item selecionado em vez do valor cru.
  const items: Record<string, string> = {};
  if (includeAll !== undefined) items[ALL] = includeAll;
  for (const option of options) items[option.value] = option.label;

  return (
    <div className="grid gap-1.5">
      <Label>{label}</Label>
      <input type="hidden" name={name} value={submitted} />
      <Select items={items} value={value} onValueChange={(v) => setValue(v ?? "")}>
        <SelectTrigger className="w-full min-w-40">
          <SelectValue placeholder={placeholder} />
        </SelectTrigger>
        <SelectContent>
          {includeAll !== undefined && (
            <SelectItem value={ALL}>{includeAll}</SelectItem>
          )}
          {options.map((option) => (
            <SelectItem key={option.value} value={option.value}>
              {option.label}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );
}

export function SubmitButton({ children }: { children: ReactNode }) {
  const { pending } = useFormStatus();

  return (
    <Button type="submit" disabled={pending}>
      {pending ? "Enviando…" : children}
    </Button>
  );
}

export function FormErrors({ state }: { state: ActionState }) {
  if (!state || !("errors" in state) || state.errors.length === 0) {
    return null;
  }

  return (
    <ul className="rounded-lg border border-destructive/30 bg-destructive/10 px-4 py-3 text-sm text-destructive">
      {state.errors.map((error) => (
        <li key={error}>{error}</li>
      ))}
    </ul>
  );
}
