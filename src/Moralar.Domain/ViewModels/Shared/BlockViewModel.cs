using System.ComponentModel.DataAnnotations;

namespace Moralar.Domain.ViewModels
{
    public class BlockViewModel
    {
        public bool Block { get; set; }
        public string Reason { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [MinLength(24, ErrorMessage = DefaultMessages.InvalidIdentifier)]
        public string TargetId { get; set; }
    }
}