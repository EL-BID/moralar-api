
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Style;

namespace Moralar.UtilityFramework.Application.Core.ViewModels
{
    public class DtParameters
    {
        //
        // Resumen:
        //     Draw counter. This is used by DataTables to ensure that the Ajax returns from
        //     server-side processing requests are drawn in sequence by DataTables (Ajax requests
        //     are asynchronous and thus can return out of sequence). This is used as part of
        //     the draw return parameter (see below).
        public int Draw { get; set; }

        //
        // Resumen:
        //     An array defining all columns in the table.
        public DtColumn[] Columns { get; set; }

        //
        // Resumen:
        //     An array defining how many columns are being ordering upon - i.e. if the array
        //     length is 1, then a single column sort is being performed, otherwise a multi-column
        //     sort is being performed.
        public DtOrder[] Order { get; set; }

        //
        // Resumen:
        //     Paging first record indicator. This is the start point in the current data set
        //     (0 index based - i.e. 0 is the first record).
        public int Start { get; set; }

        //
        // Resumen:
        //     Number of records that the table can display in the current draw. It is expected
        //     that the number of records returned will be equal to this number, unless the
        //     server has fewer records to return. Note that this can be -1 to indicate that
        //     all records should be returned (although that negates any benefits of server-side
        //     processing!)
        public int Length { get; set; }

        //
        // Resumen:
        //     Global search value. To be applied to all columns which have searchable as true.
        public DtSearch Search { get; set; }

        //
        // Resumen:
        //     Custom column that is used to further sort on the first Order column.
        public string SortOrder
        {
            get
            {
                if (Columns == null || Order == null || Order.Length == 0)
                {
                    return null;
                }

                object obj = Columns[Order[0].Column]?.Name;
                if (obj == null)
                {
                    DtColumn dtColumn = Columns.FirstOrDefault((DtColumn x) => x.Orderable);
                    if (dtColumn == null)
                    {
                        return null;
                    }

                    obj = dtColumn.Name;
                }

                return (string)obj;
            }
        }
    }

}
