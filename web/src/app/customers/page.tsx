import Link from "next/link";
import { apiGet } from "@/lib/api";
import type { Customer } from "@/lib/types";
import { formatDate } from "@/lib/format";
import { Card, EmptyState, PageHeader } from "@/components/ui";
import { ApiDown } from "@/components/api-down";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
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
      <PageHeader title="Clientes" description="Quem contrata os planos." />

      <Card className="mb-8">
        <h2 className="mb-4 text-sm font-semibold uppercase tracking-wider text-muted-foreground">
          Novo cliente
        </h2>
        <CreateCustomerForm />
      </Card>

      {customers.length === 0 ? (
        <EmptyState>Nenhum cliente cadastrado ainda.</EmptyState>
      ) : (
        <div className="rounded-xl border border-border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Nome</TableHead>
                <TableHead>E-mail</TableHead>
                <TableHead>Documento</TableHead>
                <TableHead>Desde</TableHead>
                <TableHead className="text-right">Faturas</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {customers.map((customer) => (
                <TableRow key={customer.id}>
                  <TableCell className="font-medium">{customer.name}</TableCell>
                  <TableCell className="text-muted-foreground">
                    {customer.email}
                  </TableCell>
                  <TableCell className="font-mono text-muted-foreground">
                    {customer.document}
                  </TableCell>
                  <TableCell className="text-muted-foreground">
                    {formatDate(customer.createdAt)}
                  </TableCell>
                  <TableCell className="text-right">
                    <Button
                      variant="outline"
                      size="sm"
                      nativeButton={false}
                      render={<Link href={`/invoices?customer=${customer.id}`} />}
                    >
                      Ver faturas
                    </Button>
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
