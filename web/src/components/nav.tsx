"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

const links = [
  { href: "/", label: "Visão geral" },
  { href: "/customers", label: "Clientes" },
  { href: "/plans", label: "Planos" },
  { href: "/contracts", label: "Contratos" },
  { href: "/invoices", label: "Faturas" },
];

export default function Nav() {
  const pathname = usePathname();

  return (
    <header className="border-b border-border bg-surface">
      <nav className="mx-auto flex max-w-5xl flex-wrap items-center gap-1 px-6 py-3">
        <Link href="/" className="mr-4 font-mono text-sm font-semibold">
          <span className="text-accent">$</span> subscription-manager
        </Link>
        {links.slice(1).map((link) => {
          const active =
            pathname === link.href || pathname.startsWith(`${link.href}/`);
          return (
            <Link
              key={link.href}
              href={link.href}
              className={`rounded-md px-3 py-1.5 text-sm transition-colors ${
                active
                  ? "bg-accent/10 text-accent"
                  : "text-muted hover:text-foreground"
              }`}
            >
              {link.label}
            </Link>
          );
        })}
      </nav>
    </header>
  );
}
