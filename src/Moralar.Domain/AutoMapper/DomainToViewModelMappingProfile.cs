using System;
using System.Collections.Generic;
using System.Linq;
using Moralar.Data.Entities;
using Moralar.Data.Entities.Auxiliar;
using Moralar.Data.Enum;
using Moralar.Domain.ViewModels;
using Moralar.Domain.ViewModels.Admin;
using Moralar.Domain.ViewModels.Course;
using Moralar.Domain.ViewModels.Family;
using Moralar.Domain.ViewModels.Informative;
using Moralar.Domain.ViewModels.InformativeSended;
using Moralar.Domain.ViewModels.Notification;
using Moralar.Domain.ViewModels.NotificationSended;
using Moralar.Domain.ViewModels.PropertiesInterest;
using Moralar.Domain.ViewModels.Property;
using Moralar.Domain.ViewModels.Question;
using Moralar.Domain.ViewModels.QuestionAnswer;
using Moralar.Domain.ViewModels.Quiz;
using Moralar.Domain.ViewModels.QuizFamily;
using Moralar.Domain.ViewModels.ResidencialProperty;
using Moralar.Domain.ViewModels.Schedule;
using Moralar.Domain.ViewModels.ScheduleHistory;
using Moralar.Domain.ViewModels.Video;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.ViewModels;
using Moralar.UtilityFramework.Services.Iugu.Core.Models;
using AutoMapperProfile = AutoMapper.Profile;

namespace Moralar.Domain.AutoMapper
{
    public class DomainToViewModelMappingProfile : AutoMapperProfile
    {
        public DomainToViewModelMappingProfile()
        {
            /*EXEMPLE
            CreateMap<Entity,ViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));*/
            CreateMap<IuguCreditCard, CreditCardViewModel>()
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Data.DisplayNumber))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Data.HolderName))
                .ForMember(dest => dest.ExpMonth, opt => opt.MapFrom(src => src.Data.Month))
                .ForMember(dest => dest.ExpYear, opt => opt.MapFrom(src => src.Data.Year))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Data.Brand));
            CreateMap<Bank, BankViewModel>()
                           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<AddressInfoViewModel, InfoAddressViewModel>()
                .ForMember(dest => dest.Neighborhood, opt => opt.MapFrom(src => src.Bairro))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.Cep.ToString()))
                .ForMember(dest => dest.Complement, opt => opt.MapFrom(src => src.Complemento.ToString()))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.Localidade.ToString()))
                .ForMember(dest => dest.StateUf, opt => opt.MapFrom(src => src.Uf.ToString()))
                .ForMember(dest => dest.StreetAddress, opt => opt.MapFrom(src => src.Logradouro))
                ;
            CreateMap<UserAdministrator, UserAdministratorViewModel>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Blocked, opt => opt.MapFrom(src => src.DataBlocked != null))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<Profile, ProfileViewModel>()
                //.ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo.SetPhotoProfile(src.ProviderId, null, null, null, null, 600)))
                .ForMember(dest => dest.Blocked, opt => opt.MapFrom(src => src.DataBlocked != null))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<PriorityRate, PriorityRateViewModel>().ReverseMap();
            CreateMap<State, StateDefaultViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<City, CityDefaultViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));

            #region Family
            CreateMap<Family, FamilyFirstAccessViewModel>();
            CreateMap<FamilyHolder, FamilyHolderViewModel>();
            CreateMap<FamilyHolder, FamilyHolderListViewModel>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<FamilyHolder, FamilyHolderMinViewModel>();
            CreateMap<FamilySpouse, FamilySpouseViewModel>();
            CreateMap<FamilyMember, FamilyMemberViewModel>();
            CreateMap<FamilyFinancial, FamilyFinancialViewModel>();
            CreateMap<FamilyPriorization, FamilyPriorizationViewModel>();
            CreateMap<FamilyAddress, FamilyAddressViewModel>();
            CreateMap<Family, FamilyHolderViewModel>();
            CreateMap<PriorityRate, PriorityRateViewModel>();
            CreateMap<Family, FamilyHolderDistanceViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Holder.Name))
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(src => src.Holder.Cpf))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Holder.Number));

            CreateMap<Family, FamilyHolderListViewModel>()
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Holder.Number))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Holder.Name))
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(src => src.Holder.Cpf))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Holder.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Holder.Phone))
                .ForMember(dest => dest.Blocked, opt => opt.MapFrom(src => src.DataBlocked != null ? true : false))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<Schedule, FamilyHolderListViewModel>()
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src._id.ToString()))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.HolderNumber))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.HolderName))
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(src => src.HolderCpf))
                .ForMember(dest => dest.Blocked, opt => opt.MapFrom(src => src.DataBlocked != null ? true : false))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.FamilyId));

            CreateMap<Schedule, FamilyHolderExportViewModel>()
                .ForMember(dest => dest.TypeScheduleStatus, opt => opt.MapFrom(src => src.TypeScheduleStatus.GetEnumMemberValue()))
                .ForMember(dest => dest.TypeSubject, opt => opt.MapFrom(src => src.TypeSubject.GetEnumMemberValue()))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.HolderNumber))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.HolderName))
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(src => src.HolderCpf));

            CreateMap<Schedule, ScheduleMapExportViewModel>();

            CreateMap<Family, FamilyHolderExportViewModel>()
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Holder.Number))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Holder.Name))
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(src => src.Holder.Cpf));


            CreateMap<Family, FamilyCompleteViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));

            CreateMap<Family, FamilyCompleteListViewModel>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));

            // added because its needed in the function: "DetailFamiliesMatch"
            CreateMap<FamilyPriorization, List<PriorityRateViewModel>>();

            CreateMap<Family, FamilyEditViewModel>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()))
               .ForMember(dest => dest.Holder, opt => opt.MapFrom(src => src.Holder));


            CreateMap<Family, ScheduleDetailTimeLineProcessChoosePropertyOneAndTwoViewModel>();

            CreateMap<Family, FamilyExportViewModel>()
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.DataBlocked != null ? "Inativo" : "Ativo"))
               .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Holder.Number))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Holder.Name))
               .ForMember(dest => dest.Cpf, opt => opt.MapFrom(src => src.Holder.Cpf));

            #endregion
            #region ResidencialProperty
            CreateMap<Data.Entities.Auxiliar.ResidencialPropertyPhoto, ViewModels.ResidencialProperty.ResidencialPropertyPhoto>().ForMember(
                dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl != null ? src.ImageUrl.SetPathImage(null) : "default.png"));
            CreateMap<ResidencialProperty, ResidencialPropertyViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()))
                .ForMember(dest => dest.Blocked, opt => opt.MapFrom(src => src.DataBlocked != null ? true : false))
                .ForMember(dest => dest.Project, opt => opt.MapFrom(src => src.Project));

            CreateMap<ResidencialPropertyAdress, ResidencialPropertyAdressViewModel>();
            CreateMap<ResidencialPropertyFeatures, ResidencialPropertyFeatureViewModel>();
            CreateMap<ResidencialProperty, ResidencialPropertyAdress>();
            CreateMap<ResidencialProperty, ResidencialPropertyExportViewModel>()
                .ForMember(dest => dest.Blocked, opt => opt.MapFrom(src => src.DataBlocked != null ? "Inativo" : "Ativo"))
                .ForMember(dest => dest.StreetAddress, opt => opt.MapFrom(src => src.ResidencialPropertyAdress.StreetAddress))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.ResidencialPropertyAdress.Number))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.ResidencialPropertyAdress.CityName))
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.ResidencialPropertyAdress.StateName))
                .ForMember(dest => dest.StateUf, opt => opt.MapFrom(src => src.ResidencialPropertyAdress.StateUf))
                .ForMember(dest => dest.Neighborhood, opt => opt.MapFrom(src => src.ResidencialPropertyAdress.Neighborhood))
                .ForMember(dest => dest.Complement, opt => opt.MapFrom(src => src.ResidencialPropertyAdress.Complement))
                .ForMember(dest => dest.CEP, opt => opt.MapFrom(src => src.ResidencialPropertyAdress.CEP))
                .ForMember(dest => dest.SquareFootage, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.SquareFootage.ToString()))
                .ForMember(dest => dest.PropertyValue, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.PropertyValue.ToReal(true)))
                .ForMember(dest => dest.CondominiumValue, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.CondominiumValue.ToReal(true)))
                .ForMember(dest => dest.IptuValue, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.IptuValue.ToReal(true)))
                .ForMember(dest => dest.NumberFloors, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.NumberFloors.ToString()))
                .ForMember(dest => dest.NumberOfBathrooms, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.NumberOfBathrooms.ToString()))
                .ForMember(dest => dest.NumberOfBedrooms, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.NumberOfBedrooms.ToString()))
                .ForMember(dest => dest.FloorLocation, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.FloorLocation.ToString()))
                .ForMember(dest => dest.HasAccessLadder, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.HasAccessLadder.MapBoolean()))
                .ForMember(dest => dest.HasAccessRamp, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.HasAccessRamp.MapBoolean()))
                .ForMember(dest => dest.HasAdaptedToPcd, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.HasAdaptedToPcd.MapBoolean()))
                .ForMember(dest => dest.HasCistern, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.HasCistern.MapBoolean()))
                .ForMember(dest => dest.HasElavator, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.HasElavator.MapBoolean()))
                .ForMember(dest => dest.HasGarage, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.HasGarage.MapBoolean()))
                .ForMember(dest => dest.HasServiceArea, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.HasServiceArea.MapBoolean()))
                .ForMember(dest => dest.HasWall, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.HasWall.MapBoolean()))
                .ForMember(dest => dest.HasYard, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.HasYard.MapBoolean()))
                .ForMember(dest => dest.TypeStatusResidencialProperty, opt => opt.MapFrom(src => src.TypeStatusResidencialProperty.GetEnumMemberValue()))
                .ForMember(dest => dest.Project, opt => opt.MapFrom(src => src.Project))
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => string.Join(",", src.Photo.Select(x => x.ImageUrl).ToList()).TrimEnd(',')))
                .ForMember(dest => dest.PropertyRegularization, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.PropertyRegularization))
                .ForMember(dest => dest.TypeGasInstallation, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.TypeGasInstallation.GetEnumMemberValue()))
                .ForMember(dest => dest.TypeProperty, opt => opt.MapFrom(src => src.ResidencialPropertyFeatures.TypeProperty.GetEnumMemberValue()));
            #endregion
            #region Quiz
            CreateMap<Quiz, QuizViewModel>()
               .ForMember(dest => dest.Blocked, opt => opt.MapFrom(src => src.DataBlocked != null))
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<Quiz, QuizDetailViewModel>()
         .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<Question, QuestionViewModel>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<QuestionDescription, QuestionDescriptionViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));

            CreateMap<Quiz, QuizExportViewModel>()
             .ForMember(dest => dest.Created, opt => opt.MapFrom(src => Utilities.TimeStampToDateTime(src.Created.Value)));
            CreateMap<QuizFamily, QuizFamilyViewModel>();
            //.ForMember(dest => dest.Created, opt => opt.MapFrom(src => Utilities.TimeStampToDateTime(src.Created.Value)))

            CreateMap<QuizFamily, QuizFamilyListViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()))
            .ForMember(dest => dest.TypeStatus, opt => opt.MapFrom(src => src.TypeStatus));
            CreateMap<Quiz, QuizListViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));

            CreateMap<QuizFamily, QuizFamilyExportViewModel>()
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created.MapUnixTime("dd/MM/yyyy", "-")))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.TypeStatus == 0 ? "Não respondido" : "Respondido"));

            CreateMap<Quiz, QuizFamilyExportViewModel>();

            CreateMap<QuestionAnswerAux, QuestionAnswerAuxViewModel>();

            CreateMap<QuestionAnswer, QuestionAnswerRegisterViewModel>();

            #endregion           
            #region Schedule
            CreateMap<Schedule, ScheduleRegisterViewModel>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<Schedule, ScheduleViewModel>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<Schedule, ScheduleListViewModel>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<Schedule, ScheduleDetailTimeLinePGMViewModel>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<Schedule, ScheduleExportViewModel>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.TimeStampToDateTime().ToString("dd/MM/yyyy HH:mm")))
                .ForMember(dest => dest.TypeSubject, opt => opt.MapFrom(src => src.TypeSubject.GetEnumMemberValue()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.TypeScheduleStatus.GetEnumMemberValue()));

            CreateMap<Schedule, SearchViewModel>()
                .ForMember(dest => dest.TypeSubject, opt => opt.MapFrom(src => src.TypeSubject));


            #endregion
            #region ScheduleHistory
            CreateMap<ScheduleHistory, ScheduleHistoryViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<Schedule, ScheduleHistoryViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));

            #endregion
            #region Informative
            CreateMap<Informative, InformativeListViewModel>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()))
              .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.DataBlocked != null ? true : false))
              .ForMember(dest => dest.Blocked, opt => opt.MapFrom(src => src.DataBlocked != null ? true : false))
              .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
              .ForMember(dest => dest.DatePublish, opt => opt.MapFrom(src => src.DatePublish != null ? src.DatePublish.Value.TimeStampToDateTime().ToString("dd/MM/yyyy") : null))
              ;
            CreateMap<InformativeSended, InformativeSendedDetailViewModel>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));

            CreateMap<InformativeSended, InformativeExportViewModel>()
             .ForMember(dest => dest.Read, opt => opt.MapFrom(src => src.DateViewed != null ? "Sim" : "Não"));

            CreateMap<Informative, InformativeViewModel>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()))
             .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
             .ForMember(dest => dest.DatePublish, opt => opt.MapFrom(src => src.DatePublish != null ? src.DatePublish.Value.TimeStampToDateTime().ToString("dd/MM/yyyy") : null));

            CreateMap<InformativeSended, InformativeSendedViewModel>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));

            #endregion
            #region Notification

            CreateMap<Notification, NotificationViewModel>()
                .ForMember(dest => dest.FamilyId, opt => opt.MapFrom(
                    src => src.FamilyId != null ? new List<string> { src.FamilyId } : null))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(
                    src => src._id.ToString()));

            CreateMap<NotificationSended, NotificationViewModel>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<Notification, NotificationListViewModel>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()))
             .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.DataBlocked != null ? false : true))
             .ForMember(dest => dest.Arquived, opt => opt.MapFrom(src => src.Created.Value.TimeStampToDateTime().AddDays(30).Date < DateTime.Now.Date ? true : false));

            CreateMap<NotificationSended, NotificationSendedListViewModel>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()))
             .ForMember(dest => dest.Arquived, opt => opt.MapFrom(src => src.Created.Value.TimeStampToDateTime().AddDays(30).Date < DateTime.Now.Date ? true : false));

            #endregion
            #region PropertiesInterest
            CreateMap<PropertiesInterest, PropertiesInterestRegisterViewModel>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<PropertiesInterest, PropertiesInterestViewModel>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<Family, ScheduleDetailTimeLineChoosePropertyViewModel>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));
            CreateMap<PropertiesInterestViewModel, PropertiesInterestExportViewModel>()
             .ForMember(dest => dest.HolderCpf, opt => opt.MapFrom(src => src.HolderCpf.ToString()))
             .ForMember(dest => dest.HolderName, opt => opt.MapFrom(src => src.HolderName.ToString()))
             .ForMember(dest => dest.HolderEmail, opt => opt.MapFrom(src => src.HolderEmail.ToString()))
             .ForMember(dest => dest.HolderNumber, opt => opt.MapFrom(src => src.HolderNumber.ToString()))
             .ForMember(dest => dest.Interest, opt => opt.MapFrom(src => src.Interest.ToString()));
            //.ForMember(dest => dest.FamilyId, opt => opt.MapFrom(src => src.FamilyId))
            //.ForMember(dest => dest.ResidelcialPropertyId, opt => opt.MapFrom(src => src.ResidelcialPropertyId))
            //.ForMember(dest => dest.FamilyId, opt => opt.MapFrom(src => src._id))

            #endregion
            #region Course
            CreateMap<Course, CourseViewModel>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));

            CreateMap<Course, CourseListViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()))
            .ForMember(dest => dest.Blocked, opt => opt.MapFrom(src => src.DataBlocked != null))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => Utilities.TimeStampToDateTime(src.StartDate)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => Utilities.TimeStampToDateTime(src.EndDate)));

            CreateMap<Course, CourseFamilyListViewModel>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));

            CreateMap<Course, CourseExportViewModel>()
            .ForMember(dest => dest.Blocked, opt => opt.MapFrom(src => src.DataBlocked != null ? "Inativo" : "Ativo"))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.TimeStampToDateTime().ToString("dd/MM/yyyy")))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.TimeStampToDateTime().ToString("dd/MM/yyyy")));

            #endregion
            #region Video

            CreateMap<Video, VideoViewModel>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()))
             .ForMember(dest => dest.Blocked, opt => opt.MapFrom(src => src.DataBlocked != null ? true : false))
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Thumbnail));

            CreateMap<Video, VideoListViewModel>()
             .ForMember(dest => dest.Blocked, opt => opt.MapFrom(src => src.DataBlocked))
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));

            #endregion

            CreateMap<NotificationSended, NotificationSendedViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id.ToString()));


        }
    }
}