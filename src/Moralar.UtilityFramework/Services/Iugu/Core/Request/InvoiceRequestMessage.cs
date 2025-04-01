using Moralar.UtilityFramework.Services.Iugu.Core.Entity;
using Moralar.UtilityFramework.Services.Iugu.Core.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Request
{
    public class InvoiceRequestMessage : IuguBaseErrors
    {
        //
        // Resumen:
        //     E-Mail do cliente
        [JsonProperty("email")]
        public string Email { get; set; }

        //
        // Resumen:
        //     Data de Expiração (DD/MM/AAAA) - (AAAA/MM/DD)
        [JsonProperty("due_date")]
        [Display(Name = "Data de expiração")]
        public string DueDate { get; set; }

        //
        // Resumen:
        //     Itens da Fatura
        [JsonProperty("items")]
        public List<Item> Items { get; set; }

        //
        // Resumen:
        //     Informações do Cliente para o Anti Fraude ou Boleto
        [JsonProperty("payer")]
        [IsClass]
        public PayerModel Payer { get; set; }

        //
        // Resumen:
        //     (opcional) Cliente é redirecionado para essa URL após efetuar o pagamento da
        //     Fatura pela página de Fatura da Iugu
        [JsonProperty("return_url ")]
        public string ReturnUrl { get; set; }

        //
        // Resumen:
        //     (opcional) Cliente é redirecionado para essa URL se a Fatura que estiver acessando
        //     estiver expirada
        [JsonProperty("expired_url ")]
        public string ExpiredUrl { get; set; }

        //
        // Resumen:
        //     (opcional) URL chamada para todas as notificações de Fatura, assim como os webhooks
        //     (Gatilhos) são chamados
        [JsonProperty("notification_url")]
        public string NotificationUrl { get; set; }

        //
        // Resumen:
        //     (opcional) Valor dos Impostos em centavos
        [JsonProperty("tax_cents")]
        public int TaxCents { get; set; }

        //
        // Resumen:
        //     (opcional) Booleano para Habilitar ou Desabilitar multa por atraso de pagamento
        [JsonProperty("fines")]
        [Display(Name = "Multa de atraso")]
        public bool Fines { get; set; }

        //
        // Resumen:
        //     (opcional) Determine a multa a ser cobrada para pagamentos efetuados após a data
        //     de vencimento
        [JsonProperty("late_payment_fine")]
        [Display(Name = "Multa após vencimento")]
        public string LatePaymentFine { get; set; }

        //
        // Resumen:
        //     (opcional) Booleano que determina se cobra ou não juros por dia de atraso. 1%
        //     ao mês pro rata.
        [JsonProperty("per_day_interest")]
        [Display(Name = "Juros por dia")]
        public bool PerDayInterest { get; set; }

        //
        // Resumen:
        //     (opcional) Valor dos Descontos em centavos
        [JsonProperty("discount_cents")]
        public int DiscountCents { get; set; }

        //
        // Resumen:
        //     (opcional) ID do Cliente
        [JsonProperty("customer_id")]
        public string CustomerId { get; set; }

        //
        // Resumen:
        //     (opcional) Booleano que ignora o envio do e-mail de cobrança
        [JsonProperty("ignore_due_email")]
        public bool IgnoreDueEmail { get; set; }

        //
        // Resumen:
        //     (opcional) Amarra esta Fatura com a Assinatura especificada
        [JsonProperty("subscription_id")]
        public string SubscriptionId { get; set; }

        //
        // Resumen:
        //     (opcional) Método de pagamento que será disponibilizado para esta Fatura (‘all’,
        //     ‘credit_card’ ou ‘bank_slip’). Obs: Caso esta Fatura esteja atrelada à uma Assinatura,
        //     a prioridade é herdar o valor atribuído na Assinatura; caso esta esteja atribuído
        //     o valor ‘all’, o sistema considerará o payable_with da Fatura; se não, o sistema
        //     considerará o payable_with da Assinatura.
        [JsonProperty("payable_with")]
        [Display(Name = "Metodo de pagamento aceito")]
        public string PayableWith { get; set; }

        //
        // Resumen:
        //     (opcional) Caso tenha o subscription_id, pode-se enviar o número de créditos
        //     a adicionar nessa Assinatura quando a Fatura for paga
        [JsonProperty("credits")]
        public int? Credits { get; set; }

        //
        // Resumen:
        //     (opcional) Logs da Fatura
        [JsonProperty("logs")]
        public List<Logs> Logs { get; set; }

        //
        // Resumen:
        //     (opcional) Variáveis Personalizadas
        [JsonProperty("custom_variables")]
        public List<CustomVariables> CustomVariables { get; set; }

        //
        // Resumen:
        //     (Opcional) Ativa ou desativa os descontos por pagamento antecipado. Quando true,
        //     sobrepõe as configurações de desconto da conta
        [JsonProperty("early_payment_discount")]
        public bool EarlyPaymentDiscount { get; set; }

        //
        // Resumen:
        //     (Opcional) Quantidade de dias de antecedência para o pagamento receber o desconto
        //     (Se enviado, substituirá a configuração atual da conta)
        [JsonProperty("early_payment_discounts")]
        public List<EarlyPaymentDiscountsModel> EarlyPaymentDiscounts { get; set; }

        //
        // Resumen:
        //     (Opcional) Se true, garante que a data de vencimento seja apenas em dias de semana,
        //     e não em sábados ou domingos.
        [JsonProperty("ensure_workday_due_date")]
        public bool EnsureWorkdayDueDate { get; set; }

        //
        // Resumen:
        //     (Opcional) Endereços de E-mail para cópia separados por ponto e vírgula.
        [JsonProperty("cc_emails")]
        public string CcEmails { get; set; }

        //
        // Resumen:
        //     (Opcional) Número único que identifica o pedido de compra. Opcional, ajuda a
        //     evitar o pagamento da mesma fatura.
        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        public InvoiceRequestMessage()
        {
            Items = new List<Item>();
            Logs = new List<Logs>();
            EarlyPaymentDiscounts = new List<EarlyPaymentDiscountsModel>();
            CustomVariables = new List<CustomVariables>();
            PayableWith = "bank_slip";
            EnsureWorkdayDueDate = true;
            Payer = new PayerModel();
        }
    }
}