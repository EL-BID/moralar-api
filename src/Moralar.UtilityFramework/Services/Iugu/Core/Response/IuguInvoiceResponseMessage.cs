using Moralar.UtilityFramework.Services.Iugu.Core.Entity;
using Moralar.UtilityFramework.Services.Iugu.Core.Models;
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Response
{
    public class IuguInvoiceResponseMessage : IuguBaseErrors
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("due_date")]
        public string DueDate { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("discount_cents")]
        public object DiscountCents { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("items_total_cents")]
        public int? ItemsTotalCents { get; set; }

        [JsonProperty("notification_url")]
        public object NotificationUrl { get; set; }

        [JsonProperty("return_url")]
        public object ReturnUrl { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("tax_cents")]
        public int? TaxCents { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("total_cents")]
        public int? TotalCents { get; set; }

        [JsonProperty("total_paid_cents")]
        public int? TotalPaidCents { get; set; }

        [JsonProperty("paid_at")]
        public DateTime? PaidAt { get; set; }

        [JsonProperty("taxes_paid_cents")]
        public int? TaxesPaidCents { get; set; }

        [JsonProperty("paid_cents")]
        public int? PaidCents { get; set; }

        [JsonProperty("cc_emails")]
        public object CcEmails { get; set; }

        [JsonProperty("financial_return_date")]
        public string FinancialReturnDate { get; set; }

        [JsonProperty("payable_with")]
        public string PayableWith { get; set; }

        [JsonProperty("overpaid_cents")]
        public int? OverpaidCents { get; set; }

        [JsonProperty("ignore_due_email")]
        public bool? IgnoreDueEmail { get; set; }

        [JsonProperty("ignore_canceled_email")]
        public bool? IgnoreCanceledEmail { get; set; }

        [JsonProperty("advance_fee_cents")]
        public int? AdvanceFeeCents { get; set; }

        [JsonProperty("commission_cents")]
        public int? CommissionCents { get; set; }

        [JsonProperty("early_payment_discount")]
        public bool? EarlyPaymentDiscount { get; set; }

        [JsonProperty("secure_id")]
        public string SecureId { get; set; }

        [JsonProperty("secure_url")]
        public string SecureUrl { get; set; }

        [JsonProperty("customer_id")]
        public string CustomerId { get; set; }

        [JsonProperty("customer_ref")]
        public string CustomerRef { get; set; }

        [JsonProperty("customer_name")]
        public string CustomerName { get; set; }

        [JsonProperty("user_id")]
        public object UserId { get; set; }

        [JsonProperty("total")]
        public string Total { get; set; }

        [JsonProperty("taxes_paid")]
        public string TaxesPaid { get; set; }

        [JsonProperty("total_paid")]
        public string TotalPaid { get; set; }

        [JsonProperty("total_overpaid")]
        public string TotalOverpaid { get; set; }

        [JsonProperty("commission")]
        public string Commission { get; set; }

        [JsonProperty("fines_on_occurrence_day")]
        public string FinesOnOccurrenceDay { get; set; }

        [JsonProperty("total_on_occurrence_day")]
        public string TotalOnOccurrenceDay { get; set; }

        [JsonProperty("fines_on_occurrence_day_cents")]
        public int? FinesOnOccurrenceDayCents { get; set; }

        [JsonProperty("total_on_occurrence_day_cents")]
        public int? TotalOnOccurrenceDayCents { get; set; }

        [JsonProperty("advance_fee")]
        public string AdvanceFee { get; set; }

        [JsonProperty("paid")]
        public string Paid { get; set; }

        [JsonProperty("interest")]
        public object Interest { get; set; }

        [JsonProperty("discount")]
        public object Discount { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("refundable")]
        public bool? Refundable { get; set; }

        [JsonProperty("installments")]
        public string Installments { get; set; }

        [JsonProperty("transaction_number")]
        public int? TransactionNumber { get; set; }

        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; }

        [JsonProperty("created_at_iso")]
        public DateTime? CreatedAtIso { get; set; }

        [JsonProperty("updated_at_iso")]
        public DateTime? UpdatedAtIso { get; set; }

        [JsonProperty("occurrence_date")]
        public string OccurrenceDate { get; set; }

        [JsonProperty("financial_return_dates")]
        public List<IuguFinancialReturnDatesResponse> FinancialReturnDates { get; set; }

        [JsonProperty("bank_slip")]
        public BankSlipResponseMessage BankSlip { get; set; }

        [JsonProperty("items")]
        public List<IuguItemResponse> Items { get; set; }

        [JsonProperty("early_payment_discounts")]
        public List<object> EarlyPaymentDiscounts { get; set; }

        [JsonProperty("variables")]
        public List<IuguVariableResponse> Variables { get; set; }

        [JsonProperty("custom_variables")]
        public List<CustomVariables> CustomVariables { get; set; }

        [JsonProperty("logs")]
        public List<IuguLogResponse> Logs { get; set; }
    }
}
