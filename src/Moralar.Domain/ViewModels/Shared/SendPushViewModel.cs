using Moralar.Data.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moralar.Domain.ViewModels
{
    public class SendPushViewModel
    {
        public SendPushViewModel()
        {
            Route = RouteNotification.System;
        }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        public string Message { get; set; }
        public List<string> TargetId { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        public string Title { get; set; }
        public bool IsDriver { get; set; }
        public RouteNotification Route { get; set; }
    }
}