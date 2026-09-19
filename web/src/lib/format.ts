const currency = new Intl.NumberFormat("pt-BR", {
  style: "currency",
  currency: "BRL",
});

const date = new Intl.DateTimeFormat("pt-BR", {
  day: "2-digit",
  month: "2-digit",
  year: "numeric",
  timeZone: "UTC",
});

const monthYear = new Intl.DateTimeFormat("pt-BR", {
  month: "long",
  year: "numeric",
  timeZone: "UTC",
});

export function formatMoney(value: number): string {
  return currency.format(value);
}

export function formatDate(iso: string): string {
  return date.format(new Date(iso));
}

export function formatMonth(iso: string): string {
  return monthYear.format(new Date(iso));
}
