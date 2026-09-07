# Subscription Manager

API REST para gestão de assinaturas recorrentes: clientes contratam planos, o
sistema emite as faturas mensais e controla o ciclo de vida de contratos e
faturas até o pagamento, o vencimento ou o cancelamento.

Construída em **.NET 10** com Clean Architecture, EF Core e SQL Server.
**251 testes**, incluindo integração real contra banco em container.

---

## Rodando

Precisa apenas de **Docker**.

```bash
docker compose up -d --build
```

Sobe o SQL Server, espera ele ficar saudável, aplica as migrations e popula uma
massa de demonstração. Quando terminar, abra:

**http://localhost:8080/swagger**

A base já vem com 4 planos (um deles descontinuado), 3 clientes, 4 contratos (um
suspenso) e 3 meses de faturas em estados diferentes — dá para exercitar os
filtros e as transições sem cadastrar nada antes.

Para derrubar tudo, incluindo o volume do banco:

```bash
docker compose down -v
```

### Rodando sem Docker

```bash
docker compose up -d sqlserver
dotnet ef database update --project src/SubscriptionManager.Infrastructure --startup-project src/SubscriptionManager.Api
dotnet run --project src/SubscriptionManager.Api
```

Nesse caminho as migrations não são aplicadas na subida e não há seed — o banco
começa vazio.

---

## Testes

```bash
dotnet test
```

| Projeto | Testes | O que cobre |
|---|---:|---|
| `SubscriptionManager.Domain.Tests` | 66 | Invariantes das entidades e do value object `Money` |
| `Validators.Tests` | 44 | Regras do FluentValidation |
| `UseCases.Tests` | 82 | Handlers com repositórios em mock |
| `WebApi.Tests` | 59 | HTTP de ponta a ponta contra SQL Server real |
| **Total** | **251** | |

Os testes de integração usam [Testcontainers](https://testcontainers.com/): cada
execução sobe um SQL Server descartável e roda as migrations nele. Não há banco
em memória fingindo ser relacional, então índice único, tipo de coluna e
comportamento de transação são exercitados de verdade. Precisa do Docker rodando.

---

## Arquitetura

```
src/
├── SubscriptionManager.Domain          entidades, value objects, exceções, interfaces de repositório
├── SubscriptionManager.Application     casos de uso, DTOs, validadores, mappers
├── SubscriptionManager.Infrastructure  EF Core, repositórios, migrations
└── SubscriptionManager.Api             controllers, DI, filtro de exceção
```

As dependências apontam para dentro: `Api → Application → Domain` e
`Infrastructure → Domain`. O `Domain` não referencia nenhum dos outros, e é onde
ficam as interfaces que o `Infrastructure` implementa.

**As entidades são ricas, não sacos de propriedades.** Todo setter é privado e o
estado só muda por método que valida a transição:

```csharp
public void Pay(DateTime paidAt)
{
    if (Status != InvoiceStatus.Pending && Status != InvoiceStatus.Overdue)
    {
        throw new ConflictException("A fatura não está em um estado válido para ser paga.");
    }

    PaidAt = paidAt;
    Status = InvoiceStatus.Paid;
}
```

Não existe caminho para uma fatura paga ser paga de novo, porque não existe
`invoice.Status = ...` fora da entidade.

**Erro vira resposta HTTP em um lugar só.** As exceções de negócio herdam de
`SubscriptionManagerException`, que carrega o próprio status, e um
`ExceptionFilter` as converte em resposta. Nenhum controller tem `try/catch`:

| Exceção | Status |
|---|---|
| `ErrorOnValidationException` | 400 |
| `NotFoundException` | 404 |
| `ConflictException` | 409 |
| qualquer outra | 500, com log e sem vazar detalhe interno |

---

## Modelo

**Customer** contrata um **Plan** através de um **Contract**, que gera
**Invoices** mensais. Valores monetários usam o value object `Money`.

Contrato e fatura têm máquina de estados própria:

```
Contract:  Active ⇄ Suspended → Cancelled

Invoice:   Pending → Paid → Refunded
             ↓  ↘
          Overdue → Paid
             ↓  ↘
          Cancelled
```

Regras que o modelo garante:

- **Um cliente não assina o mesmo plano duas vezes.** Vale para contrato ativo ou
  suspenso; se ele cancelar, pode reassinar depois.
- **Plano descontinuado não aceita adesão nova**, mas os contratos existentes
  continuam sendo faturados normalmente.
- **A geração mensal é idempotente.** Rodar duas vezes a mesma competência não
  duplica fatura, e contrato suspenso ou cancelado não gera cobrança.

---

## Endpoints

### Clientes e planos

| Verbo | Rota | Descrição |
|---|---|---|
| `POST` | `/api/customers` | Cadastra cliente |
| `GET` | `/api/customers` | Lista clientes |
| `GET` | `/api/customers/{id}` | Busca cliente |
| `POST` | `/api/plans` | Cadastra plano |
| `GET` | `/api/plans` | Lista planos |
| `GET` | `/api/plans/{id}` | Busca plano |
| `PATCH` | `/api/plans/{id}/deactivate` | Descontinua o plano |

### Contratos

| Verbo | Rota | Descrição |
|---|---|---|
| `POST` | `/api/contracts` | Cria contrato — 409 se duplicado ou plano inativo |
| `GET` | `/api/contracts` | Lista contratos |
| `GET` | `/api/contracts/{id}` | Busca contrato |
| `GET` | `/api/contracts/{id}/invoices` | Faturas do contrato |
| `PATCH` | `/api/contracts/{id}/suspend` | Suspende |
| `PATCH` | `/api/contracts/{id}/reactivate` | Reativa |
| `PATCH` | `/api/contracts/{id}/cancel` | Cancela — corpo `{ endDate? }` |

### Faturas

| Verbo | Rota | Descrição |
|---|---|---|
| `POST` | `/api/invoices/generate` | Emite as faturas da competência — `{ referenceMonth }` |
| `GET` | `/api/invoices` | Lista com filtro `?status=&month=yyyy-MM` |
| `POST` | `/api/invoices/mark-overdue` | Marca em lote as vencidas — `{ referenceDate? }` |
| `PATCH` | `/api/invoices/{id}/pay` | Paga — corpo `{ paidAt? }` |
| `PATCH` | `/api/invoices/{id}/overdue` | Marca vencida — corpo `{ referenceDate? }` |
| `PATCH` | `/api/invoices/{id}/refund` | Estorna |
| `PATCH` | `/api/invoices/{id}/cancel` | Cancela |

Nas rotas com corpo opcional, a data omitida vale `DateTime.UtcNow`.

### Experimentando

```bash
# fatura vencida da massa de demonstração
curl "http://localhost:8080/api/invoices?status=Overdue"

# tentar assinar o plano descontinuado devolve 409
curl -X POST http://localhost:8080/api/contracts \
  -H "Content-Type: application/json" \
  -d '{"customerId":"<id>","planId":"<id do Legado 2024>","startDate":"2026-01-01"}'
```

---

## Decisões técnicas

O raciocínio de cada mudança está nas descrições dos
[pull requests](https://github.com/kevindevdbs/subscription-manager/pulls?q=is%3Apr+is%3Aclosed) —
cada um documenta o que foi entregue, as alternativas descartadas e o que ficou
de fora de propósito. Um resumo do que considero mais relevante:

**Exceção de negócio nasce na entidade, não no handler.** Uma versão anterior
embrulhava cada transição num `try/catch` que convertia
`InvalidOperationException` em 409. O catch era largo demais: qualquer
`InvalidOperationException` vinda de outro lugar viraria "conflito de negócio"
com a mensagem interna no corpo da resposta, em vez de 500. Lançar
`ConflictException` no ponto que decide a regra é exato, e removeu 7 blocos de
`try/catch`.

**PATCH para transição, POST para operação em lote.** As rotas com `{id}` alteram
um campo de um recurso existente e repetir a chamada leva ao mesmo estado. Já
`generate` e `mark-overdue` não endereçam recurso nenhum: rodam sobre vários e
reportam quantos mudaram.

**A regra de contrato duplicado vive em duas camadas.** Uma consulta no handler
devolve 409 com mensagem legível; um índice único filtrado
(`WHERE Status <> Cancelled`) garante a regra quando duas requisições
concorrentes passam pela consulta ao mesmo tempo. O filtro é o que permite
reassinar um plano depois de cancelá-lo — índice único cru bloquearia isso para
sempre.

**A fatura congela o preço na emissão.** Ela copia o valor do plano em vez de
referenciar o `Money` dele, então reajuste futuro não reescreve fatura já
emitida. Isso saiu da correção de um bug: `Money` é *owned entity* do plano e da
fatura, e compartilhar a instância rastreada quebrava o `SaveChanges` assim que
dois contratos ativos usavam o mesmo plano.

**Migration na subida é exceção, não padrão.** O `docker compose` liga
`Database__MigrateOnStartup` para o projeto ficar utilizável com um comando. Fora
dele o padrão é desligado, porque em ambiente real migration é passo de deploy —
várias instâncias subindo juntas tentariam migrar o mesmo banco ao mesmo tempo.

---

## O que ficou de fora

Consciente, não esquecido:

- **Autenticação e autorização.** A API é aberta.
- **Agendamento.** `generate` e `mark-overdue` existem como endpoint, mas nada os
  chama sozinho — falta um hosted service ou cron.
- **Régua de cobrança.** `Overdue` e `Suspended` existem e a geração já pula
  contrato suspenso, mas nada liga automaticamente um ao outro (vencer → lembrar
  → suspender → cancelar).
- **Pagamento como entidade própria.** Hoje a fatura carimba `PaidAt`; não há
  registro de meio de pagamento, tentativa recusada ou pagamento parcial.
- **Rateio na troca de plano.** Mudar de plano é cancelar e criar outro, sem
  cálculo proporcional.
- **Moeda no `Money`.** Só existe valor; o sistema assume uma moeda única.
- **Paginação** nas listagens e **concorrência otimista** nas transições.

---

## Stack

.NET 10 · ASP.NET Core · EF Core 10 · SQL Server 2022 · FluentValidation ·
xUnit · Shouldly · Moq · Bogus · Testcontainers · Swagger
