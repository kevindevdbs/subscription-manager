# Subscription Manager — Web

Painel em **Next.js** (App Router) para a API do Subscription Manager: clientes,
planos, contratos e faturas, com as transições de estado de cada um.

## Como funciona

O objetivo era um front simples, sem biblioteca de cache ou de data-fetching. A
escolha que sustenta isso:

**Server Components leem, Server Actions escrevem.** O servidor Next fala com a
API .NET de servidor para servidor. O navegador nunca chama a API direto, então:

- não há CORS a configurar no back-end;
- a URL da API não é exposta ao cliente;
- a leitura é um `fetch` com `cache: "no-store"` — sempre o dado atual, sem
  React Query, SWR ou revalidação por tag. É uma tela de operação, não conteúdo.

Depois de uma escrita, o Server Action chama `revalidatePath` e a lista
re-renderiza no servidor. Nenhum estado de dados vive no cliente.

As únicas dependências além do Next são `tailwindcss` e `server-only`.

## Rodando

Precisa da API no ar. A forma mais rápida é subir a stack pelo `docker compose`
na raiz do repositório (a API fica em `http://localhost:8080`):

```bash
docker compose up -d
```

Depois, aqui:

```bash
npm install
npm run dev
```

Abra `http://localhost:3000`.

A URL da API vem de `API_BASE_URL` (veja `.env.example`), com padrão
`http://localhost:8080`. Rodando a API por `dotnet run` em vez do Docker, ela
sobe em `http://localhost:5270` — ajuste o `.env.local`.

## Estrutura

```
src/
  lib/
    api.ts        # fetch server-to-server: apiGet (leitura) e apiSend (escrita)
    types.ts      # espelho dos DTOs e enums da API, com rótulos em português
    format.ts     # moeda e data em pt-BR
  components/     # UI compartilhada (client onde precisa de interação)
  app/
    page.tsx                 # visão geral
    customers | plans | contracts | invoices/
      page.tsx    # Server Component: lê e monta a tela
      actions.ts  # Server Actions: POST/PATCH na API + revalidatePath
      *-form.tsx   # Client Component do formulário (useActionState)
```
