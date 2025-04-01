using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class ItemAdvance
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("scheduled_date")]
        public string ScheduledDate { get; set; }

        [JsonProperty("invoice_id")]
        public string InvoiceId { get; set; }

        [JsonProperty("customer_ref")]
        public string CustomerRef { get; set; }

        [JsonProperty("total")]
        public string Total { get; set; }

        [JsonProperty("taxes")]
        public string Taxes { get; set; }

        [JsonProperty("client_share")]
        public string ClientShare { get; set; }

        [JsonProperty("commission")]
        public string Commission { get; set; }

        [JsonProperty("number_of_installments")]
        public long NumberOfInstallments { get; set; }

        [JsonProperty("installment")]
        public long Installment { get; set; }
    }
}
