using System;

namespace ViewYourPayments.DataImport.Domain.Domain
{
    public class PaymentApiRequest
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string DimensionCode { get; set; }

        public string DimensionValue { get; set; }

        public string SkipToken { get; set; }

        public string Company { get; set; }

        public string VersionNumber { get; set; }
    }
}
