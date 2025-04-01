using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace Moralar.UtilityFramework.Application.Core.ViewModels
{
    public class DtColumn
    {
        //
        // Resumen:
        //     Column's data source, as defined by columns.data.
        public string Data { get; set; }

        //
        // Resumen:
        //     Column's name, as defined by columns.name.
        public string Name { get; set; }

        //
        // Resumen:
        //     Flag to indicate if this column is searchable (true) or not (false). This is
        //     controlled by columns.searchable.
        public bool Searchable { get; set; }

        //
        // Resumen:
        //     Flag to indicate if this column is orderable (true) or not (false). This is controlled
        //     by columns.orderable.
        public bool Orderable { get; set; }

        //
        // Resumen:
        //     Specific search value.
        public DtSearch Search { get; set; }
    }

}
