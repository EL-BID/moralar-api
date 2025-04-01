
namespace Moralar.UtilityFramework.Application.Core.ViewModels
{
    public class AddressViewModel
    {
        //
        // Resumen:
        //     ENDEREÇO FORMATADO
        public string FormatedAddress { get; set; }

        //
        // Resumen:
        //     RUA
        public string Street { get; set; }

        //
        // Resumen:
        //     NUMERO
        public string Number { get; set; }

        //
        // Resumen:
        //     ESTADO
        public string State { get; set; }

        //
        // Resumen:
        //     CIDADE
        public string City { get; set; }

        //
        // Resumen:
        //     PAIS
        public string Country { get; set; }

        //
        // Resumen:
        //     CEP
        public string PostalCode { get; set; }

        //
        // Resumen:
        //     BAIRROS
        public string Neighborhood { get; set; }

        //
        // Resumen:
        //     BAIRRO
        public Geometry Geometry { get; set; }

        //
        // Resumen:
        //     INFORMA SE OCORREU UM ERRO
        public bool Erro { get; set; }

        //
        // Resumen:
        //     MENSAGEM DE ERRO
        public string ErroMessage { get; set; }
    }

}
