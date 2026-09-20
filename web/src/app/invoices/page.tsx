import { apiGet } from "@/lib/api";
import type { Contract, Customer, Invoice } from "@/lib/types";
import { invoiceStatusLabel, invoiceStatusOptions } from "@/lib/types";
import { formatDate, formatMoney, formatMonth } from "@/lib/format";
import { EmptyState, InvoiceStatusBadge, PageHeader } from "@/components/ui";
import { ApiDown } from "@/components/api-down";
import { ActionButton } from "@/components/action-button";
import { Field, FormSelect } from "@/components/form";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
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
        className="mb-6 flex flex-wrap items-end gap-3 rounded-xl border border-border bg-card p-4"
      >
        <FormSelect
          label="Cliente"
          name="customer"
          defaultValue={customer}
          includeAll="Todos"
          options={customers.map((c) => ({ value: c.id, label: c.name }))}
        />
        <FormSelect
          label="Situação"
          name="status"
          defaultValue={status}
          includeAll="Todas"
          options={invoiceStatusOptions.map((option) => ({
            value: option,
            label: invoiceStatusLabel[option],
          }))}
        />
        <Field
          label="Competência"
          name="month"
          type="month"
          required={false}
          defaultValue={month}
        />
        <Button type="submit" variant="outline">
          Filtrar
        </Button>
      </form>

      {rows.length === 0 ? (
        <EmptyState>Nenhuma fatura para o filtro atual.</EmptyState>
      ) : (
        <div className="rounded-xl border border-border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Cliente</TableHead>
                <TableHead>Competência</TableHead>
                <TableHead>Valor</TableHead>
                <TableHead>Vencimento</TableHead>
                <TableHead>Situação</TableHead>
                <TableHead className="text-right">Ações</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {rows.map((invoice) => (
                <TableRow key={invoice.id}>
                  <TableCell className="font-medium">
                    {customerName.get(
                      customerOfContract.get(invoice.contractId) ?? "",
                    ) ?? "—"}
                  </TableCell>
                  <TableCell className="capitalize">
                    {formatMonth(invoice.referenceMonth)}
                  </TableCell>
                  <TableCell className="font-mono">
                    {formatMoney(invoice.amount)}
                  </TableCell>
                  <TableCell className="text-muted-foreground">
                    {formatDate(invoice.dueDate)}
                  </TableCell>
                  <TableCell>
                    <InvoiceStatusBadge status={invoice.status} />
                  </TableCell>
                  <TableCell>
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
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      )}
    </>
  );
}
