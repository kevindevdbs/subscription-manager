import Link from "next/link";
import { apiGet } from "@/lib/api";
import type { Contract, Customer, Invoice, Plan } from "@/lib/types";
import { formatMoney } from "@/lib/format";
import { Card, PageHeader, InvoiceStatusBadge } from "@/components/ui";
import { ApiDown } from "@/components/api-down";

export const dynamic = "force-dynamic";

async function loadOverview() {
  const [customers, plans, contracts, invoices] = await Promise.all([
    apiGet<Customer[]>("/api/customers"),
    apiGet<Plan[]>("/api/plans"),
    apiGet<Contract[]>("/api/contracts"),
    apiGet<Invoice[]>("/api/invoices"),
  ]);
  return { customers, plans, contracts, invoices };
}

function Stat({ label, value, href }: { label: string; value: number; href: string }) {
  return (
    <Link
      href={href}
      className="rounded-lg border border-border bg-surface p-6 transition-colors hover:border-accent/60"
    >
      <div className="font-mono text-3xl font-bold text-accent">{value}</div>
      <div className="mt-1 text-sm text-muted">{label}</div>
    </Link>
  );
}

export default async function DashboardPage() {
  let data;
  try {
    data = await loadOverview();
  } catch {
    return (
      <>
        <PageHeader title="Visão geral" />
        <ApiDown />
      </>
    );
  }

  const { customers, plans, contracts, invoices } = data;
  const activeContracts = contracts.filter((c) => c.status === "Active").length;
  const pending = invoices.filter((i) => i.status === "Pending");
  const overdue = invoices.filter((i) => i.status === "Overdue");
  const overdueTotal = overdue.reduce((sum, i) => sum + i.amount, 0);

  return (
    <>
      <PageHeader
        title="Visão geral"
        description="Situação atual das assinaturas."
      />

      <div className="grid grid-cols-2 gap-4 sm:grid-cols-4">
        <Stat label="Clientes" value={customers.length} href="/customers" />
        <Stat label="Planos" value={plans.length} href="/plans" />
        <Stat label="Contratos ativos" value={activeContracts} href="/contracts" />
        <Stat label="Faturas" value={invoices.length} href="/invoices" />
      </div>

      <div className="mt-4 grid gap-4 sm:grid-cols-2">
        <Card>
          <div className="flex items-center justify-between">
            <span className="text-sm text-muted">A receber (pendentes)</span>
            <InvoiceStatusBadge status="Pending" />
          </div>
          <div className="mt-2 font-mono text-2xl font-bold">
            {formatMoney(pending.reduce((sum, i) => sum + i.amount, 0))}
          </div>
          <div className="mt-1 text-xs text-muted">{pending.length} fatura(s)</div>
        </Card>
        <Card>
          <div className="flex items-center justify-between">
            <span className="text-sm text-muted">Em atraso</span>
            <InvoiceStatusBadge status="Overdue" />
          </div>
          <div className="mt-2 font-mono text-2xl font-bold text-red-400">
            {formatMoney(overdueTotal)}
          </div>
          <div className="mt-1 text-xs text-muted">{overdue.length} fatura(s)</div>
        </Card>
      </div>
    </>
  );
}
