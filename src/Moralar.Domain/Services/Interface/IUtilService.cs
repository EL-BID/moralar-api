using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Moralar.Data.Entities;
using Moralar.Data.Enum;
using Moralar.Domain.ViewModels;

namespace Moralar.Domain.Services.Interface
{
    public interface IUtilService
    {
        Task RegisterLogAction(LocalAction localAction, TypeAction typeAction, TypeResposible typeResposible, string message, string responsibleId = null, string responsibleName = null, string referenceId = null, string justification = null, Exception ex = null);
        Task LogUserAdministrationAction(string userId, string message, TypeAction typeAction, LocalAction localAction, string referenceId = null);
        void UpdateCascate(Family familyEntity);
        string GetFlag(string flag);
        Task<InfoAddressViewModel> GetInfoFromZipCode(string zipCode);
        Task<InfoAddressViewModel?> getAddressFromZipCode(string zipCode);
        Task SendNotify(string title, string content, string email, List<string> deviceId, ForType fortype = ForType.Family, string familyId = null, string titlePush = null, string contentPush = null, string? module = null, string? SeenBy = null);
        Task sentNotificationOfContemplateToEquipeTTSAsync(Family family, string id, string residencialCode);
    }
}