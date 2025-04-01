using System.Globalization;
using System.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Services.Iugu.Core.Entity;
using Moralar.UtilityFramework.Services.Iugu.Core.Interface;
using Moralar.UtilityFramework.Services.Iugu.Core.Models;
using Moralar.UtilityFramework.Services.Iugu.Core.Request;
using Moralar.UtilityFramework.Services.Iugu.Core.Response;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.UtilityFramework.Services.Iugu.Core
{
    public class IuguService : IIuguCustomerServices, IIuguPaymentMethodService, IIuguChargeServices, IIuguMarketPlaceServices, IIuguService
    {
        private const string BaseUrl = "https://api.iugu.com/v1";

        private readonly IuguCredentials _iuguCredentials;

        private static IConfigurationRoot Configuration { get; set; }

        public IuguService()
        {
            Configuration = Utilities.GetConfigurationRoot();
            _iuguCredentials = Configuration.GetSection("IUGU").Get<IuguCredentials>();
            _iuguCredentials.KeyUsage = (_iuguCredentials.Sandbox ? _iuguCredentials.TestKey : _iuguCredentials.LiveKey);
        }

        //
        // Resumen:
        //     SAVE CLIENT
        //
        // Parámetros:
        //   customer:
        public IuguCustomer SaveClient(IuguCustomerCreated customer)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/customers")
                {
                    Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.POST);
                string value = JsonConvert.SerializeObject(customer);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguCustomer> result = client.Execute<IuguCustomer>(restRequest).Result;
                IuguCustomer iuguCustomer = result.Data ?? new IuguCustomer();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguCustomerCreated>(_iuguCredentials.ShowContent);
                iuguCustomer.Error = iuguBaseErrors.Error;
                iuguCustomer.MessageError = iuguBaseErrors.MessageError;
                iuguCustomer.HasError = iuguBaseErrors.HasError;
                return iuguCustomer;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorre um erro", innerException);
            }
        }

        //
        // Resumen:
        //     UPDATE NAME/EMAIL CLIENT IUGU
        //
        // Parámetros:
        //   customer:
        public IuguCustomer UpdateClient(IuguCustomerCreated customer)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/customers")
                {
                    Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.PUT);
                string value = JsonConvert.SerializeObject(new
                {
                    name = customer.Name,
                    email = customer.Email
                });
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguCustomer> result = client.Execute<IuguCustomer>(restRequest).Result;
                IuguCustomer iuguCustomer = result.Data ?? new IuguCustomer();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguCustomerCreated>(_iuguCredentials.ShowContent);
                iuguCustomer.Error = iuguBaseErrors.Error;
                iuguCustomer.MessageError = iuguBaseErrors.MessageError;
                iuguCustomer.HasError = iuguBaseErrors.HasError;
                return iuguCustomer;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorre um erro", innerException);
            }
        }

        public IuguCustomer SetDefaultCartao(string clientId, string cardId, string cpfCnpj)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/customers/" + clientId)
                {
                    Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.PUT);
                string value = JsonConvert.SerializeObject(new
                {
                    default_payment_method_id = cardId,
                    cpf_cnpj = cpfCnpj
                });
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguCustomer> result = client.Execute<IuguCustomer>(restRequest).Result;
                IuguCustomer iuguCustomer = result.Data ?? new IuguCustomer();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguCustomer>(_iuguCredentials.ShowContent);
                iuguCustomer.Error = iuguBaseErrors.Error;
                iuguCustomer.MessageError = iuguBaseErrors.MessageError;
                iuguCustomer.HasError = iuguBaseErrors.HasError;
                return iuguCustomer;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public IuguCustomer SetDefaultCartao(string clientId, string cardId)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/customers/" + clientId)
                {
                    Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.PUT);
                string value = JsonConvert.SerializeObject(new
                {
                    default_payment_method_id = cardId
                });
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguCustomer> result = client.Execute<IuguCustomer>(restRequest).Result;
                IuguCustomer iuguCustomer = result.Data ?? new IuguCustomer();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguCustomer>(_iuguCredentials.ShowContent);
                iuguCustomer.Error = iuguBaseErrors.Error;
                iuguCustomer.MessageError = iuguBaseErrors.MessageError;
                iuguCustomer.HasError = iuguBaseErrors.HasError;
                return iuguCustomer;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguCustomer> SaveClientAsync(IuguCustomerCreated customer)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/customers")
                {
                    Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.POST);
                string value = JsonConvert.SerializeObject(customer);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguCustomer> obj = await client.Execute<IuguCustomer>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguCustomer iuguCustomer = obj.Data ?? new IuguCustomer();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguCustomerCreated>(_iuguCredentials.ShowContent);
                iuguCustomer.Error = iuguBaseErrors.Error;
                iuguCustomer.MessageError = iuguBaseErrors.MessageError;
                iuguCustomer.HasError = iuguBaseErrors.HasError;
                return iuguCustomer;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorre um erro", innerException);
            }
        }

        public async Task<IuguCustomer> UpdateClientAsync(IuguCustomerCreated customer)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/customers")
                {
                    Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.PUT);
                string value = JsonConvert.SerializeObject(new
                {
                    name = customer.Name,
                    email = customer.Email
                });
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguCustomer> obj = await client.Execute<IuguCustomer>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguCustomer iuguCustomer = obj.Data ?? new IuguCustomer();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguCustomerCreated>(_iuguCredentials.ShowContent);
                iuguCustomer.Error = iuguBaseErrors.Error;
                iuguCustomer.MessageError = iuguBaseErrors.MessageError;
                iuguCustomer.HasError = iuguBaseErrors.HasError;
                return iuguCustomer;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorre um erro", innerException);
            }
        }

        public async Task DeleteClientAsync(string id)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/customers/" + id)
                {
                    Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "")
                };
                RestRequest request = new RestRequest(Method.DELETE);
                RestResponse restResponse = await client.Execute(request).ConfigureAwait(continueOnCapturedContext: false);
                if (restResponse.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Erro ao remover cliente :  " + restResponse.Content);
                }
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorre um erro", innerException);
            }
        }

        public async Task<IuguCustomer> SetDefaultCartaoAsync(string clientId, string cardId, string cpfCnpj)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/customers/" + clientId)
                {
                    Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.PUT);
                string value = JsonConvert.SerializeObject(new
                {
                    default_payment_method_id = cardId,
                    cpf_cnpj = cpfCnpj
                });
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguCustomer> obj = await client.Execute<IuguCustomer>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguCustomer iuguCustomer = obj.Data ?? new IuguCustomer();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguCustomer>(_iuguCredentials.ShowContent);
                iuguCustomer.Error = iuguBaseErrors.Error;
                iuguCustomer.MessageError = iuguBaseErrors.MessageError;
                iuguCustomer.HasError = iuguBaseErrors.HasError;
                return iuguCustomer;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguCustomer> SetDefaultCartaoAsync(string clientId, string cardId)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/customers/" + clientId)
                {
                    Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.PUT);
                string value = JsonConvert.SerializeObject(new
                {
                    default_payment_method_id = cardId
                });
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguCustomer> obj = await client.Execute<IuguCustomer>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguCustomer iuguCustomer = obj.Data ?? new IuguCustomer();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguCustomer>(_iuguCredentials.ShowContent);
                iuguCustomer.Error = iuguBaseErrors.Error;
                iuguCustomer.MessageError = iuguBaseErrors.MessageError;
                iuguCustomer.HasError = iuguBaseErrors.HasError;
                return iuguCustomer;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     Remover um cliente
        //
        // Parámetros:
        //   id:
        public void DeleteClient(string id)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/customers/" + id)
                {
                    Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "")
                };
                RestRequest request = new RestRequest(Method.DELETE);
                RestResponse result = client.Execute(request).Result;
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Erro ao remover cliente :  " + result.Content);
                }
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorre um erro", innerException);
            }
        }

        //
        // Resumen:
        //     GERATE TOKEN FOR CREDIT CARD
        //
        // Parámetros:
        //   model:
        public IuguPaymentMethodToken SaveCreditCard(IuguPaymentMethodToken model)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/payment_token");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "");
                model.AccountId = _iuguCredentials.AccoundId;
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguPaymentMethodToken> result = restClient.Execute<IuguPaymentMethodToken>(restRequest).Result;
                IuguPaymentMethodToken iuguPaymentMethodToken = result.Data ?? new IuguPaymentMethodToken();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguPaymentMethodToken>(_iuguCredentials.ShowContent);
                iuguPaymentMethodToken.Error = iuguBaseErrors.Error;
                iuguPaymentMethodToken.MessageError = iuguBaseErrors.MessageError;
                iuguPaymentMethodToken.HasError = iuguBaseErrors.HasError;
                return iuguPaymentMethodToken;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     REGISTER CREDIT CARD FOR PROFILE
        //
        // Parámetros:
        //   model:
        //
        //   clienteId:
        public IuguCreditCard LinkCreditCardClient(IuguCustomerPaymentMethod model, string clienteId)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/customers/" + clienteId + "/payment_methods");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "");
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguCreditCard> result = restClient.Execute<IuguCreditCard>(restRequest).Result;
                IuguCreditCard iuguCreditCard = result.Data ?? new IuguCreditCard();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguCustomerPaymentMethod>(_iuguCredentials.ShowContent);
                iuguCreditCard.Error = iuguBaseErrors.Error;
                iuguCreditCard.MessageError = iuguBaseErrors.MessageError;
                iuguCreditCard.HasError = iuguBaseErrors.HasError;
                return iuguCreditCard;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     REMOVE UM CART�O DO CLIENTE
        //
        // Parámetros:
        //   clientId:
        //
        //   cardId:
        public IuguCreditCard RemoverCredCard(string clientId, string cardId)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/customers/" + clientId + "/payment_methods/" + cardId);
                RestRequest request = new RestRequest(Method.DELETE);
                restClient.Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "");
                IRestResponse<IuguCreditCard> result = restClient.Execute<IuguCreditCard>(request).Result;
                IuguCreditCard iuguCreditCard = result.Data ?? new IuguCreditCard();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguCreditCard>(_iuguCredentials.ShowContent);
                iuguCreditCard.Error = iuguBaseErrors.Error;
                iuguCreditCard.MessageError = iuguBaseErrors.MessageError;
                iuguCreditCard.HasError = iuguBaseErrors.HasError;
                return iuguCreditCard;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguPaymentMethodToken> SaveCreditCardAsync(IuguPaymentMethodToken model)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/payment_token");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "");
                model.AccountId = _iuguCredentials.AccoundId;
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguPaymentMethodToken> obj = await restClient.Execute<IuguPaymentMethodToken>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguPaymentMethodToken iuguPaymentMethodToken = obj.Data ?? new IuguPaymentMethodToken();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguPaymentMethodToken>(_iuguCredentials.ShowContent);
                iuguPaymentMethodToken.Error = iuguBaseErrors.Error;
                iuguPaymentMethodToken.MessageError = iuguBaseErrors.MessageError;
                iuguPaymentMethodToken.HasError = iuguBaseErrors.HasError;
                return iuguPaymentMethodToken;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IEnumerable<IuguCreditCard>> ListarCredCardsAsync(string clientId)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/customers/" + clientId + "/payment_methods");
                RestRequest request = new RestRequest(Method.GET);
                restClient.Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "");
                return (await restClient.Execute<IEnumerable<IuguCreditCard>>(request).ConfigureAwait(continueOnCapturedContext: false)).Data;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguCreditCard> LinkCreditCardClientAsync(IuguCustomerPaymentMethod model, string clienteId)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/customers/" + clienteId + "/payment_methods");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "");
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguCreditCard> obj = await restClient.Execute<IuguCreditCard>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguCreditCard iuguCreditCard = obj.Data ?? new IuguCreditCard();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguCustomerPaymentMethod>(_iuguCredentials.ShowContent);
                iuguCreditCard.Error = iuguBaseErrors.Error;
                iuguCreditCard.MessageError = iuguBaseErrors.MessageError;
                iuguCreditCard.HasError = iuguBaseErrors.HasError;
                return iuguCreditCard;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguCreditCard> BuscarCredCardsAsync(string clientId, string cardId)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/customers/" + clientId + "/payment_methods/" + cardId);
                RestRequest request = new RestRequest(Method.GET);
                restClient.Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "");
                IRestResponse<IuguCreditCard> obj = await restClient.Execute<IuguCreditCard>(request).ConfigureAwait(continueOnCapturedContext: false);
                IuguCreditCard iuguCreditCard = obj.Data ?? new IuguCreditCard();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguCreditCard>(_iuguCredentials.ShowContent);
                iuguCreditCard.Error = iuguBaseErrors.Error;
                iuguCreditCard.MessageError = iuguBaseErrors.MessageError;
                iuguCreditCard.HasError = iuguBaseErrors.HasError;
                return iuguCreditCard;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguCreditCard> RemoverCredCardAsync(string clientId, string cardId)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/customers/" + clientId + "/payment_methods/" + cardId);
                RestRequest request = new RestRequest(Method.DELETE);
                restClient.Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "");
                IRestResponse<IuguCreditCard> obj = await restClient.Execute<IuguCreditCard>(request).ConfigureAwait(continueOnCapturedContext: false);
                IuguCreditCard iuguCreditCard = obj.Data ?? new IuguCreditCard();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguCreditCard>(_iuguCredentials.ShowContent);
                iuguCreditCard.Error = iuguBaseErrors.Error;
                iuguCreditCard.MessageError = iuguBaseErrors.MessageError;
                iuguCreditCard.HasError = iuguBaseErrors.HasError;
                return iuguCreditCard;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     LISTA OS CARTÕES DE UM CLIENTE
        //
        // Parámetros:
        //   clientId:
        public IEnumerable<IuguCreditCard> ListarCredCards(string clientId)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/customers/" + clientId + "/payment_methods");
                RestRequest request = new RestRequest(Method.GET);
                restClient.Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "");
                return restClient.Execute<IEnumerable<IuguCreditCard>>(request).Result.Data;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     BUSCA CARTÃO NA BASE DA IUGU
        //
        // Parámetros:
        //   clientId:
        //
        //   cardId:
        public IuguCreditCard BuscarCredCards(string clientId, string cardId)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/customers/" + clientId + "/payment_methods/" + cardId);
                RestRequest request = new RestRequest(Method.GET);
                restClient.Authenticator = new HttpBasicAuthenticator(_iuguCredentials.KeyUsage, "");
                IRestResponse<IuguCreditCard> result = restClient.Execute<IuguCreditCard>(request).Result;
                IuguCreditCard iuguCreditCard = result.Data ?? new IuguCreditCard();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguCreditCard>(_iuguCredentials.ShowContent);
                iuguCreditCard.Error = iuguBaseErrors.Error;
                iuguCreditCard.MessageError = iuguBaseErrors.MessageError;
                iuguCreditCard.HasError = iuguBaseErrors.HasError;
                return iuguCreditCard;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguInvoiceResponseMessage> GerarFaturaAsync(InvoiceRequestMessage model, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/invoices");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguInvoiceResponseMessage> obj = await restClient.Execute<IuguInvoiceResponseMessage>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguInvoiceResponseMessage iuguInvoiceResponseMessage = obj.Data ?? new IuguInvoiceResponseMessage();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<InvoiceRequestMessage>(_iuguCredentials.ShowContent);
                iuguInvoiceResponseMessage.Error = iuguBaseErrors.Error;
                iuguInvoiceResponseMessage.MessageError = iuguBaseErrors.MessageError;
                iuguInvoiceResponseMessage.HasError = iuguBaseErrors.HasError;
                return iuguInvoiceResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public IuguInvoiceResponseMessage GerarFatura(InvoiceRequestMessage model, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/invoices");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguInvoiceResponseMessage> result = restClient.Execute<IuguInvoiceResponseMessage>(restRequest).Result;
                IuguInvoiceResponseMessage iuguInvoiceResponseMessage = result.Data ?? new IuguInvoiceResponseMessage();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<InvoiceRequestMessage>(_iuguCredentials.ShowContent);
                iuguInvoiceResponseMessage.Error = iuguBaseErrors.Error;
                iuguInvoiceResponseMessage.MessageError = iuguBaseErrors.MessageError;
                iuguInvoiceResponseMessage.HasError = iuguBaseErrors.HasError;
                return iuguInvoiceResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguInvoiceResponseMessage> CancelarFaturaAsync(string invoiceId, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/invoices/" + invoiceId + "/cancel");
                RestRequest restRequest = new RestRequest(Method.PUT);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                IRestResponse<IuguInvoiceResponseMessage> obj = await restClient.Execute<IuguInvoiceResponseMessage>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguInvoiceResponseMessage iuguInvoiceResponseMessage = obj.Data ?? new IuguInvoiceResponseMessage();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguInvoiceResponseMessage>(_iuguCredentials.ShowContent);
                iuguInvoiceResponseMessage.Error = iuguBaseErrors.Error;
                iuguInvoiceResponseMessage.MessageError = iuguBaseErrors.MessageError;
                iuguInvoiceResponseMessage.HasError = iuguBaseErrors.HasError;
                return iuguInvoiceResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public IuguInvoiceResponseMessage CancelarFatura(string invoiceId, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/invoices/" + invoiceId + "/cancel");
                RestRequest restRequest = new RestRequest(Method.PUT);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                IRestResponse<IuguInvoiceResponseMessage> result = restClient.Execute<IuguInvoiceResponseMessage>(restRequest).Result;
                IuguInvoiceResponseMessage iuguInvoiceResponseMessage = result.Data ?? new IuguInvoiceResponseMessage();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguInvoiceResponseMessage>(_iuguCredentials.ShowContent);
                iuguInvoiceResponseMessage.Error = iuguBaseErrors.Error;
                iuguInvoiceResponseMessage.MessageError = iuguBaseErrors.MessageError;
                iuguInvoiceResponseMessage.HasError = iuguBaseErrors.HasError;
                return iuguInvoiceResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     TRANSA��O DIRETA OU COM CART�O DE CREDITO SALVO
        //
        // Parámetros:
        //   model:
        //
        //   clienteId:
        //
        //   idCartao:
        //
        //   token:
        //
        //   apiToken:
        //     em caso de subconta
        public IuguChargeResponse TransacaoCrediCard(IuguChargeRequest model, string clienteId, string idCartao = null, string token = null, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/charge");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                object value = (string.IsNullOrEmpty(idCartao) ? ((object)new
                {
                    api_token = ((!string.IsNullOrEmpty(apiToken)) ? apiToken : _iuguCredentials.KeyUsage),
                    email = model.Email,
                    token = token,
                    Months = (model.Months ?? 1),
                    customer_id = clienteId,
                    items = model.InvoiceItems
                }) : ((object)new
                {
                    api_token = ((!string.IsNullOrEmpty(apiToken)) ? apiToken : _iuguCredentials.KeyUsage),
                    email = model.Email,
                    customer_payment_method_id = idCartao,
                    customer_id = clienteId,
                    Months = (model.Months ?? 1),
                    items = model.InvoiceItems
                }));
                string value2 = JsonConvert.SerializeObject(value);
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value2, ParameterType.RequestBody);
                IRestResponse<IuguChargeResponse> result = restClient.Execute<IuguChargeResponse>(restRequest).Result;
                IuguChargeResponse iuguChargeResponse = result.Data ?? new IuguChargeResponse();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguChargeRequest>(_iuguCredentials.ShowContent);
                iuguChargeResponse.Error = iuguBaseErrors.Error;
                iuguChargeResponse.MessageError = iuguBaseErrors.MessageError;
                iuguChargeResponse.HasError = iuguBaseErrors.HasError || !iuguChargeResponse.Success;
                if (!string.IsNullOrEmpty(iuguChargeResponse?.LR) && !iuguChargeResponse.LR.SuccessTransaction())
                {
                    iuguChargeResponse.MsgLR = IuguUtility.STATUS_LR(iuguChargeResponse.LR);
                    iuguChargeResponse.HasError = true;
                    iuguChargeResponse.MessageError = iuguChargeResponse.MsgLR;
                }

                return iuguChargeResponse;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public IuguInvoiceResponseMessage GetFatura(string invoiceId, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/invoices/" + invoiceId);
                RestRequest restRequest = new RestRequest(Method.GET);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                IRestResponse<IuguInvoiceResponseMessage> result = restClient.Execute<IuguInvoiceResponseMessage>(restRequest).Result;
                IuguInvoiceResponseMessage iuguInvoiceResponseMessage = result.Data ?? new IuguInvoiceResponseMessage();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguInvoiceResponseMessage>(_iuguCredentials.ShowContent);
                iuguInvoiceResponseMessage.Error = iuguBaseErrors.Error;
                iuguInvoiceResponseMessage.MessageError = iuguBaseErrors.MessageError;
                iuguInvoiceResponseMessage.HasError = iuguBaseErrors.HasError;
                return iuguInvoiceResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     ESTORNAR VALOR COBRADO NO CART�O DE CREDITO
        //
        // Parámetros:
        //   invoiceId:
        //     ID DA FATURA
        public IuguChargeResponse EstornarFatura(string invoiceId, int? refundCents = null, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/invoices/" + invoiceId + "/refund");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                if (refundCents.HasValue && refundCents > 1)
                {
                    string value = JsonConvert.SerializeObject(new IuguRefundModel
                    {
                        PartialValueRefundCents = refundCents.GetValueOrDefault()
                    });
                    restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                }

                IRestResponse<IuguChargeResponse> result = restClient.Execute<IuguChargeResponse>(restRequest).Result;
                IuguChargeResponse iuguChargeResponse = result.Data ?? new IuguChargeResponse();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguChargeResponse>(_iuguCredentials.ShowContent);
                iuguChargeResponse.Error = iuguBaseErrors.Error;
                iuguChargeResponse.MessageError = iuguBaseErrors.MessageError;
                iuguChargeResponse.HasError = iuguBaseErrors.HasError || !iuguChargeResponse.Success;
                if (!string.IsNullOrEmpty(iuguChargeResponse?.LR) && !iuguChargeResponse.LR.SuccessTransaction())
                {
                    iuguChargeResponse.MsgLR = IuguUtility.STATUS_LR(iuguChargeResponse.LR);
                    iuguChargeResponse.HasError = true;
                    iuguChargeResponse.MessageError = iuguChargeResponse.MsgLR;
                }

                return result.Data;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     GERAR ASSINATURA
        //
        // Parámetros:
        //   apiToken:
        //
        //   model:
        public SubscriptionResponseMessage GerarAssinatura(SubscriptionRequestMessage model, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/subscriptions")
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.POST);
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<SubscriptionResponseMessage> result = client.Execute<SubscriptionResponseMessage>(restRequest).Result;
                SubscriptionResponseMessage subscriptionResponseMessage = result.Data ?? new SubscriptionResponseMessage();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<SubscriptionRequestMessage>(_iuguCredentials.ShowContent);
                subscriptionResponseMessage.Error = iuguBaseErrors.Error;
                subscriptionResponseMessage.MessageError = iuguBaseErrors.MessageError;
                subscriptionResponseMessage.HasError = iuguBaseErrors.HasError;
                return subscriptionResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public SubscriptionResponseMessage BuscarAssinatura(string subscriptionId, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/subscriptions/" + subscriptionId)
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest request = new RestRequest(Method.GET);
                IRestResponse<SubscriptionResponseMessage> result = client.Execute<SubscriptionResponseMessage>(request).Result;
                SubscriptionResponseMessage subscriptionResponseMessage = result.Data ?? new SubscriptionResponseMessage();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<SubscriptionResponseMessage>(_iuguCredentials.ShowContent);
                subscriptionResponseMessage.Error = iuguBaseErrors.Error;
                subscriptionResponseMessage.MessageError = iuguBaseErrors.MessageError;
                subscriptionResponseMessage.HasError = iuguBaseErrors.HasError;
                return subscriptionResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public SubscriptionResponseMessage RemoverAssinatura(string subscriptionId, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/subscriptions/" + subscriptionId)
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest request = new RestRequest(Method.DELETE);
                IRestResponse<SubscriptionResponseMessage> result = client.Execute<SubscriptionResponseMessage>(request).Result;
                SubscriptionResponseMessage subscriptionResponseMessage = result.Data ?? new SubscriptionResponseMessage();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<SubscriptionResponseMessage>(_iuguCredentials.ShowContent);
                subscriptionResponseMessage.Error = iuguBaseErrors.Error;
                subscriptionResponseMessage.MessageError = iuguBaseErrors.MessageError;
                subscriptionResponseMessage.HasError = iuguBaseErrors.HasError;
                return subscriptionResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public SubscriptionResponseMessage AlterarPlanoDaAssinatura(string subscriptionId, string planIdentifier, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/subscriptions/" + subscriptionId + "/change_plan/" + planIdentifier)
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest request = new RestRequest(Method.POST);
                IRestResponse<SubscriptionResponseMessage> result = client.Execute<SubscriptionResponseMessage>(request).Result;
                SubscriptionResponseMessage subscriptionResponseMessage = result.Data ?? new SubscriptionResponseMessage();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<SubscriptionResponseMessage>(_iuguCredentials.ShowContent);
                subscriptionResponseMessage.Error = iuguBaseErrors.Error;
                subscriptionResponseMessage.MessageError = iuguBaseErrors.MessageError;
                subscriptionResponseMessage.HasError = iuguBaseErrors.HasError;
                return subscriptionResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     ACTIVE SIGNATURE
        //
        // Parámetros:
        //   signatureId:
        //
        //   apiToken:
        public SubscriptionResponseMessage AtivarAsinatura(string signatureId, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/subscriptions/" + signatureId + "/activate");
                RestRequest request = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                IRestResponse<SubscriptionResponseMessage> result = restClient.Execute<SubscriptionResponseMessage>(request).Result;
                SubscriptionResponseMessage subscriptionResponseMessage = result.Data ?? new SubscriptionResponseMessage();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<SubscriptionResponseMessage>(_iuguCredentials.ShowContent);
                subscriptionResponseMessage.Error = iuguBaseErrors.Error;
                subscriptionResponseMessage.MessageError = iuguBaseErrors.MessageError;
                subscriptionResponseMessage.HasError = iuguBaseErrors.HasError;
                return subscriptionResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     CANCELAR ASSINATURA
        //
        // Parámetros:
        //   signatureId:
        //
        //   apiToken:
        public SubscriptionResponseMessage SuspenderAssinatura(string signatureId, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/subscriptions/" + signatureId + "/suspend");
                RestRequest request = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                IRestResponse<SubscriptionResponseMessage> result = restClient.Execute<SubscriptionResponseMessage>(request).Result;
                SubscriptionResponseMessage subscriptionResponseMessage = result.Data ?? new SubscriptionResponseMessage();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<SubscriptionResponseMessage>(_iuguCredentials.ShowContent);
                subscriptionResponseMessage.Error = iuguBaseErrors.Error;
                subscriptionResponseMessage.MessageError = iuguBaseErrors.MessageError;
                subscriptionResponseMessage.HasError = iuguBaseErrors.HasError;
                return subscriptionResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     CRIAR PLANO
        //
        // Parámetros:
        //   model:
        //
        //   apiToken:
        public IuguPlanModel CriarPlano(IuguPlanModel model, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/plans")
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.POST);
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguPlanModel> result = client.Execute<IuguPlanModel>(restRequest).Result;
                IuguPlanModel iuguPlanModel = result.Data ?? new IuguPlanModel();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguPlanModel>(_iuguCredentials.ShowContent);
                iuguPlanModel.Error = iuguBaseErrors.Error;
                iuguPlanModel.MessageError = iuguBaseErrors.MessageError;
                iuguPlanModel.HasError = iuguBaseErrors.HasError;
                return iuguPlanModel;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     ATUALIZAR PLANO
        //
        // Parámetros:
        //   planId:
        //
        //   model:
        //
        //   apiToken:
        public IuguPlanModel UpdatePlano(string planId, IuguPlanModel model, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/plans/" + planId)
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.PUT);
                string value = JsonConvert.SerializeObject(model)?.Replace("\"identifier\":null,", "");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguPlanModel> result = client.Execute<IuguPlanModel>(restRequest).Result;
                IuguPlanModel iuguPlanModel = result.Data ?? new IuguPlanModel();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguPlanModel>(_iuguCredentials.ShowContent);
                iuguPlanModel.Error = iuguBaseErrors.Error;
                iuguPlanModel.MessageError = iuguBaseErrors.MessageError;
                iuguPlanModel.HasError = iuguBaseErrors.HasError;
                return iuguPlanModel;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public void RemoverPlano(string planId, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/plans/" + planId)
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest request = new RestRequest(Method.DELETE);
                if (client.Execute(request).Result.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Ocorreu um erro ao remover o plano");
                }
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguChargeResponse> TransacaoCrediCardAsync(IuguChargeRequest model, string clienteId, string idCartao = null, string token = null, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/charge");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                object value = (string.IsNullOrEmpty(idCartao) ? ((object)new
                {
                    api_token = ((!string.IsNullOrEmpty(apiToken)) ? apiToken : _iuguCredentials.KeyUsage),
                    email = model.Email,
                    token = token,
                    Months = (model.Months ?? 1),
                    customer_id = clienteId,
                    items = model.InvoiceItems
                }) : ((object)new
                {
                    api_token = ((!string.IsNullOrEmpty(apiToken)) ? apiToken : _iuguCredentials.KeyUsage),
                    email = model.Email,
                    customer_payment_method_id = idCartao,
                    customer_id = clienteId,
                    Months = (model.Months ?? 1),
                    items = model.InvoiceItems
                }));
                string value2 = JsonConvert.SerializeObject(value);
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value2, ParameterType.RequestBody);
                IRestResponse<IuguChargeResponse> obj = await restClient.Execute<IuguChargeResponse>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguChargeResponse iuguChargeResponse = obj.Data ?? new IuguChargeResponse();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguChargeRequest>(_iuguCredentials.ShowContent);
                iuguChargeResponse.Error = iuguBaseErrors.Error;
                iuguChargeResponse.MessageError = iuguBaseErrors.MessageError;
                iuguChargeResponse.HasError = iuguBaseErrors.HasError || !iuguChargeResponse.Success;
                if (!string.IsNullOrEmpty(iuguChargeResponse?.LR) && !iuguChargeResponse.LR.SuccessTransaction())
                {
                    iuguChargeResponse.MsgLR = IuguUtility.STATUS_LR(iuguChargeResponse.LR);
                    iuguChargeResponse.HasError = true;
                    iuguChargeResponse.MessageError = iuguChargeResponse.MsgLR;
                }

                return iuguChargeResponse;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguInvoiceResponseMessage> GetFaturaAsync(string invoiceId, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/invoices/" + invoiceId);
                RestRequest restRequest = new RestRequest(Method.GET);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                IRestResponse<IuguInvoiceResponseMessage> obj = await restClient.Execute<IuguInvoiceResponseMessage>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguInvoiceResponseMessage iuguInvoiceResponseMessage = obj.Data ?? new IuguInvoiceResponseMessage();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguInvoiceResponseMessage>(_iuguCredentials.ShowContent);
                iuguInvoiceResponseMessage.Error = iuguBaseErrors.Error;
                iuguInvoiceResponseMessage.MessageError = iuguBaseErrors.MessageError;
                iuguInvoiceResponseMessage.HasError = iuguBaseErrors.HasError;
                return iuguInvoiceResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguChargeResponse> EstornarFaturaAsync(string invoiceId, int? refundCents = null, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/invoices/" + invoiceId + "/refund");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                if (refundCents.HasValue && refundCents > 1)
                {
                    string value = JsonConvert.SerializeObject(new IuguRefundModel
                    {
                        PartialValueRefundCents = refundCents.GetValueOrDefault()
                    });
                    restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                }

                IRestResponse<IuguChargeResponse> obj = await restClient.Execute<IuguChargeResponse>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguChargeResponse iuguChargeResponse = obj.Data ?? new IuguChargeResponse();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguChargeResponse>(_iuguCredentials.ShowContent);
                iuguChargeResponse.Error = iuguBaseErrors.Error;
                iuguChargeResponse.MessageError = iuguBaseErrors.MessageError;
                iuguChargeResponse.HasError = iuguBaseErrors.HasError || !iuguChargeResponse.Success;
                if (!string.IsNullOrEmpty(iuguChargeResponse?.LR) && !iuguChargeResponse.LR.SuccessTransaction())
                {
                    iuguChargeResponse.MsgLR = IuguUtility.STATUS_LR(iuguChargeResponse.LR);
                    iuguChargeResponse.HasError = true;
                    iuguChargeResponse.MessageError = iuguChargeResponse.MsgLR;
                }

                return obj.Data;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<SubscriptionResponseMessage> GerarAssinaturaAsync(SubscriptionRequestMessage model, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/subscriptions")
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.POST);
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<SubscriptionResponseMessage> obj = await client.Execute<SubscriptionResponseMessage>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                SubscriptionResponseMessage subscriptionResponseMessage = obj.Data ?? new SubscriptionResponseMessage();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<SubscriptionRequestMessage>(_iuguCredentials.ShowContent);
                subscriptionResponseMessage.Error = iuguBaseErrors.Error;
                subscriptionResponseMessage.MessageError = iuguBaseErrors.MessageError;
                subscriptionResponseMessage.HasError = iuguBaseErrors.HasError;
                return subscriptionResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<SubscriptionResponseMessage> BuscarAssinaturaAsync(string subscriptionId, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/subscriptions/" + subscriptionId)
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest request = new RestRequest(Method.GET);
                IRestResponse<SubscriptionResponseMessage> obj = await client.Execute<SubscriptionResponseMessage>(request).ConfigureAwait(continueOnCapturedContext: false);
                SubscriptionResponseMessage subscriptionResponseMessage = obj.Data ?? new SubscriptionResponseMessage();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<SubscriptionResponseMessage>(_iuguCredentials.ShowContent);
                subscriptionResponseMessage.Error = iuguBaseErrors.Error;
                subscriptionResponseMessage.MessageError = iuguBaseErrors.MessageError;
                subscriptionResponseMessage.HasError = iuguBaseErrors.HasError;
                return subscriptionResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<SubscriptionResponseMessage> RemoverAssinaturaAsync(string subscriptionId, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/subscriptions/" + subscriptionId)
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest request = new RestRequest(Method.DELETE);
                IRestResponse<SubscriptionResponseMessage> obj = await client.Execute<SubscriptionResponseMessage>(request).ConfigureAwait(continueOnCapturedContext: false);
                SubscriptionResponseMessage subscriptionResponseMessage = obj.Data ?? new SubscriptionResponseMessage();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<SubscriptionResponseMessage>(_iuguCredentials.ShowContent);
                subscriptionResponseMessage.Error = iuguBaseErrors.Error;
                subscriptionResponseMessage.MessageError = iuguBaseErrors.MessageError;
                subscriptionResponseMessage.HasError = iuguBaseErrors.HasError;
                return subscriptionResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<SubscriptionResponseMessage> AlterarPlanoDaAssinaturaAsync(string subscriptionId, string planIdentifier, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/subscriptions/" + subscriptionId + "/change_plan/" + planIdentifier)
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest request = new RestRequest(Method.POST);
                IRestResponse<SubscriptionResponseMessage> obj = await client.Execute<SubscriptionResponseMessage>(request).ConfigureAwait(continueOnCapturedContext: false);
                SubscriptionResponseMessage subscriptionResponseMessage = obj.Data ?? new SubscriptionResponseMessage();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<SubscriptionResponseMessage>(_iuguCredentials.ShowContent);
                subscriptionResponseMessage.Error = iuguBaseErrors.Error;
                subscriptionResponseMessage.MessageError = iuguBaseErrors.MessageError;
                subscriptionResponseMessage.HasError = iuguBaseErrors.HasError;
                return subscriptionResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<SubscriptionResponseMessage> AtivarAsinaturaAsync(string signatureId, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/subscriptions/" + signatureId + "/activate");
                RestRequest request = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                IRestResponse<SubscriptionResponseMessage> obj = await restClient.Execute<SubscriptionResponseMessage>(request).ConfigureAwait(continueOnCapturedContext: false);
                SubscriptionResponseMessage subscriptionResponseMessage = obj.Data ?? new SubscriptionResponseMessage();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<SubscriptionResponseMessage>(_iuguCredentials.ShowContent);
                subscriptionResponseMessage.Error = iuguBaseErrors.Error;
                subscriptionResponseMessage.MessageError = iuguBaseErrors.MessageError;
                subscriptionResponseMessage.HasError = iuguBaseErrors.HasError;
                return subscriptionResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<SubscriptionResponseMessage> SuspenderAssinaturaAsync(string signatureId, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/subscriptions/" + signatureId + "/suspend");
                RestRequest request = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                IRestResponse<SubscriptionResponseMessage> obj = await restClient.Execute<SubscriptionResponseMessage>(request).ConfigureAwait(continueOnCapturedContext: false);
                SubscriptionResponseMessage subscriptionResponseMessage = obj.Data ?? new SubscriptionResponseMessage();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<SubscriptionResponseMessage>(_iuguCredentials.ShowContent);
                subscriptionResponseMessage.Error = iuguBaseErrors.Error;
                subscriptionResponseMessage.MessageError = iuguBaseErrors.MessageError;
                subscriptionResponseMessage.HasError = iuguBaseErrors.HasError;
                return subscriptionResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguPlanModel> CriarPlanoAsync(IuguPlanModel model, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/plans")
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.POST);
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguPlanModel> obj = await client.Execute<IuguPlanModel>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguPlanModel iuguPlanModel = obj.Data ?? new IuguPlanModel();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguPlanModel>(_iuguCredentials.ShowContent);
                iuguPlanModel.Error = iuguBaseErrors.Error;
                iuguPlanModel.MessageError = iuguBaseErrors.MessageError;
                iuguPlanModel.HasError = iuguBaseErrors.HasError;
                return iuguPlanModel;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguPlanModel> UpdatePlanoAsync(string planId, IuguPlanModel model, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/plans/" + planId)
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.PUT);
                string value = JsonConvert.SerializeObject(model)?.Replace("\"identifier\":null,", "");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguPlanModel> obj = await client.Execute<IuguPlanModel>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguPlanModel iuguPlanModel = obj.Data ?? new IuguPlanModel();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguPlanModel>(_iuguCredentials.ShowContent);
                iuguPlanModel.Error = iuguBaseErrors.Error;
                iuguPlanModel.MessageError = iuguBaseErrors.MessageError;
                iuguPlanModel.HasError = iuguBaseErrors.HasError;
                return iuguPlanModel;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task RemoverPlanoAsync(string planId, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/plans/" + planId)
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest request = new RestRequest(Method.DELETE);
                if ((await client.Execute(request).ConfigureAwait(continueOnCapturedContext: false)).StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Ocorreu um erro ao remover o plano");
                }
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public ListIuguAdvanceResponse GetRecebiveis(int? start = null, int? limit = null, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/financial_transaction_requests")
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.GET);
                if (limit.HasValue)
                {
                    restRequest.AddParameter("limit", limit, ParameterType.GetOrPost);
                }

                if (start.HasValue)
                {
                    restRequest.AddParameter("start", start, ParameterType.GetOrPost);
                }

                return client.Execute<ListIuguAdvanceResponse>(restRequest).Result.Data;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<ListIuguAdvanceResponse> GetRecebiveisAsync(int? start = null, int? limit = null, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/financial_transaction_requests")
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.GET);
                if (limit.HasValue)
                {
                    restRequest.AddParameter("limit", limit, ParameterType.GetOrPost);
                }

                if (start.HasValue)
                {
                    restRequest.AddParameter("start", start, ParameterType.GetOrPost);
                }

                return (await client.Execute<ListIuguAdvanceResponse>(restRequest).ConfigureAwait(continueOnCapturedContext: false)).Data;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public IuguAdvanceSimulationResponse SimularAntecipacao(IuguAdvanceRequest model, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/financial_transaction_requests/advance_simulation")
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.POST);
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguAdvanceSimulationResponse> result = client.Execute<IuguAdvanceSimulationResponse>(restRequest).Result;
                IuguAdvanceSimulationResponse iuguAdvanceSimulationResponse = result.Data ?? new IuguAdvanceSimulationResponse();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguAdvanceRequest>(_iuguCredentials.ShowContent);
                iuguAdvanceSimulationResponse.Error = iuguBaseErrors.Error;
                iuguAdvanceSimulationResponse.MessageError = iuguBaseErrors.MessageError;
                iuguAdvanceSimulationResponse.HasError = iuguBaseErrors.HasError;
                return iuguAdvanceSimulationResponse;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguAdvanceSimulationResponse> SimularAntecipacaoAsync(IuguAdvanceRequest model, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/financial_transaction_requests/advance_simulation")
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.POST);
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguAdvanceSimulationResponse> obj = await client.Execute<IuguAdvanceSimulationResponse>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguAdvanceSimulationResponse iuguAdvanceSimulationResponse = obj.Data ?? new IuguAdvanceSimulationResponse();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguAdvanceRequest>(_iuguCredentials.ShowContent);
                iuguAdvanceSimulationResponse.Error = iuguBaseErrors.Error;
                iuguAdvanceSimulationResponse.MessageError = iuguBaseErrors.MessageError;
                iuguAdvanceSimulationResponse.HasError = iuguBaseErrors.HasError;
                return iuguAdvanceSimulationResponse;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public IuguAdvanceSimulationResponse SolicitarAntecipacao(IuguAdvanceRequest model, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/financial_transaction_requests/advance")
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.POST);
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguAdvanceSimulationResponse> result = client.Execute<IuguAdvanceSimulationResponse>(restRequest).Result;
                IuguAdvanceSimulationResponse iuguAdvanceSimulationResponse = result.Data ?? new IuguAdvanceSimulationResponse();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguAdvanceRequest>(_iuguCredentials.ShowContent);
                iuguAdvanceSimulationResponse.Error = iuguBaseErrors.Error;
                iuguAdvanceSimulationResponse.MessageError = iuguBaseErrors.MessageError;
                iuguAdvanceSimulationResponse.HasError = iuguBaseErrors.HasError;
                return iuguAdvanceSimulationResponse;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguAdvanceSimulationResponse> SolicitarAntecipacaoAsync(IuguAdvanceRequest model, string apiToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://api.iugu.com/v1/financial_transaction_requests/advance")
                {
                    Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "")
                };
                RestRequest restRequest = new RestRequest(Method.POST);
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguAdvanceSimulationResponse> obj = await client.Execute<IuguAdvanceSimulationResponse>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguAdvanceSimulationResponse iuguAdvanceSimulationResponse = obj.Data ?? new IuguAdvanceSimulationResponse();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguAdvanceRequest>(_iuguCredentials.ShowContent);
                iuguAdvanceSimulationResponse.Error = iuguBaseErrors.Error;
                iuguAdvanceSimulationResponse.MessageError = iuguBaseErrors.MessageError;
                iuguAdvanceSimulationResponse.HasError = iuguBaseErrors.HasError;
                return iuguAdvanceSimulationResponse;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     REPASSAR VALOR PARA SUBCONTA
        //
        // Parámetros:
        //   apiTokenSubConta:
        //     API TOKEN DA SUBCONTA IRA RECEBER O VALOR
        //
        //   accoutId:
        //     ID DA SUBCONTA QUE IRA RECEBER O VALOR
        //
        //   valorDecimal:
        //     VALOR DO REPASSE
        //
        //   apiTokenMaster:
        //     API TOKEM DA CONTA MASTER CASO N�O INFORMADO SERA UTILIZADO DAS CONFIGURA��ES
        //
        //
        //   toWithdraw:
        //     REPASSE AUTOMATICO
        public IuguTransferModel RepasseValores(string apiTokenSubConta, string accoutId, decimal valorDecimal, string apiTokenMaster = null, bool toWithdraw = true)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/transfers");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(apiTokenMaster ?? _iuguCredentials.KeyUsage, "");
                string value = JsonConvert.SerializeObject(new IuguTrasferValuesModel
                {
                    AmoutCents = Convert.ToInt32($"{valorDecimal * 100m:0}"),
                    Receive = accoutId
                });
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguTransferModel> result = restClient.Execute<IuguTransferModel>(restRequest).Result;
                IuguTransferModel iuguTransferModel = result.Data ?? new IuguTransferModel();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguTransferModel>(_iuguCredentials.ShowContent);
                iuguTransferModel.Error = iuguBaseErrors.Error;
                iuguTransferModel.MessageError = iuguBaseErrors.MessageError;
                iuguTransferModel.HasError = iuguBaseErrors.HasError;
                if (result.StatusCode == HttpStatusCode.OK && string.IsNullOrEmpty(iuguTransferModel.MessageError) && valorDecimal >= 5m && toWithdraw)
                {
                    new Task(delegate
                    {
                        SolicitarSaque(accoutId, valorDecimal, apiTokenSubConta);
                    }).Start();
                }

                return result.Data;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     SOLICITAR TRANSFERENCIA DE VALORES DA IUGU PARA UMA SUBCONTA OU CONTA MASTER
        //
        //
        // Parámetros:
        //   accoutId:
        //
        //   valorSaque:
        //
        //   apiToken:
        public IuguWithdrawalModel SolicitarSaque(string accoutId, decimal valorSaque, string apiToken)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/accounts/" + accoutId + "/request_withdraw");
                RestRequest restRequest = new RestRequest(Method.POST);
                var value = new
                {
                    amount = valorSaque.ToString(CultureInfo.InvariantCulture)
                };
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                string value2 = JsonConvert.SerializeObject(value);
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value2, ParameterType.RequestBody);
                IRestResponse<IuguWithdrawalResponse> result = restClient.Execute<IuguWithdrawalResponse>(restRequest).Result;
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguWithdrawalResponse>(_iuguCredentials.ShowContent);
                IuguWithdrawalModel result2 = new IuguWithdrawalModel
                {
                    Error = iuguBaseErrors.Error,
                    MessageError = iuguBaseErrors.MessageError,
                    HasError = iuguBaseErrors.HasError
                };
                if (result.Data != null)
                {
                    return new IuguWithdrawalModel
                    {
                        Agencia = result.Data.BankAddress?.BankAg,
                        Banco = result.Data.BankAddress?.Bank,
                        Conta = result.Data.BankAddress?.BankCc,
                        AccountId = result.Data?.AccountId,
                        WithdrawalId = result.Data?.Id,
                        Valor = result.Data?.Amount,
                        Type = result.Data?.BankAddress?.AccountType
                    };
                }

                return result2;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     Cadastar SubConta
        //
        // Parámetros:
        //   model:
        public IuguAccountCreateResponseModel CriarSubConta(IuguAccountRequestModel model)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/marketplace/create_account");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(model.ApiToken ?? _iuguCredentials.KeyUsage, "");
                model.ApiToken = model.ApiToken ?? _iuguCredentials.KeyUsage;
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguAccountCreateResponseModel> result = restClient.Execute<IuguAccountCreateResponseModel>(restRequest).Result;
                IuguAccountCreateResponseModel iuguAccountCreateResponseModel = result.Data ?? new IuguAccountCreateResponseModel();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguAccountRequestModel>(_iuguCredentials.ShowContent);
                iuguAccountCreateResponseModel.Error = iuguBaseErrors.Error;
                iuguAccountCreateResponseModel.MessageError = iuguBaseErrors.MessageError;
                iuguAccountCreateResponseModel.HasError = iuguBaseErrors.HasError;
                return iuguAccountCreateResponseModel;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     verificar subConta
        //
        // Parámetros:
        //   model:
        //
        //   userApiToken:
        //
        //   accoutId:
        public IuguVerifyAccountModel VerificarSubConta(IuguAccountVerificationModel model, string userApiToken, string accoutId)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/accounts/" + accoutId + "/request_verification");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(userApiToken, "");
                model.Data.Bank = model.Data.Bank.GetBankName();
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguVerifyAccountModel> result = restClient.Execute<IuguVerifyAccountModel>(restRequest).Result;
                IuguVerifyAccountModel iuguVerifyAccountModel = result.Data ?? new IuguVerifyAccountModel();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguAccountVerificationModel>(_iuguCredentials.ShowContent);
                iuguVerifyAccountModel.Error = iuguBaseErrors.Error;
                iuguVerifyAccountModel.MessageError = iuguBaseErrors.MessageError;
                iuguVerifyAccountModel.HasError = iuguBaseErrors.HasError;
                iuguVerifyAccountModel.AlreadyVerified = iuguVerifyAccountModel.Error.ToDictionary<List<string>>().CheckAlreadyVerified();
                return iuguVerifyAccountModel;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     UPDATE DE DADOS BANCARIOS
        //
        // Parámetros:
        //   model:
        //
        //   userApiToken:
        public SimpleResponseMessage AtualizarDadosBancariosSubConta(IuguUpdateDataBank model, string userApiToken)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/bank_verification/");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(userApiToken, "");
                model.Bank = model.Bank.GetCodeBank();
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<SimpleResponseMessage> result = restClient.Execute<SimpleResponseMessage>(restRequest).Result;
                SimpleResponseMessage simpleResponseMessage = result.Data ?? new SimpleResponseMessage();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguUpdateDataBank>(_iuguCredentials.ShowContent);
                simpleResponseMessage.Error = iuguBaseErrors.Error;
                simpleResponseMessage.MessageError = iuguBaseErrors.MessageError;
                simpleResponseMessage.HasError = iuguBaseErrors.HasError;
                return simpleResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     VERIFICAR ATUALIZA��ES DE DADOS BANCARIOS
        //
        // Parámetros:
        //   model:
        //
        //   userApiToken:
        public List<IuguBankVerificationResponse> VerificarAtualizacaoDadosBancariosSubConta(string userApiToken)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/bank_verification/");
                RestRequest request = new RestRequest(Method.GET);
                restClient.Authenticator = new HttpBasicAuthenticator(userApiToken, "");
                return restClient.Execute<List<IuguBankVerificationResponse>>(request).Result.Data ?? new List<IuguBankVerificationResponse>();
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     RETORNA INFO DA SUBCONTA
        //
        // Parámetros:
        //   accoutId:
        //     ID DA SUB CONTA
        //
        //   apiToken:
        //     API TOKEN DA SUBCONTA
        public IuguAccountCompleteModel GetInfoSubConta(string accoutId, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/accounts/" + accoutId);
                RestRequest restRequest = new RestRequest(Method.GET);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                IRestResponse<IuguAccountCompleteModel> result = restClient.Execute<IuguAccountCompleteModel>(restRequest).Result;
                IuguAccountCompleteModel iuguAccountCompleteModel = result.Data ?? new IuguAccountCompleteModel();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<IuguAccountCompleteModel>(_iuguCredentials.ShowContent);
                iuguAccountCompleteModel.Error = iuguBaseErrors.Error;
                iuguAccountCompleteModel.MessageError = iuguBaseErrors.MessageError;
                iuguAccountCompleteModel.HasError = iuguBaseErrors.HasError;
                return iuguAccountCompleteModel;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public IuguAccountCompleteModel ConfigurarSubConta(AccountConfigurationRequestMessage configurationRequest, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/accounts/configuration");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                string value = JsonConvert.SerializeObject(configurationRequest);
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguAccountCompleteModel> result = restClient.Execute<IuguAccountCompleteModel>(restRequest).Result;
                IuguAccountCompleteModel iuguAccountCompleteModel = result.Data ?? new IuguAccountCompleteModel();
                IuguBaseErrors iuguBaseErrors = result.IuguMapErrors<AccountConfigurationRequestMessage>(_iuguCredentials.ShowContent);
                iuguAccountCompleteModel.Error = iuguBaseErrors.Error;
                iuguAccountCompleteModel.MessageError = iuguBaseErrors.MessageError;
                iuguAccountCompleteModel.HasError = iuguBaseErrors.HasError;
                return iuguAccountCompleteModel;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        //
        // Resumen:
        //     LISTAR SUBCONTAS
        //
        // Parámetros:
        //   filter:
        //     filtro
        //
        //   apiToken:
        //     API TOKEN OPTIONAL
        public IEnumerable<IuguAccountsModel> GetAccounts(IuguAccountFilterModel filter, string apiToken = null)
        {
            try
            {
                RestClient obj = ((filter != null) ? new RestClient("https://api.iugu.com/v1/marketplace?limit=" + filter.Limit + "&start=" + filter.Start + "&query=" + filter.Query) : new RestClient("https://api.iugu.com/v1/marketplace"));
                RestRequest restRequest = new RestRequest(Method.GET);
                obj.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                return obj.Execute<IEnumerable<IuguAccountsModel>>(restRequest).Result.Data.ToList();
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguTransferModel> RepasseValoresAsync(string apiTokenSubConta, string accoutId, decimal valorDecimal, string apiTokenMaster = null, bool toWithdraw = true)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/transfers");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(apiTokenMaster ?? _iuguCredentials.KeyUsage, "");
                string value = JsonConvert.SerializeObject(new IuguTrasferValuesModel
                {
                    AmoutCents = Convert.ToInt32($"{valorDecimal * 100m:0}"),
                    Receive = accoutId
                });
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguTransferModel> obj = await restClient.Execute<IuguTransferModel>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguTransferModel iuguTransferModel = obj.Data ?? new IuguTransferModel();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguTransferModel>(_iuguCredentials.ShowContent);
                iuguTransferModel.Error = iuguBaseErrors.Error;
                iuguTransferModel.MessageError = iuguBaseErrors.MessageError;
                iuguTransferModel.HasError = iuguBaseErrors.HasError;
                if (obj.StatusCode == HttpStatusCode.OK && string.IsNullOrEmpty(iuguTransferModel.MessageError) && valorDecimal >= 5m && toWithdraw)
                {
                    new Task(delegate
                    {
                        SolicitarSaque(accoutId, valorDecimal, apiTokenSubConta);
                    }).Start();
                }

                return obj.Data;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguWithdrawalModel> SolicitarSaqueAsync(string accoutId, decimal valorSaque, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/accounts/" + accoutId + "/request_withdraw");
                RestRequest restRequest = new RestRequest(Method.POST);
                var value = new
                {
                    amount = valorSaque.ToString(CultureInfo.InvariantCulture)
                };
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                string value2 = JsonConvert.SerializeObject(value);
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value2, ParameterType.RequestBody);
                IRestResponse<IuguWithdrawalResponse> restResponse = await restClient.Execute<IuguWithdrawalResponse>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguBaseErrors iuguBaseErrors = restResponse.IuguMapErrors<IuguWithdrawalResponse>(_iuguCredentials.ShowContent);
                IuguWithdrawalModel result = new IuguWithdrawalModel
                {
                    Error = iuguBaseErrors.Error,
                    MessageError = iuguBaseErrors.MessageError,
                    HasError = iuguBaseErrors.HasError
                };
                if (restResponse.Data != null)
                {
                    return new IuguWithdrawalModel
                    {
                        Agencia = restResponse.Data.BankAddress?.BankAg,
                        Banco = restResponse.Data.BankAddress?.Bank,
                        Conta = restResponse.Data.BankAddress?.BankCc,
                        AccountId = restResponse.Data?.AccountId,
                        WithdrawalId = restResponse.Data?.Id,
                        Valor = restResponse.Data?.Amount,
                        Type = restResponse.Data?.BankAddress?.AccountType
                    };
                }

                return result;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguAccountCreateResponseModel> CriarSubContaAsync(IuguAccountRequestModel model)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/marketplace/create_account");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(model.ApiToken ?? _iuguCredentials.KeyUsage, "");
                model.ApiToken = model.ApiToken ?? _iuguCredentials.KeyUsage;
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguAccountCreateResponseModel> obj = await restClient.Execute<IuguAccountCreateResponseModel>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguAccountCreateResponseModel iuguAccountCreateResponseModel = obj.Data ?? new IuguAccountCreateResponseModel();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguAccountRequestModel>(_iuguCredentials.ShowContent);
                iuguAccountCreateResponseModel.Error = iuguBaseErrors.Error;
                iuguAccountCreateResponseModel.MessageError = iuguBaseErrors.MessageError;
                iuguAccountCreateResponseModel.HasError = iuguBaseErrors.HasError;
                return iuguAccountCreateResponseModel;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguVerifyAccountModel> VerificarSubContaAsync(IuguAccountVerificationModel model, string userApiToken, string accoutId)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/accounts/" + accoutId + "/request_verification");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(userApiToken, "");
                model.Data.Bank = model.Data.Bank.GetBankName();
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguVerifyAccountModel> obj = await restClient.Execute<IuguVerifyAccountModel>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguVerifyAccountModel iuguVerifyAccountModel = obj.Data ?? new IuguVerifyAccountModel();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguAccountDataVerificationModel>(_iuguCredentials.ShowContent);
                iuguVerifyAccountModel.Error = iuguBaseErrors.Error;
                iuguVerifyAccountModel.MessageError = iuguBaseErrors.MessageError;
                iuguVerifyAccountModel.HasError = iuguBaseErrors.HasError;
                iuguVerifyAccountModel.AlreadyVerified = iuguVerifyAccountModel.Error.ToDictionary<List<string>>().CheckAlreadyVerified();
                return iuguVerifyAccountModel;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<SimpleResponseMessage> AtualizarDadosBancariosSubContaAsync(IuguUpdateDataBank model, string userApiToken)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/bank_verification/");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(userApiToken, "");
                model.Bank = model.Bank.GetCodeBank();
                string value = JsonConvert.SerializeObject(model);
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<SimpleResponseMessage> obj = await restClient.Execute<SimpleResponseMessage>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                SimpleResponseMessage simpleResponseMessage = obj.Data ?? new SimpleResponseMessage();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguUpdateDataBank>(_iuguCredentials.ShowContent);
                simpleResponseMessage.Error = iuguBaseErrors.Error;
                simpleResponseMessage.MessageError = iuguBaseErrors.MessageError;
                simpleResponseMessage.HasError = iuguBaseErrors.HasError;
                return simpleResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<List<IuguBankVerificationResponse>> VerificarAtualizacaoDadosBancariosSubContaAsync(string userApiToken)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/bank_verification/");
                RestRequest request = new RestRequest(Method.GET);
                restClient.Authenticator = new HttpBasicAuthenticator(userApiToken, "");
                return (await restClient.Execute<List<IuguBankVerificationResponse>>(request).ConfigureAwait(continueOnCapturedContext: false)).Data ?? new List<IuguBankVerificationResponse>();
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguAccountCompleteModel> GetInfoSubContaAsync(string accoutId, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/accounts/" + accoutId);
                RestRequest restRequest = new RestRequest(Method.GET);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                IRestResponse<IuguAccountCompleteModel> obj = await restClient.Execute<IuguAccountCompleteModel>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguAccountCompleteModel iuguAccountCompleteModel = obj.Data ?? new IuguAccountCompleteModel();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<IuguAccountCompleteModel>(_iuguCredentials.ShowContent);
                iuguAccountCompleteModel.Error = iuguBaseErrors.Error;
                iuguAccountCompleteModel.MessageError = iuguBaseErrors.MessageError;
                iuguAccountCompleteModel.HasError = iuguBaseErrors.HasError;
                return iuguAccountCompleteModel;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IuguAccountCompleteModel> ConfigurarSubContaAsync(AccountConfigurationRequestMessage configurationRequest, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/accounts/configuration");
                RestRequest restRequest = new RestRequest(Method.POST);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                string value = JsonConvert.SerializeObject(configurationRequest);
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddParameter("application/json", value, ParameterType.RequestBody);
                IRestResponse<IuguAccountCompleteModel> obj = await restClient.Execute<IuguAccountCompleteModel>(restRequest).ConfigureAwait(continueOnCapturedContext: false);
                IuguAccountCompleteModel iuguAccountCompleteModel = obj.Data ?? new IuguAccountCompleteModel();
                IuguBaseErrors iuguBaseErrors = obj.IuguMapErrors<AccountConfigurationRequestMessage>(_iuguCredentials.ShowContent);
                iuguAccountCompleteModel.Error = iuguBaseErrors.Error;
                iuguAccountCompleteModel.MessageError = iuguBaseErrors.MessageError;
                iuguAccountCompleteModel.HasError = iuguBaseErrors.HasError;
                return iuguAccountCompleteModel;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<IEnumerable<IuguAccountsModel>> GetAccountsAsync(IuguAccountFilterModel filter, string apiToken = null)
        {
            try
            {
                RestClient obj = ((filter != null) ? new RestClient("https://api.iugu.com/v1/marketplace?limit=" + filter.Limit + "&start=" + filter.Start + "&query=" + filter.Query) : new RestClient("https://api.iugu.com/v1/marketplace"));
                RestRequest restRequest = new RestRequest(Method.GET);
                obj.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                restRequest.AddHeader("cache-control", "no-cache");
                restRequest.AddHeader("content-type", "application/json");
                return (await obj.Execute<IEnumerable<IuguAccountsModel>>(restRequest).ConfigureAwait(continueOnCapturedContext: false)).Data.ToList();
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public IuguVerifyAccountModel SendRequestVerification(DataBankViewModel model, string userApiKey, string accountKey, bool useGoogleVerificationAddress = false, string googleKey = null)
        {
            AddressViewModel addressViewModel = null;
            if (useGoogleVerificationAddress)
            {
                addressViewModel = Utilities.GetInfoFromAdressLocation(model.Address, googleKey);
            }

            string bankCc = model.BankAccount.ValidAccoutBank(model.Bank, model.AccountType);
            return VerificarSubConta(new IuguAccountVerificationModel
            {
                Data = new IuguAccountDataVerificationModel
                {
                    Name = model.FantasyName,
                    CompanyName = model.SocialName,
                    Bank = model.Bank,
                    BankAg = model.BankAgency.ValidAgencyBank(model.Bank),
                    Address = model.Address,
                    AccountType = model.AccountType,
                    AutomaticTransfer = model.AutomaticTransfer,
                    AutomaticValidation = model.AutomaticValidation,
                    BankCc = bankCc,
                    BusinessType = model.BusinessType,
                    PersonType = model.PersonType,
                    Cnpj = model.Cnpj,
                    RespCpf = model.AccountableCpf,
                    PriceRange = model.PriceRange,
                    Telphone = model.AccountablePhone,
                    RespName = model.AccountableName,
                    City = ((!useGoogleVerificationAddress) ? model.City : (addressViewModel?.City ?? model.City)),
                    State = ((!useGoogleVerificationAddress) ? model.State : (addressViewModel?.State ?? model.State)),
                    PhysicalProducts = model.PhysicalProducts,
                    Cep = model.Cep,
                    Cpf = model.Cpf
                }
            }, userApiKey, accountKey);
        }

        public async Task<IuguVerifyAccountModel> SendRequestVerificationAsync(DataBankViewModel model, string userApiKey, string accountKey, bool useGoogleVerificationAddress = false, string googleKey = null)
        {
            AddressViewModel addressViewModel = null;
            if (useGoogleVerificationAddress)
            {
                addressViewModel = Utilities.GetInfoFromAdressLocation(model.Address, googleKey);
            }

            string bankCc = model.BankAccount.ValidAccoutBank(model.Bank, model.AccountType);
            return await VerificarSubContaAsync(new IuguAccountVerificationModel
            {
                Data = new IuguAccountDataVerificationModel
                {
                    Name = model.FantasyName,
                    CompanyName = model.SocialName,
                    Bank = model.Bank,
                    BankAg = model.BankAgency.ValidAgencyBank(model.Bank),
                    Address = model.Address,
                    AccountType = model.AccountType,
                    AutomaticTransfer = model.AutomaticTransfer,
                    AutomaticValidation = model.AutomaticValidation,
                    BankCc = bankCc,
                    BusinessType = model.BusinessType,
                    PersonType = model.PersonType,
                    Cnpj = model.Cnpj,
                    RespCpf = model.AccountableCpf,
                    PriceRange = model.PriceRange,
                    Telphone = model.AccountablePhone,
                    RespName = model.AccountableName,
                    City = ((!useGoogleVerificationAddress) ? model.City : (addressViewModel?.City ?? model.City)),
                    State = ((!useGoogleVerificationAddress) ? model.State : (addressViewModel?.State ?? model.State)),
                    PhysicalProducts = model.PhysicalProducts,
                    Cep = model.Cep,
                    Cpf = model.Cpf
                }
            }, userApiKey, accountKey);
        }

        public async Task<SimpleResponseMessage> UpdateDataBankAsync(DataBankViewModel model, string liveKey)
        {
            string account = model.BankAccount.ValidAccoutBank(model.Bank, model.AccountType);
            return await AtualizarDadosBancariosSubContaAsync(new IuguUpdateDataBank
            {
                AccountType = model.AccountType.GetTypeAccout(),
                Bank = model.Bank.GetCodeBank(),
                AutomaticValidation = model.AutomaticValidation.ToString(),
                Agency = model.BankAgency.ValidAgencyBank(model.Bank),
                Account = account
            }, liveKey).ConfigureAwait(continueOnCapturedContext: false);
        }

        public SimpleResponseMessage UpdateDataBank(DataBankViewModel model, string liveKey)
        {
            string account = model.BankAccount.ValidAccoutBank(model.Bank, model.AccountType);
            return AtualizarDadosBancariosSubConta(new IuguUpdateDataBank
            {
                AccountType = model.AccountType.GetTypeAccout(),
                Bank = model.Bank.GetCodeBank(),
                AutomaticValidation = model.AutomaticValidation.ToString(),
                Agency = model.BankAgency.ValidAgencyBank(model.Bank),
                Account = account
            }, liveKey);
        }

        public IuguBaseMarketPlace SendVerifyOrUpdateDataBank(DataBankViewModel model, bool newRegister, string liveKey = null, string userApiKey = null, string accountKey = null, long? lastVerification = null, string marketplaceName = null, bool useGoogleVerificationAddress = false, string googleKey = null)
        {
            IuguBaseMarketPlace iuguBaseMarketPlace = new IuguBaseMarketPlace();
            try
            {
                model.Bank = model.Bank.GetBankName();
                long num = new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds();
                if (lastVerification.HasValue && lastVerification >= num)
                {
                    iuguBaseMarketPlace.HasError = true;
                    iuguBaseMarketPlace.MessageError = "Não e possível atualizar os dados bancários, já existe uma verificação de dados bancários em aberto.";
                    return iuguBaseMarketPlace;
                }

                if (newRegister && string.IsNullOrEmpty(liveKey) && string.IsNullOrEmpty(userApiKey) && string.IsNullOrEmpty(accountKey))
                {
                    if (string.IsNullOrEmpty(marketplaceName))
                    {
                        iuguBaseMarketPlace.HasError = true;
                        iuguBaseMarketPlace.MessageError = "Informe o nome do market place";
                        return iuguBaseMarketPlace;
                    }

                    IuguAccountCreateResponseModel iuguAccountCreateResponseModel = CriarSubConta(new IuguAccountRequestModel
                    {
                        Name = marketplaceName,
                        CommissionPercent = 0
                    });
                    if (iuguAccountCreateResponseModel.HasError)
                    {
                        iuguBaseMarketPlace = iuguBaseMarketPlace.MapErrorDataBank(iuguAccountCreateResponseModel);
                        return iuguBaseMarketPlace;
                    }

                    iuguBaseMarketPlace.AccountKey = (accountKey = iuguAccountCreateResponseModel.AccountId);
                    iuguBaseMarketPlace.LiveKey = (liveKey = iuguAccountCreateResponseModel.LiveApiToken);
                    iuguBaseMarketPlace.TestKey = iuguAccountCreateResponseModel.TestApiToken;
                    iuguBaseMarketPlace.UserApiKey = (userApiKey = iuguAccountCreateResponseModel.UserToken);
                    iuguBaseMarketPlace.IsNewRegister = true;
                }
                else if (string.IsNullOrEmpty(liveKey) && string.IsNullOrEmpty(userApiKey) && string.IsNullOrEmpty(accountKey))
                {
                    iuguBaseMarketPlace.HasError = true;
                    iuguBaseMarketPlace.MessageError = "Verifique as chaves informadas do marketplace";
                    return iuguBaseMarketPlace;
                }

                string bankAccount = model.BankAccount.ValidAccoutBank(model.Bank, model.AccountType, model.PersonType);
                string text = model.BankAgency.ValidAgencyBank(model.Bank);
                if (!model.LastRequestVerification.HasValue)
                {
                    IuguVerifyAccountModel iuguVerifyAccountModel = SendRequestVerification(model, userApiKey, accountKey, useGoogleVerificationAddress, googleKey);
                    if (!iuguVerifyAccountModel.HasError)
                    {
                        iuguBaseMarketPlace.InVerification = DateTimeOffset.Now.ToUnixTimeSeconds();
                        iuguBaseMarketPlace.UpdateDataBank = DateTimeOffset.Now.ToUnixTimeSeconds();
                        iuguBaseMarketPlace.AccoutableCpf = model.AccountableCpf;
                        iuguBaseMarketPlace.AccoutableName = model.AccountableName;
                        iuguBaseMarketPlace.AccountType = model.AccountType;
                        iuguBaseMarketPlace.Bank = model.Bank;
                        iuguBaseMarketPlace.BankAccount = bankAccount;
                        iuguBaseMarketPlace.BankAgency = text;
                        iuguBaseMarketPlace.PersonType = model.PersonType;
                        iuguBaseMarketPlace.Cnpj = model.Cnpj;
                        iuguBaseMarketPlace.CustomMessage = "Dados bancários atualizados com sucesso.";
                        return iuguBaseMarketPlace;
                    }

                    iuguBaseMarketPlace = ((!iuguVerifyAccountModel.AlreadyVerified) ? iuguBaseMarketPlace.MapErrorDataBank(iuguVerifyAccountModel) : InternalUpdateDataBank(iuguBaseMarketPlace, model, bankAccount, text, liveKey));
                    return iuguBaseMarketPlace;
                }

                iuguBaseMarketPlace = InternalUpdateDataBank(iuguBaseMarketPlace, model, bankAccount, text, liveKey);
            }
            catch (Exception ex)
            {
                iuguBaseMarketPlace.HasError = true;
                iuguBaseMarketPlace.Error = new
                {
                    error = $"{ex.InnerException} {ex.Message}".Trim()
                };
            }

            return iuguBaseMarketPlace;
        }

        public async Task<IuguBaseMarketPlace> SendVerifyOrUpdateDataBankAsync(DataBankViewModel model, bool newRegister, string liveKey = null, string userApiKey = null, string accountKey = null, long? lastVerification = null, string marketplaceName = null, bool useGoogleVerificationAddress = false, string googleKey = null)
        {
            IuguBaseMarketPlace response = new IuguBaseMarketPlace();
            try
            {
                model.Bank = model.Bank.GetBankName();
                long num = new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds();
                if (lastVerification.HasValue && lastVerification >= num)
                {
                    response.HasError = true;
                    response.MessageError = "Não e possível atualizar os dados bancários, já existe uma verificação de dados bancários em aberto.";
                    return response;
                }

                if (newRegister && string.IsNullOrEmpty(liveKey) && string.IsNullOrEmpty(userApiKey) && string.IsNullOrEmpty(accountKey))
                {
                    if (string.IsNullOrEmpty(marketplaceName))
                    {
                        response.HasError = true;
                        response.MessageError = "Informe o nome do market place";
                        return response;
                    }

                    IuguAccountCreateResponseModel iuguAccountCreateResponseModel = await CriarSubContaAsync(new IuguAccountRequestModel
                    {
                        Name = marketplaceName,
                        CommissionPercent = 0
                    }).ConfigureAwait(continueOnCapturedContext: false);
                    if (iuguAccountCreateResponseModel.HasError)
                    {
                        response = response.MapErrorDataBank(iuguAccountCreateResponseModel);
                        return response;
                    }

                    IuguBaseMarketPlace iuguBaseMarketPlace = response;
                    string accountId;
                    accountKey = (accountId = iuguAccountCreateResponseModel.AccountId);
                    iuguBaseMarketPlace.AccountKey = accountId;
                    IuguBaseMarketPlace iuguBaseMarketPlace2 = response;
                    liveKey = (accountId = iuguAccountCreateResponseModel.LiveApiToken);
                    iuguBaseMarketPlace2.LiveKey = accountId;
                    response.TestKey = iuguAccountCreateResponseModel.TestApiToken;
                    IuguBaseMarketPlace iuguBaseMarketPlace3 = response;
                    userApiKey = (accountId = iuguAccountCreateResponseModel.UserToken);
                    iuguBaseMarketPlace3.UserApiKey = accountId;
                    response.IsNewRegister = true;
                }
                else if (string.IsNullOrEmpty(liveKey) && string.IsNullOrEmpty(userApiKey) && string.IsNullOrEmpty(accountKey))
                {
                    response.HasError = true;
                    response.MessageError = "Verifique as chaves informadas do marketplace";
                    return response;
                }

                string bankAccount = model.BankAccount.ValidAccoutBank(model.Bank, model.AccountType, model.PersonType);
                string agencyAccount = model.BankAgency.ValidAgencyBank(model.Bank);
                bool flag = false;
                if (!string.IsNullOrEmpty(accountKey))
                {
                    IuguAccountCompleteModel iuguAccountCompleteModel = await GetInfoSubContaAsync(accountKey, liveKey);
                    flag = !iuguAccountCompleteModel.HasError && iuguAccountCompleteModel?.LastVerificationRequestStatus != null;
                }

                if (!(model.LastRequestVerification.HasValue && flag))
                {
                    IuguVerifyAccountModel iuguVerifyAccountModel = await SendRequestVerificationAsync(model, userApiKey, accountKey, useGoogleVerificationAddress, googleKey).ConfigureAwait(continueOnCapturedContext: false);
                    if (iuguVerifyAccountModel.HasError)
                    {
                        response = ((!iuguVerifyAccountModel.AlreadyVerified) ? response.MapErrorDataBank(iuguVerifyAccountModel) : (await InternalUpdateDataBankAsync(response, model, bankAccount, agencyAccount, liveKey).ConfigureAwait(continueOnCapturedContext: false)));
                        return response;
                    }

                    response.InVerification = DateTimeOffset.Now.ToUnixTimeSeconds();
                    response.UpdateDataBank = DateTimeOffset.Now.ToUnixTimeSeconds();
                    response.AccoutableCpf = model.AccountableCpf;
                    response.AccoutableName = model.AccountableName;
                    response.AccountType = model.AccountType;
                    response.Bank = model.Bank;
                    response.BankAccount = bankAccount;
                    response.BankAgency = agencyAccount;
                    response.PersonType = model.PersonType;
                    response.Cnpj = model.Cnpj;
                    response.CustomMessage = "Dados bancários atualizados com sucesso.";
                    return response;
                }

                response = await InternalUpdateDataBankAsync(response, model, bankAccount, agencyAccount, liveKey).ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.Error = new
                {
                    error = $"{ex.InnerException} {ex.Message}".Trim()
                };
            }

            return response;
        }

        private IuguBaseMarketPlace InternalUpdateDataBank(IuguBaseMarketPlace response, DataBankViewModel model, string bankAccount, string agencyAccount, string liveKey)
        {
            try
            {
                SimpleResponseMessage simpleResponseMessage = UpdateDataBank(model, liveKey);
                if (!simpleResponseMessage.Success || simpleResponseMessage.HasError)
                {
                    response = response.MapErrorDataBank(simpleResponseMessage);
                    if (string.IsNullOrEmpty(response.MessageError))
                    {
                        response.MessageError = "Não foi possível atualizar os dados bancários, verifique os dados e tente novamente.";
                    }

                    return response;
                }

                response.InVerification = DateTimeOffset.Now.ToUnixTimeSeconds();
                response.UpdateDataBank = DateTimeOffset.Now.ToUnixTimeSeconds();
                response.AccountType = model.AccountType;
                response.Bank = model.Bank;
                response.BankAccount = bankAccount;
                response.BankAgency = agencyAccount;
                response.Cnpj = model.Cnpj;
                response.CustomMessage = "Dados bancários atualizados com sucesso";
                return response;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.MessageError = "Ocorreu um erro inesperado";
                response.Error = new
                {
                    error = $"{ex.InnerException} {ex.Message}".Trim()
                };
                return response;
            }
        }

        private async Task<IuguBaseMarketPlace> InternalUpdateDataBankAsync(IuguBaseMarketPlace response, DataBankViewModel model, string bankAccount, string agencyAccount, string liveKey)
        {
            try
            {
                SimpleResponseMessage simpleResponseMessage = await UpdateDataBankAsync(model, liveKey).ConfigureAwait(continueOnCapturedContext: false);
                if (!simpleResponseMessage.Success || simpleResponseMessage.HasError)
                {
                    response = response.MapErrorDataBank(simpleResponseMessage);
                    if (string.IsNullOrEmpty(response.MessageError))
                    {
                        response.MessageError = "Não foi possível atualizar os dados bancários, verifique os dados e tente novamente.";
                    }

                    return response;
                }

                response.InVerification = DateTimeOffset.Now.ToUnixTimeSeconds();
                response.UpdateDataBank = DateTimeOffset.Now.ToUnixTimeSeconds();
                response.AccountType = model.AccountType;
                response.Bank = model.Bank;
                response.BankAccount = bankAccount;
                response.BankAgency = agencyAccount;
                response.Cnpj = model.Cnpj;
                response.CustomMessage = "Dados bancários atualizados com sucesso";
                return response;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.MessageError = "Ocorreu um erro inesperado";
                response.Error = new
                {
                    error = $"{ex.InnerException} {ex.Message}".Trim()
                };
                return response;
            }
        }

        public async Task<List<WebHookViewModel>> GetWebhookLogAsync(string invoiceId, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/web_hook_logs/" + invoiceId);
                RestRequest request = new RestRequest(Method.GET);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                return (await restClient.Execute<List<WebHookViewModel>>(request).ConfigureAwait(continueOnCapturedContext: false)).Data;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public List<WebHookViewModel> GetWebhookLog(string invoiceId, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/web_hook_logs/" + invoiceId);
                RestRequest request = new RestRequest(Method.GET);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                return restClient.Execute<List<WebHookViewModel>>(request).Result.Data;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public WebHookViewModel ResendWebhook(string webhookId, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/web_hook_logs/" + webhookId + "/retry");
                RestRequest request = new RestRequest(Method.GET);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                return restClient.Execute<WebHookViewModel>(request).Result.Data;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }

        public async Task<WebHookViewModel> ResendWebhookAsync(string webhookId, string apiToken = null)
        {
            try
            {
                RestClient restClient = new RestClient("https://api.iugu.com/v1/web_hook_logs/" + webhookId + "/retry");
                RestRequest request = new RestRequest(Method.GET);
                restClient.Authenticator = new HttpBasicAuthenticator(apiToken ?? _iuguCredentials.KeyUsage, "");
                return (await restClient.Execute<WebHookViewModel>(request).ConfigureAwait(continueOnCapturedContext: false)).Data;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro", innerException);
            }
        }
    }
}
