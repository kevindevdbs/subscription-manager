import Link from "next/link";
import type { ReactNode } from "react";
import { cn } from "cn";
import { Card as ShadCard } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
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
        <h1 className="text-2xl font-semibold tracking-tight">{title}</h1>
        {description && (
          <p className="mt-1 text-sm text-muted-foreground">{description}</p>
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
  return <ShadCard className={cn("p-6", className)}>{children}</ShadCard>;
}

export function EmptyState({ children }: { children: ReactNode }) {
  return (
    <div className="rounded-xl border border-dashed border-border p-10 text-center text-sm text-muted-foreground">
      {children}
    </div>
  );
}

const contractVariant = {
  Active: "success",
  Suspended: "warning",
  Cancelled: "secondary",
} as const;

export function ContractStatusBadge({ status }: { status: ContractStatus }) {
  return (
    <Badge variant={contractVariant[status]}>{contractStatusLabel[status]}</Badge>
  );
}

const invoiceVariant = {
  Pending: "warning",
  Paid: "success",
  Overdue: "destructive",
  Cancelled: "secondary",
  Refunded: "info",
} as const;

export function InvoiceStatusBadge({ status }: { status: InvoiceStatus }) {
  return (
    <Badge variant={invoiceVariant[status]}>{invoiceStatusLabel[status]}</Badge>
  );
}

export function LinkButton({
  href,
  children,
}: {
  href: string;
  children: ReactNode;
}) {
  return (
    <Button nativeButton={false} render={<Link href={href} />}>
      {children}
    </Button>
  );
}
