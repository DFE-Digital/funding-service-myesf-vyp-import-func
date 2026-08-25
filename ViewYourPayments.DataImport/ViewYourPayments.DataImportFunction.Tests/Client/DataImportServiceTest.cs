using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ViewYourPayments.Core.Enums.Logging;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Models.Payments;
using ViewYourPayments.DataImport.Domain;
using ViewYourPayments.DataImport.Domain.Clients;
using ViewYourPayments.DataImport.Domain.Domain;
using ViewYourPayments.DataImport.Domain.DomainClients;


namespace ViewYourPayments.DataImportFunction.Tests.Client
{
    [TestClass]
    public class DataImportServiceTest
    {
        private const string _dateFormat = "yyyy/MM/dd";
        Mock<IApplicationLogger> mockLogger = new Mock<IApplicationLogger>(MockBehavior.Strict);
        Mock<IViewYourPaymentsDbClient> mockviewYourPaymentClient = new Mock<IViewYourPaymentsDbClient>(MockBehavior.Strict);
        Mock<IApiClient> mockApimApi = new Mock<IApiClient>(MockBehavior.Strict);
        Mock<IDeSerializeApimJson> mockDeserializer = new Mock<IDeSerializeApimJson>(MockBehavior.Strict);

        [TestInitialize]
        public void Setup()
        {
            mockApimApi.Reset();
            mockDeserializer.Reset();
            mockviewYourPaymentClient.Reset();
            mockLogger.Reset();
            //mockRemittanceClient.SetupSet(o => o.BatchNumber = 2);
        }

        [TestMethod]
        public void WhenSettingFalse_EnsureExit()
        {
            // Arrange
            var actualAudits = new List<DataImportAudit>();

            mockLogger.Setup(x => x.LogTrace(It.IsAny<string>(), It.IsAny<BusinessArea>(), It.IsAny<LoggingSeverity>()));
            var dataImportSetting = new DataImportSetting { UseFinanceAPI = false };
            mockviewYourPaymentClient
                .Setup(o => o.InsertAudit(It.IsAny<IEnumerable<DataImportAudit>>()))
                .Callback((IEnumerable<DataImportAudit> audits) => actualAudits.AddRange(audits));

            var dataImportService = new DataImportService(mockApimApi.Object, mockDeserializer.Object, mockviewYourPaymentClient.Object, mockLogger.Object, dataImportSetting);

            // Act 
            dataImportService.CallApimAndInsertNewRemittances();

            // Assert 
            mockviewYourPaymentClient
               .Verify(o => o.InsertAudit(It.IsAny<IEnumerable<DataImportAudit>>()), Times.Exactly(1));

            Assert.AreEqual(3, actualAudits.Count);
            actualAudits.Find(x => x.Message.Equals("Remittance run started")).Should().NotBeNull();
            actualAudits.Find(x => x.Message.Equals("Remittance run finished")).Should().NotBeNull();
            actualAudits.Find(x => x.Message.Equals("Remittance run finished")).Should().NotBeNull();

            mockDeserializer.Verify(o => o.GetPaymentSummaries(It.IsAny<string>()), Times.Never());
        }

        [TestMethod]
        public void WhenOneCompanyReturnedAndNoExistingRecord_EnsureOneApiCall()
        {
            // Arrange audits
            var actualAudits = new List<DataImportAudit>();
            var dateTo = new DateTime(2019, 01, 01);
            int? latestBatchNumber = null;
            var dataImportSetting = new DataImportSetting
            {
                UseFinanceAPI = true,
                DataImportFromDate = "2019-01-01",
                Company = "COMP1",
                DimensionCode = "DC",
                DimensionValues = "DV",
                RetryCounter = 1,
                RetryGapDurationMilliseconds = 1000,
                NavApiCompaniesSettings = new[] { new NavApiCompanySettings { CompanyName= "COMP1", IncludeDimensionQuery=true },
                new NavApiCompanySettings { CompanyName= "COMP2", IncludeDimensionQuery=false }}
            };


            // Arrange mocks

            mockApimApi
                .Setup(o => o.MakeRequest(It.IsAny<PaymentApiRequest>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync("");

            mockApimApi.Setup(o => o.DisposeApi());

            mockApimApi.Setup(o => o.UriCallWithParameters).Returns("");
            mockDeserializer
                        .Setup(o => o.GetPaymentSummaries(It.IsAny<string>()))
                        .Returns(new PaymentDeSerializeResponse { Payments = new List<PaymentSummary>() });

            mockviewYourPaymentClient
                .Setup(o => o.InsertAudit(It.IsAny<IEnumerable<DataImportAudit>>()))
                .Callback((IEnumerable<DataImportAudit> audits) => actualAudits.AddRange(audits));
            mockviewYourPaymentClient
              .Setup(o => o.GetLatestBatchNumber()).Returns(latestBatchNumber);
            mockviewYourPaymentClient
                .Setup(o => o.GetLatestDataImportHistoryRecordByCompanyName(It.IsAny<string>())).Returns(new DataImportHistory { BatchId = 1, Id = 1, SkipTokenNumber = null });

            mockviewYourPaymentClient
                .Setup(o => o.InsertRemittances(It.IsAny<IEnumerable<PaymentSummary>>(), It.IsAny<DataImportHistory>(), It.IsAny<NavApiCompanySettings>(), It.IsAny<DateTime>()))
                .Returns(10);

            mockLogger.Setup(x => x.LogTrace(It.IsAny<string>(), It.IsAny<BusinessArea>(), It.IsAny<LoggingSeverity>()));
            mockLogger.Setup(x => x.LogException(It.IsAny<Exception>(), It.IsAny<String>()));

            // Arrange testing instance
            var dataImportService = new DataImportService(mockApimApi.Object, mockDeserializer.Object, mockviewYourPaymentClient.Object, mockLogger.Object, dataImportSetting);

            // Act 
            dataImportService.CallApimAndInsertNewRemittances();

            // Assert 
            mockviewYourPaymentClient
               .Verify(o => o.InsertAudit(It.IsAny<IEnumerable<DataImportAudit>>()), Times.Exactly(1));
            actualAudits.Find(x => x.Message.Equals("Get Remittances for company COMP1")).Should().NotBeNull();
            mockApimApi.Verify(o => o.MakeRequest(It.IsAny<PaymentApiRequest>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void WhenTwoCompanyReturnedAndHaveSpaceInName_EnsureTwoApiCallWithCorrectCompanyName()
        {
            // Arrange audits
            var actualAudits = new List<DataImportAudit>();
            var dateTo = new DateTime(2019, 01, 01);
            var dataImportSetting = new DataImportSetting
            {
                UseFinanceAPI = true,
                DataImportFromDate = "2019-01-01",
                Company = " COMP1 , COMP2 ",
                DimensionCode = "DC",
                DimensionValues = "DV",
                RetryCounter = 1,
                RetryGapDurationMilliseconds = 1000,
                NavApiCompaniesSettings = new[] { new NavApiCompanySettings { CompanyName= "COMP1", IncludeDimensionQuery=true },
                new NavApiCompanySettings { CompanyName= "COMP2", IncludeDimensionQuery=false }}
            };

            mockApimApi
                .Setup(o => o.MakeRequest(It.IsAny<PaymentApiRequest>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync("");

            mockApimApi.Setup(o => o.DisposeApi());

            mockApimApi.Setup(o => o.UriCallWithParameters).Returns("");
            mockDeserializer
                        .Setup(o => o.GetPaymentSummaries(It.IsAny<string>()))
                        .Returns(new PaymentDeSerializeResponse { Payments = new List<PaymentSummary>() });

            mockviewYourPaymentClient
                .Setup(o => o.InsertAudit(It.IsAny<IEnumerable<DataImportAudit>>()))
                .Callback((IEnumerable<DataImportAudit> audits) => actualAudits.AddRange(audits));
            mockviewYourPaymentClient
              .Setup(o => o.GetLatestBatchNumber()).Returns(1);
            mockviewYourPaymentClient
                .Setup(o => o.GetLatestDataImportHistoryRecordByCompanyName(It.IsAny<string>())).Returns(new DataImportHistory { BatchId = 1, Id = 1, SkipTokenNumber = null });

            mockviewYourPaymentClient
                .Setup(o => o.InsertRemittances(It.IsAny<IEnumerable<PaymentSummary>>(), It.IsAny<DataImportHistory>(), It.IsAny<NavApiCompanySettings>(), It.IsAny<DateTime>()))
                .Returns(10);

            mockLogger.Setup(x => x.LogTrace(It.IsAny<string>(), It.IsAny<BusinessArea>(), It.IsAny<LoggingSeverity>()));
            mockLogger.Setup(x => x.LogException(It.IsAny<Exception>(), It.IsAny<String>()));

            // Arrange testing instance
            var dataImportService = new DataImportService(mockApimApi.Object, mockDeserializer.Object, mockviewYourPaymentClient.Object, mockLogger.Object, dataImportSetting);

            // Act 
            dataImportService.CallApimAndInsertNewRemittances();

            // Assert 
            mockviewYourPaymentClient
               .Verify(o => o.InsertAudit(It.IsAny<IEnumerable<DataImportAudit>>()), Times.Exactly(1));

            actualAudits.Find(x => x.Message.Equals("Get Remittances for company COMP1")).Should().NotBeNull();
            actualAudits.Find(x => x.Message.Equals("Get Remittances for company COMP2")).Should().NotBeNull();
            mockviewYourPaymentClient.Verify(o => o.GetLatestDataImportHistoryRecordByCompanyName("COMP1"), Times.Exactly(1));
            mockviewYourPaymentClient.Verify(o => o.GetLatestDataImportHistoryRecordByCompanyName("COMP2"), Times.Exactly(1));
            mockviewYourPaymentClient.Verify(o => o.GetLatestDataImportHistoryRecordByCompanyName(It.IsAny<string>()), Times.Exactly(2));
            mockApimApi.Verify(o => o.MakeRequest(It.IsAny<PaymentApiRequest>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        }



        [TestMethod, TestCategory("Unit- dataImportService")]
        public void WhenEmptyPaymentList_InsertMethodShouldNotBeCalled()
        {
            // Arrange mocks
            mockApimApi
                .Setup(o => o.MakeRequest(It.IsAny<PaymentApiRequest>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync("");


            var dataImportSetting = new DataImportSetting
            {
                UseFinanceAPI = true,
                Company = "COMP1,COMP2",
                DataImportFromDate = "2019-01-01",
                DataImportToDate = "2019-01-01",
                DimensionCode = "",
                DimensionValues = "",
                NavApiCompaniesSettings = new[] { new NavApiCompanySettings { CompanyName= "COMP1", IncludeDimensionQuery=true },
                new NavApiCompanySettings { CompanyName= "COMP2", IncludeDimensionQuery=false }}
            };
            mockviewYourPaymentClient
              .Setup(o => o.GetLatestBatchNumber()).Returns(1);
            mockviewYourPaymentClient
                .Setup(o => o.GetLatestDataImportHistoryRecordByCompanyName(It.IsAny<string>())).Returns(new DataImportHistory { BatchId = 1, Id = 1, SkipTokenNumber = null });

            mockApimApi.Setup(o => o.DisposeApi());
            mockApimApi.Setup(o => o.UriCallWithParameters).Returns("");

            mockDeserializer
                .Setup(o => o.GetPaymentSummaries(It.IsAny<string>()))
                .Returns(new PaymentDeSerializeResponse { Payments = new List<PaymentSummary>() });



            mockviewYourPaymentClient
                .Setup(o => o.InsertAudit(It.IsAny<IEnumerable<DataImportAudit>>()));

            mockLogger.Setup(x => x.LogException(It.IsAny<Exception>(), It.IsAny<String>()));

            mockviewYourPaymentClient
                .Setup(o => o.InsertRemittances(It.IsAny<IEnumerable<PaymentSummary>>(), It.IsAny<DataImportHistory>(), It.IsAny<NavApiCompanySettings>(), It.IsAny<DateTime>())).Returns(0);


            mockLogger.Setup(x => x.LogTrace(It.IsAny<string>(), It.IsAny<BusinessArea>(), It.IsAny<LoggingSeverity>()));

            // Arrange testing instance
            var dataImportService = new DataImportService(mockApimApi.Object, mockDeserializer.Object, mockviewYourPaymentClient.Object, mockLogger.Object, dataImportSetting);

            // Act 
            dataImportService.CallApimAndInsertNewRemittances();

            // Assert 
            mockviewYourPaymentClient
                .Verify(o => o.InsertRemittances(It.Is((IEnumerable<PaymentSummary> payments) => payments.Count() == 0), It.IsAny<DataImportHistory>(), It.IsAny<NavApiCompanySettings>(), It.IsAny<DateTime>()), Times.Never);
        }

        [TestMethod]
        public void WhenNonEmptyPaymentListWithOutSkipToken_InsertMethodShouldBeCalledOnce()
        {
            // Arrange mocks
            var singleJson = File.ReadAllText(".\\Resources\\Json\\SinglePaymentSingleLine.json");

            mockApimApi
               .Setup(o => o.MakeRequest(It.IsAny<PaymentApiRequest>(), It.IsAny<int>(), It.IsAny<int>()))
               .ReturnsAsync(singleJson);
            mockviewYourPaymentClient
              .Setup(o => o.GetLatestBatchNumber()).Returns(1);
            mockviewYourPaymentClient
                .Setup(o => o.GetLatestDataImportHistoryRecordByCompanyName(It.IsAny<string>())).Returns(new DataImportHistory { BatchId = 1, Id = 1, SkipTokenNumber = null });

            var dataImportSetting = new DataImportSetting
            {
                UseFinanceAPI = true,
                Company = "COMP1,COMP2",
                DataImportFromDate = "2019-01-01",
                DataImportToDate = "2019-01-01",
                DimensionCode = "DC",
                DimensionValues = "DV",
                RetryCounter = 1,
                RetryGapDurationMilliseconds = 1000,
                NavApiCompaniesSettings = new[] { new NavApiCompanySettings { CompanyName= "COMP1", IncludeDimensionQuery=true },
                new NavApiCompanySettings { CompanyName= "COMP2", IncludeDimensionQuery=false }}
            };
            mockApimApi.Setup(o => o.DisposeApi());
            mockApimApi.Setup(o => o.UriCallWithParameters).Returns("");

            mockDeserializer
                .Setup(o => o.GetPaymentSummaries(It.IsAny<string>()))
                .Returns(new PaymentDeSerializeResponse { Payments = new List<PaymentSummary> { new PaymentSummary { Id = 1, Ukprn = "1234" } } });

            mockviewYourPaymentClient
                .Setup(o => o.InsertAudit(It.IsAny<IEnumerable<DataImportAudit>>()));

            mockLogger.Setup(x => x.LogException(It.IsAny<Exception>(), It.IsAny<String>()));

            mockviewYourPaymentClient
                .Setup(o => o.InsertRemittances(It.IsAny<IEnumerable<PaymentSummary>>(), It.IsAny<DataImportHistory>(), It.IsAny<NavApiCompanySettings>(), It.IsAny<DateTime>())).Returns(0);


            mockLogger.Setup(x => x.LogTrace(It.IsAny<string>(), It.IsAny<BusinessArea>(), It.IsAny<LoggingSeverity>()));

            // Arrange testing instance
            var dataImportService = new DataImportService(mockApimApi.Object, mockDeserializer.Object, mockviewYourPaymentClient.Object, mockLogger.Object, dataImportSetting);

            // Act 
            dataImportService.CallApimAndInsertNewRemittances();

            // Assert 
            mockviewYourPaymentClient
                .Verify(o => o.InsertRemittances(It.Is((IEnumerable<PaymentSummary> payments) => payments.Count() == 1), It.IsAny<DataImportHistory>(), It.IsAny<NavApiCompanySettings>(), It.IsAny<DateTime>()), Times.Exactly(2));
        }

        [TestMethod]
        public void EnsureDisposeApi_CalledOnce()
        {
            // Arrange - Mocks
            var dataImportSetting = new DataImportSetting
            {
                UseFinanceAPI = true,
                DataImportFromDate = "2019-01-01",
                Company = "COMP1,COMP2",
                DimensionCode = "DC",
                DimensionValues = "DV",
                RetryCounter = 1,
                RetryGapDurationMilliseconds = 1000,
                NavApiCompaniesSettings = new[] { new NavApiCompanySettings { CompanyName= "COMP1", IncludeDimensionQuery=true },
                new NavApiCompanySettings { CompanyName= "COMP2", IncludeDimensionQuery=false }}
            };
            mockviewYourPaymentClient
               .Setup(o => o.GetLatestBatchNumber()).Returns(1);

            mockviewYourPaymentClient
                .Setup(o => o.GetLatestDataImportHistoryRecordByCompanyName(It.IsAny<string>())).Returns(new DataImportHistory { BatchId = 1, Id = 1, SkipTokenNumber = null });

            mockApimApi
                .Setup(o => o.MakeRequest(It.IsAny<PaymentApiRequest>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(""); ;

            mockApimApi.Setup(o => o.DisposeApi());
            mockApimApi.Setup(o => o.UriCallWithParameters).Returns("");

            mockDeserializer
                .Setup(o => o.GetPaymentSummaries(It.IsAny<string>()))
                .Returns(new PaymentDeSerializeResponse { Payments = new List<PaymentSummary>() });


            mockviewYourPaymentClient
                .Setup(o => o.InsertAudit(It.IsAny<IEnumerable<DataImportAudit>>()));

            mockLogger.Setup(x => x.LogException(It.IsAny<Exception>(), It.IsAny<String>()));

            mockviewYourPaymentClient
                .Setup(o => o.InsertRemittances(It.IsAny<IEnumerable<PaymentSummary>>(), It.IsAny<DataImportHistory>(), It.IsAny<NavApiCompanySettings>(), It.IsAny<DateTime>())).Returns(0);

            mockLogger.Setup(x => x.LogTrace(It.IsAny<string>(), It.IsAny<BusinessArea>(), It.IsAny<LoggingSeverity>()));

            mockLogger.Setup(x => x.LogException(It.IsAny<Exception>(), It.IsAny<String>()));

            // Arrange - Testing Instance
            var dataImportService = new DataImportService(mockApimApi.Object, mockDeserializer.Object, mockviewYourPaymentClient.Object, mockLogger.Object, dataImportSetting);

            // Act 
            dataImportService.CallApimAndInsertNewRemittances();

            // Assert 
            mockApimApi.Verify(o => o.DisposeApi(), Times.Once);
        }

        [TestMethod]
        public void GetApiRequest_When_RunAsOneTimeJobWithConfigDates_Flag_IsOn_ShouldReturnConfigDates()
        {
            var startDate = new DateTime(2018, 01, 01);
            var endDate = new DateTime(2020, 01, 01);

            var dataImportSetting = new DataImportSetting
            {
                UseFinanceAPI = true,
                DataImportFromDate = startDate.ToString(_dateFormat),
                DataImportToDate = endDate.ToString(_dateFormat),
                Company = "COMP1,COMP2",
                DimensionCode = "DC",
                DimensionValues = "DV",
                RetryCounter = 1,
                RetryGapDurationMilliseconds = 1000,
                RunAsOneTimeJobWithConfigDates = true,
                NavApiCompaniesSettings = new[] { new NavApiCompanySettings { CompanyName= "COMP1", IncludeDimensionQuery=true },
                new NavApiCompanySettings { CompanyName= "COMP2", IncludeDimensionQuery=false }}
            };
            var dataImportHistory = new DataImportHistory
            {
                BatchId = 1,
                Id = 1,
                RunFromDate = new DateTime(2019, 11, 11),
                RunToDate = new DateTime(2019, 12, 12),
                SkipTokenNumber = "1"
            };
            var companySettings = new NavApiCompanySettings { CompanyName = "AC", IncludeDimensionQuery = true };
            var dataImportService = new DataImportService(mockApimApi.Object, mockDeserializer.Object, mockviewYourPaymentClient.Object, mockLogger.Object, dataImportSetting);
            var result = dataImportService.GetApiRequest(dataImportSetting, dataImportHistory, companySettings, null);
            result.FromDate.Should().Be(startDate);
            result.ToDate.Should().Be(endDate);
            result.SkipToken.Should().BeNull();
        }

        [TestMethod]
        public void GetApiRequest_WhenFixedDateFlagIsOffAnd_ImportHistoryIsNull_ShouldReturnConfigDateAsStartDate()
        {
            var startDate = new DateTime(2018, 01, 01);
            var endDate = new DateTime(2020, 01, 01);

            var dataImportSetting = new DataImportSetting
            {
                UseFinanceAPI = true,
                DataImportFromDate = startDate.ToString(_dateFormat),
                DataImportToDate = endDate.ToString(_dateFormat),
                Company = "COMP1,COMP2",
                DimensionCode = "DC",
                DimensionValues = "DV",
                RetryCounter = 1,
                RetryGapDurationMilliseconds = 1000,
                RunAsOneTimeJobWithConfigDates = false,
                NavApiCompaniesSettings = new[] { new NavApiCompanySettings { CompanyName= "COMP1", IncludeDimensionQuery=true },
                new NavApiCompanySettings { CompanyName= "COMP2", IncludeDimensionQuery=false }}
            };
            var companySettings = new NavApiCompanySettings { CompanyName = "AC", IncludeDimensionQuery = true };

            var dataImportService = new DataImportService(mockApimApi.Object, mockDeserializer.Object, mockviewYourPaymentClient.Object, mockLogger.Object, dataImportSetting);
            var result = dataImportService.GetApiRequest(dataImportSetting, null, companySettings, null);
            result.FromDate.Should().Be(startDate);
            result.SkipToken.Should().Be(null);
        }

        [TestMethod]
        public void GetApiRequest_When_FixedDateFlagIsOffAnd_ImportHistoryHasRecordWithSkipTokenAsZero_ShouldReturnLastRunEndDateAsStartDate()
        {
            var startDate = new DateTime(2018, 01, 01);
            var endDate = new DateTime(2020, 01, 01);

            var dataImportSetting = new DataImportSetting
            {
                UseFinanceAPI = true,
                DataImportFromDate = startDate.ToString(_dateFormat),
                DataImportToDate = endDate.ToString(_dateFormat),
                Company = "COMP1,COMP2",
                DimensionCode = "DC",
                DimensionValues = "DV",
                RetryCounter = 1,
                RetryGapDurationMilliseconds = 1000,
                RunAsOneTimeJobWithConfigDates = false,
                NavApiCompaniesSettings = new[] { new NavApiCompanySettings { CompanyName= "COMP1", IncludeDimensionQuery=true },
                new NavApiCompanySettings { CompanyName= "COMP2", IncludeDimensionQuery=false }}
            };
            var dataImportHistory = new DataImportHistory
            {
                BatchId = 1,
                Id = 1,
                RunFromDate = new DateTime(2019, 11, 11),
                RunToDate = new DateTime(2019, 12, 12),
                SkipTokenNumber = null
            };
            var companySettings = new NavApiCompanySettings { CompanyName = "AC", IncludeDimensionQuery = false };
            var dataImportService = new DataImportService(mockApimApi.Object, mockDeserializer.Object, mockviewYourPaymentClient.Object, mockLogger.Object, dataImportSetting);
            var result = dataImportService.GetApiRequest(dataImportSetting, dataImportHistory, companySettings, null);
            result.FromDate.Should().Be(dataImportHistory.RunToDate);
            result.SkipToken.Should().BeNull();
            result.DimensionCode.Should().BeNull();
            result.DimensionValue.Should().BeNull();
        }

        [TestMethod]
        public void GetApiRequest_When_FixedDateFlagIsOffAnd_ImportHistoryHasRecordWithSkipTokenIsNotZero_ShouldReturnValueFromHistory()
        {
            var startDate = new DateTime(2018, 01, 01);
            var endDate = new DateTime(2020, 01, 01);

            var dataImportSetting = new DataImportSetting
            {
                UseFinanceAPI = true,
                DataImportFromDate = startDate.ToString(_dateFormat),
                DataImportToDate = endDate.ToString(_dateFormat),
                Company = "COMP1,COMP2",
                DimensionCode = "DC",
                DimensionValues = "DV",
                RetryCounter = 1,
                RetryGapDurationMilliseconds = 1000,
                RunAsOneTimeJobWithConfigDates = false,
                NavApiCompaniesSettings = new[] { new NavApiCompanySettings { CompanyName= "COMP1", IncludeDimensionQuery=true },
                new NavApiCompanySettings { CompanyName= "COMP2", IncludeDimensionQuery=false }}
            };
            var dataImportHistory = new DataImportHistory
            {
                BatchId = 1,
                Id = 1,
                RunFromDate = new DateTime(2019, 11, 11),
                RunToDate = new DateTime(2019, 12, 12),
                SkipTokenNumber = "5",
            };
            var companySettings = new NavApiCompanySettings { CompanyName = "AC", IncludeDimensionQuery = true };

            var dataImportService = new DataImportService(mockApimApi.Object, mockDeserializer.Object, mockviewYourPaymentClient.Object, mockLogger.Object, dataImportSetting);
            var result = dataImportService.GetApiRequest(dataImportSetting, dataImportHistory, companySettings, null);
            result.FromDate.Should().Be(dataImportHistory.RunFromDate);
            result.ToDate.Should().Be(dataImportHistory.RunToDate);
            result.SkipToken.Should().Be(dataImportHistory.SkipTokenNumber);
            result.DimensionCode.Should().Be(dataImportSetting.DimensionCode);
            result.DimensionValue.Should().Be(dataImportSetting.DimensionValues);
        }

        [TestMethod]
        public void GetApiRequest_When_StartDateIsGreaterThenEndDate_ShouldThrowException()
        {
            var endDate = new DateTime(2018, 01, 01);
            var startDate = new DateTime(2020, 01, 01);
            mockLogger.Setup(x => x.LogTrace(It.IsAny<string>(), It.IsAny<BusinessArea>(), It.IsAny<LoggingSeverity>()));
            mockLogger.Setup(x => x.LogException(It.IsAny<Exception>(), It.IsAny<String>()));
            var dataImportSetting = new DataImportSetting
            {
                UseFinanceAPI = true,
                DataImportFromDate = startDate.ToString(_dateFormat),
                DataImportToDate = endDate.ToString(_dateFormat),
                Company = "COMP1,COMP2",
                DimensionCode = "DC",
                DimensionValues = "DV",
                RetryCounter = 1,
                RetryGapDurationMilliseconds = 1000,
                RunAsOneTimeJobWithConfigDates = true,
                NavApiCompaniesSettings = new[] { new NavApiCompanySettings { CompanyName= "COMP1", IncludeDimensionQuery=true },
                new NavApiCompanySettings { CompanyName= "COMP2", IncludeDimensionQuery=false }}
            };

            var companySettings = new NavApiCompanySettings { CompanyName = "AC", IncludeDimensionQuery = true };
            var dataImportService = new DataImportService(mockApimApi.Object, mockDeserializer.Object, mockviewYourPaymentClient.Object, mockLogger.Object, dataImportSetting);
            try
            {
                var result = dataImportService.GetApiRequest(dataImportSetting, null, companySettings, null);
            }
            catch (ArgumentException ex)
            {
                var errorMessage = $"Error: Start-date is greater than end-date. [DateFrom: {startDate}], [DateTo: {endDate}]";

                ex.Message.Should().Be(errorMessage);
            }
        }
    }
}