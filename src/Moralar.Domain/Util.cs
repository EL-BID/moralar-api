using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

using Moralar.Data.Entities;
using Moralar.Data.Entities.Auxiliar;
using Moralar.Data.Enum;
using Moralar.Domain.ViewModels;
using Moralar.Domain.ViewModels.Family;
using Moralar.Domain.ViewModels.ResidencialProperty;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.DataValidation.Contracts;
using OfficeOpenXml.Style;
using RestSharp;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.ViewModels;
using Moralar.UtilityFramework.Application.Core.JwtMiddleware;
using Moralar.UtilityFramework.Configuration;

using NodaTime;
using NodaTime.Text;

namespace Moralar.Domain
{
    public static class Util
    {
        public static List<string> AcceptedFiles = new List<string>() { ".xlsx", ".xls" };

        private static IStringLocalizer _localizer { get; set; }
        public static string GetClientIp() => Utilities.HttpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        public static Dictionary<string, string> GetTemplateVariables()
        {

            var dataBody = new Dictionary<string, string>();
            try
            {
                dataBody.Add("__bg-cardbody__", "#F2F3F3");
                dataBody.Add("__bg-cardfooter__", "#A5559A");
                dataBody.Add("__cl-body__", "#000000");

                ConfigurationHelper conf = new ConfigurationHelper();

                dataBody.Add("{{ baseUrl }}", conf.getBaseUrl().Replace("upload", "images"));
                dataBody.Add("{{ contact }}", Utilities.GetConfigurationRoot().GetSection("contactEmail").Get<string>());
                dataBody.Add("{{ appName }}", BaseConfig.ApplicationName);
            }
            catch (Exception)
            {

                //unused
            }

            return dataBody;

        }

        public static long? ToUnixCustom(this string date)
        {
            try
            {

                var dateTime = date.TryParseAnyDate();

                return new DateTimeOffset(dateTime).ToUnixTimeSeconds();

            }
            catch (Exception)
            {
                return (long?)null;
            }
        }

        public static long? ToUnixCustomV2(string date)
        {
            try
            {
                if (!string.IsNullOrEmpty(date))
                {
                    string input = date.Replace(" 00:00:00", " 05:00:00");
                    var formats = new[] { "yyyy-MM-dd HH:mm:ss", "dd/MM/yyyy HH:mm:ss", "dd-MM-yyyy HH:mm:ss",
                        "MM/dd/yyyy HH:mm:ss", "dd-MM-yyyy HH:mm:ss", "yyyy/MM/dd HH:mm:ss" };
                    DateTime dateTime;

                    if (DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dateTime))
                    {
                        var instant = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
                        return new DateTimeOffset(instant).ToUnixTimeSeconds();
                    }
                }
                return (long?)null;
            }
            catch (Exception)
            {
                return (long?)null;
            }
        }



        /// <summary>
        ///  CONVERTER STRING EM ENUM
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToEnumCustom<T>(this string str)
        {
            var enumType = typeof(T);
            foreach (var name in Enum.GetNames(enumType))
            {
                var enumMemberAttribute =
                    ((EnumMemberAttribute[])enumType.GetField(name)
                        .GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
                if (enumMemberAttribute.Value == str) return (T)Enum.Parse(enumType, name);
            }

            //throw exception or whatever handling you want or
            return default;
        }

        public static async Task<List<TV>> ReadAndValidationExcel<TV>(IFormFile file, int sheetIndex = 0) where TV : new()
        {
                ReturnViewModel isInvalidState = null;
                var response = new List<TV>();
                
                string listErrors = null;
                try
                {
                    using (ExcelPackage package = new ExcelPackage(file.OpenReadStream()))
                    {
                        ExcelWorksheet sheet = package.Workbook.Worksheets[sheetIndex];
                        var listEntityViewModel = sheet.ConvertSheetToObjects<TV>();


                        int lastRowNumber = listEntityViewModel.Count();
                        for (int i = 0; i < listEntityViewModel.Count(); i++)
                        {
                            isInvalidState = listEntityViewModel[i].ModelIsValid(customStart: "R" + (i + 2).ToString());


                            int typeError = 0;
                            if (isInvalidState != null && isInvalidState.Message != null)
                            {
                                if (isInvalidState.Message.Contains("Data_de_Nascimento"))
                                {
                                    var val = ((listEntityViewModel.ToList())[i] as Moralar.Domain.ViewModels.Family.FamilyImportViewModel).Data_de_Nascimento;
                                    typeError = val == null ? (int)TypeErrorExcel.Null : (int)TypeErrorExcel.WrongDate;
                                    if (
                                        // TODO: Disable format on deploy to cloud
                                        !validDateStringFormat(val) ||
                                        val == null)
                                    {
                                        listErrors += isInvalidState.Message + ";" + typeError + (lastRowNumber.Equals(i + 1) ? "" : ',');
                                    }
                                }
                                else
                                {
                                    typeError = GetFieldTypeInfo(listEntityViewModel[i]);
                                    listErrors += isInvalidState.Message + ";" + typeError + (lastRowNumber.Equals(i + 1) ? "" : ',');
                                }
                            }
                        }

                        if (listErrors is not null) { 
                            throw new Exception(listErrors);
                        }

                        response = listEntityViewModel.ToList();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(listErrors);
                }

            return response;
        }

        private static bool excludeFields(ReturnViewModel isInvalidState)
        {
            return (!isInvalidState.Message.Contains("Data_de_Nascimento"));
        }

        public static bool validDateStringFormat(string val)
        {
            try
            {
                DateTime tempDate;
                var formats = new[] { "yyyy-MM-dd HH:mm:ss", "dd/MM/yyyy HH:mm:ss", "dd-MM-yyyy HH:mm:ss",
                    "MM/dd/yyyy HH:mm:ss", "dd-MM-yyyy HH:mm:ss", "yyyy/MM/dd HH:mm:ss" };
                bool isValidFormat = DateTime.TryParseExact(val, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate);
                return isValidFormat;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Function to determine field type info
        private static int GetFieldTypeInfo(object entity)
        {
            int index = 0;
            foreach (var property in entity.GetType().GetProperties())
            {
                var value = property.GetValue(entity, null);

                if (value is not null)
                {
                    int typeError = 0;

                    switch (Type.GetTypeCode(value.GetType()))
                    {
                        case TypeCode.Int32:
                        case TypeCode.Double:
                            typeError = 1;
                            break;
                        default:
                            break;
                    }
                    index++;
                    return typeError;
                }
            }
            return 0;
        }
        

        public static IList<T> ConvertSheetToObjects<T>(this ExcelWorksheet worksheet) where T : new()
        {
            Func<CustomAttributeData, bool> func = (CustomAttributeData y) => y.AttributeType == typeof(Column);
            List<ExportInfo> list = typeof(T).GetProperties().Select((PropertyInfo p, int i) => new ExportInfo
            {
                Property = p,
                Column = (p.GetCustomAttributes<Column>().FirstOrDefault()?.ColumnIndex ?? (i + 1))
            }).ToList();
            IOrderedEnumerable<int> source = from x in worksheet.Cells.Select((ExcelRangeBase cell) => cell.Start.Row).Distinct()
                                             orderby x
                                             select x;
            List<T> list2 = new List<T>();
            for (int j = 2; j <= source.Count(); j++)
            {
                T val = new T();
                bool flag = false;
                for (int k = 0; k < list.Count(); k++)
                {
                    ExportInfo exportInfo = list[k];
                    ExcelRange excelRange = worksheet.Cells[j, exportInfo.Column];
                    if (!string.IsNullOrEmpty(excelRange.GetValue<string>()) && exportInfo.SetValueCustom(val, excelRange) && !flag)
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

        public static FamilyHolder SetHolder(this FamilyImportViewModel model)
        {
            var response = new FamilyHolder();

            try
            {
                response.Scholarity = model.Escolaridade.ToEnumCustom<TypeScholarity>();
                response.Birthday = ToUnixCustomV2(model.Data_de_Nascimento);
                response.Cpf = model.Cpf_do_titular.OnlyNumbers();
                response.Email = string.IsNullOrEmpty(model.E_mail) ? null : model.E_mail.ToLower();
                response.Genre = model.Genero.ToEnumCustom<TypeGenre>();
                response.Name = model.Nome_do_titular;
                response.Phone = model.Telefone.OnlyNumbers();
                response.Number = model.Numero_do_cadastro;

            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }

        public static List<TypeSubject> GetTypeSubjectTimeline()
            => new List<TypeSubject>() { TypeSubject.ReuniaoPGM, TypeSubject.EscolhaDoImovel, TypeSubject.Mudanca, TypeSubject.AcompanhamentoPosMudança };

        public static FamilySpouse SetSpouse(this FamilyImportViewModel model)
        {
            var response = new FamilySpouse();
            try
            {
                response.Birthday = ToUnixCustomV2(model.Data_de_Nascimento_do_Conjuge);
                response.Genre = string.IsNullOrEmpty(model.Genero_Conjuge) == false ? model.Genero_Conjuge.ToEnumCustom<TypeGenre>() : (TypeGenre?)null;
                response.SpouseScholarity = string.IsNullOrEmpty(model.Escolaridade_Conjuge) == false ? model.Escolaridade_Conjuge.ToEnumCustom<TypeScholarity>() : (TypeScholarity?)null;
                response.Name = model.Nome_do_Conjuge;
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }
        public static async Task<FamilyAddress> SetAddress(this FamilyImportViewModel model)
        {
            var response = new FamilyAddress();
            try
            {
                var infoAddressByZipCode = await GetInfoZipCode(model.CEP);

                if (infoAddressByZipCode == null)
                    throw new Exception("Cep não encontrado");

                response.CEP = model.CEP.OnlyNumbers();
                response.StreetAddress = model.StreetAddress;
                response.Number = model.Number;
                response.CityId = infoAddressByZipCode.CityId;
                response.CityName = model.CityName;
                response.StateId = infoAddressByZipCode.StateId;
                response.StateName = model.StateName;
                response.StateUf = model.StateUf;
                response.Complement = model.Complement;
                response.Neighborhood = model.Neighborhood;

            }
            catch (Exception)
            {

                throw;
            }

            return response;

        }

        public static string StripHTML(this string input)
            => Regex.Replace(input, "<.*?>", String.Empty);

        public static FamilyFinancial SetFinancial(this FamilyImportViewModel model)
        {
            var response = new FamilyFinancial();
            try
            {
                response.FamilyIncome = (decimal)model.Renda_Familiar.ToDouble();
                response.IncrementValue = (decimal)model.Valor_Incremento.ToDouble();
                response.MaximumPurchase = (decimal)model.Valor_para_compra_de_imovel.ToDouble();
                response.PropertyValueForDemolished = (decimal)model.Valor_imovel_demolido.ToDouble();
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }
        public static FamilyPriorization SetPriorization(this FamilyImportViewModel model)
        {
            var response = new FamilyPriorization();

            try
            {
                response.WorkFront = model.Frente_de_Obras.MapPriorityRate(1);
                response.PermanentDisabled = model.Deficiencia_que_demande_imovel_acessivel.MapPriorityRate(2);
                response.ElderlyOverEighty = model.Idoso_acima_de_80_anos.MapPriorityRate(3);
                response.WomanServedByProtectiveMeasure = model.Mulher_atendida_por_medida_protetiva.MapPriorityRate(4);
                response.FemaleBreadwinner = model.Mulher_chefe_de_familia.MapPriorityRate(5);
                response.SingleParent = model.Monoparental.MapPriorityRate(6);
                response.FamilyWithMoreThanFivePeople = model.Familia_com_mais_5_pessoas.MapPriorityRate(7);
                response.ChildUnderEighteen = model.Filhos_menores_de_18_anos.MapPriorityRate(8);
                response.HeadOfHouseholdWithoutIncome = model.Chefe_de_familia_sem_renda.MapPriorityRate(9);
                response.BenefitOfContinuedProvision = model.Beneficio_de_prestacao_continuada.MapPriorityRate(10);
                response.FamilyPurse = model.Bolsa_Familia.MapPriorityRate(11);
                response.InvoluntaryCohabitation = model.Coabitacao_involuntaria.MapPriorityRate(12);
                response.FamilyIncomeOfUpTwoMinimumWages = model.Renda_familiar_de_ate_dois_salarios_minimos.MapPriorityRate(13);
                response.YearsInSextyAndSeventyNine = model.Idoso_60_ate_79_anos.MapPriorityRate(14);
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }

        public static PriorityRate MapPriorityRate(this string value, int rate)
        {
            return new PriorityRate()
            {
                Rate = rate,
                Value = value.ToBoolean()
            };
        }

        public static string MapReason(this string reason)
            => string.IsNullOrEmpty(reason) ? "" : $"Motivo: {reason}";

        public static ResidencialPropertyFeatures MapFetures(this ResidencialPropertyImportViewModel model)
        {
            var response = new ResidencialPropertyFeatures();
            try
            {

                response.TypeProperty = model.TypeProperty.ToEnum<TypeProperty>();
                response.HasAccessLadder = model.HasAccessLadder.ToBoolean();
                response.CondominiumValue = model.CondominiumValue.ToDouble();
                response.FloorLocation = model.FloorLocation.ToString();
                response.TypeGasInstallation = model.TypeGasInstallation.ToEnum<TypePropertyGasInstallation>();
                response.HasAccessRamp = model.HasAccessRamp.ToBoolean();
                response.SquareFootage = model.SquareFootage.ToDouble();
                response.HasAdaptedToPcd = model.HasAdaptedToPcd.ToBoolean();
                response.HasCistern = model.HasCistern.ToBoolean();
                response.HasElavator = model.HasElavator.ToBoolean();
                response.PropertyValue = model.PropertyValue.ToDouble();
                response.PropertyRegularization = model.PropertyRegularization.ToEnumv2<TypePropertyRegularization>();
                response.NumberOfBedrooms = model.NumberOfBedrooms.ToInt();
                response.HasGarage = model.HasGarage.ToBoolean();
                response.HasServiceArea = model.HasServiceArea.ToBoolean();
                response.HasWall = model.HasWall.ToBoolean();
                response.HasYard = model.HasYard.ToBoolean();
                response.IptuValue = model.IptuValue.ToDouble();
                response.NumberFloors = model.NumberFloors.ToInt();
                response.NumberOfBathrooms = model.NumberOfBathrooms.ToInt();

            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

        public static async Task<ResidencialPropertyAdress> MapAddress(this ResidencialPropertyImportViewModel model)
        {

            var response = new ResidencialPropertyAdress();

            try
            {
                var infoAddressByZipCode = await GetInfoZipCode(model.CEP);

                if (infoAddressByZipCode == null)
                    throw new Exception("Cep não encontrado");

                var infoLocation = Utilities.GetInfoFromAdressLocation($"{infoAddressByZipCode.StreetAddress}, {model.Number} - {infoAddressByZipCode.Neighborhood}, {infoAddressByZipCode.CityName} - {infoAddressByZipCode.StateUf}");

                response.CEP = model.CEP.OnlyNumbers();
                response.CityId = infoAddressByZipCode.CityId;
                response.CityName = infoAddressByZipCode.CityName;
                response.Neighborhood = infoAddressByZipCode.Neighborhood;
                response.Complement = model.Complement;
                response.Number = model.Number;
                response.StateId = infoAddressByZipCode.StateId;
                response.StateName = infoAddressByZipCode.StateName;
                response.StreetAddress = infoAddressByZipCode.StreetAddress;
                response.StateUf = infoAddressByZipCode.StateUf;

                if (infoLocation.Erro == false)
                {
                    response.Latitude = infoLocation.Geometry.Location.Lng;
                    response.Longitude = infoLocation.Geometry.Location.Lng;
                }

            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }

        public static bool CheckHasField(string[] fields, string field)
         => fields.Count(x => x.ToLower() == field.ToLower()) > 0;

        public static async Task<InfoAddressViewModel> GetInfoZipCode(string zipCode)
        {
            try
            {
                var zipCodeFormat = zipCode?.OnlyNumbers().PadLeft(8, '0');

                if (string.IsNullOrEmpty(zipCodeFormat))
                    throw new Exception("Informe um CEP");

                ConfigurationHelper conf = new ConfigurationHelper();


                var client = new RestClient(conf.getBaseUrl().Replace("/content/upload/", null));
                var request = new RestRequest($"/api/v1/City/GetInfoFromZipCode/{zipCodeFormat}", Method.GET);


                request.AddHeader("accept", "application/json");
                request.AddHeader("content-type", "application/json");

                var response = await client.Execute<ReturnViewModel>(request);

                if (response.StatusCode != HttpStatusCode.BadRequest && response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Ocorreu um erro ao informações do CEP {zipCode}");

                if (response.Data == null || response.Data.Erro)
                    throw new Exception(response.Data?.Message ?? $"CEP {zipCode} não encontrado");

                return JsonConvert.DeserializeObject<InfoAddressViewModel>(JsonConvert.SerializeObject(response.Data.Data));

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string ToReal(this decimal price, bool notAround = false)
        {
            return notAround
                ? string.Format(new CultureInfo("pt-BR"), "{0:C}", price)
                : string.Format(new CultureInfo("pt-BR"), "{0:C}", Math.Truncate(price * 100) / 100);
        }

        public static string ToReal(this double price, bool notAround = false) => notAround
                ? string.Format(new CultureInfo("pt-BR"), "{0:C}", price)
                : string.Format(new CultureInfo("pt-BR"), "{0:C}", Math.Truncate(price * 100) / 100);

        public static string MapBoolean(this bool value)
         => value == true ? "Sim" : "Não";
        public static int ToInt(this string value)
        {
            var response = 0;
            try
            {
                int.TryParse(value, out response);
            }
            catch (Exception) {/*UNUSED*/}

            return response;
        }

        public static bool ToBoolean(this string value)
            => value?.ToLower() == "sim" ? true : false;


        public static double ToDouble(this string balanceAvailableForWithdraw)
        {
            var value = 0D;
            try
            {
                var pattern = @"(R\$|BRL)";

                balanceAvailableForWithdraw = Regex.Replace(balanceAvailableForWithdraw, pattern, string.Empty).Trim();
                balanceAvailableForWithdraw = balanceAvailableForWithdraw.Replace(',', '.');

                double.TryParse(balanceAvailableForWithdraw, NumberStyles.Any, CultureInfo.InvariantCulture, out value);

                return value;
            }
            catch (Exception)
            {
                return value;
            }
        }

        public static Color GetColorFromHex(string hex)
        {
            return (Color)new ColorConverter().ConvertFromString(hex);
        }



        /// <summary>
        ///  EXPORT LIST ENTITY TO TABLE IN EXCEL FILE
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="path"></param>
        /// <param name="workSheetName"></param>
        /// <param name="fileName"></param>
        /// <param name="hexBColor"></param>
        /// <param name="hexTxtColor"></param>
        /// <param name="autoFit"></param>
        /// <param name="ext"></param>
        public static void ExportToExcelMultiWorksheet(string path, List<string> workSheetNames,
            string fileName = "Export", string hexBColor = null, string hexTxtColor = null, bool autoFit = true,
            string ext = ".xlsx")
        {
            Color bColor = string.IsNullOrEmpty(hexBColor) ?
                Color.FromArgb(68, 114, 196) : GetColorFromHex(hexBColor);
            var txtColor = string.IsNullOrEmpty(hexTxtColor) ? Color.White : GetColorFromHex(hexTxtColor);

            var sFileName = $"{fileName.Split('.')[0]}{ext}";

            #region FilePrepare

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var file = new FileInfo(Path.Combine(path, sFileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(path, sFileName));
            }

            #endregion

            using (var package = new ExcelPackage(file))
            {

                var worksheet = package.Workbook.Worksheets.Add(workSheetNames[0]);

                var t = typeof(FamilyImportViewModel);
                var headings = t.GetProperties();
                for (var a = 0; a < headings.Count(); a++)
                {
                    int num = a + 1;
                    var name = headings[a].GetCustomAttribute<DisplayAttribute>();

                    DisplayAttribute customAttribute = headings[a].GetCustomAttribute<DisplayAttribute>();

                    worksheet.Cells[1, a + 1].Value = name?.Name ?? headings[a].Name;
                    DropDownExcel customAttribute2 = headings[a].GetCustomAttribute<DropDownExcel>();

                    if (customAttribute2 == null)
                    {
                        continue;
                    }

                    Array values = Enum.GetValues(customAttribute2.Options);
                    List<string> list = new List<string>();
                    foreach (int item in values)
                    {
                        list.Add(customAttribute2.Options.GetEnumMemberValueCustom(Enum.GetName(customAttribute2.Options, item)));
                    }

                    string address1 = ExcelCellBase.GetAddress(2, num, 1048576, num);
                    IExcelDataValidationList excelDataValidationList = worksheet.DataValidations.AddListValidation(address1);
                    excelDataValidationList.ShowErrorMessage = true;
                    excelDataValidationList.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                    excelDataValidationList.ErrorTitle = "Valor inválido";
                    excelDataValidationList.Error = "Selecione um item da lista";
                    for (int j = 0; j < list.Count(); j++)
                    {
                        excelDataValidationList.Formula.Values.Add(list[j]);
                    }

                    excelDataValidationList.AllowBlank = customAttribute2.AllowBlank;
                    excelDataValidationList.Validate();

                }

                var address = worksheet.Cells[1, headings.Count()]?.Address;

                using (var rng = worksheet.Cells[$"A1:{address ?? "AD1"}"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(bColor);
                    rng.Style.Font.Color.SetColor(txtColor);
                }

                if (autoFit)
                    worksheet.Cells.AutoFitColumns();

                var worksheet2 = package.Workbook.Worksheets.Add(workSheetNames[1]);


                var t2 = typeof(FamilyMemberImportViewModel);
                var headings2 = t2.GetProperties();
                for (var a = 0; a < headings2.Count(); a++)
                {
                    int num = a + 1;
                    var name = headings2[a].GetCustomAttribute<DisplayAttribute>();
                    worksheet2.Cells[1, a + 1].Value = name?.Name ?? headings2[a].Name;

                    DropDownExcel customAttribute2 = headings2[a].GetCustomAttribute<DropDownExcel>();

                    if (customAttribute2 == null)
                    {
                        continue;
                    }

                    Array values = Enum.GetValues(customAttribute2.Options);
                    List<string> list = new List<string>();
                    foreach (int item in values)
                    {
                        list.Add(customAttribute2.Options.GetEnumMemberValueCustom(Enum.GetName(customAttribute2.Options, item)));
                    }

                    string address1 = ExcelCellBase.GetAddress(2, num, 1048576, num);
                    IExcelDataValidationList excelDataValidationList = worksheet2.DataValidations.AddListValidation(address1);
                    excelDataValidationList.ShowErrorMessage = true;
                    excelDataValidationList.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                    excelDataValidationList.ErrorTitle = "Valor inválido";
                    excelDataValidationList.Error = "Selecione um item da lista";
                    for (int j = 0; j < list.Count(); j++)
                    {
                        excelDataValidationList.Formula.Values.Add(list[j]);
                    }

                    excelDataValidationList.AllowBlank = customAttribute2.AllowBlank;
                    excelDataValidationList.Validate();

                }

                var address2 = worksheet2.Cells[1, headings2.Count()]?.Address;

                using (var rng = worksheet2.Cells[$"A1:{address2 ?? "AD1"}"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(bColor);
                    rng.Style.Font.Color.SetColor(txtColor);
                }

                if (autoFit)
                    worksheet2.Cells.AutoFitColumns();

                package.Save(); //Save the workbook.
            }

        }

        /// <summary>
        /// JUNTAR LISTA DE STRING EM UMA STRING = RESULT (MAÇA, BANANA, ETC)
        /// </summary>
        /// <param name="list"></param>
        /// <param name="aggregate"></param>
        /// <returns></returns>
        public static string CustomJoin(this List<string> list, string aggregate = ", ")
        {
            try
            {
                return string.Join(aggregate, list)?.Trim()?.TrimEnd(',');
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static string MapAnswer(this string questionId, List<QuestionAnswerAux> questionAnswers)
        {
            var listAnswer = new List<string>();
            try
            {
                for (int i = 0; i < questionAnswers.Count(); i++)
                {
                    if (questionAnswers[i].QuestionId == questionId)
                        listAnswer.Add(SplitAnswers(questionAnswers[i].AnswerDescription));
                }

            }
            catch (Exception) { }

            return listAnswer.Distinct().ToList().CustomJoin();
        }

        public static string SplitAnswers(string input)
        {
            if (input.Contains('{'))
            {
                // Remove the curly braces and split the string
                var items = input.Trim('{', '}').Split(new[] { "}{" }, StringSplitOptions.None);

                // Join the items with a comma
                return string.Join(", ", items);
            } else
            {
                return input;
            }
        }


        /// <summary>
        ///  EXPORT LIST ENTITY TO TABLE IN EXCEL FILE
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <param name="path"></param>
        /// <param name="workSheetName">NOME DA WORKSHEET</param>
        /// <param name="fileName"></param>
        /// <param name="hexBColor"></param>
        /// <param name="hexTxtColor"></param>
        /// <param name="autoFit">AUTO RESIZE DA COLUNA</param>
        /// <param name="ext"></param>
        /// <param name="addWorksheet">NOVA WORKSHEET EM ARQUIVO EXISTENTE</param>
        /// <param name="forceText">FORÇAR FORMATAÇÃO TEXTO EM TODOS CAMPOS</param>
        public static void ExportToExcel<T>(List<T> entitys, string path, string workSheetName = "Result",
            string fileName = "Export", string hexBColor = null, string hexTxtColor = null, bool autoFit = true,
            string ext = ".xlsx", bool addWorksheet = false, bool forceText = false)
        {
            Color bColor = string.IsNullOrEmpty(hexBColor) ?
                Color.FromArgb(68, 114, 196) : GetColorFromHex(hexBColor);
            var txtColor = string.IsNullOrEmpty(hexTxtColor) ? Color.White : GetColorFromHex(hexTxtColor);

            var sFileName = $"{fileName.Split('.')[0]}{ext}";

            #region FilePrepare

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var file = new FileInfo(Path.Combine(path, sFileName));
            if (file.Exists && addWorksheet == false)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(path, sFileName));
            }

            #endregion

            using (var package = new ExcelPackage(file))
            {
                var worksheet = package.Workbook.Worksheets.Add(workSheetName);

                //HEADER TABLE
                var t = typeof(T);
                var headings = t.GetProperties();
                for (var i = 0; i < headings.Count(); i++)
                {
                    var column = i + 1;
                    var name = headings[i].GetCustomAttribute<DisplayAttribute>();
                    worksheet.Cells[1, column].Value = name?.Name ?? headings[i].Name;

                    var dropDownExcel = headings[i].GetCustomAttribute<DropDownExcel>();


                    if (forceText)
                        worksheet.Cells[ExcelRange.GetAddress(2, column, ExcelPackage.MaxRows, column)].Style.Numberformat.Format = "@";


                    if (dropDownExcel != null)
                    {
                        var enumValues = Enum.GetValues(dropDownExcel.Options);

                        var options = new List<string>();

                        foreach (int value in enumValues)
                            options.Add(dropDownExcel.Options.GetEnumMemberValueCustom(Enum.GetName(dropDownExcel.Options, value)));

                        var range = ExcelRange.GetAddress(2, column, ExcelPackage.MaxRows, column);

                        var validation = worksheet.DataValidations.AddListValidation(range);

                        validation.ShowErrorMessage = true;
                        validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                        validation.ErrorTitle = "Valor inválido";
                        validation.Error = "Selecione um item da lista";

                        for (int item = 0; item < options.Count(); item++)
                            validation.Formula.Values.Add(options[item]);

                        validation.AllowBlank = dropDownExcel.AllowBlank;
                        validation.Validate();
                    }
                }

                var address = worksheet.Cells[1, headings.Count()]?.Address;

                //BODY DA TABLE
                // OLD MODE
                //if (entitys.Any()) worksheet.Cells["A2"].LoadFromCollection(entitys, 0);
                // NEW MODE
                if (entitys.Any())
                {
                    int row = 2; // Start after header
                    foreach (var entity in entitys)
                    {
                        int col = 1;
                        foreach (var prop in entity.GetType().GetProperties())
                        {
                            worksheet.Cells[row, col].Value = prop.GetValue(entity);
                            col++;
                        }
                        row++;
                    }
                }

                using (var rng = worksheet.Cells[$"A1:{address ?? "AD1"}"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(bColor);
                    rng.Style.Font.Color.SetColor(txtColor);
                }

                if (autoFit)
                    worksheet.Cells.AutoFitColumns();

                package.Save(); //Save the workbook.
            }
        }



        public static ReturnViewModel ModelIsValid<TEntity>(this TEntity entity, bool onlyValidFields = false, string[] customValidFields = null, string[] ignoredFields = null, string customStart = null) where TEntity : new()
        {
            var context = new ValidationContext(entity);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            var result = Validator.TryValidateObject(entity, context, validationResults, true);

            return ModelIsValid(validationResults, onlyValidFields, customValidFields, ignoredFields, customStart);
        }

        public static ReturnViewModel ModelIsValid(List<System.ComponentModel.DataAnnotations.ValidationResult> validationResults, bool onlyValidFields = false, string[] customValidFields = null, string[] ignoredFields = null, string startMessage = null)
        {

            string memberName = null;

            if (validationResults != null && validationResults.Count() > 0)
            {
                var errors = new Dictionary<string, string>();

                for (int i = 0; i < validationResults.Count; i++)
                {
                    var validationItem = validationResults[i];
                    memberName = validationItem.MemberNames.FirstOrDefault();

                    if (validationItem.ErrorMessage.Contains("Verifique o formato"))
                    {
                        errors.Add(memberName, validationItem.ErrorMessage);
                    }

                    if (string.IsNullOrEmpty(memberName) ||
                        (ignoredFields != null && ignoredFields.Count(x => x.ContainsIgnoreCase(memberName)) > 0))
                    {
                        continue;
                    }
                }
                bool haveErrors = memberName != null;
                return new ReturnViewModel()
                {
                    Erro = haveErrors,
                    Errors = errors,
                    Message = haveErrors  ? $"{startMessage}::{memberName.Replace(" ", "_")}"  : null
                };
            }
            else
            {
                return null;

            }

        }

        public static Claim SetRole(TypeProfile typeProfile) => new Claim(ClaimTypes.Role, Enum.GetName(typeProfile.GetType(), typeProfile));
        public static Claim GetUserName(this HttpRequest httpRequest)
        {
            var userName = httpRequest.GetClaimFromToken("UserName");
            if (string.IsNullOrEmpty(userName) == false)
                return new Claim("UserName", userName);
            return null;
        }


        public static List<string> GetDiferentFields<T, TY>(this T target, TY source) where T : class
          where TY : class
        {

            var response = new List<string>();
            try
            {
                foreach (var prop in source.GetType().GetProperties())
                {
                    var targetValue = Utilities.GetValueByProperty(target, prop.Name);

                    var sourceValue = Utilities.GetValueByProperty(source, prop.Name);

                    if (Equals(targetValue, sourceValue) == false)
                        response.Add(prop.Name);
                }
            }
            catch (Exception)
            {

                /*unused*/
            }
            return response;
        }
        public static string GetFieldError<T>(int column, int line)
        {
            try
            {
                var propertyInfo = typeof(T).GetProperties().FirstOrDefault(x => x.GetCustomAttribute<Column>().ColumnIndex == column);

                if (propertyInfo == null)
                    return $"O valor da coluna {column} na linha {line} está inválido";

                return $"O valor do campo \"{propertyInfo.GetCustomAttribute<DisplayAttribute>()?.Name ?? propertyInfo.Name}\" está inválido";

            }
            catch (Exception ex)
            {

                return $"{ex.InnerException} {ex.Message}".TrimEnd();
            }
        }

        public static TypeProfile GetRole(this HttpRequest request)
        {

            var role = request.GetClaimFromToken(ClaimTypes.Role);

            if (string.IsNullOrEmpty(role))
                throw new ArgumentNullException(nameof(ClaimTypes.Role), "Tipo de usuário não identificado");

            return (TypeProfile)Enum.Parse(typeof(TypeProfile), role);
        }
        public static List<SelectItemEnumViewModel> GetMembersOfEnum<T>()
        {
            try
            {
                if (typeof(T).GetTypeInfo().IsEnum == false)
                    throw new ArgumentException("Type must be an enum");

                return Enum.GetValues(typeof(T))
                    .Cast<T>()
                    .Select(x => new SelectItemEnumViewModel()
                    {
                        Value = (int)(object)x,
                        Name = x.ToString(),
                    }).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static Language GetCurrentLocale(this HttpRequest request)
        {
            try
            {
                if (request.Headers.Keys.Count(x => x == "Accept-Language") > 0)
                {
                    return (Language)Enum.Parse(typeof(Language), request.Headers.GetValue("Accept-Language"), true);
                }
            }
            catch (Exception)
            {
                /**/
            }
            return Language.En;
        }

        public static T SetIfDifferentCustom<T, TY>(this T target, TY source) where T : class
                   where TY : class
        {
            var allProperties = source.GetType().GetProperties().ToList();

            for (int i = 0; i < allProperties.Count(); i++)
            {
                var sourceValue = Utilities.GetValueByProperty(source, allProperties[i].Name);

                // VERIFICA SE EXISTE VALOR OU ACEITA NULL
                if (Equals(sourceValue, null) == false)
                    Utilities.SetPropertyValue(target, allProperties[i].Name, sourceValue);
            }
            return target;
        }

        /// <summary>
        ///  METODO PARA OBTER OS CAMPOS QUE FORAM RECEBIDOS NO JSON  UTILIZADO EM CONJUNTO COM SETIFDIFERENT PARA SIMULAR
        ///  METODO PUT
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string[] GetFieldsFromBodyCustom(this IHttpContextAccessor httpContext)
        {
            var dic = new string[] { };
            try
            {

                if (httpContext.HttpContext.Request.Body.CanSeek)
                {
                    httpContext.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(httpContext.HttpContext.Request.Body))
                    {
                        var body = reader.ReadToEnd();
                        dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(body)
                            .Select(x => x.Key?.UppercaseFirst()).ToArray();
                    }
                }
            }
            catch (Exception)
            {
                //ignored
            }

            return dic;
        }

        public static (bool Birthday, bool Gender) HasValidMember(Family entityFamily, int startTargetAudienceAge, int endTargetAudienceAge, TypeGenre? typeGender)
        {
            try
            {
                var validGender = false;
                var listAges = new List<int>();
                var listTypeGender = new List<TypeGenre>();

                if (entityFamily.Holder.Birthday != null)
                {
                    listAges.Add(entityFamily.Holder.Birthday.Value.TimeStampToDateTime().CalculeAge());
                }

                if (entityFamily.Holder.Genre != null)
                {
                    listTypeGender.Add(entityFamily.Holder.Genre.GetValueOrDefault());
                }

                for (int i = 0; i < entityFamily.Members.Count(); i++)
                {
                    if (entityFamily.Members[i].Birthday > 0)
                        listAges.Add(entityFamily.Members[i].Birthday.TimeStampToDateTime().CalculeAge());
                }

                var validBirthday = listAges.Count(age => age >= startTargetAudienceAge && age <= endTargetAudienceAge) > 0;

                if (typeGender == TypeGenre.Todos)
                {
                    validGender = true;
                }
                else
                {
                    validGender = (typeGender == null || listTypeGender.Count(x => x == typeGender) > 0);
                }

                return (validBirthday, validGender);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public static dynamic GetPayloadPush(RouteNotification route = RouteNotification.System)
        {

            dynamic payloadPush = new JObject();

            payloadPush.route = (int)route;

            return payloadPush;

        }

        public static dynamic GetSettingsPush()
        {

            dynamic settings = new JObject();

            settings.ios_badgeType = "Increase";
            settings.ios_badgeCount = 1;
            //settings.android_channel_id = ""; /*solicitar para equipe mobile*/

            return settings;

        }


        //
        // Summary:
        //     METODO PARA OBTER O ATRIBUTO MEMBERVALUE
        //
        // Parameters:
        //   typeEnum:
        //
        //   value:
        //
        // Type parameters:
        //   T:
        //
        // Exceptions:
        //   T:System.Exception:
        public static string GetEnumMemberValueCustom<T>(this Type typeEnum, T value)
        {
            if (!typeEnum.GetTypeInfo().IsEnum)
            {
                throw new Exception("Informe typo Enum");
            }

            return typeEnum.GetTypeInfo().DeclaredMembers.SingleOrDefault((MemberInfo x) => x.Name == value.ToString())?.GetCustomAttribute<EnumMemberAttribute>(inherit: false)?.Value;
        }

        public static string MapUnixTime(this long? unixTime, string format = "dd/MM/yyyy HH:mm", string defaultResponse = "Não informado")
        {
            if (!unixTime.HasValue || unixTime == 0)
            {
                return defaultResponse;
            }

            return unixTime.GetValueOrDefault().TimeStampToDateTime().ToString(format);
        }

        public static List<ResidencialPropertyImportViewModel> formatDataOfResidencialPropertyImportViewModel(List<ResidencialPropertyImportViewModel> model)
        {
            foreach (var item in model)
            {
                foreach (var property in item.GetType().GetProperties())
                {
                    if (property.PropertyType == typeof(string))
                    {
                        var value = (string)property.GetValue(item);
                        if (value != null)
                        {
                            // Remove ",00" if it exists
                            if (value.EndsWith(",00"))
                            {
                                value = value.Substring(0, value.Length - 3);
                            }
                            // Remove all "." characters
                            value = value.Replace(".", "");
                            property.SetValue(item, value);
                        }
                    }
                }
            }
            return model;
        }

        public static string ShortenDescription(string description, int maxLength)
        {
            if (description.Length <= maxLength)
            {
                return description;
            }

            string[] words = description.Split(' ');
            string shortDescription = "";

            foreach (var word in words)
            {
                if ((shortDescription + word).Length > maxLength)
                {
                    break;
                }
                shortDescription += word + " ";
            }

            return shortDescription.TrimEnd() + "..";
        }

    }
}
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Property)]
public class DropDownExcel : Attribute
{
    public Type Options
    {
        get;
        set;
    }

    public bool AllowBlank
    {
        get;
        set;
    }
}
