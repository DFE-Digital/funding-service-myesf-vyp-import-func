using AutoMapper;
using ViewYourPayments.Core.Models.Payments;

namespace ViewYourPayments.DataImport.Domain.MapingProfiles
{
    public class PaymentSummaryToPaymentSummaryStagingMappingProfile : Profile
    {
        public PaymentSummaryToPaymentSummaryStagingMappingProfile()
        {
            CreateMap<PaymentSummary, PaymentSummaryStaging>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PaymentIdentifier, opt => opt.MapFrom(src => src.PaymentIdentifier))
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(src => src.Ukprn))
                .ForMember(dest => dest.PaymentLine, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.PaymentTotal, opt => opt.MapFrom(src => src.PaymentTotal))
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate))
                .ForMember(dest => dest.VendorIdentifier, opt => opt.MapFrom(src => src.VendorIdentifier))
                .ForMember(dest => dest.DataImportHistory, opt => opt.MapFrom(src => src.DataImportHistory))
                .ForMember(dest => dest.DataImportHistoryId, opt => opt.MapFrom(src => src.DataImportHistoryId))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn));
        }
    }
}
