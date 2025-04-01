namespace Moralar.UtilityFramework.Application.Core.ViewModels
{
    public class DtResult<T>
    {
        //
        // Resumen:
        //     The draw counter that this object is a response to - from the draw parameter
        //     sent as part of the data request. Note that it is strongly recommended for security
        //     reasons that you cast this parameter to an integer, rather than simply echoing
        //     back to the client what it sent in the draw parameter, in order to prevent Cross
        //     Site Scripting (XSS) attacks.
        public int Draw { get; set; }

        //
        // Resumen:
        //     Total records, before filtering (i.e. the total number of records in the database)
        public int RecordsTotal { get; set; }

        //
        // Resumen:
        //     Total records, after filtering (i.e. the total number of records after filtering
        //     has been applied - not just the number of records being returned for this page
        //     of data).
        public int RecordsFiltered { get; set; }

        //
        // Resumen:
        //     The data to be displayed in the table. This is an array of data source objects,
        //     one for each row, which will be used by DataTables. Note that this parameter's
        //     name can be changed using the ajaxDT option's dataSrc property.
        public List<T> Data { get; set; }

        //
        // Resumen:
        //     INFORMA SE DEU ERRO
        public bool Erro { get; set; }

        //
        // Resumen:
        //     MENSAGEM DE ERRO
        public string MessageEx { get; set; }

        public DtResult()
        {
            Data = new List<T>();
        }
    }

}
