using Moralar.UtilityFramework.Services.Iugu.Core.Entity;
using Moralar.UtilityFramework.Services.Iugu.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Request
{
    public class SubscriptionRequestMessage : IuguBaseErrors
    {
        //
        // Resumen:
        //     ID do Cliente
        [JsonProperty("customer_id")]
        public string CustomerId { get; set; }

        //
        // Resumen:
        //     Identificador do Plano. Só é enviado para assinaturas que não são credits_based
        [JsonProperty("plan_identifier")]
        public string PlanId { get; set; }

        //
        // Resumen:
        //     Data de Expiração (Também é a data da próxima cobrança)
        [JsonProperty("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        //
        // Resumen:
        //     Apenas Cria a Assinatura se a Cobrança for bem sucedida. Isso só funciona caso
        //     o cliente já tenha uma forma de pagamento padrão cadastrada
        [JsonProperty("only_on_charge_success ")]
        public bool? OnlyOnChargeSuccess { get; set; }

        //
        // Resumen:
        //     Método de pagamento que será disponibilizado para as Faturas desta Assinatura
        //     (all, credit_card ou bank_slip). Obs: Dependendo do valor, este atributo será
        //     herdado, pois a prioridade é herdar o valor atribuído ao Plano desta Assinatura;
        //     Caso este esteja atribuído o valor ‘all’, o sistema considerará o payable_with
        //     da Assinatura; se não, o sistema considerará o payable_with do Plano
        [JsonProperty("payable_with ")]
        public string PayableWith { get; set; }

        //
        // Resumen:
        //     É uma assinatura baseada em créditos
        [JsonProperty("credits_based ")]
        public bool? IsCreditBased { get; set; }

        //
        // Resumen:
        //     Preço em centavos da recarga para assinaturas baseadas em crédito
        [JsonProperty("price_cents ")]
        public int? PriceCents { get; set; }

        //
        // Resumen:
        //     Quantidade de créditos adicionados a cada ciclo, só enviado para assinaturas
        //     credits_based
        [JsonProperty("credits_cycle")]
        public int? CreditsCycle { get; set; }

        //
        // Resumen:
        //     Quantidade de créditos que ativa o ciclo, por ex: Efetuar cobrança cada vez que
        //     a assinatura tenha apenas 1 crédito sobrando. Esse 1 crédito é o credits_min
        [JsonProperty("credits_min ")]
        public int? CreditsMin { get; set; }

        //
        // Resumen:
        //     Itens de Assinatura, sendo que estes podem ser recorrentes ou de cobrança única
        [JsonProperty("subitems")]
        public List<SubscriptionSubitem> Subitems { get; set; }

        //
        // Resumen:
        //     Variáveis Personalizadas
        [JsonProperty("custom_variables")]
        public List<CustomVariables> CustomVariables { get; set; }
    }
}
