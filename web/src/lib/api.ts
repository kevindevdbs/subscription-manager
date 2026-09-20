import "server-only";

// O servidor Next fala com a API .NET server-to-server: o navegador nunca chama
// a API direto, então não há CORS a resolver nem URL de API exposta ao cliente.
const API_BASE = process.env.API_BASE_URL ?? "http://localhost:8080";

export class ApiError extends Error {
  constructor(public readonly messages: string[]) {
    super(messages.join(" "));
    this.name = "ApiError";
  }
}

async function parseErrors(response: Response): Promise<string[]> {
  try {
    const body = await response.json();
    if (Array.isArray(body?.errors) && body.errors.length > 0) {
      return body.errors as string[];
    }
  } catch {
    // corpo vazio ou não-JSON: cai na mensagem genérica abaixo.
  }
  return [`A API respondeu ${response.status}.`];
}

// Leitura. cache: "no-store" mantém a tela sempre com o dado atual, sem
// biblioteca de cache — é uma tela de operação, não uma página de conteúdo.
export async function apiGet<T>(path: string): Promise<T> {
  const response = await fetch(`${API_BASE}${path}`, { cache: "no-store" });

  if (!response.ok) {
    throw new ApiError(await parseErrors(response));
  }

  return response.json() as Promise<T>;
}

export type SendResult<T> =
  | { ok: true; data: T }
  | { ok: false; errors: string[] };

// Escrita (POST/PATCH). Devolve os erros da API em vez de lançar, para o
// formulário mostrar a mensagem sem derrubar a página.
export async function apiSend<T = unknown>(
  path: string,
  method: "POST" | "PATCH",
  body?: unknown,
): Promise<SendResult<T>> {
  let response: Response;

  try {
    response = await fetch(`${API_BASE}${path}`, {
      method,
      cache: "no-store",
      headers: body === undefined ? {} : { "Content-Type": "application/json" },
      body: body === undefined ? undefined : JSON.stringify(body),
    });
  } catch {
    return {
      ok: false,
      errors: ["Não foi possível falar com a API. Ela está no ar?"],
    };
  }

  if (!response.ok) {
    return { ok: false, errors: await parseErrors(response) };
  }

  const text = await response.text();
  const data = text.length > 0 ? (JSON.parse(text) as T) : (undefined as T);
  return { ok: true, data };
}
