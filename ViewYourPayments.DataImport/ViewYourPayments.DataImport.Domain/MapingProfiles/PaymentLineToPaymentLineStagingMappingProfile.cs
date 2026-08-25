using AutoMapper;
using ViewYourPayments.Core.Models.Payments;

namespace ViewYourPayments.DataImport.Domain.MapingProfiles
{
    public class PaymentLineToPaymentLineStagingMappingProfile : Profile
    {
        public PaymentLineToPaymentLineStagingMappingProfile()
        {
            CreateMap<PaymentLine, PaymentLineStaging>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PaymentLineIdentifier, opt => opt.MapFrom(src => src.PaymentLineIdentifier))
                .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract))
                .ForMember(dest => dest.Establishment, opt => opt.MapFrom(src => src.Establishment))
                .ForMember(dest => dest.EstablishmentDescription, opt => opt.MapFrom(src => src.EstablishmentDescription))
                .ForMember(dest => dest.PaymentLineDescription, opt => opt.MapFrom(src => src.PaymentLineDescription))
                .ForMember(dest => dest.PaymentLineGroupDescription, opt => opt.MapFrom(src => src.PaymentLineGroupDescription))
                .ForMember(dest => dest.PaymentSummary, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.PaymentSummaryId, opt => opt.MapFrom(src => src.PaymentSummaryId))
                .ForMember(dest => dest.LineAmount, opt => opt.MapFrom(src => src.LineAmount))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn))
                .ForMember(dest => dest.FundingType, opt => opt.MapFrom(src => src.FundingType))
                .ForMember(dest => dest.PostingDate, opt => opt.MapFrom(src => src.PostingDate))
                .ForMember(dest => dest.BudgetGroup, opt => opt.MapFrom(src => src.BudgetGroup))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName));
        }
    }
}
