import Link from "next/link";
import { apiGet } from "@/lib/api";
import type { Customer } from "@/lib/types";
import { formatDate } from "@/lib/format";
import { Card, EmptyState, PageHeader } from "@/components/ui";
import { ApiDown } from "@/components/api-down";
import { CreateCustomerForm } from "./create-form";

export const dynamic = "force-dynamic";

export default async function CustomersPage() {
  let customers: Customer[];
  try {
    customers = await apiGet<Customer[]>("/api/customers");
  } catch {
    return (
      <>
        <PageHeader title="Clientes" />
        <ApiDown />
      </>
    );
  }

  return (
    <>
      <PageHeader
        title="Clientes"
        description="Quem contrata os planos."
      />

      <Card className="mb-8">
        <h2 className="mb-4 text-sm font-semibold uppercase tracking-wider text-muted">
          Novo cliente
        </h2>
        <CreateCustomerForm />
      </Card>

      {customers.length === 0 ? (
        <EmptyState>Nenhum cliente cadastrado ainda.</EmptyState>
      ) : (
        <div className="overflow-x-auto rounded-lg border border-border">
          <table className="w-full text-sm">
            <thead className="bg-surface text-left text-muted">
              <tr>
                <th className="px-4 py-3 font-medium">Nome</th>
                <th className="px-4 py-3 font-medium">E-mail</th>
                <th className="px-4 py-3 font-medium">Documento</th>
                <th className="px-4 py-3 font-medium">Desde</th>
                <th className="px-4 py-3 font-medium text-right">Faturas</th>
              </tr>
            </thead>
            <tbody>
              {customers.map((customer) => (
                <tr key={customer.id} className="border-t border-border">
                  <td className="px-4 py-3">{customer.name}</td>
                  <td className="px-4 py-3 text-muted">{customer.email}</td>
                  <td className="px-4 py-3 font-mono text-muted">
                    {customer.document}
                  </td>
                  <td className="px-4 py-3 text-muted">
                    {formatDate(customer.createdAt)}
                  </td>
                  <td className="px-4 py-3 text-right">
                    <Link
                      href={`/invoices?customer=${customer.id}`}
                      className="rounded-md border border-border px-3 py-1.5 text-xs font-medium text-muted transition-colors hover:border-accent hover:text-accent"
                    >
                      Ver faturas
                    </Link>
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
