namespace Moralar.UtilityFramework.Application.Core.ViewModels
{
    public class DtOrder
    {
        //
        // Resumen:
        //     Column to which ordering should be applied. This is an index reference to the
        //     columns array of information that is also submitted to the server.
        public int Column { get; set; }

        //
        // Resumen:
        //     Ordering direction for this column. It will be dt-string asc or dt-string desc
        //     to indicate ascending ordering or descending ordering, respectively.
        public DtOrderDir Dir { get; set; }
    }
}
