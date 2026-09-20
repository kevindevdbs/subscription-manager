import { Card } from "./ui";

export function ApiDown() {
  return (
    <Card className="border-red-500/30">
      <h2 className="mb-1 font-semibold text-red-400">API fora do ar</h2>
      <p className="text-sm text-muted">
        Não foi possível falar com a API. Suba o back-end e recarregue a página.
      </p>
      <pre className="mt-3 overflow-x-auto rounded-md bg-surface-2 px-3 py-2 font-mono text-xs text-muted">
        docker compose up -d
      </pre>
    </Card>
  );
}
