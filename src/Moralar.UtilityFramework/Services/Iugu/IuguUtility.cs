
using Microsoft.AspNetCore.Http;
using Moralar.UtilityFramework.Services.Iugu.Core.Entity;
using Moralar.UtilityFramework.Services.Iugu.Core.Models;
using Moralar.UtilityFramework.Services.Iugu.Core.Response;
using Moralar.UtilityFramework.Services.Iugu.Core;
using Newtonsoft.Json;
using RestSharp;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Moralar.UtilityFramework.Application.Core;

namespace Moralar.UtilityFramework.Services.Iugu
{
    public static class IuguUtility
    {
        public static string RemoveOperacao(this string conta, string bank)
        {
            bank = bank?.ToLower()?.TrimSpaces();
            if (!string.IsNullOrEmpty(conta) && (bank == "caixaeconômica" || bank == "104") && (conta.StartsWith("013") || conta.StartsWith("001")) && conta.Length > 3)
            {
                return conta?.Substring(3);
            }

            return conta;
        }

        public static double ToReal(this int cent)
        {
            return (double)cent / 100.0;
        }

        public static FeeViewModel CalculeFees(string invoiceId, double value, double feeClient, double feeMega, double feeIugu = 2.51, double feeAdvance = 2.5, bool hasAdvance = false, string apiToken = null)
        {
            FeeViewModel feeViewModel = new FeeViewModel();
            IuguInvoiceResponseMessage iuguInvoiceResponseMessage = null;
            try
            {
                IuguService iuguService = new IuguService();
                if (!string.IsNullOrEmpty(invoiceId))
                {
                    iuguInvoiceResponseMessage = iuguService.GetFatura(invoiceId, apiToken);
                }

                if (!string.IsNullOrEmpty(invoiceId) && iuguInvoiceResponseMessage == null)
                {
                    throw new NullReferenceException("Fatura não encontrada");
                }

                if (!hasAdvance && iuguInvoiceResponseMessage != null && iuguInvoiceResponseMessage.FinancialReturnDates != null && iuguInvoiceResponseMessage.FinancialReturnDates.Count > 0 && iuguInvoiceResponseMessage.FinancialReturnDates[0].Advanced == true)
                {
                    hasAdvance = true;
                }

                if (iuguInvoiceResponseMessage != null)
                {
                    double num = iuguInvoiceResponseMessage.TotalCents.GetValueOrDefault().ToReal();
                    value = ((value < num) ? num : value);
                }

                if (iuguInvoiceResponseMessage != null && iuguInvoiceResponseMessage.TaxesPaidCents.HasValue)
                {
                    feeViewModel.IuguValue = iuguInvoiceResponseMessage.TaxesPaidCents.GetValueOrDefault().ToReal();
                }
                else
                {
                    feeViewModel.IuguValue = value.GetValueOfPercent(feeIugu).NotAround();
                }

                if (iuguInvoiceResponseMessage != null && iuguInvoiceResponseMessage.AdvanceFeeCents.HasValue)
                {
                    feeViewModel.IuguAdvanceValue = iuguInvoiceResponseMessage.AdvanceFeeCents.GetValueOrDefault().ToReal();
                }
                else
                {
                    feeViewModel.IuguAdvanceValue = ((!hasAdvance) ? 0.0 : value.GetValueOfPercent(feeAdvance).NotAround());
                }

                feeViewModel.IuguFeesValue = (feeViewModel.IuguValue + feeViewModel.IuguAdvanceValue).NotAround();
                feeViewModel.ClientValue = value.GetValueOfPercent(feeClient).NotAround();
                feeViewModel.MegaValue = value.GetValueOfPercent(feeMega).NotAround();
                feeViewModel.GrossValue = value;
                feeViewModel.TotalFeesValue = (feeViewModel.MegaValue + feeViewModel.ClientValue + feeViewModel.IuguValue + feeViewModel.IuguAdvanceValue).NotAround();
                feeViewModel.NetValue = (value - feeViewModel.TotalFeesValue).NotAround();
            }
            catch (Exception ex)
            {
                feeViewModel.Error = true;
                feeViewModel.ErrorMessage = $"{ex.InnerException} {ex.Message}".Trim();
            }

            return feeViewModel;
        }

        public static IuguBaseMarketPlace MapErrorDataBank<T>(this IuguBaseMarketPlace response, T responseError) where T : IuguBaseErrors
        {
            try
            {
                response.Error = responseError.Error;
                response.HasError = true;
                response.MessageError = responseError.MessageError;
            }
            catch (Exception ex)
            {
                response.Error = new
                {
                    error = $"{ex.InnerException} {ex.Message}".Trim()
                };
                response.HasError = true;
                response.MessageError = "Ocorreu um erro inesperado";
            }

            return response;
        }

        public static string GetBankName(this string bankCode)
        {
            string text = bankCode?.OnlyNumbers();
            if (!string.IsNullOrEmpty(bankCode) && text.Length > 0)
            {
                return text switch
                {
                    "341" => "Itaú",
                    "237" => "Bradesco",
                    "104" => "Caixa Econômica",
                    "001" => "Banco do Brasil",
                    "033" => "Santander",
                    "041" => "Banrisul",
                    "748" => "Sicredi",
                    "756" => "Sicoob",
                    "077" => "Inter",
                    "070" => "BRB",
                    "085" => "Via Credi",
                    "655" => "Neon",
                    "260" => "Nubank",
                    "290" => "Pagseguro",
                    "212" => "Banco Original",
                    "422" => "Safra",
                    "364" => "Gerencianet Pagamentos",
                    "136" => "Unicred",
                    "021" => "Banestes",
                    "746" => "Modal",
                    _ => bankCode,
                };
            }

            return bankCode;
        }

        //
        // Resumen:
        //     gera response de erro
        //
        // Parámetros:
        //   response:
        public static string MapErrors(this IRestResponse response)
        {
            switch (response.StatusCode)
            {
                case (HttpStatusCode)422:
                    return JsonConvert.DeserializeObject<IuguErrors422>(response.Content).Errors.FirstOrDefault().Value.FirstOrDefault();
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.NotFound:
                    return JsonConvert.DeserializeObject<IuguError>(response.Content)?.Errors;
                case (HttpStatusCode)0:
                    return "Não foi possivel estabelecer uma conexão com o servidor";
                default:
                    return null;
            }
        }

        //
        // Resumen:
        //     gera response de erro
        //
        // Parámetros:
        //   response:
        //
        //   showContent:
        public static IuguBaseErrors IuguMapErrors<T>(this IRestResponse response, bool showContent) where T : IuguBaseErrors
        {
            IuguBaseErrors iuguBaseErrors = new IuguBaseErrors();
            if (showContent)
            {
                iuguBaseErrors.Content = response.Content;
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                case (HttpStatusCode)422:
                    {
                        IuguErrors422 iuguErrors = null;
                        try
                        {
                            List<string> values = (from Match match in new Regex("(\"\\w+\":?(\".*?\"|\\[\".*?\"\\]))").Matches(response.Content)
                                                   select match.Value.Contains("[") ? match.Value : (match.Value.Replace(":\"", ":[\"") + "]")).ToList();
                            iuguErrors = JsonConvert.DeserializeObject<IuguErrors422>("{\"errors\":{" + string.Join(", ", values) + "}}");
                        }
                        catch (Exception)
                        {
                            if (!string.IsNullOrEmpty(response.Content) && !response.Content.Contains("errors\":"))
                            {
                                response.Content = "{\"errors\":" + response.Content + "}";
                            }

                            iuguErrors = JsonConvert.DeserializeObject<IuguErrors422>(response.Content);
                        }

                        List<PropertyInfo> list = typeof(T).GetProperties().ToList();
                        string[] arrayField = iuguErrors.Errors.FirstOrDefault().Key.Split('.');
                        string text = string.Empty;
                        try
                        {
                            int num = list.Count((PropertyInfo x) => x.GetCustomAttribute<IsClass>() != null);
                            PropertyInfo propertyInfo = list.Find((PropertyInfo x) => x.GetCustomAttribute<JsonPropertyAttribute>() != null && x.GetCustomAttribute<JsonPropertyAttribute>().PropertyName == arrayField[0]);
                            if (propertyInfo == null && num > 0)
                            {
                                List<PropertyInfo> list2 = list.Where((PropertyInfo x) => x.GetCustomAttribute<IsClass>() != null).ToList();
                                for (int i = 0; i < num; i++)
                                {
                                    PropertyInfo propertyInfo2 = list2[i].PropertyType.GetProperties().FirstOrDefault((PropertyInfo x) => x.GetCustomAttribute<JsonPropertyAttribute>().PropertyName == arrayField[0]);
                                    if (propertyInfo2 != null)
                                    {
                                        text = propertyInfo2.GetCustomAttribute<DisplayAttribute>()?.Name ?? propertyInfo2?.Name;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                text = propertyInfo.GetCustomAttribute<DisplayAttribute>()?.Name ?? propertyInfo?.Name;
                            }

                            if (string.IsNullOrEmpty(text))
                            {
                                text = iuguErrors.Errors.FirstOrDefault().Key;
                            }
                        }
                        catch (Exception)
                        {
                            if (string.IsNullOrEmpty(text))
                            {
                                text = iuguErrors.Errors.FirstOrDefault().Key;
                            }
                        }

                        string text2 = (text + " " + iuguErrors.Errors.FirstOrDefault().Value.FirstOrDefault()).Trim();
                        if (!string.IsNullOrEmpty(text2))
                        {
                            text2 = Regex.Replace(text2, "^base", "").Trim();
                        }

                        Dictionary<string, List<string>> errors = iuguErrors.Errors;
                        iuguBaseErrors.Error = errors;
                        iuguBaseErrors.MessageError = text2;
                        iuguBaseErrors.HasError = true;
                        break;
                    }
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.NotFound:
                    try
                    {
                        iuguBaseErrors.MessageError = ((!response.Content.Contains("[") || !response.Content.Contains(":")) ? JsonConvert.DeserializeObject<IuguError>(response.Content)?.Errors : JsonConvert.DeserializeObject<IuguErrorsArray>(response.Content)?.Errors[0]);
                    }
                    catch (Exception)
                    {
                        iuguBaseErrors.MessageError = response.Content;
                    }

                    iuguBaseErrors.HasError = true;
                    break;
                case (HttpStatusCode)0:
                    iuguBaseErrors.MessageError = "Não foi possivel estabelecer uma conexão com o servidor";
                    iuguBaseErrors.HasError = true;
                    break;
            }

            return iuguBaseErrors;
        }

        public static bool SuccessTransaction(this string LR)
        {
            if (!(LR == "00") && !(LR == "000"))
            {
                return LR == "11";
            }

            return true;
        }

        public static IuguBaseErrors IuguMapErrors(this IRestResponse response)
        {
            IuguBaseErrors iuguBaseErrors = new IuguBaseErrors();
            switch (response.StatusCode)
            {
                case (HttpStatusCode)422:
                    {
                        IuguErrors422 iuguErrors = JsonConvert.DeserializeObject<IuguErrors422>(response.Content);
                        string text = $"{iuguErrors.Errors.FirstOrDefault().Key} {iuguErrors.Errors.FirstOrDefault().Value}".Trim();
                        if (!string.IsNullOrEmpty(text))
                        {
                            text = Regex.Replace(text, "^base", "").Trim();
                        }

                        Dictionary<string, List<string>> errors = iuguErrors.Errors;
                        iuguBaseErrors.Error = errors;
                        iuguBaseErrors.MessageError = text;
                        iuguBaseErrors.HasError = true;
                        break;
                    }
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.NotFound:
                    iuguBaseErrors.MessageError = JsonConvert.DeserializeObject<IuguError>(response.Content)?.Errors;
                    iuguBaseErrors.HasError = true;
                    break;
                case (HttpStatusCode)0:
                    iuguBaseErrors.MessageError = "Não foi possivel estabelecer uma conexão com o servidor";
                    iuguBaseErrors.HasError = true;
                    break;
            }

            return new IuguBaseErrors();
        }

        //
        // Resumen:
        //     valida dados bancarios
        //
        // Parámetros:
        //   conta:
        //
        //   banco:
        //     BANCO = NOME
        //
        //   typeAccount:
        //     TIPO DE CONTA
        //
        //   typePerson:
        //     Pessoa Física / Pessoa Júridica
        public static string ValidAccoutBank(this string conta, string banco, string typeAccount, string typePerson = "Pessoa Física")
        {
            conta = conta.RemoveOperacao(banco);
            switch (banco.ToLower())
            {
                case "via credi":
                case "085":
                    return conta.MaskAccountBank("00000000000\\-0", 11);
                case "nubank":
                case "260":
                    return conta.MaskAccountBank("0000000000\\-0", 10);
                case "banrisul":
                case "041":
                case "sicoob":
                case "756":
                case "inter":
                case "077":
                case "brb":
                case "070":
                case "modal":
                case "746":
                    return conta.MaskAccountBank("0000000000\\-0", 9);
                case "banco do brasil":
                case "001":
                case "santander":
                case "033":
                case "caixa econômica":
                case "104":
                case "pagseguro":
                case "290":
                case "safra":
                case "422":
                case "banestes":
                case "021":
                case "unicred":
                case "136":
                case "gerencianet pagamentos do brasil":
                case "364":
                    conta = conta.MaskAccountBank("00000000\\-0", 8);
                    if (typeAccount.Equals("Poupança") && (banco.Equals("Caixa Econômica") || banco.Equals("104")) && conta.Length > 9)
                    {
                        conta = "013" + conta;
                    }

                    if (typeAccount.Equals("Corrente") && (banco.Equals("Caixa Econômica") || banco.Equals("104")) && conta.Length > 9)
                    {
                        conta = "001" + conta;
                    }

                    return conta;
                case "bradesco":
                case "237":
                case "neon":
                case "votorantim":
                case "655":
                case "banco original":
                case "212":
                    return conta.MaskAccountBank("0000000\\-0", 7);
                case "itaú":
                case "341":
                    return conta.MaskAccountBank("00000\\-0", 5);
                case "sicredi":
                case "748":
                    return conta.MaskAccountBank("000000", 5, hasHyphen: false);
                default:
                    return conta.MaskAccountBank("000000000\\-0", 9);
            }
        }

        public static string MaskAccountBank(this string account, string mask, int minLength, bool hasHyphen = true)
        {
            string[] array = account.Split('-');
            try
            {
                if (account.IndexOf("-", StringComparison.Ordinal) == -1)
                {
                    account = Convert.ToUInt64(account).ToString(mask);
                    array = account.Split('-');
                    account = array[0];
                }
                else
                {
                    account = array[0];
                }

                if (account.Length > minLength)
                {
                    account = account.Substring(0, minLength);
                }
                else
                {
                    while (account.Length < minLength)
                    {
                        account = "0" + account.Trim();
                    }
                }

                string text = ((array.Length > 1) ? (account + "-" + array[1]) : (account + "-0"));
                return (!hasHyphen) ? text?.OnlyNumbers()?.Trim() : text?.TrimSpaces();
            }
            catch (Exception)
            {
                return account;
            }
        }

        //
        // Resumen:
        //     VALIDAR AGENCIA
        //
        // Parámetros:
        //   agencia:
        //
        //   banco:
        public static string ValidAgencyBank(this string agencia, string banco)
        {
            switch (banco.ToLower())
            {
                case "banco do brasil":
                case "bradesco":
                case "001":
                case "237":
                    {
                        string[] array = agencia.Split('-');
                        if (array == null || array.Length == 0)
                        {
                            return agencia;
                        }

                        string obj = array[0];
                        string text = ((array.Length > 1 && !string.IsNullOrEmpty(array[1])) ? array[1] : "0");
                        return (obj + "-" + text).TrimSpaces();
                    }
                default:
                    return agencia.OnlyNumbers()?.TrimSpaces();
            }
        }

        //
        // Parámetros:
        //   bank:
        public static string GetCodeBank(this string bank)
        {
            return bank.ToLower() switch
            {
                "bradesco" => "237",
                "banco do brasil" => "001",
                "santander" => "033",
                "caixa econômica" => "104",
                "sicredi" => "748",
                "sicoob" => "756",
                "brb" => "070",
                "inter" => "077",
                "banrisul" => "041",
                "safra" => "422",
                "banco original" => "212",
                "pagseguro" => "290",
                "nubank" => "260",
                "neon" => "655",
                "via credi" => "085",
                "votorantim" => "655",
                "modal" => "746",
                "banestes" => "021",
                "unicred" => "136",
                "gerencianet pagamentos" => "364",
                "itaú" => "341",
                _ => bank,
            };
        }

        //
        // Resumen:
        //     ONLY USE UPDATE DATA BANK
        //
        // Parámetros:
        //   typeAccount:
        public static string GetTypeAccout(this string typeAccount)
        {
            if (typeAccount.ToLower() == "corrente")
            {
                return "cc";
            }

            return "cp";
        }

        //
        // Resumen:
        //     Retorna mensagem de erro de acordo com código da IUGU
        //
        // Parámetros:
        //   codeErro:
        public static string STATUS_LR(string codeErro)
        {
            switch (codeErro)
            {
                case "01":
                    return "Transação referida pelo emissor";
                case "03":
                    return "Não foi encontrada a transação";
                case "04":
                    return "Cartão com restrição";
                case "05":
                case "60":
                    return "Transação não autorizada";
                case "06":
                case "96":
                case "76":
                    return "Tente novamente";
                case "07":
                case "41":
                case "62":
                case "63":
                    return "Cartão com restrição";
                case "08":
                    return "Código de segurança inválido";
                case "10 ":
                    return "Não é permitido o envio do cartão";
                case "12":
                case "82":
                    return "Transação inválida";
                case "13":
                    return "Valor inválido";
                case "14":
                    return "Cartão inválido";
                case "15":
                    return "Emissor inválido";
                case "51":
                    return "Saldo insuficiente";
                case "54":
                    return "Cartão vencido";
                case "78":
                    return "Cartão não foi desbloqueado pelo portador";
                case "99":
                    return "Sistema do banco temporariamente fora de operação";
                case "58":
                    return "Transação não permitida";
                case "57":
                    return "Transação não permitida ou não autorizada";
                case "91":
                    return "Banco indisponível";
                case "81":
                    return "Transação negada";
                default:
                    return null;
            }
        }

        public static IuguTriggerModel SetAllProperties(this IuguTriggerModel model, IFormCollection form)
        {
            if (form == null)
            {
                return model;
            }

            string text = form["data[amount]"];
            int.TryParse(form["data[number_of_installments]"], out var result);
            double.TryParse(text?.Replace(",", "").Replace(".", ","), out var result2);
            model.Data.AccountId = form["data[account_id]"];
            model.Data.SubscriptionId = form["data[subscription_id]"];
            model.Data.NumberOfInstallments = result;
            model.Data.CustomerName = form["data[customer_name]"];
            model.Data.CustomerEmail = form["data[customer_email]"];
            model.Data.ExpiresAt = form["data[expires_at]"];
            model.Data.PlanIdentifier = form["data[plan_identifier]"];
            model.Data.ChargeLimitCents = form["data[charge_limit_cents]"];
            model.Data.Amount = result2;
            return model;
        }

        public static Dictionary<string, TValue> ToDictionary<TValue>(this object obj)
        {
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, TValue>>(JsonConvert.SerializeObject(obj));
            }
            catch (Exception)
            {
                return new Dictionary<string, TValue>();
            }
        }

        public static bool CheckAlreadyVerified(this IDictionary<string, List<string>> dictionary)
        {
            try
            {
                return dictionary.Any((KeyValuePair<string, List<string>> x) => x.Value.Any((string y) => y.Contains("conta já verificada")));
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
