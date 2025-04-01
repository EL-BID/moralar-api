using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace Moralar.UtilityFramework.Application.Core
{
    public class LimitElementsAttribute : ValidationAttribute
    {
        private readonly int _minElements;

        private readonly int? _maxElements;

        public LimitElementsAttribute(int minElements = 1, int maxElements = int.MaxValue)
        {
            _minElements = minElements;
            _maxElements = maxElements;
        }

        public override bool IsValid(object value)
        {
            if (!(value is IList list))
            {
                return false;
            }

            bool flag = list.Count >= _minElements;
            if (flag && _maxElements.HasValue)
            {
                flag = list.Count >= _minElements && list.Count <= _maxElements;
            }

            return flag;
        }
    }

}
