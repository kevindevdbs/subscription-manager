"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { Button } from "@/components/ui/button";

const links = [
  { href: "/customers", label: "Clientes" },
  { href: "/plans", label: "Planos" },
  { href: "/contracts", label: "Contratos" },
  { href: "/invoices", label: "Faturas" },
];

export default function Nav() {
  const pathname = usePathname();

  return (
    <header className="border-b border-border bg-card">
      <nav className="mx-auto flex max-w-5xl flex-wrap items-center gap-1 px-6 py-3">
        <Link href="/" className="mr-4 font-mono text-sm font-semibold">
          <span className="text-primary">$</span> subscription-manager
        </Link>
        {links.map((link) => {
          const active =
            pathname === link.href || pathname.startsWith(`${link.href}/`);
          return (
            <Button
              key={link.href}
              variant={active ? "secondary" : "ghost"}
              size="sm"
              nativeButton={false}
              render={<Link href={link.href} />}
            >
              {link.label}
            </Button>
          );
        })}
      </nav>
    </header>
  );
}
