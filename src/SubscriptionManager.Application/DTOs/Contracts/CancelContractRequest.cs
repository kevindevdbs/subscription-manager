namespace SubscriptionManager.Application.DTOs.Contracts;

/// <summary>
/// <paramref name="EndDate"/> é opcional: quando omitido o contrato é encerrado
/// na data corrente.
/// </summary>
public record CancelContractRequest(DateTime? EndDate = null);
