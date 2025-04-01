namespace Moralar.UtilityFramework.Application.Core.ViewModels
{
    public class DtSearch
    {
        //
        // Resumen:
        //     Global search value. To be applied to all columns which have searchable as true.
        public string Value { get; set; }

        //
        // Resumen:
        //     true if the global filter should be treated as a regular expression for advanced
        //     searching, false otherwise. Note that normally server-side processing scripts
        //     will not perform regular expression searching for performance reasons on large
        //     data sets, but it is technically possible and at the discretion of your script.
        public bool Regex { get; set; }
    }
}
