// Espelha os DTOs e enums da API (SubscriptionManager.Application / .Domain).

export type Plan = {
  id: string;
  name: string;
  monthlyPrice: number;
  isActive: boolean;
};

export type Customer = {
  id: string;
  name: string;
  email: string;
  document: string;
  createdAt: string;
};

export type ContractStatus = "Active" | "Suspended" | "Cancelled";

export type Contract = {
  id: string;
  customerId: string;
  planId: string;
  startDate: string;
  endDate: string | null;
  status: ContractStatus;
};

export type InvoiceStatus =
  | "Pending"
  | "Paid"
  | "Overdue"
  | "Cancelled"
  | "Refunded";

export type Invoice = {
  id: string;
  contractId: string;
  amount: number;
  dueDate: string;
  referenceMonth: string;
  paidAt: string | null;
  status: InvoiceStatus;
};

// Rótulos em português para exibição, mantendo o valor da API por baixo.

export const contractStatusLabel: Record<ContractStatus, string> = {
  Active: "Ativo",
  Suspended: "Suspenso",
  Cancelled: "Cancelado",
};

export const invoiceStatusLabel: Record<InvoiceStatus, string> = {
  Pending: "Pendente",
  Paid: "Paga",
  Overdue: "Vencida",
  Cancelled: "Cancelada",
  Refunded: "Estornada",
};

export const invoiceStatusOptions = Object.keys(
  invoiceStatusLabel,
) as InvoiceStatus[];
