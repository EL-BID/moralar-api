using System;
using System.Collections.Generic;
using System.Text;

namespace Moralar.Domain.ViewModels.Family
{
    public class FamilyRegisterMember
    {
        public string FamilyId { get; set; }
        public List<FamilyMemberViewModel> Members { get; set; }
    }
}
