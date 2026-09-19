import { apiGet } from "@/lib/api";
import type { Contract, Customer, Plan } from "@/lib/types";
import { formatDate, formatMoney } from "@/lib/format";
import {
  Card,
  ContractStatusBadge,
  EmptyState,
  PageHeader,
} from "@/components/ui";
import { ApiDown } from "@/components/api-down";
import { ActionButton } from "@/components/action-button";
import { CreateContractForm } from "./create-form";
import {
  cancelContract,
  reactivateContract,
  suspendContract,
} from "./actions";

export const dynamic = "force-dynamic";

export default async function ContractsPage() {
  let customers: Customer[];
  let plans: Plan[];
  let contracts: Contract[];
  try {
    [customers, plans, contracts] = await Promise.all([
      apiGet<Customer[]>("/api/customers"),
      apiGet<Plan[]>("/api/plans"),
      apiGet<Contract[]>("/api/contracts"),
    ]);
  } catch {
    return (
      <>
        <PageHeader title="Contratos" />
        <ApiDown />
      </>
    );
  }

  const customerName = new Map(customers.map((c) => [c.id, c.name]));
  const planById = new Map(plans.map((p) => [p.id, p]));
  const canCreate = customers.length > 0 && plans.some((p) => p.isActive);

  return (
    <>
      <PageHeader
        title="Contratos"
        description="Um cliente por plano. Suspender pausa o faturamento; cancelar encerra o contrato."
      />

      <Card className="mb-8">
        <h2 className="mb-4 text-sm font-semibold uppercase tracking-wider text-muted">
          Novo contrato
        </h2>
        {canCreate ? (
          <CreateContractForm customers={customers} plans={plans} />
        ) : (
          <p className="text-sm text-muted">
            É preciso ter ao menos um cliente e um plano ativo para criar um
            contrato.
          </p>
        )}
      </Card>

      {contracts.length === 0 ? (
        <EmptyState>Nenhum contrato ainda.</EmptyState>
      ) : (
        <div className="overflow-x-auto rounded-lg border border-border">
          <table className="w-full text-sm">
            <thead className="bg-surface text-left text-muted">
              <tr>
                <th className="px-4 py-3 font-medium">Cliente</th>
                <th className="px-4 py-3 font-medium">Plano</th>
                <th className="px-4 py-3 font-medium">Início</th>
                <th className="px-4 py-3 font-medium">Situação</th>
                <th className="px-4 py-3 font-medium text-right">Ações</th>
              </tr>
            </thead>
            <tbody>
              {contracts.map((contract) => {
                const plan = planById.get(contract.planId);
                return (
                  <tr key={contract.id} className="border-t border-border">
                    <td className="px-4 py-3">
                      {customerName.get(contract.customerId) ?? "—"}
                    </td>
                    <td className="px-4 py-3">
                      {plan ? plan.name : "—"}
                      {plan && (
                        <span className="ml-2 font-mono text-xs text-muted">
                          {formatMoney(plan.monthlyPrice)}
                        </span>
                      )}
                    </td>
                    <td className="px-4 py-3 text-muted">
                      {formatDate(contract.startDate)}
                    </td>
                    <td className="px-4 py-3">
                      <ContractStatusBadge status={contract.status} />
                    </td>
                    <td className="px-4 py-3">
                      <div className="flex justify-end gap-2">
                        {contract.status === "Active" && (
                          <ActionButton
                            action={suspendContract}
                            id={contract.id}
                            label="Suspender"
                          />
                        )}
                        {contract.status === "Suspended" && (
                          <ActionButton
                            action={reactivateContract}
                            id={contract.id}
                            label="Reativar"
                          />
                        )}
                        {contract.status !== "Cancelled" && (
                          <ActionButton
                            action={cancelContract}
                            id={contract.id}
                            label="Cancelar"
                            tone="danger"
                            confirmMessage="Cancelar este contrato? A ação não tem volta."
                          />
                        )}
                      </div>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      )}
    </>
  );
}
