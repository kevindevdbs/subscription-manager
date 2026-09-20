import { Card } from "./ui";

export function ApiDown() {
  return (
    <Card className="border-destructive/30">
      <h2 className="mb-1 font-semibold text-destructive">API fora do ar</h2>
      <p className="text-sm text-muted-foreground">
        Não foi possível falar com a API. Suba o back-end e recarregue a página.
      </p>
      <pre className="mt-3 overflow-x-auto rounded-md bg-muted px-3 py-2 font-mono text-xs text-muted-foreground">
        docker compose up -d
      </pre>
    </Card>
  );
}
