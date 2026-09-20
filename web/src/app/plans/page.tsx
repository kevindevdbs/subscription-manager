import { apiGet } from "@/lib/api";
import type { Plan } from "@/lib/types";
import { formatMoney } from "@/lib/format";
import { Card, EmptyState, PageHeader } from "@/components/ui";
import { ApiDown } from "@/components/api-down";
import { ActionButton } from "@/components/action-button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { CreatePlanForm } from "./create-form";
import { deactivatePlan } from "./actions";

export const dynamic = "force-dynamic";

export default async function PlansPage() {
  let plans: Plan[];
  try {
    plans = await apiGet<Plan[]>("/api/plans");
  } catch {
    return (
      <>
        <PageHeader title="Planos" />
        <ApiDown />
      </>
    );
  }

  return (
    <>
      <PageHeader
        title="Planos"
        description="Um plano desativado não aceita contratos novos, mas segue faturando os existentes."
      />

      <Card className="mb-8">
        <h2 className="mb-4 text-sm font-semibold uppercase tracking-wider text-muted-foreground">
          Novo plano
        </h2>
        <CreatePlanForm />
      </Card>

      {plans.length === 0 ? (
        <EmptyState>Nenhum plano cadastrado ainda.</EmptyState>
      ) : (
        <div className="rounded-xl border border-border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Plano</TableHead>
                <TableHead>Mensalidade</TableHead>
                <TableHead>Situação</TableHead>
                <TableHead className="text-right">Ações</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {plans.map((plan) => (
                <TableRow key={plan.id}>
                  <TableCell className="font-medium">{plan.name}</TableCell>
                  <TableCell className="font-mono">
                    {formatMoney(plan.monthlyPrice)}
                  </TableCell>
                  <TableCell className="text-muted-foreground">
                    {plan.isActive ? "Ativo" : "Desativado"}
                  </TableCell>
                  <TableCell className="text-right">
                    {plan.isActive && (
                      <ActionButton
                        action={deactivatePlan}
                        id={plan.id}
                        label="Desativar"
                        tone="danger"
                        confirmMessage={`Desativar o plano "${plan.name}"?`}
                      />
                    )}
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
