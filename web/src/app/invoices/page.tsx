import { apiGet } from "@/lib/api";
import type { Contract, Customer, Invoice } from "@/lib/types";
import { invoiceStatusLabel, invoiceStatusOptions } from "@/lib/types";
import { formatDate, formatMoney, formatMonth } from "@/lib/format";
import {
  EmptyState,
  InvoiceStatusBadge,
  PageHeader,
} from "@/components/ui";
import { ApiDown } from "@/components/api-down";
import { ActionButton } from "@/components/action-button";
import { cancelInvoice, payInvoice, refundInvoice } from "./actions";

export const dynamic = "force-dynamic";

export default async function InvoicesPage({
  searchParams,
}: {
  searchParams: Promise<{ status?: string; month?: string; customer?: string }>;
}) {
  const { status = "", month = "", customer = "" } = await searchParams;

  // status e month o back-end filtra; customer é cruzado aqui, porque a API
  // filtra fatura por situação e competência, não por cliente.
  const query = new URLSearchParams();
  if (status) query.set("status", status);
  if (month) query.set("month", month);
  const suffix = query.toString() ? `?${query.toString()}` : "";

  let customers: Customer[];
  let contracts: Contract[];
  let invoices: Invoice[];
  try {
    [customers, contracts, invoices] = await Promise.all([
      apiGet<Customer[]>("/api/customers"),
      apiGet<Contract[]>("/api/contracts"),
      apiGet<Invoice[]>(`/api/invoices${suffix}`),
    ]);
  } catch {
    return (
      <>
        <PageHeader title="Faturas" />
        <ApiDown />
      </>
    );
  }

  const customerOfContract = new Map(contracts.map((c) => [c.id, c.customerId]));
  const customerName = new Map(customers.map((c) => [c.id, c.name]));

  const rows = customer
    ? invoices.filter(
        (invoice) => customerOfContract.get(invoice.contractId) === customer,
      )
    : invoices;

  return (
    <>
      <PageHeader
        title="Faturas"
        description="A emissão e o vencimento rodam sozinhos todo dia. Aqui você acompanha e registra pagamentos."
      />

      <form
        method="get"
        className="mb-6 flex flex-wrap items-end gap-3 rounded-lg border border-border bg-surface p-4"
      >
        <label className="block">
          <span className="mb-1.5 block text-sm text-muted">Cliente</span>
          <select
            name="customer"
            defaultValue={customer}
            className="rounded-md border border-border bg-surface-2 px-3 py-2 text-sm outline-none focus:border-accent"
          >
            <option value="">Todos</option>
            {customers.map((option) => (
              <option key={option.id} value={option.id}>
                {option.name}
              </option>
            ))}
          </select>
        </label>
        <label className="block">
          <span className="mb-1.5 block text-sm text-muted">Situação</span>
          <select
            name="status"
            defaultValue={status}
            className="rounded-md border border-border bg-surface-2 px-3 py-2 text-sm outline-none focus:border-accent"
          >
            <option value="">Todas</option>
            {invoiceStatusOptions.map((option) => (
              <option key={option} value={option}>
                {invoiceStatusLabel[option]}
              </option>
            ))}
          </select>
        </label>
        <label className="block">
          <span className="mb-1.5 block text-sm text-muted">Competência</span>
          <input
            name="month"
            type="month"
            defaultValue={month}
            className="rounded-md border border-border bg-surface-2 px-3 py-2 text-sm outline-none focus:border-accent"
          />
        </label>
        <button
          type="submit"
          className="rounded-md border border-border px-4 py-2 text-sm transition-colors hover:border-accent hover:text-accent"
        >
          Filtrar
        </button>
      </form>

      {rows.length === 0 ? (
        <EmptyState>Nenhuma fatura para o filtro atual.</EmptyState>
      ) : (
        <div className="overflow-x-auto rounded-lg border border-border">
          <table className="w-full text-sm">
            <thead className="bg-surface text-left text-muted">
              <tr>
                <th className="px-4 py-3 font-medium">Cliente</th>
                <th className="px-4 py-3 font-medium">Competência</th>
                <th className="px-4 py-3 font-medium">Valor</th>
                <th className="px-4 py-3 font-medium">Vencimento</th>
                <th className="px-4 py-3 font-medium">Situação</th>
                <th className="px-4 py-3 font-medium text-right">Ações</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((invoice) => (
                <tr key={invoice.id} className="border-t border-border">
                  <td className="px-4 py-3">
                    {customerName.get(
                      customerOfContract.get(invoice.contractId) ?? "",
                    ) ?? "—"}
                  </td>
                  <td className="px-4 py-3 capitalize">
                    {formatMonth(invoice.referenceMonth)}
                  </td>
                  <td className="px-4 py-3 font-mono">
                    {formatMoney(invoice.amount)}
                  </td>
                  <td className="px-4 py-3 text-muted">
                    {formatDate(invoice.dueDate)}
                  </td>
                  <td className="px-4 py-3">
                    <InvoiceStatusBadge status={invoice.status} />
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex justify-end gap-2">
                      {(invoice.status === "Pending" ||
                        invoice.status === "Overdue") && (
                        <>
                          <ActionButton
                            action={payInvoice}
                            id={invoice.id}
                            label="Pagar"
                          />
                          <ActionButton
                            action={cancelInvoice}
                            id={invoice.id}
                            label="Cancelar"
                            tone="danger"
                            confirmMessage="Cancelar esta fatura?"
                          />
                        </>
                      )}
                      {invoice.status === "Paid" && (
                        <ActionButton
                          action={refundInvoice}
                          id={invoice.id}
                          label="Estornar"
                          tone="danger"
                          confirmMessage="Estornar esta fatura?"
                        />
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </>
  );
}
