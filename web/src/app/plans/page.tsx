import { apiGet } from "@/lib/api";
import type { Plan } from "@/lib/types";
import { formatMoney } from "@/lib/format";
import { Card, EmptyState, PageHeader } from "@/components/ui";
import { ApiDown } from "@/components/api-down";
import { ActionButton } from "@/components/action-button";
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
        <h2 className="mb-4 text-sm font-semibold uppercase tracking-wider text-muted">
          Novo plano
        </h2>
        <CreatePlanForm />
      </Card>

      {plans.length === 0 ? (
        <EmptyState>Nenhum plano cadastrado ainda.</EmptyState>
      ) : (
        <div className="overflow-x-auto rounded-lg border border-border">
          <table className="w-full text-sm">
            <thead className="bg-surface text-left text-muted">
              <tr>
                <th className="px-4 py-3 font-medium">Plano</th>
                <th className="px-4 py-3 font-medium">Mensalidade</th>
                <th className="px-4 py-3 font-medium">Situação</th>
                <th className="px-4 py-3 font-medium text-right">Ações</th>
              </tr>
            </thead>
            <tbody>
              {plans.map((plan) => (
                <tr key={plan.id} className="border-t border-border">
                  <td className="px-4 py-3">{plan.name}</td>
                  <td className="px-4 py-3 font-mono">
                    {formatMoney(plan.monthlyPrice)}
                  </td>
                  <td className="px-4 py-3 text-muted">
                    {plan.isActive ? "Ativo" : "Desativado"}
                  </td>
                  <td className="px-4 py-3 text-right">
                    {plan.isActive && (
                      <ActionButton
                        action={deactivatePlan}
                        id={plan.id}
                        label="Desativar"
                        tone="danger"
                        confirmMessage={`Desativar o plano "${plan.name}"?`}
                      />
                    )}
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
