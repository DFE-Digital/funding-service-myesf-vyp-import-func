using AutoMapper;
using ViewYourPayments.Core.Models.Payments;

namespace ViewYourPayments.DataImport.Domain.MapingProfiles
{
    public class MappingProfilesRegistration : Profile
    {
        public MappingProfilesRegistration()
        {
            CreateMap<PaymentSummary, PaymentSummaryStaging>().ReverseMap();
            CreateMap<PaymentLine, PaymentLineStaging>().ReverseMap();
        }
    }
}
