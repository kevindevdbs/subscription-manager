import Link from "next/link";
import type { ReactNode } from "react";
import type { ContractStatus, InvoiceStatus } from "@/lib/types";
import { contractStatusLabel, invoiceStatusLabel } from "@/lib/types";

export function PageHeader({
  title,
  description,
  action,
}: {
  title: string;
  description?: string;
  action?: ReactNode;
}) {
  return (
    <div className="mb-8 flex flex-wrap items-end justify-between gap-4">
      <div>
        <h1 className="text-2xl font-semibold">{title}</h1>
        {description && (
          <p className="mt-1 text-sm text-muted">{description}</p>
        )}
      </div>
      {action}
    </div>
  );
}

export function Card({
  children,
  className = "",
}: {
  children: ReactNode;
  className?: string;
}) {
  return (
    <div
      className={`rounded-lg border border-border bg-surface p-6 ${className}`}
    >
      {children}
    </div>
  );
}

export function EmptyState({ children }: { children: ReactNode }) {
  return (
    <div className="rounded-lg border border-dashed border-border bg-surface/50 p-10 text-center text-sm text-muted">
      {children}
    </div>
  );
}

const badgeStyles: Record<string, string> = {
  green: "bg-green-500/10 text-green-400 border-green-500/30",
  amber: "bg-amber-500/10 text-amber-400 border-amber-500/30",
  red: "bg-red-500/10 text-red-400 border-red-500/30",
  gray: "bg-white/5 text-muted border-border",
  blue: "bg-blue-500/10 text-blue-400 border-blue-500/30",
};

function Badge({ tone, children }: { tone: string; children: ReactNode }) {
  return (
    <span
      className={`inline-block rounded-full border px-2.5 py-0.5 text-xs font-medium ${badgeStyles[tone]}`}
    >
      {children}
    </span>
  );
}

const contractTone: Record<ContractStatus, string> = {
  Active: "green",
  Suspended: "amber",
  Cancelled: "gray",
};

export function ContractStatusBadge({ status }: { status: ContractStatus }) {
  return <Badge tone={contractTone[status]}>{contractStatusLabel[status]}</Badge>;
}

const invoiceTone: Record<InvoiceStatus, string> = {
  Pending: "amber",
  Paid: "green",
  Overdue: "red",
  Cancelled: "gray",
  Refunded: "blue",
};

export function InvoiceStatusBadge({ status }: { status: InvoiceStatus }) {
  return <Badge tone={invoiceTone[status]}>{invoiceStatusLabel[status]}</Badge>;
}

export function LinkButton({
  href,
  children,
}: {
  href: string;
  children: ReactNode;
}) {
  return (
    <Link
      href={href}
      className="rounded-md border border-accent bg-accent/10 px-4 py-2 text-sm font-medium text-accent transition-colors hover:bg-accent hover:text-background"
    >
      {children}
    </Link>
  );
}
