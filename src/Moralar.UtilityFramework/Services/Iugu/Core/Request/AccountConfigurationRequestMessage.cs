
using Moralar.UtilityFramework.Services.Iugu.Core.Models;
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Request
{
    public class AccountConfigurationRequestMessage : IuguBaseErrors
    {
        //
        // Resumen:
        //     Percentual de comissionamento enviado para a conta que gerencia o marketplace
        //     (Valor entre 0 e 70)
        [JsonProperty("commission_percent")]
        public int? CommissionPercent { get; set; }

        //
        // Resumen:
        //     Saque automático
        [JsonProperty("auto_withdraw")]
        public bool AutoWithdraw { get; set; }

        //
        // Resumen:
        //     Multa
        [JsonProperty("fines")]
        public bool Fines { get; set; }

        //
        // Resumen:
        //     Juros de mora
        [JsonProperty("per_day_interest")]
        public bool PerDayInterest { get; set; }

        //
        // Resumen:
        //     Valor da multa em % (Ex: 2)
        [JsonProperty("late_payment_fine")]
        public decimal LatePaymentFine { get; set; }

        //
        // Resumen:
        //     Antecipação Automática. Só pode ser atribuído caso a conta tenha a funcionalidade
        //     de Novo Adiantamento habilitado (entre em contato com o Suporte para habilitar)
        [JsonProperty("auto_advance")]
        public bool AutoAdvance { get; set; }

        //
        // Resumen:
        //     Opções de Antecipação Automática. Obrigatório caso auto_advance seja true. Recebe
        //     os valores 'daily' (Antecipação diária), 'weekly' (Antecipação semanal, que ocorre
        //     no dia da semana determinado pelo usuário), 'monthly' (Antecipação mensal, que
        //     ocorre no dia do mês determinado pelo usuário) e 'days_after_payment' (Antecipação
        //     que ocorre em um número de dias após o pagamento especificado pelo usuário)
        [JsonProperty("auto_advance_type")]
        public string AutoAdvanceType { get; set; }

        //
        // Resumen:
        //     Obrigatório caso auto_advance seja true e auto_advance_type diferente de 'daily'.
        //     Em caso de auto_advance_type = weekly, recebe valores de 0 a 6 (Número correspondente
        //     ao dia da semana que a antecipação será realizada, 0 para domingo, 1 para segunda
        //     e assim por diante). Em caso de auto_advance_type = monthly, recebe valores de
        //     1 a 28 (Número correspondente ao dia do mês que a antecipação será realizada).
        //     Em caso de auto_advance_type = days_after_payment, recebe valores de 1 a 30 (Número
        //     de dias após o pagamento em que a antecipação será realizada)
        [JsonProperty("auto_advance_option")]
        public string AutoAdvanceOption { get; set; }

        //
        // Resumen:
        //     Configurações de Boleto Bancário
        [JsonProperty("bank_slip")]
        [IsClass]
        public BankSlipOptions BankSlipOptions { get; set; }

        //
        // Resumen:
        //     Configurações de Cartão de Crédito
        [JsonProperty("credit_card")]
        [IsClass]
        public CreditCardOptions CreditCardOptions { get; set; }
    }

}
