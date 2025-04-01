using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Extensions;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.ViewModels;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using IronSoftware.Drawing;
using SkiaSharp;
using Moralar.UtilityFramework.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Moralar.UtilityFramework.Application.Core
{
    public static class Utilities
    {
        private static readonly Random Random = new Random();


        public static IHttpContextAccessor HttpContextAccessor;

        private static readonly char[] SeparatorChars = new char[4] { ';', '|', '\t', ',' };

        //
        // Resumen:
        //     METODO PARA OBTER O DEFINIR CURRENT HTTPCONTEXT
        //
        // Parámetros:
        //   httpContextAccessor:
        public static void SetHttpContext(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public static string getFilePathFromServer(string folderPath, string rootPath)
        {
            // Ensure the path exists
            string finalPath = Path.Combine(rootPath, folderPath);
            if (!Directory.Exists(finalPath))
            {
                Directory.CreateDirectory(finalPath);
            }

            return finalPath;
        }


        //
        // Resumen:
        //     RETORNAR URL COM PROTOCOLO (HTTP/HTTPS) DA CHAMADA REALIZADA
        //
        // Parámetros:
        //   url:
        //     url
        public static string SetCurrentProtocol(this string url)
        {
            if (HttpContextAccessor == null || HttpContextAccessor.HttpContext == null)
            {
                return url;
            }

            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            int num = url.IndexOf(':');
            if (num > 0)
            {
                url = HttpContextAccessor.HttpContext.Request.Scheme + ":" + url.Substring(num + 1);
            }

            return url;
        }

        //
        // Resumen:
        //     IF STRING CONTAINS CASEINSENSITIVE
        //
        // Parámetros:
        //   txt:
        //
        //   search:
        public static bool ContainsIgnoreCase(this string txt, string search)
        {
            if (!string.IsNullOrEmpty(txt))
            {
                return txt.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
            }

            return false;
        }

        //
        // Resumen:
        //     REPASSE DE TAG POR VALOR
        //
        // Parámetros:
        //   txt:
        //     TEXTO
        //
        //   substituicao:
        //     CHAVES
        public static string ReplaceTag(this string txt, IDictionary<string, string> substituicao)
        {
            if (!string.IsNullOrEmpty(txt))
            {
                txt = substituicao?.Aggregate(txt, (string current, KeyValuePair<string, string> item) => current.Replace(item.Key, item.Value));
            }

            return txt;
        }

        //
        // Resumen:
        //     NÚMERO RAMDOMICO VERDADEIRO
        //
        // Parámetros:
        //   min:
        //
        //   max:
        public static async Task<int> TrueRamdom(int min = 1, int max = 1)
        {
            try
            {
                RestClient client = new RestClient($"http://www.random.org/integers/?num=1&min={min}&max={max}&col=1&base=10&format=plain&rnd=new");
                RestRequest request = new RestRequest(Method.GET);
                int.TryParse((await client.Execute<string>(request)).Content, out var result);
                return result;
            }
            catch (Exception)
            {
                return int.Parse(RandomInt(max.ToString().Length));
            }
        }

        //
        // Resumen:
        //     SETAR VALORES DEFAULT EM CAMPOS NULL
        //
        // Parámetros:
        //   entity:
        //
        // Parámetros de tipo:
        //   T:
        public static T RemoveAllNull<T>(this T entity) where T : class
        {
            PropertyInfo[] properties = entity.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                try
                {
                    if (propertyInfo.GetValue(entity, null) == null)
                    {
                        if (propertyInfo.PropertyType == typeof(string))
                        {
                            SetPropertyValue(entity, propertyInfo.Name, "");
                        }

                        if (propertyInfo.PropertyType == typeof(int?))
                        {
                            SetPropertyValue(entity, propertyInfo.Name, 0);
                        }

                        if (propertyInfo.PropertyType == typeof(long?))
                        {
                            SetPropertyValue(entity, propertyInfo.Name, 0L);
                        }

                        if (propertyInfo.PropertyType == typeof(double?))
                        {
                            SetPropertyValue(entity, propertyInfo.Name, 0.0);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            return entity;
        }

        //
        // Resumen:
        //     REMOVER ACENTOS
        //
        // Parámetros:
        //   text:
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            string text2 = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();
            string text3 = text2;
            foreach (char c in text3)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        //
        // Resumen:
        //     OBTER VALOR DE UMA PROP / RETORNA NULL CASO NÃO EXISTA
        //
        // Parámetros:
        //   x:
        //
        //   propName:
        public static object GetValueByProperty(object x, string propName)
        {
            return x.GetType().GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public)?.GetValue(x, null);
        }

        //
        // Resumen:
        //     RETORNA OBJ APENAS COM CAMPOS ALTERADOS
        //
        // Parámetros:
        //   entity:
        //
        //   newValues:
        //
        // Parámetros de tipo:
        //   T:
        //
        //   Y:
        public static object OnlyChanged<T, Y>(T entity, Y newValues)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (PropertyInfo item in entity.GetType().GetProperties().ToList())
            {
                object valueByProperty = GetValueByProperty(entity, item.Name);
                object valueByProperty2 = GetValueByProperty(newValues, item.Name);
                if (!object.Equals(valueByProperty, valueByProperty2))
                {
                    dictionary.Add((((JsonPropertyAttribute)(item.GetCustomAttributes(typeof(JsonPropertyAttribute), inherit: false)?.FirstOrDefault()))?.PropertyName ?? item.Name) ?? "", valueByProperty2);
                }
            }

            return ToGenericData(dictionary);
        }

        //
        // Resumen:
        //     GERAR OBJ DINAMY ANONIMO PARA INCLUIR PROPS MANUALMENTE
        //
        // Parámetros:
        //   obj:
        public static dynamic ToDynamic(this object obj)
        {
            try
            {
                return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(obj), typeof(ExpandoObject));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //
        // Resumen:
        //     GERAR OBJ DINAMY ANONIMO PARA INCLUIR PROPS MANUALMENTE
        //
        // Parámetros:
        //   dict:
        public static dynamic ToGenericData(Dictionary<string, object> dict)
        {
            ExpandoObject expandoObject = new ExpandoObject();
            ICollection<KeyValuePair<string, object>> collection = expandoObject;
            foreach (KeyValuePair<string, object> item in dict)
            {
                collection.Add(item);
            }

            return expandoObject;
        }

        //
        // Resumen:
        //     SETA VALOR EM UMA PROP POR NAME
        //
        // Parámetros:
        //   obj:
        //
        //   propName:
        //
        //   value:
        public static void SetPropertyValue(object obj, string propName, object value)
        {
            obj.GetType().GetProperty(propName)?.SetValue(obj, value, null);
        }

        //
        // Resumen:
        //     METODO PARA OBTER OS CAMPOS QUE FORAM RECEBIDOS NO JSON UTILIZADO EM CONJUNTO
        //     COM SETIFDIFERENT PARA SIMULAR METODO PUT
        //
        // Parámetros:
        //   httpContext:
        public static string[] GetFieldsFromBody(this HttpContext httpContext)
        {
            string[] result = new string[0];
            try
            {
                if (httpContext.Request.Body.CanSeek)
                {
                    httpContext.Request.Body.Seek(0L, SeekOrigin.Begin);
                    using StreamReader streamReader = new StreamReader(httpContext.Request.Body);
                    result = (from x in JsonConvert.DeserializeObject<Dictionary<string, object>>(streamReader.ReadToEnd())
                              select x.Key?.UppercaseFirst()).ToArray();
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        //
        // Resumen:
        //     METODO PARA OBTER OS CAMPOS QUE FORAM RECEBIDOS NO JSON UTILIZADO EM CONJUNTO
        //     COM SETIFDIFERENT PARA SIMULAR METODO PUT
        //
        // Parámetros:
        //   httpContext:
        public static string[] GetFieldsFromBody(this IHttpContextAccessor httpContext)
        {
            string[] result = new string[0];
            try
            {
                if (httpContext.HttpContext.Request.Body.CanSeek)
                {
                    httpContext.HttpContext.Request.Body.Seek(0L, SeekOrigin.Begin);
                    using StreamReader streamReader = new StreamReader(httpContext.HttpContext.Request.Body);
                    result = (from x in JsonConvert.DeserializeObject<Dictionary<string, object>>(streamReader.ReadToEnd())
                              select x.Key?.UppercaseFirst()).ToArray();
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        //
        // Resumen:
        //     METODO PARA OBTER OS CAMPOS QUE FORAM RECEBIDOS NO JSON UTILIZADO EM CONJUNTO
        //     COM SETIFDIFERENT PARA SIMULAR METODO PUT
        //
        // Parámetros:
        //   httpContext:
        public static async Task<string[]> GetFieldsFromBodyAsync([FromBody] Dictionary<string, object> body)
        {
            var result = new List<string>();
            if (body != null)
            {
                try
                {
                    var fields = body;
                    result = fields.Keys.Select(key => key?.UppercaseFirst()).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error on get changed fields of a PUT request", ex);
                }
            }

            return result.ToArray();
        }

        public static Dictionary<string, object> ConvertObjectToDictionary(object obj)
        {
            // Serialize the object to a JSON string
            var jsonString = JsonConvert.SerializeObject(obj);

            // Deserialize the JSON string to a Dictionary<string, object>
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

            return dictionary;
        }

        //
        // Resumen:
        //     METODO PARA ALTERAR HORARIO DE UM DATETIME
        //
        // Parámetros:
        //   dateTime:
        //
        //   hours:
        //
        //   minutes:
        //
        //   seconds:
        //
        //   milliseconds:
        public static DateTime ChangeTime(this DateTime dateTime, int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hours, minutes, seconds, milliseconds, dateTime.Kind);
        }

        //
        // Resumen:
        //     UPDATE GENERICO POR PROP ALTERADAS
        //
        // Parámetros:
        //   target:
        //
        //   source:
        //
        // Parámetros de tipo:
        //   T:
        //
        //   TY:
        public static T SetIfDifferent<T, TY>(this T target, TY source) where T : class where TY : class
        {
            PropertyInfo[] properties = source.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                DataPropertie dataPropertie = (DataPropertie)propertyInfo.GetCustomAttributes(typeof(DataPropertie), inherit: false).FirstOrDefault();
                object valueByProperty = GetValueByProperty(target, dataPropertie?.PropertieName ?? propertyInfo.Name);
                object valueByProperty2 = GetValueByProperty(source, propertyInfo.Name);
                if (!object.Equals(valueByProperty, valueByProperty2) && !propertyInfo.IsDefined(typeof(IsReadOnly)) && (!object.Equals(valueByProperty2, null) || !propertyInfo.IsDefined(typeof(IsNotNull))))
                {
                    SetPropertyValue(target, dataPropertie?.PropertieName ?? propertyInfo.Name, valueByProperty2);
                }
            }

            return target;
        }

        //
        // Resumen:
        //     Obter nome dos campos alterados
        public static List<string> GetDiferentFields<T, TY>(this T target, TY source) where T : class where TY : class
        {
            List<string> list = new List<string>();
            try
            {
                PropertyInfo[] properties = source.GetType().GetProperties();
                foreach (PropertyInfo propertyInfo in properties)
                {
                    object valueByProperty = GetValueByProperty(target, propertyInfo.Name);
                    object valueByProperty2 = GetValueByProperty(source, propertyInfo.Name);
                    if (!object.Equals(valueByProperty, valueByProperty2))
                    {
                        list.Add(propertyInfo.Name);
                    }
                }
            }
            catch (Exception)
            {
            }

            return list;
        }

        //
        // Resumen:
        //     UPDATE GENERICO POR PROP ALTERADAS COM CAMPOS INFORMADOS
        //
        // Parámetros:
        //   fields:
        //
        //   target:
        //
        //   source:
        //
        // Parámetros de tipo:
        //   T:
        //
        //   TY:
        public static T SetIfDifferent<T, TY>(this T target, TY source, List<string> fields) where T : class where TY : class
        {
            for (int i = 0; i < fields.Count; i++)
            {
                PropertyInfo property = source.GetType().GetProperty(fields[i]);
                if (property != null)
                {
                    DataPropertie dataPropertie = (DataPropertie)property.GetCustomAttributes(typeof(DataPropertie), inherit: false).FirstOrDefault();
                    object valueByProperty = GetValueByProperty(target, dataPropertie?.PropertieName ?? property.Name);
                    object valueByProperty2 = GetValueByProperty(source, property.Name);
                    if (!object.Equals(valueByProperty, valueByProperty2) && !property.IsDefined(typeof(IsReadOnly)) && (!object.Equals(valueByProperty2, null) || !property.IsDefined(typeof(IsNotNull))))
                    {
                        SetPropertyValue(target, dataPropertie?.PropertieName ?? property.Name, valueByProperty2);
                    }
                }
            }

            return target;
        }

        //
        // Resumen:
        //     UPDATE GENERICO POR PROP ALTERADAS COM CAMPOS INFORMADOS
        //
        // Parámetros:
        //   fields:
        //
        //   target:
        //
        //   source:
        //
        // Parámetros de tipo:
        //   T:
        //
        //   TY:
        public static T SetIfDifferent<T, TY>(this T target, TY source, string[] fields) where T : class where TY : class
        {
            for (int i = 0; i < fields.Length; i++)
            {
                PropertyInfo property = source.GetType().GetProperty(fields[i]);
                if (property != null)
                {
                    DataPropertie dataPropertie = (DataPropertie)property.GetCustomAttributes(typeof(DataPropertie), inherit: false).FirstOrDefault();
                    object valueByProperty = GetValueByProperty(target, dataPropertie?.PropertieName ?? property.Name);
                    object valueByProperty2 = GetValueByProperty(source, property.Name);
                    if (!object.Equals(valueByProperty, valueByProperty2) && !property.IsDefined(typeof(IsReadOnly)) && (!object.Equals(valueByProperty2, null) || !property.IsDefined(typeof(IsNotNull))))
                    {
                        SetPropertyValue(target, dataPropertie?.PropertieName ?? property.Name, valueByProperty2);
                    }
                }
            }

            return target;
        }

        //
        // Resumen:
        //     SALVAR IMAGEM DA ROTA PELO PATH
        //
        // Parámetros:
        //   imagePath:
        //
        //   pathRoute:
        //
        //   width:
        //
        //   height:
        //
        //   type:
        public static async Task SaveImageToRoute(string imagePath, string pathRoute, int width = 750, int height = 360, string type = "roadmap")
        {
            string value = GetConfigurationRoot().GetValue<string>("googleMapsKey");
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception("Informe a googleMapsKey no settings/Config.json");
            }

            if (string.IsNullOrEmpty(imagePath))
            {
                throw new Exception("Informe o caminho onde será salvo o arquivo");
            }

            if (string.IsNullOrEmpty(pathRoute))
            {
                throw new Exception("Informe o path da rota realizada");
            }

            string requestUri = $"https://maps.googleapis.com/maps/api/staticmap?size={width}x{height}&maptype={type}&path=weight:3%7Ccolor:0x000000ff%7Cenc:{pathRoute}&key={value}";
            using HttpClient client = new HttpClient();
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            using Stream contentStream = await (await client.SendAsync(request)).Content.ReadAsStreamAsync();
            using Stream stream = new FileStream(imagePath, FileMode.Create, FileAccess.Write, FileShare.None, 3145728, useAsync: true);
            await contentStream.CopyToAsync(stream);
        }

        //
        // Resumen:
        //     CONVERTER PARA DICIONARIO
        //
        // Parámetros:
        //   obj:
        //
        // Parámetros de tipo:
        //   TValue:
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

        //
        // Resumen:
        //     GERATE TIME SPAM USE LIB TOKEN
        //
        // Parámetros:
        //   from:
        //
        //   value:
        public static TimeSpan GerateTimeSpan(this string from, double value)
        {
            return from.ToLower() switch
            {
                "fromdays" => TimeSpan.FromDays(value),
                "fromhours" => TimeSpan.FromHours(value),
                "frommilliseconds" => TimeSpan.FromMilliseconds(value),
                "fromseconds" => TimeSpan.FromSeconds(value),
                "fromminutes" => TimeSpan.FromMinutes(value),
                _ => TimeSpan.Zero,
            };
        }

        //
        // Resumen:
        //     CALCULAR IDADE
        //
        // Parámetros:
        //   birthDate:
        public static int CalculeAge(this long birthDate)
        {
            DateTime dateTime = birthDate.TimeStampToDateTime();
            DateTime today = DateTime.Today;
            int num = today.Year - dateTime.Year;
            if (dateTime > today.AddYears(-num))
            {
                num--;
            }

            return num;
        }

        //
        // Resumen:
        //     CALCULAR IDADE
        //
        // Parámetros:
        //   birthDate:
        public static int CalculeAge(this DateTime birthDate)
        {
            DateTime today = DateTime.Today;
            int num = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-num))
            {
                num--;
            }

            return num;
        }

        //
        // Resumen:
        //     gerar hash em Md5
        //
        // Parámetros:
        //   input:
        public static string GerarHashMd5(string input)
        {
            if (string.IsNullOrEmpty(BaseConfig.SecretKey))
            {
                throw new Exception("Informe o secretKey");
            }

            byte[] array = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder stringBuilder = new StringBuilder();
            byte[] array2 = array;
            foreach (byte b in array2)
            {
                stringBuilder.Append(b.ToString("x2"));
            }

            return $"{stringBuilder}{BaseConfig.SecretKey}";
        }

        //
        // Resumen:
        //     CALCULAR DISTANCIA
        //
        // Parámetros:
        //   origemLat:
        //
        //   origemLng:
        //
        //   destinoLat:
        //
        //   destinoLng:
        //
        //   unit:
        //     VALUES K , M , N | DEFAULT = K
        public static double GetDistance(double origemLat, double origemLng, double destinoLat, double destinoLng, char unit = 'K')
        {
            double num = Math.PI * origemLat / 180.0;
            double num2 = Math.PI * destinoLat / 180.0;
            double num3 = origemLng - destinoLng;
            double d = Math.PI * num3 / 180.0;
            double d2 = Math.Sin(num) * Math.Sin(num2) + Math.Cos(num) * Math.Cos(num2) * Math.Cos(d);
            d2 = Math.Acos(d2);
            d2 = d2 * 180.0 / Math.PI;
            d2 = d2 * 60.0 * 1.1515;
            return unit switch
            {
                'K' => d2 * 1.609344,
                'N' => d2 * 0.8684,
                'M' => d2,
                _ => d2,
            };
        }

        //
        // Resumen:
        //     VALIDAR CNPJ
        //
        // Parámetros:
        //   cnpj:
        public static bool ValidCnpj(this string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj))
            {
                return false;
            }

            int[] array = new int[12]
            {
            5, 4, 3, 2, 9, 8, 7, 6, 5, 4,
            3, 2
            };
            int[] array2 = new int[13]
            {
            6, 5, 4, 3, 2, 9, 8, 7, 6, 5,
            4, 3, 2
            };
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
            {
                return false;
            }

            if (cnpj.Distinct().Count() == 1)
            {
                return false;
            }

            string text = cnpj.Substring(0, 12);
            int num = 0;
            for (int i = 0; i < 12; i++)
            {
                num += int.Parse(text[i].ToString()) * array[i];
            }

            int num2 = num % 11;
            string text2 = ((num2 >= 2) ? (11 - num2) : 0).ToString();
            text += text2;
            num = 0;
            for (int j = 0; j < 13; j++)
            {
                num += int.Parse(text[j].ToString()) * array2[j];
            }

            num2 = num % 11;
            text2 += ((num2 >= 2) ? (11 - num2) : 0);
            return cnpj.EndsWith(text2);
        }

        //
        // Resumen:
        //     VALIDAR CPF
        //
        // Parámetros:
        //   cpf:
        public static bool ValidCpf(this string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
            {
                return false;
            }

            cpf = cpf.Trim().OnlyNumbers();
            if (cpf.Distinct().Count() == 1)
            {
                return false;
            }

            if (cpf.Length != 11)
            {
                return false;
            }

            int[] array = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] array2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string text = cpf.Substring(0, 9);
            int num = 0;
            for (int i = 0; i < 9; i++)
            {
                num += int.Parse(text[i].ToString()) * array[i];
            }

            int num2 = num % 11;
            string text2 = ((num2 >= 2) ? (11 - num2) : 0).ToString();
            text += text2;
            num = 0;
            for (int j = 0; j < 10; j++)
            {
                num += int.Parse(text[j].ToString()) * array2[j];
            }

            num2 = num % 11;
            text2 += ((num2 >= 2) ? (11 - num2) : 0);
            return cpf.EndsWith(text2);
        }

        //
        // Resumen:
        //     VALIDAR EMAIL
        //
        // Parámetros:
        //   email:
        public static bool ValidEmail(this string email)
        {
            try
            {
                return new EmailAddressAttribute().IsValid(email);
            }
            catch (Exception)
            {
                return false;
            }
        }

        //
        // Resumen:
        //     PARSE STRING dd/MM/yyyy HH:mm:ss TO DATETIME
        //
        // Parámetros:
        //   dateValue:
        public static DateTime TryParseAnyDate(this string dateValue)
        {
            try
            {
                IFormatProvider provider = new CultureInfo("pt-BR");
                if (DateTime.TryParse(dateValue, provider, DateTimeStyles.None, out var result))
                {
                    return result;
                }

                throw new Exception("Verifique a data informada");
            }
            catch (Exception innerException)
            {
                throw new Exception("Verifique a data informada", innerException);
            }
        }

        //
        // Resumen:
        //     remove all spaces
        //
        // Parámetros:
        //   text:
        public static string TrimSpaces(this string text)
        {
            return text?.Replace(" ", "");
        }

        //
        // Resumen:
        //     GENERATE RANDOM STRING
        //
        // Parámetros:
        //   length:
        public static string RandomString(int length)
        {
            return new string((from s in Enumerable.Repeat("AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYtZz0123456789", length)
                               select s[Random.Next(s.Length)]).ToArray());
        }

        //
        // Resumen:
        //     GENERATE RANDOM STRING
        //
        // Parámetros:
        //   length:
        public static string RandomInt(int length)
        {
            return new string((from s in Enumerable.Repeat("0123456789", length)
                               select s[Random.Next(s.Length)]).ToArray());
        }

        //
        // Resumen:
        //     CONVERTE TIMESTAMP EM DATETIME
        //
        // Parámetros:
        //   unixTimeStamp:
        public static DateTime TimeStampToDateTime(this long unixTimeStamp)
        {
            try
            {
                if (unixTimeStamp == 0L)
                {
                    throw new ArgumentException("Informe um timestamp válido");
                }

                bool isMilliseconds = unixTimeStamp.ToString().Length > 10;

                long unixTimeStampInSeconds = isMilliseconds ? unixTimeStamp / 1000 : unixTimeStamp;
                return DateTimeOffset.FromUnixTimeSeconds(unixTimeStampInSeconds).DateTime.ToLocalTime();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao converter timestamp", ex);
            }
        }

        public static DateTime TimeStampToDateTimeLocalZone(this long unixTimeStamp)
        {
            try
            {
                if (unixTimeStamp == 0L)
                {
                    throw new Exception("Informe um timestamp valido");
                }

                bool isMilliseconds = unixTimeStamp.ToString().Length > 10;

                long unixTimeStampInSeconds = isMilliseconds ? unixTimeStamp / 1000 : unixTimeStamp;

                var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimeStampInSeconds);

                // Convert to UTC-5
                var utcMinus5 = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                return TimeZoneInfo.ConvertTime(dateTimeOffset, utcMinus5).DateTime;
            }
            catch (Exception)
            {
                throw new Exception("Informe um timestamp valido");
            }
        }

        //
        // Resumen:
        //     RETORNA APENAS NUMEROS
        //
        // Parámetros:
        //   text:
        public static string OnlyNumbers(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            text = Regex.Replace(text, "[^\\w\\s]", "");
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            text = Regex.Replace(text, "[^\\d$]", "");
            return text.TrimSpaces();
        }

        //
        // Resumen:
        //     GET INFO BY LOCATION
        //
        // Parámetros:
        //   lat:
        //
        //   lng:
        //
        //   key:
        //     CUSTOM KEY GMAPS
        public static AddressViewModel GetInfoLocation(double lat, double lng, string key = "")
        {
            if (string.IsNullOrEmpty(key))
            {
                key = GetConfigurationRoot().GetValue<string>("googleMapsKey");
                if (string.IsNullOrEmpty(key))
                {
                    throw new Exception("Informe a googleMapsKey no settings/Config.json");
                }
            }

            if (Math.Abs(lat) < 0.0 || Math.Abs(lng) < 0.0)
            {
                return new AddressViewModel
                {
                    Erro = true
                };
            }

            AddressViewModel addressViewModel = new AddressViewModel();
            string text = lat.ToString(CultureInfo.InvariantCulture).Replace(',', '.');
            string text2 = lng.ToString(CultureInfo.InvariantCulture).Replace(',', '.');
            RestClient client = new RestClient("https://maps.googleapis.com/maps/api/geocode/json?sensor=false&language=pt-br&latlng=" + text + "," + text2 + "&key=" + key);
            RestRequest request = new RestRequest(Method.GET);
            IRestResponse<GmapsResult> result = client.Execute<GmapsResult>(request).Result;
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return new AddressViewModel
                {
                    Erro = true,
                    ErroMessage = result?.Data?.ErroMessage
                };
            }

            if (!result.Data.Results.Any())
            {
                return new AddressViewModel
                {
                    Erro = true,
                    ErroMessage = result?.Data?.ErroMessage
                };
            }

            Result result2 = result.Data.Results.First();
            List<AddressComponents> addressComponents = result2.AddressComponents;
            addressViewModel.FormatedAddress = result2.FormattedAddress;
            addressViewModel.Street = addressComponents.FirstOrDefault((AddressComponents x) => x.Types.Any() && x.Types.First() == "route")?.LongName;
            addressViewModel.Number = addressComponents.FirstOrDefault((AddressComponents x) => x.Types.Any() && x.Types.First() == "street_number")?.ShortName;
            addressViewModel.City = addressComponents.FirstOrDefault((AddressComponents x) => x.Types.Any() && x.Types.First() == "administrative_area_level_2")?.ShortName;
            addressViewModel.State = addressComponents.FirstOrDefault((AddressComponents x) => x.Types.Any() && x.Types.First() == "administrative_area_level_1")?.ShortName;
            addressViewModel.Country = addressComponents.FirstOrDefault((AddressComponents x) => x.Types.Any() && x.Types.First() == "country")?.ShortName;
            addressViewModel.Neighborhood = addressComponents.FirstOrDefault((AddressComponents x) => x.Types.Any() && x.Types.First() == "political")?.ShortName;
            addressViewModel.PostalCode = addressComponents.FirstOrDefault((AddressComponents x) => x.Types.Any() && x.Types.First() == "postal_code")?.ShortName;
            addressViewModel.ErroMessage = result.Data?.ErroMessage;
            return addressViewModel;
        }

        //
        // Resumen:
        //     OBTER VALOR DE UMA PORCENTAGEM
        //
        // Parámetros:
        //   totalValue:
        //
        //   percent:
        public static double GetValueOfPercent(this double totalValue, double percent)
        {
            return percent * totalValue / 100.0;
        }

        //
        // Resumen:
        //     OBTER PORCENTAGEM DE UM VALOR
        //
        // Parámetros:
        //   totalValue:
        //
        //   partOfValue:
        public static double GetPercentOfValue(this double totalValue, double partOfValue)
        {
            return partOfValue / totalValue * 100.0;
        }

        //
        // Resumen:
        //     GET INFO BY LOCATION
        //
        // Parámetros:
        //   address:
        //
        //   key:
        //     KEY GMAPS
        public static AddressViewModel GetInfoFromAdressLocation(string address, string key = "")
        {
            if (string.IsNullOrEmpty(key))
            {
                key = GetConfigurationRoot().GetValue<string>("googleMapsKey");
                if (string.IsNullOrEmpty(key))
                {
                    throw new Exception("Informe a googleMapsKey no settings/Config.json");
                }
            }

            AddressViewModel addressViewModel = new AddressViewModel();
            RestClient client = new RestClient("https://maps.googleapis.com/maps/api/geocode/json?new_forward_geocoder=true&address=" + address + "&key=" + key);
            RestRequest request = new RestRequest(Method.GET);
            IRestResponse<GmapsResult> result = client.Execute<GmapsResult>(request).Result;
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return new AddressViewModel
                {
                    Erro = true,
                    ErroMessage = result?.Data?.ErroMessage
                };
            }

            if (!result.Data.Results.Any())
            {
                return new AddressViewModel
                {
                    Erro = true,
                    ErroMessage = result?.Data?.ErroMessage
                };
            }

            Result result2 = result.Data.Results.First();
            List<AddressComponents> addressComponents = result2.AddressComponents;
            addressViewModel.FormatedAddress = result2.FormattedAddress;
            addressViewModel.Street = addressComponents.FirstOrDefault((AddressComponents x) => x.Types.Any() && x.Types.First() == "route")?.LongName;
            addressViewModel.Number = addressComponents.FirstOrDefault((AddressComponents x) => x.Types.Any() && x.Types.First() == "street_number")?.ShortName;
            addressViewModel.City = addressComponents.FirstOrDefault((AddressComponents x) => x.Types.Any() && x.Types.First() == "administrative_area_level_2")?.ShortName;
            addressViewModel.State = addressComponents.FirstOrDefault((AddressComponents x) => x.Types.Any() && x.Types.First() == "administrative_area_level_1")?.ShortName;
            addressViewModel.Country = addressComponents.FirstOrDefault((AddressComponents x) => x.Types.Any() && x.Types.First() == "country")?.ShortName;
            addressViewModel.Neighborhood = addressComponents.FirstOrDefault((AddressComponents x) => x.Types.Any() && x.Types.First() == "political")?.ShortName;
            addressViewModel.PostalCode = addressComponents.FirstOrDefault((AddressComponents x) => x.Types.Any() && x.Types.First() == "postal_code")?.ShortName;
            addressViewModel.Geometry = result2.Geometry;
            addressViewModel.ErroMessage = result.ErrorMessage;
            return addressViewModel;
        }

        //
        // Resumen:
        //     get unique name from pathRoute
        //
        // Parámetros:
        //   name:
        //
        //   savePath:
        //
        //   ext:
        public static string GetUniqueFileName(string name, string savePath, string ext)
        {
            name = name.Replace(ext, "").Replace(" ", "_");
            name = Regex.Replace(name, "[^\\w\\s]", "");
            string text = name;
            int num = 0;
            if (!File.Exists(savePath + text + ext))
            {
                return text.Replace(' ', '_');
            }

            do
            {
                num++;
                text = $"{name}_{num}";
            }
            while (File.Exists(savePath + text + ext));
            return text.Replace(' ', '_');
        }

        //
        // Resumen:
        //     EXCECUTE WITH DESERIALIZE OBJECT
        //
        // Parámetros:
        //   client:
        //
        //   request:
        public static async Task<IRestResponse<T>> Execute<T>(this RestClient client, RestRequest request)
        {
            TaskCompletionSource<IRestResponse> taskCompletion = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, delegate (IRestResponse r)
            {
                taskCompletion.SetResult(r);
            });
            RestResponse restResponse = (RestResponse)(await taskCompletion.Task);
            IRestResponse<T> restResponse2 = restResponse.ToAsyncResponse<T>();
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                restResponse2.Data = JsonConvert.DeserializeObject<T>(restResponse.Content, settings);
            }
            catch
            {
            }

            return restResponse2;
        }

        //
        // Resumen:
        //     EXCECUTE WITH DESERIALIZE OBJECT
        //
        // Parámetros:
        //   client:
        //
        //   request:
        public static async Task<RestResponse> Execute(this RestClient client, RestRequest request)
        {
            TaskCompletionSource<IRestResponse> taskCompletion = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, delegate (IRestResponse r)
            {
                taskCompletion.SetResult(r);
            });
            return (RestResponse)(await taskCompletion.Task);
        }

        //
        // Resumen:
        //     MASCARA DE TELEFONE
        //
        // Parámetros:
        //   telefone:
        //
        //   showZero:
        public static string MaskTelefone(this string telefone, bool showZero = false)
        {
            if (string.IsNullOrEmpty(telefone))
            {
                return telefone;
            }

            telefone = telefone.OnlyNumbers();
            while (telefone.StartsWith("0"))
            {
                telefone = telefone.TrimStart('0');
            }

            if (telefone.Length < 11)
            {
                if (showZero)
                {
                    telefone = "0" + telefone;
                }

                if (!showZero)
                {
                    return Convert.ToUInt64(telefone).ToString("(00) 0000\\-0000");
                }

                return Convert.ToUInt64(telefone).ToString("(000) 0000\\-0000");
            }

            if (telefone.Length > 12)
            {
                return telefone;
            }

            if (telefone.Length > 11)
            {
                telefone = telefone.Substring(0, 11);
            }

            if (showZero)
            {
                telefone = "0" + telefone;
            }

            if (!showZero)
            {
                return Convert.ToUInt64(telefone).ToString("(00) 00000\\-0000");
            }

            return Convert.ToUInt64(telefone).ToString("(000) 00000\\-0000");
        }

        //
        // Resumen:
        //     RETORNO DE SUCESS
        //
        // Parámetros:
        //   message:
        //
        //   data:
        public static ReturnViewModel ReturnSuccess(string message = "Sucesso", object data = null)
        {
            return new ReturnViewModel
            {
                Data = data,
                Message = message
            };
        }

        //
        // Resumen:
        //     RETORNO DE ERRO COM EXCEPTION
        //
        // Parámetros:
        //   ex:
        //
        //   message:
        //
        //   data:
        //
        //   errors:
        //
        //   responseList:
        public static ReturnViewModel ReturnErro(this Exception ex, string message = "Ocorreu um erro, verifique e tente novamente", object data = null, object errors = null, bool responseList = false)
        {
            string messageEx = $"{ex.InnerException} {ex.Message}";
            return new ReturnViewModel
            {
                Data = ((!responseList) ? data : new List<object>()),
                Erro = true,
                Errors = errors,
                MessageEx = messageEx,
                Message = message
            };
        }

        //
        // Resumen:
        //     RETORNO DE ERRO FORÇADO
        //
        // Parámetros:
        //   message:
        //
        //   data:
        //
        //   errors:
        //
        //   responseList:
        //
        //   messageEx:
        public static ReturnViewModel ReturnErro(string message = "Ocorreu um erro, verifique e tente novamente", object data = null, object errors = null, bool responseList = false, string messageEx = null)
        {
            return new ReturnViewModel
            {
                Data = ((!responseList) ? data : new List<object>()),
                Erro = true,
                Errors = errors,
                Message = message,
                MessageEx = messageEx
            };
        }

        //
        // Resumen:
        //     RETORNA PRIMEIRO NOME
        //
        // Parámetros:
        //   text:
        public static string GetLastName(this string text)
        {
            string[] array = text.Trim()?.Split(' ') ?? new string[0];
            if (array.Length != 0)
            {
                return string.Join(" ", array.Skip(1).ToList());
            }

            return "";
        }

        //
        // Resumen:
        //     RETORNA SOBRENOME COMPLETO
        //
        // Parámetros:
        //   text:
        public static string GetFirstName(this string text)
        {
            string[] array = text?.Trim().Split(' ') ?? new string[0];
            if (array.Length == 0)
            {
                return text?.Trim();
            }

            return array[0];
        }

        //
        // Resumen:
        //     REMOVE ESPAÇOS CAMPOS STRING
        //
        // Parámetros:
        //   input:
        //
        //   removeEmoji:
        //
        //   regex:
        //
        // Parámetros de tipo:
        //   TSelf:
        public static TSelf TrimStringProperties<TSelf>(this TSelf input, bool removeEmoji = false, string regex = null)
        {
            if (input == null)
            {
                throw new Exception("Verifique os dados informados. json inválido");
            }

            (from p in input.GetType().GetProperties()
             where p.PropertyType == typeof(string)
             select p).ToList().ForEach(delegate (PropertyInfo stringProperty)
             {
                 string text = (string)stringProperty.GetValue(input, null);
                 if (text != null)
                 {
                     if (removeEmoji && !string.IsNullOrEmpty(text))
                     {
                         text = Regex.Replace(text, "\\p{Cs}", "");
                     }

                     if (!string.IsNullOrEmpty(regex) && !string.IsNullOrEmpty(text))
                     {
                         text = Regex.Replace(text, regex, "");
                     }
                 }

                 stringProperty.SetValue(input, text?.Trim(), null);
             });
            return input;
        }

        //
        // Resumen:
        //     VALIDA CAMPOS
        //
        // Parámetros:
        //   model:
        //
        //   propertyValid:
        [Obsolete("Please use ValidModelState")]
        public static string ValidModel(this object model, string[] propertyValid)
        {
            string result = "Verifique os dados informados e tente novamente.";
            propertyValid = propertyValid ?? new string[0];
            if (model == null)
            {
                return result;
            }

            List<PropertyInfo> source = (from x in model.GetType().GetProperties()
                                         where propertyValid.Contains(x.Name)
                                         select x).ToList();
            result = "Informe os campos: " + (from pi in source
                                              where pi.PropertyType == typeof(string)
                                              let name = pi.Name
                                              let value = (string)pi.GetValue(model)
                                              where string.IsNullOrEmpty(value)
                                              select name).Aggregate("", (string current, string name) => current + " " + name.LowercaseFirst() + "; ");
            if (!(result == "Informe os campos: "))
            {
                return result.Trim().TrimEnd(';');
            }

            return null;
        }

        //
        // Resumen:
        //     VALIDATE LATITUDE FORMAT VALUE
        //
        // Parámetros:
        //   value:
        public static bool LatIsValid(this double value)
        {
            if (!(value < -90.0))
            {
                return value > 90.0;
            }

            return true;
        }

        //
        // Resumen:
        //     VALIDADE LONGITUDE FORMAT VALUE
        //
        // Parámetros:
        //   value:
        public static bool LngIsValid(this double value)
        {
            if (!(value < -180.0))
            {
                return value > 180.0;
            }

            return true;
        }

        //
        // Resumen:
        //     CAST STRING VALUE TO ENUM
        //
        // Parámetros:
        //   value:
        //
        //   ignoreCase:
        //
        // Parámetros de tipo:
        //   T:
        public static T ToEnum<T>(this string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        //
        // Resumen:
        //     valid model from data anotations
        //
        // Parámetros:
        //   modelState:
        //
        //   ignoreFields:
        //     properties ignore
        //
        //   message:
        //     message retrun
        public static ReturnViewModel ValidModelState(this ModelStateDictionary modelState, string[] ignoreFields = null, string message = null)
        {
            try
            {
                if (modelState.IsValid)
                {
                    return null;
                }

                List<string> listFields = ignoreFields?.ToList() ?? new List<string>();


                listFields.Add("Id");

                Dictionary<string, List<string>> dictionary = modelState.Keys.Where((string x) => !listFields.Contains(x)).ToList().ToDictionary((string item) => item.LowercaseFirst(), (string item) => modelState[item].Errors.Select((ModelError x) => x.ErrorMessage).ToList());

                if (dictionary.Keys.Any(key => key.EndsWith(".Id")))
                {
                    var keysToRemove = dictionary.Keys.Where(key => key.EndsWith(".Id")).ToList();

                    foreach (var key in keysToRemove)
                    {
                        dictionary.Remove(key);
                    }
                }

                if (!dictionary.Any())
                {
                    return null;
                }

                if (!string.IsNullOrEmpty(message))
                {
                    return new ReturnViewModel
                    {
                        Errors = dictionary,
                        Erro = true,
                        Message = message
                    };
                }

                message = dictionary.Values.FirstOrDefault()?.FirstOrDefault() ?? "";
                return new ReturnViewModel
                {
                    Errors = modelState.Keys.Where((string x) => !listFields.Contains(x)).ToDictionary((string item) => item, (string item) => modelState[item].Errors.Select((ModelError x) => x.ErrorMessage).ToList()),
                    Erro = true,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return ex.ReturnErro("Ocorreu um erro inesperado");
            }
        }

        //
        // Resumen:
        //     VALIDAÇÃO DE MODEL VIA DATA ANOTATIONS
        //
        // Parámetros:
        //   modelState:
        //
        //   ignoreFields:
        public static ReturnViewModel ValidModelState(this ModelStateDictionary modelState, params string[] ignoreFields)
        {
            try
            {

                if (modelState.IsValid)
                {
                    return null;
                }

                List<string> listFields = ignoreFields?.ToList() ?? new List<string>();
                listFields.Add("Id");

                Dictionary<string, List<string>> dictionary = modelState.Keys.Where((string x) => !listFields.Contains(x)).ToList().ToDictionary((string item) => item.LowercaseFirst(), (string item) => modelState[item].Errors.Select((ModelError x) => x.ErrorMessage).ToList());

                if (dictionary.Keys.Any(key => key.EndsWith(".Id")))
                {
                    var keysToRemove = dictionary.Keys.Where(key => key.EndsWith(".Id")).ToList();

                    foreach (var key in keysToRemove)
                    {
                        dictionary.Remove(key);
                    }
                }


                if (!dictionary.Any())
                {
                    return null;
                }

                string message = dictionary.Values.FirstOrDefault()?.FirstOrDefault() ?? "";
                return new ReturnViewModel
                {
                    Errors = modelState.Keys.Where((string x) => !listFields.Contains(x)).ToDictionary((string item) => item, (string item) => modelState[item].Errors.Select((ModelError x) => x.ErrorMessage).ToList()),
                    Erro = true,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return ex.ReturnErro("Ocorreu um erro inesperado");
            }
        }

        //
        // Resumen:
        //     VALID STATE MODEL ONLY SELECTED FIELDS
        //
        // Parámetros:
        //   modelState:
        //
        //   validFields:
        public static ReturnViewModel ValidModelStateOnlyFields(this ModelStateDictionary modelState, params string[] validFields)
        {
            try
            {
                if (modelState.IsValid)
                {
                    return null;
                }

                List<string> listFields = new List<string>();
                if (validFields != null)
                {
                    listFields = validFields.ToList();
                }

                Dictionary<string, List<string>> dictionary = modelState.Keys.Where((string x) => listFields.Contains(x)).ToList().ToDictionary((string item) => item.LowercaseFirst(), (string item) => modelState[item].Errors.Select((ModelError x) => x.ErrorMessage).ToList());
                if (!dictionary.Any())
                {
                    return null;
                }

                string message = dictionary.Values.FirstOrDefault()?.FirstOrDefault() ?? "";
                return new ReturnViewModel
                {
                    Errors = modelState.Keys.Where((string x) => listFields.Contains(x)).ToDictionary((string item) => item, (string item) => modelState[item].Errors.Select((ModelError x) => x.ErrorMessage).ToList()),
                    Erro = true,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return ex.ReturnErro("Ocorreu um erro inesperado");
            }
        }

        //
        // Resumen:
        //     CLEAR INVALID PROPERTIES IN MODELSTATE
        //
        // Parámetros:
        //   modelState:
        //
        //   properties:
        public static void ClearInvalidProperties(this ModelStateDictionary modelState, params string[] properties)
        {
            foreach (string key in properties)
            {
                modelState[key].Errors.Clear();
                modelState[key].ValidationState = ModelValidationState.Valid;
            }
        }

        //
        // Resumen:
        //     DIFERENÇA ENTRE UNIX DATE EM STRING
        //
        // Parámetros:
        //   dateTime:
        public static string TimeAgo(this long dateTime)
        {
            TimeSpan timeSpan = DateTime.Now.Subtract(dateTime.TimeStampToDateTime().ToLocalTime());
            if (timeSpan <= TimeSpan.FromSeconds(60.0))
            {
                return $"{timeSpan.Seconds} segundos";
            }

            if (timeSpan <= TimeSpan.FromMinutes(60.0))
            {
                return (timeSpan.Minutes > 1) ? $"{timeSpan.Minutes} minutos" : "1 minuto";
            }

            if (timeSpan <= TimeSpan.FromHours(24.0))
            {
                return (timeSpan.Hours > 1) ? $"{timeSpan.Hours} horas" : "1 hora";
            }

            if (timeSpan <= TimeSpan.FromDays(30.0))
            {
                return (timeSpan.Days > 1) ? $"{timeSpan.Days} dias" : "Ontem";
            }

            if (timeSpan <= TimeSpan.FromDays(365.0))
            {
                return (timeSpan.Days > 59) ? $"{timeSpan.Days / 30} meses" : "1 mês";
            }

            return (timeSpan.Days > 729) ? $"{timeSpan.Days / 365} anos" : "1 ano";
        }

        //
        // Resumen:
        //     DIFERENÇA ENTRE HORARIOS EM STRING
        //
        // Parámetros:
        //   dateTime:
        public static string TimeAgo(this DateTime dateTime)
        {
            TimeSpan timeSpan = DateTime.Now.Subtract(dateTime.ToLocalTime());
            if (timeSpan <= TimeSpan.FromSeconds(60.0))
            {
                return $"{timeSpan.Seconds} segundos";
            }

            if (timeSpan <= TimeSpan.FromMinutes(60.0))
            {
                return (timeSpan.Minutes > 1) ? $"{timeSpan.Minutes} minutos" : "1 minuto";
            }

            if (timeSpan <= TimeSpan.FromHours(24.0))
            {
                return (timeSpan.Hours > 1) ? $"{timeSpan.Hours} horas" : "1 hora";
            }

            if (timeSpan <= TimeSpan.FromDays(30.0))
            {
                return (timeSpan.Days > 1) ? $"{timeSpan.Days} dias" : "Ontem";
            }

            if (timeSpan <= TimeSpan.FromDays(365.0))
            {
                return (timeSpan.Days > 59) ? $"{timeSpan.Days / 30} meses" : "1 mês";
            }

            return (timeSpan.Days > 729) ? $"{timeSpan.Days / 365} anos" : "1 ano";
        }

        //
        // Resumen:
        //     OBTEM VALOR DA CLAIM POR NOME
        //
        // Parámetros:
        //   principal:
        //
        //   pCampo:
        public static string GetClaim(this ClaimsPrincipal principal, string pCampo)
        {
            ClaimsIdentity claimsIdentity = principal.Identities.First();
            try
            {
                return claimsIdentity.Claims.FirstOrDefault((Claim p) => p.Type == pCampo)?.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //
        // Resumen:
        //     SET CUSTOM MASK IN STRING
        //
        // Parámetros:
        //   txt:
        //
        //   mask:
        //
        //   pattern:
        public static string SetCustomMask(this string txt, string mask, string pattern)
        {
            try
            {
                if (string.IsNullOrEmpty(txt))
                {
                    return txt;
                }

                return Regex.Replace(txt, pattern, mask);
            }
            catch (Exception)
            {
                return txt;
            }
        }

        //
        // Resumen:
        //     SET MASK PHONE
        //
        // Parámetros:
        //   strNumber:
        public static string SetMaskPhone(this string strNumber)
        {
            try
            {
                string text = strNumber.OnlyNumbers();
                string format = "{0:(00) 0000-0000}";
                long num = Convert.ToInt64(text);
                switch (text.Length)
                {
                    case 11:
                        format = "{0:(00) 00000-0000}";
                        break;
                    case 12:
                        format = "{0:+00 (00) 0000-0000}";
                        break;
                    case 13:
                        format = "{0:+00 (00) 00000-0000}";
                        break;
                }

                return string.Format(format, num);
            }
            catch (Exception)
            {
                return strNumber;
            }
        }

        //
        // Resumen:
        //     RETORNA FOTO DO FACEBOOK DEACORDO COM FACEBOOKID
        //
        // Parámetros:
        //   facebookId:
        //
        //   width:
        public static string GetPhotoFacebookGraph(this string facebookId, int width = 300)
        {
            if (!string.IsNullOrEmpty(facebookId))
            {
                return $"https://graph.facebook.com/{facebookId}/picture?width={width}";
            }

            return null;
        }

        //
        // Resumen:
        //     RETORNA FOTO DO GOOGLE+ DE ACORDO COM GOOGLEID
        //
        // Parámetros:
        //   googleId:
        //
        //   width:
        public static string GetPhotoGooglePlus(this string googleId, int width = 300)
        {
            if (!string.IsNullOrEmpty(googleId))
            {
                return $"https://plus.google.com/s2/photos/profile/{googleId}?sz={width}";
            }

            return null;
        }

        //
        // Resumen:
        //     GET CONFIGURAÇÃO ROOT
        //
        // Parámetros:
        //   path:
        //     APOS O ROOT
        //
        //   fileName:
        //     NOME DO ARQUIVO.JSON
        public static IConfigurationRoot GetConfigurationRoot(string path = "Settings", string fileName = "Config")
        {
            string basePath = Directory.GetCurrentDirectory() + "/" + path;
            return new ConfigurationBuilder().SetBasePath(basePath).AddJsonFile(fileName + ".json").Build();
        }

        //
        // Resumen:
        //     CONVERT DATETIME TO TIMESTAMP
        //
        // Parámetros:
        //   dateTime:
        public static long ToTimeStamp(DateTime dateTime = default(DateTime))
        {
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }

        //
        // Resumen:
        //     CONVERT MONEY TO CENTS EXMPLE R$ 10,00 = 10000
        //
        // Parámetros:
        //   price:
        public static int ToCent(this decimal price)
        {
            return Convert.ToInt32($"{price * 100m:0}");
        }

        //
        // Resumen:
        //     CONVERT MONEY TO CENTS EXMPLE R$ 10,00 = 10000
        //
        // Parámetros:
        //   price:
        public static int ToCent(this double price)
        {
            return Convert.ToInt32($"{price * 100.0:0}");
        }

        //
        // Resumen:
        //     First char to upper
        //
        // Parámetros:
        //   text:
        public static string UppercaseFirst(this string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return char.ToUpper(text[0]) + text.Substring(1);
            }

            return string.Empty;
        }

        //
        // Resumen:
        //     First char to lower
        //
        // Parámetros:
        //   text:
        public static string LowercaseFirst(this string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return char.ToLower(text[0]) + text.Substring(1);
            }

            return string.Empty;
        }

        //
        // Resumen:
        //     get first day of month
        //
        // Parámetros:
        //   dateTime:
        public static DateTime FirstDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        //
        // Resumen:
        //     get last day of month
        //
        // Parámetros:
        //   dateTime:
        public static DateTime LastDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddDays(-1.0);
        }

        //
        // Resumen:
        //     RETURN DECIMAL TO PLACES NOT AROUND
        //
        // Parámetros:
        //   price:
        //     value for notAround
        public static decimal NotAround(this decimal price)
        {
            return Math.Truncate(price * 100m) / 100m;
        }

        //
        // Resumen:
        //     RETURN DECIMAL TO PLACES NOT AROUND
        //
        // Parámetros:
        //   price:
        //     value for notAround
        public static double NotAround(this double price)
        {
            return (double)Math.Truncate((decimal)price * 100m) / 100.0;
        }

        //
        // Resumen:
        //     MAPEAMENTO DE ERRORS
        //
        // Parámetros:
        //   response:
        public static BaseErrorsViewModel MapErrors(this IRestResponse response)
        {
            switch (response.StatusCode)
            {
                case (HttpStatusCode)422:
                    {
                        Errors422ViewModel errors422ViewModel = JsonConvert.DeserializeObject<Errors422ViewModel>(response.Content);
                        string messageError = errors422ViewModel.Errors.FirstOrDefault().Value.FirstOrDefault();
                        Dictionary<string, List<string>> errors = errors422ViewModel.Errors;
                        return new BaseErrorsViewModel
                        {
                            Error = errors,
                            MessageError = messageError
                        };
                    }
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.NotFound:
                    return new BaseErrorsViewModel
                    {
                        Error = null,
                        MessageError = JsonConvert.DeserializeObject<ErrorsViewModel>(response.Content)?.Errors
                    };
                case (HttpStatusCode)0:
                    return new BaseErrorsViewModel
                    {
                        Error = null,
                        MessageError = "Não foi possivel estabelecer uma conexão com o servidor"
                    };
                default:
                    return new BaseErrorsViewModel();
            }
        }

        //
        // Resumen:
        //     METHOD LOG
        //
        // Parámetros:
        //   path:
        public static List<AspNetLogViewModel> GetLog(string path)
        {
            new List<AspNetLogViewModel>();
            try
            {
                string text;
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
                    text = streamReader.ReadToEnd();
                }

                text = text.Replace("\r\n", "");
                text = text.Replace("}{", "},{");
                return JsonConvert.DeserializeObject<List<AspNetLogViewModel>>("[" + text + "]");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static JObject ToJson(this IFormCollection obj)
        {
            dynamic val = new JObject();
            try
            {
                if (obj.Files.Count() > 0)
                {
                    for (int i = 0; i < obj.Files.Count(); i++)
                    {
                        IFormFile file = obj.Files[i];
                        int num = obj.Files.Count((IFormFile x) => x.Name == file.Name);
                        val.Add((num <= 1) ? file.Name : $"{file.Name}[{i}]", file.FileName);
                    }
                }

                if (obj.Keys.Count() > 0)
                {
                    foreach (string key in obj.Keys)
                    {
                        if (obj[key].Count > 1)
                        {
                            JArray jArray = new JArray();
                            for (int j = 0; j < obj[key].Count; j++)
                            {
                                jArray.Add(obj[key][j]);
                            }

                            val.Add(key, jArray);
                        }
                        else
                        {
                            string text = obj[key][0];
                            val.Add(key, text);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return val;
        }

        //
        // Resumen:
        //     dd/MM/yyyy TO UNIX DATE
        //
        // Parámetros:
        //   date:
        public static long? ToUnix(this string date)
        {
            long? result = null;
            try
            {
                if (!string.IsNullOrEmpty(date))
                {
                    DateTime dateTime = (date + " 00:00:00").TryParseAnyDate();
                    result = new DateTimeOffset(dateTime).ToUnixTimeSeconds();
                    return result;
                }
            }
            catch (Exception)
            {
                return result;
            }

            return result;
        }

        //
        // Resumen:
        //     PRÓXIMO DIA DA SEMANA (busca o próximo dia da semana especificado)
        //
        // Parámetros:
        //   today:
        //
        //   dayOfWeek:
        public static DateTime NextDayOfWeek(DateTime today, DayOfWeek dayOfWeek)
        {
            today = today.AddDays((double)(7 - dayOfWeek));
            return today;
        }

        //
        // Resumen:
        //     INDENTA STRING JSON
        //
        // Parámetros:
        //   json:
        public static string JsonPrettify(this string json)
        {
            try
            {
                using StringReader reader = new StringReader(json);
                using StringWriter stringWriter = new StringWriter();
                JsonTextReader reader2 = new JsonTextReader(reader);
                JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter);
                jsonTextWriter.Formatting = Formatting.Indented;
                jsonTextWriter.WriteToken(reader2);
                return stringWriter.ToString();
            }
            catch (Exception)
            {
                return json;
            }
        }

        //
        // Resumen:
        //     GET ROOT NAME OFF XML
        //
        // Parámetros:
        //   xmlString:
        public static string GetRootName(this string xmlString)
        {
            return XDocument.Parse(xmlString).Root?.Name.ToString();
        }

        //
        // Resumen:
        //     MAP XML DYNAMIC
        //
        // Parámetros:
        //   xmlString:
        //
        // Parámetros de tipo:
        //   T:
        public static T MapXmlElement<T>(this string xmlString) where T : class
        {
            try
            {
                string rootName = xmlString.GetRootName();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName ?? "response"));
                T val = null;
                using StringReader textReader = new StringReader(xmlString);
                return (T)xmlSerializer.Deserialize(textReader);
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro ao converter o retorno.", innerException);
            }
        }

        //
        // Resumen:
        //     CREATED LOG FILE
        //
        // Parámetros:
        //   data:
        //
        //   root:
        //
        //   ex:
        public static void LogFile(object data, string root, Exception ex = null)
        {
            Task.Run(delegate
            {
                try
                {
                    StringBuilder stringBuilder = new StringBuilder("");
                    DateTime now = DateTime.Now;
                    stringBuilder.AppendLine($"{now:dd/MM/yyyy HH:mm:ss}");
                    string text = JsonConvert.SerializeObject(data, Formatting.Indented);
                    stringBuilder.AppendLine("Data : " + text).AppendLine();
                    if (ex != null)
                    {
                        stringBuilder.AppendLine($"Exception : {ex.InnerException} {ex.Message}".Trim());
                    }

                    if (!Directory.Exists(root))
                    {
                        Directory.CreateDirectory(root);
                    }

                    File.WriteAllText(root, stringBuilder.ToString());
                }
                catch (Exception)
                {
                }
            });
        }

        //
        // Resumen:
        //     SET PATH IN IMAGE FOR PROFILE
        //
        // Parámetros:
        //   photo:
        //
        //   facebookId:
        //
        //   googleId:
        //
        //   instagramId:
        //
        //   twitterId:
        //
        //   customBaseUrl:
        //
        //   width:
        public static string SetPhotoProfile(this string photo, string facebookId = null, string googleId = null, string instagramId = null, string twitterId = null, string customBaseUrl = null, int width = 600)
        {
            ConfigurationHelper conf = new ConfigurationHelper();

            string text = (string.IsNullOrEmpty(customBaseUrl) ? conf.getBaseUrl() : customBaseUrl);
            Regex regex = new Regex("^(http|https):");
            if (!string.IsNullOrEmpty(photo) && regex.IsMatch(photo))
            {
                return photo;
            }

            if (string.IsNullOrEmpty(photo) && !string.IsNullOrEmpty(facebookId))
            {
                return facebookId.GetPhotoFacebookGraph(width);
            }

            if (string.IsNullOrEmpty(photo) && !string.IsNullOrEmpty(googleId))
            {
                return googleId.GetPhotoGooglePlus(width);
            }

            if (string.IsNullOrEmpty(photo) && !string.IsNullOrEmpty(instagramId))
            {
                return $"https://avatars.io/instagram/{instagramId}/{width}";
            }

            if (string.IsNullOrEmpty(photo) && !string.IsNullOrEmpty(twitterId))
            {
                return $"https://avatars.io/twitter/{twitterId}/{width}";
            }

            if (string.IsNullOrEmpty(photo))
            {
                return BaseConfig.DefaultUrl.SetCurrentProtocol();
            }

            return (text + photo.Replace(text, "")).SetCurrentProtocol();
        }

        //
        // Resumen:
        //     REMOVE PATH IMAGE
        //
        // Parámetros:
        //   image:
        //
        //   customBaseUrl:
        public static string RemovePathImage(this string image, string customBaseUrl = null)
        {
            ConfigurationHelper conf = new ConfigurationHelper();

            string text = (string.IsNullOrEmpty(customBaseUrl) ? conf.getBaseUrl() : customBaseUrl);
            if (string.IsNullOrEmpty(text))
            {
                throw new Exception("Informe a url base");
            }

            if (!string.IsNullOrEmpty(image))
            {
                return Regex.Replace(image, text, "", RegexOptions.IgnoreCase);
            }

            return image;
        }

        //
        // Resumen:
        //     REMOVE PATH IMAGE
        //
        // Parámetros:
        //   image:
        public static string RemovePathImage(this string image)
        {
            return image?.Split('/').LastOrDefault();
        }

        //
        // Resumen:
        //     SET PATH IMAGE
        //
        // Parámetros:
        //   image:
        //
        //   customBaseUrl:
        public static string SetPathImage(this string image, string customBaseUrl = null)
        {
            Regex regex = new Regex("^(http|https):");
            if (!string.IsNullOrEmpty(image) && regex.IsMatch(image))
            {
                return image;
            }

            ConfigurationHelper conf = new ConfigurationHelper();

            string text = (string.IsNullOrEmpty(customBaseUrl) ? conf.getBaseUrl() : customBaseUrl).SetCurrentProtocol();
            if (!string.IsNullOrEmpty(image))
            {
                return text + image.Replace(text, "");
            }

            return BaseConfig.DefaultUrl.SetCurrentProtocol();
        }


        //
        // Resumen:
        //     SHOW TIME IN FORMAT TIME AGO
        //
        // Parámetros:
        //   span:
        public static string ToReadableString(this TimeSpan span)
        {
            string text = ((span.Duration().Days > 0) ? string.Format("{0:0} dia{1}, ", span.Days, (span.Days == 1) ? string.Empty : "s") : string.Empty) + ((span.Duration().Hours > 0) ? string.Format("{0:0} hora{1}, ", span.Hours, (span.Hours == 1) ? string.Empty : "s") : string.Empty) + ((span.Duration().Minutes > 0) ? string.Format("{0:0} minuto{1}, ", span.Minutes, (span.Minutes == 1) ? string.Empty : "s") : string.Empty) + ((span.Duration().Seconds > 0) ? string.Format("{0:0} segundo{1}", span.Seconds, (span.Seconds == 1) ? string.Empty : "s") : string.Empty);
            if (text.EndsWith(", "))
            {
                text = text.Substring(0, text.Length - 2);
            }

            if (string.IsNullOrEmpty(text))
            {
                text = "0 segundos";
            }

            return text;
        }

        //
        // Resumen:
        //     CONVERTER STRING EM ENUM
        //
        // Parámetros:
        //   str:
        //
        // Parámetros de tipo:
        //   T:
        public static T ToEnum<T>(this string str)
        {
            Type typeFromHandle = typeof(T);
            string[] names = Enum.GetNames(typeFromHandle);
            foreach (string text in names)
            {
                if (((EnumMemberAttribute[])typeFromHandle.GetField(text).GetCustomAttributes(typeof(EnumMemberAttribute), inherit: true)).Single().Value == str)
                {
                    return (T)Enum.Parse(typeFromHandle, text);
                }
            }

            return default(T);
        }

        public static T ToEnumv2<T>(this string value) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(value));
            }

            return (T)Enum.Parse(typeof(T), value);
        }

        //
        // Resumen:
        //     OBTER VALOR DE UMA COLUNA DO EXCEL
        //
        // Parámetros:
        //   col:
        //
        //   tnew:
        //
        //   val:
        //
        // Parámetros de tipo:
        //   T:
        public static bool SetValueCustom<T>(this ExportInfo col, T tnew, ExcelRange val)
        {
            try
            {
                if (val.Value == null)
                {
                    col.Property.SetValue(tnew, null);
                    return true;
                }

                if (col.Property.PropertyType == typeof(bool))
                {
                    col.Property.SetValue(tnew, val.GetValue<bool>());
                    return true;
                }

                if (col.Property.PropertyType == typeof(int))
                {
                    col.Property.SetValue(tnew, val.GetValue<int>());
                    return true;
                }

                if (col.Property.PropertyType == typeof(long))
                {
                    col.Property.SetValue(tnew, val.GetValue<long>());
                    return true;
                }

                if (col.Property.PropertyType == typeof(double))
                {
                    col.Property.SetValue(tnew, val.GetValue<double>());
                    return true;
                }

                if (col.Property.PropertyType == typeof(decimal))
                {
                    col.Property.SetValue(tnew, val.GetValue<decimal>());
                    return true;
                }

                if (col.Property.PropertyType == typeof(DateTime))
                {
                    col.Property.SetValue(tnew, val.GetValue<DateTime>());
                    return true;
                }

                if (col.Property.PropertyType == typeof(TimeSpan))
                {
                    col.Property.SetValue(tnew, val.GetValue<TimeSpan>());
                    return true;
                }

                if (col.Property.PropertyType == typeof(string))
                {
                    col.Property.SetValue(tnew, val.GetValue<string>());
                    return true;
                }

                PropertyInfo propertyInfo = tnew.GetType().GetProperties().ToList()
                    .Find((PropertyInfo x) => x.Name.Equals(col.Property.Name, StringComparison.CurrentCultureIgnoreCase));
                if (propertyInfo != null && propertyInfo.PropertyType.GetTypeInfo().IsEnum)
                {
                    col.Property.SetValue(tnew, GetEnumFromDescription(val.GetValue<string>(), propertyInfo.PropertyType));
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //
        // Resumen:
        //     Metodo para adicionar ..em texto longo
        //
        // Parámetros:
        //   text:
        //
        //   maxChars:
        public static string Truncate(this string text, int maxChars)
        {
            if (!string.IsNullOrEmpty(text) && text.Length > maxChars && maxChars != 0)
            {
                return text.Substring(0, maxChars - 3) + "...";
            }

            return text;
        }

        //
        // Resumen:
        //     OBTER SEPARADOR CSV OU TXT
        //
        // Parámetros:
        //   csvFilePath:
        public static char DetectSeparator(string csvFilePath)
        {
            return DetectSeparator(File.ReadAllLines(csvFilePath));
        }

        //
        // Resumen:
        //     OBTER SEPARADOR CSV OU TXT
        //
        // Parámetros:
        //   lines:
        public static char DetectSeparator(string[] lines)
        {
            return (from sep in SeparatorChars
                    select new
                    {
                        Separator = sep,
                        Found = from line in lines
                                group line by line.Count((char ch) => ch == sep)
                    } into res
                    orderby res.Found.Count((IGrouping<int, string> grp) => grp.Key > 0) descending, res.Found.Count()
                    select res).First().Separator;
        }

        //
        // Resumen:
        //     OBTER CAMPO COM ERRO NA LEITURA DO CSV OU TXT
        //
        // Parámetros:
        //   column:
        //
        //   line:
        //
        // Parámetros de tipo:
        //   T:
        public static string GetFieldError<T>(int column, int line)
        {
            try
            {
                PropertyInfo propertyInfo = typeof(T).GetProperties().FirstOrDefault((PropertyInfo x) => x.GetCustomAttribute<Column>().ColumnIndex == column);
                if (propertyInfo == null)
                {
                    return $"O valor da coluna {column} na linha {line} está inválido";
                }

                return "O valor do campo \"" + (propertyInfo.GetCustomAttribute<DisplayAttribute>()?.Name ?? propertyInfo.Name) + "\" está inválido";
            }
            catch (Exception ex)
            {
                return $"{ex.InnerException} {ex.Message}".TrimEnd();
            }
        }

        //
        // Resumen:
        //     METODO PARA EXPORTAR PARA CSV
        //
        // Parámetros:
        //   itens:
        //
        //   delimiter:
        //
        //   cultureInfo:
        //
        // Parámetros de tipo:
        //   T:
        public static string ExportToCsv<T>(this IList<T> itens, string delimiter = ";", CultureInfo cultureInfo = null) where T : class
        {
            var source = (from x in typeof(T).GetProperties()
                          where x.GetCustomAttribute<JsonIgnoreAttribute>() == null
                          select new
                          {
                              Order = (x.GetCustomAttribute<JsonPropertyAttribute>()?.Order ?? 0),
                              Prop = (x.GetCustomAttribute<DisplayAttribute>(inherit: false)?.Name ?? x.Name)
                          } into x
                          orderby x.Order
                          select x).ToList();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Join(delimiter, source.Select(x => x.Prop)));
            int i;
            for (i = 0; i < itens.Count; i++)
            {
                new List<object>();
                var list = (from x in itens[i].GetType().GetProperties()
                            where x.GetCustomAttribute<JsonIgnoreAttribute>() == null
                            select new
                            {
                                Order = (x.GetCustomAttribute<JsonPropertyAttribute>()?.Order ?? 0),
                                Value = x.GetValue(itens[i], null),
                                Type = x.PropertyType
                            } into x
                            orderby x.Order
                            select x).ToList();
                string text = string.Empty;
                foreach (var item in list)
                {
                    if (item.Value != null)
                    {
                        string text2 = "";
                        text2 = ((cultureInfo == null) ? item.Value.ToString() : ((item.Type == typeof(double)) ? double.Parse(item.Value.ToString(), cultureInfo.NumberFormat).ToString(cultureInfo) : ((item.Type != typeof(decimal)) ? item.Value.ToString() : decimal.Parse(item.Value.ToString(), cultureInfo.NumberFormat).ToString(cultureInfo))));
                        text2 = text2.Replace("\"", "\"\"");
                        if (text2.Contains(delimiter))
                        {
                            text2 = "\"" + text2 + "\"";
                        }

                        if (text2.Contains("\r"))
                        {
                            text2 = text2.Replace("\r", " ");
                        }

                        if (text2.Contains("\n"))
                        {
                            text2 = text2.Replace("\n", " ");
                        }

                        text = text + text2 + delimiter;
                    }
                    else
                    {
                        text = text + string.Empty + delimiter;
                    }
                }

                stringBuilder.AppendLine(text.Remove(text.Length - delimiter.Length));
            }

            return stringBuilder.ToString();
        }

        //
        // Resumen:
        //     MAPEAR BOLEANO EM STRING - SIM / NÃO PARA EXCEL
        //
        // Parámetros:
        //   value:
        //
        //   defaultResponse:
        public static string MapBoolean(this bool? value, string defaultResponse = "Não informado")
        {
            if (!value.HasValue)
            {
                return defaultResponse;
            }

            if (value != false)
            {
                return "SIM";
            }

            return "NÃO";
        }

        //
        // Resumen:
        //     FORMATAR VALORES EM REAIS PARA EXCEL
        //
        // Parámetros:
        //   value:
        //
        //   format:
        //
        //   defaultResponse:
        public static string MapMoney(this double? value, string format = "C", string defaultResponse = "Não informado")
        {
            if (!value.HasValue)
            {
                return defaultResponse;
            }

            return value.GetValueOrDefault().ToString(format);
        }

        //
        // Resumen:
        //     OBTER IP REMOTO
        public static string GetClientIp()
        {
            return HttpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }

        //
        // Resumen:
        //     Formatar data como string DD/MM/YYYY HH:MM
        //
        // Parámetros:
        //   unixTime:
        //
        //   format:
        //
        //   defaultResponse:
        public static string MapUnixTime(this long? unixTime, string format = "dd/MM/yyyy HH:mm", string defaultResponse = "Não informado")
        {
            if (!unixTime.HasValue || unixTime == 0)
            {
                return defaultResponse;
            }

            return unixTime.GetValueOrDefault().TimeStampToDateTime().ToString(format);
        }

        //
        // Resumen:
        //     Formatar data como string DD/MM/YYYY HH:MM
        //
        // Parámetros:
        //   date:
        //
        //   format:
        //
        //   defaultResponse:
        public static string MapDateTime(this DateTime? date, string format = "dd/MM/yyyy HH:mm", string defaultResponse = "Não informado")
        {
            if (!date.HasValue || date == default(DateTime))
            {
                return defaultResponse;
            }

            return date.GetValueOrDefault().ToString(format);
        }

        //
        // Resumen:
        //     METODO PARA LER XLSX E CONVERTER PARA LISTA DE CLASS
        //
        // Parámetros:
        //   worksheet:
        //
        // Parámetros de tipo:
        //   T:
        public static IList<T> ConvertSheetToObjects<T>(this ExcelWorksheet worksheet) where T : new()
        {
            try
            {
                Func<CustomAttributeData, bool> columnOnly = (CustomAttributeData y) => y.AttributeType == typeof(Column);
                List<ExportInfo> list = (from x in typeof(T).GetProperties()
                                         where x.CustomAttributes.Count(columnOnly) > 0
                                         select x into p
                                         select new ExportInfo
                                         {
                                             Property = p,
                                             Column = p.GetCustomAttributes<Column>().First().ColumnIndex
                                         }).ToList();
                IOrderedEnumerable<int> source = from x in worksheet.Cells.Select((ExcelRangeBase cell) => cell.Start.Row).Distinct()
                                                 orderby x
                                                 select x;
                List<T> list2 = new List<T>();
                for (int i = 2; i <= source.Count(); i++)
                {
                    T val = new T();
                    bool flag = false;
                    for (int j = 0; j < list.Count(); j++)
                    {
                        ExportInfo exportInfo = list[j];
                        ExcelRange val2 = worksheet.Cells[i, exportInfo.Column];
                        if (exportInfo.SetValueCustom(val, val2) && !flag)
                        {
                            flag = true;
                        }
                    }

                    if (flag)
                    {
                        list2.Add(val);
                    }
                }

                return list2;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //
        // Resumen:
        //     METODO PARA OBTER VALOR DE UM HEADER
        //
        // Parámetros:
        //   headers:
        //
        //   key:
        public static string GetValue(this IHeaderDictionary headers, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new Exception("Informe a key");
                }

                headers.TryGetValue(key, out var value);
                return value.ToString();
            }
            catch (Exception)
            {
            }

            return null;
        }

        //
        // Resumen:
        //     METODO PARA REMOVER ACENTOS
        //
        // Parámetros:
        //   text:
        public static string RemoveAccents(this string text)
        {
            StringBuilder stringBuilder = new StringBuilder();
            char[] array = text.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char c in array)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }

        //
        // Resumen:
        //     METODO PARA OBTER VALOR INT DE UM ENUM PELO DESCRIPTION DELE
        //
        // Parámetros:
        //   enumVal:
        //     VALOR DO ENUM
        //
        //   enumType:
        //     TIPO DO ENUM
        public static object GetEnumFromDescription(string enumVal, Type enumType)
        {
            return ((FieldInfo)enumType.GetMembers().ToList().Find((MemberInfo x) => x.GetCustomAttribute<EnumMemberAttribute>(inherit: false)?.Value == enumVal))?.GetValue(enumType);
        }

        //
        // Resumen:
        //     METODO PARA OBTER ENUM PELO MEMBER
        //
        // Parámetros:
        //   value:
        //
        // Parámetros de tipo:
        //   T:
        public static string GetEnumMemberValue<T>(this T value) where T : struct, IConvertible
        {
            return typeof(T).GetTypeInfo().DeclaredMembers.SingleOrDefault((MemberInfo x) => x.Name == value.ToString())?.GetCustomAttribute<EnumMemberAttribute>(inherit: false)?.Value;
        }

        //
        // Resumen:
        //     EXPORT LIST ENTITY TO TABLE IN EXCEL FILE
        //
        // Parámetros:
        //   entitys:
        //
        //   path:
        //
        //   workSheetName:
        //
        //   fileName:
        //
        //   hexBColor:
        //
        //   hexTxtColor:
        //
        //   autoFit:
        //
        //   ext:
        //
        // Parámetros de tipo:
        //   T:
        public static void ExportToExcel<T>(List<T> entitys, string path, string workSheetName = "Result", string fileName = "Export", string hexBColor = null, string hexTxtColor = null, bool autoFit = true, string ext = ".xlsx")
        {
            Color colorFromHex = GetColorFromHex(Color.FromArgb(68, 114, 196), hexBColor);
            Color colorFromHex2 = GetColorFromHex(Color.White, hexTxtColor);
            string path2 = fileName.Split('.')[0] + ext;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            FileInfo fileInfo = new FileInfo(Path.Combine(path, path2));
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
                fileInfo = new FileInfo(Path.Combine(path, path2));
            }

            using ExcelPackage excelPackage = new ExcelPackage(fileInfo);
            ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add(workSheetName);
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Count(); i++)
            {
                DisplayAttribute customAttribute = properties[i].GetCustomAttribute<DisplayAttribute>();
                excelWorksheet.Cells[1, i + 1].Value = customAttribute?.Name ?? properties[i].Name;
            }

            string text = excelWorksheet.Cells[1, properties.Count()]?.Address;
            if (entitys.Any())
            {
                int row = 2; // Start after header
                foreach (var entity in entitys)
                {
                    int col = 1;
                    foreach (var prop in entity.GetType().GetProperties())
                    {
                        excelWorksheet.Cells[row, col].Value = prop.GetValue(entity);
                        col++;
                    }
                    row++;
                }
            }


            using (ExcelRange excelRange = excelWorksheet.Cells["A1:" + (text ?? "AD1")])
            {
                excelRange.Style.Font.Bold = true;
                excelRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                excelRange.Style.Fill.BackgroundColor.SetColor(colorFromHex);
                excelRange.Style.Font.Color.SetColor(colorFromHex2);
            }

            if (autoFit)
            {
                excelWorksheet.Cells.AutoFitColumns();
            }

            excelPackage.Save();
        }

        //
        // Resumen:
        //     GET COLOR ENTITY FOR STRING HEX OF COLOR
        //
        // Parámetros:
        //   defaultColor:
        //
        //   hexColor:
        public static Color GetColorFromHex(Color? defaultColor, string hexColor)
        {
            if (string.IsNullOrEmpty(hexColor))
            {
                return defaultColor ?? Color.Black;
            }

            if (!hexColor.Contains("#"))
            {
                hexColor = "#" + hexColor;
            }

            try
            {
                return FromHtml(hexColor);
            }
            catch (Exception)
            {
                return defaultColor ?? Color.Black;
            }
        }

        private static Color FromHtml(String hexColor)
        {
            SKColor skColor = SKColor.Parse(hexColor);
            return Color.FromArgb(skColor.Alpha, skColor.Red, skColor.Green, skColor.Blue);
        }

        private static bool IsEmptyColor(Color c)
        {
            return c == null || (c.A == 0 && c.R == 0 && c.G == 0 && c.B == 0);
        }

        //
        // Resumen:
        //     DISTINCTY BY PROPERTY
        //
        // Parámetros:
        //   source:
        //
        //   keySelector:
        //
        // Parámetros de tipo:
        //   TSource:
        //
        //   TKey:
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource item in source)
            {
                if (seenKeys.Add(keySelector(item)))
                {
                    yield return item;
                }
            }
        }

        //
        // Resumen:
        //     ADD PARAMETER QUERY STRING IN URL WITH CLASS
        //
        // Parámetros:
        //   url:
        //
        //   entityParameters:
        //
        // Parámetros de tipo:
        //   T:
        public static string AddQueryStringParameters<T>(this string url, T entityParameters)
        {
            string text = "";
            try
            {
                if (object.Equals(entityParameters, null))
                {
                    return url;
                }

                if (string.IsNullOrEmpty(url))
                {
                    throw new Exception("Informe o campo URL para incluir os parametros na url");
                }

                PropertyInfo[] properties = typeof(T).GetProperties();
                foreach (PropertyInfo propertyInfo in properties)
                {
                    object value = propertyInfo.GetValue(entityParameters, null);
                    if (!object.Equals(value, null))
                    {
                        string text2 = propertyInfo.GetAttribute<JsonPropertyAttribute>()?.PropertyName ?? propertyInfo.Name?.LowercaseFirst();
                        if (!string.IsNullOrEmpty(text2))
                        {
                            text += $"{text2}={value}&";
                        }
                    }
                }

                url += ("?" + text).TrimEnd('&', '?').Trim();
                return url;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //
        // Resumen:
        //     METODO PARA ATUALIZAR ARQUIVO JSON
        //
        // Parámetros:
        //   key:
        //
        //   value:
        //
        //   locationAndFileName:
        //
        // Parámetros de tipo:
        //   T:
        public static void AddOrUpdatePropertyInFileJson<T>(string key, T value, string locationAndFileName = "appSettings.json")
        {
            try
            {
                string path = Path.Combine(AppContext.BaseDirectory, locationAndFileName);
                dynamic val = JsonConvert.DeserializeObject(File.ReadAllText(path));
                string text = key.Split(':')[0];
                if (string.IsNullOrEmpty(text))
                {
                    val[text] = value;
                }
                else
                {
                    string text2 = key.Split(':')[1];
                    val[text][text2] = value;
                }

                string contents = JsonConvert.SerializeObject(val, Formatting.Indented);
                File.WriteAllText(path, contents);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //
        // Resumen:
        //     LER JSON E MAPEAR EM UMA CLASS
        //
        // Parámetros:
        //   path:
        //
        // Parámetros de tipo:
        //   T:
        public static T LoadJson<T>(this string path)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }

        //
        // Resumen:
        //     METODO PARA CRIPTOGRAFAR UM TEXTO
        //
        // Parámetros:
        //   key:
        //
        //   plainText:
        public static string EncryptString(string key, string plainText)
        {
            byte[] iV = new byte[16];
            byte[] inArray;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iV;
                ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);
                using MemoryStream memoryStream = new MemoryStream();
                using CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
                using (StreamWriter streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(plainText);
                }

                inArray = memoryStream.ToArray();
            }

            return Convert.ToBase64String(inArray);
        }

        //
        // Resumen:
        //     METODO PARA DESCRIPTOGRAFAR UM TEXTO
        //
        // Parámetros:
        //   key:
        //
        //   cipherText:
        public static string DecryptString(string key, string cipherText)
        {
            byte[] iV = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iV;
            ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
            using MemoryStream stream = new MemoryStream(buffer);
            using CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
            using StreamReader streamReader = new StreamReader(stream2);
            return streamReader.ReadToEnd();
        }
    }

}
