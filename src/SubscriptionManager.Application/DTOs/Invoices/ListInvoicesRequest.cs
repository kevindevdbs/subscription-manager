using System.Globalization;
using SubscriptionManager.Domain.Enums;

namespace SubscriptionManager.Application.DTOs.Invoices;

public record ListInvoicesRequest(string? Status, string? Month)
{
    public const string MonthFormat = "yyyy-MM";

    /// <summary>
    /// Converte <see cref="Status"/> no enum correspondente.
    /// Retorna <c>false</c> apenas quando o valor foi informado e é inválido.
    /// </summary>
    public bool TryGetStatus(out InvoiceStatus? status)
    {
        status = null;

        if (string.IsNullOrWhiteSpace(Status))
        {
            return true;
        }

        if (!Enum.TryParse<InvoiceStatus>(Status, ignoreCase: true, out var parsed) || !Enum.IsDefined(parsed))
        {
            return false;
        }

        status = parsed;
        return true;
    }

    /// <summary>
    /// Converte <see cref="Month"/> (formato <c>yyyy-MM</c>) no primeiro dia do mês.
    /// Retorna <c>false</c> apenas quando o valor foi informado e é inválido.
    /// </summary>
    public bool TryGetMonth(out DateTime? month)
    {
        month = null;

        if (string.IsNullOrWhiteSpace(Month))
        {
            return true;
        }

        if (!DateTime.TryParseExact(Month, MonthFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            return false;
        }

        month = parsed;
        return true;
    }
}
