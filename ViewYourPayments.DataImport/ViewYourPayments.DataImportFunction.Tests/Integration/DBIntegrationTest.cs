using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Models.Payments;
using ViewYourPayments.DataImport.Domain.Domain;
using ViewYourPayments.DataImport.Domain.DomainClients;
using ViewYourPayments.DataImport.Domain.MapingProfiles;
using ViewYourPayments.DataImport.Domain.Services;
using ViewYourPayments.DataImportFunction.Tests.Helpers;

namespace ViewYourPayments.DataImportFunction.Tests.Integration
{
    [TestClass]
    public class DBIntegrationTest
    {
        static Mock<IApplicationLogger> mocklog = new Mock<IApplicationLogger>();
        MockViewYourPaymentsDbClient testDBAccess;
        ViewYourPaymentsDbClient _viewYourPaymentsDbClient;
        private readonly string _connectionString;

        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfilesRegistration());
        });

        private readonly IMapper mockMapper;

        public DBIntegrationTest()
        {
            mockMapper = new Mapper(config);
            _connectionString = ConfigHelper.GetConnectionString("ViewYourPaymentsDbContext");
            testDBAccess = new MockViewYourPaymentsDbClient(new DataService(), mocklog.Object, _connectionString, mockMapper);
            _viewYourPaymentsDbClient = new ViewYourPaymentsDbClient(new DataService(), mocklog.Object, _connectionString, mockMapper);
        }

        [TestInitialize]
        public void Setup()
        {
            testDBAccess.CleanDB();
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfilesRegistration());
            });

            mockMapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod, TestCategory("DB-Integration")]
        public void InsertRemittances_WhenCalledWithSingleValideRecord_EnsureRecordInserted()
        {
            // Arrange 
            var paymentDetails = new List<PaymentLine>
            {
                new PaymentLineBuilder().Build()
            };

            var payment =
                new PaymentSummaryBuilder()
                    .SetPaymentLine(paymentDetails)
                    .Build();

            // Act
            DataImportHistory dataImportHistory = new DataImportHistory
            {
                BatchId = 1,
                RequestedUrl = "Test",
                RunFromDate = DateTime.Now.AddDays(-1),
                RunToDate = DateTime.Now,
                SkipTokenNumber = "1212",
            };

            var fakeCompanySetting = new NavApiCompanySettings
            {
                CompanyName = "fake"
            };

            _viewYourPaymentsDbClient.InsertRemittances(new List<PaymentSummary> { payment }, dataImportHistory, fakeCompanySetting);

            // Assert
            var payments = testDBAccess.GetRemittanceViaPaymentId(payment.PaymentIdentifier.ToString());

            Assert.AreEqual(1, payments.Count);
            var expected = payments.First();

            // Assert Summary
            Assert.AreEqual(payment.PaymentDate, expected.PaymentDate);
            Assert.AreEqual(payment.PaymentIdentifier, expected.PaymentIdentifier);
            Assert.AreEqual(payment.PaymentTotal, expected.PaymentTotal);
            Assert.AreEqual(payment.Ukprn, expected.Ukprn);

            // Assert Detail
            Assert.AreEqual(1, expected.PaymentLine.Count);
            Assert.AreEqual(paymentDetails[0].Contract, expected.PaymentLine[0].Contract);
            Assert.AreEqual(paymentDetails[0].LineAmount, expected.PaymentLine[0].LineAmount);
            Assert.AreEqual(paymentDetails[0].PaymentLineGroupDescription, expected.PaymentLine[0].PaymentLineGroupDescription);
            Assert.AreEqual(paymentDetails[0].PaymentLineDescription, expected.PaymentLine[0].PaymentLineDescription);
            Assert.AreEqual(paymentDetails[0].PaymentLineIdentifier, expected.PaymentLine[0].PaymentLineIdentifier);
        }

        [TestMethod, TestCategory("DB-Integration")]
        [DataRow("ANL-abcd", "Apprenticeships Carry-in")]
        [DataRow("ANLP-asd", "Procured Non-Levy Apprenticeships")]
        [DataRow("16ED-xyz", "16-19 Funding")]
        [DataRow("XX-SS", "Other")]
        [DataRow("DL-jlj", "Non-Procured Adult Education Budget")]
        public void InsertRemittances_PaymentLineWithContractCode_CorrectBudgetGroupDescription(string ContractCode, string expectedBudgetGroup)
        {
            // Arrange 
            var paymentDetails = new List<PaymentLine>
            {
                new PaymentLineBuilder().Build()
            };
            paymentDetails[0].Contract = ContractCode;

            var payment =
                new PaymentSummaryBuilder()
                    .SetPaymentLine(paymentDetails)
                    .Build();

            // Act
            DataImportHistory dataImportHistory = new DataImportHistory
            {
                BatchId = 1,
                RequestedUrl = "Test",
                RunFromDate = DateTime.Now.AddDays(-1),
                RunToDate = DateTime.Now,
                SkipTokenNumber = "1212",
            };

            var fakeCompanySetting = new NavApiCompanySettings
            {
                CompanyName = "fake"
            };

            _viewYourPaymentsDbClient.InsertRemittances(new List<PaymentSummary> { payment }, dataImportHistory, fakeCompanySetting);

            // Assert
            var payments = testDBAccess.GetRemittanceViaPaymentId(payment.PaymentIdentifier.ToString());

            Assert.AreEqual(1, payments.Count);
            var expected = payments.First();

            // Assert Summary
            Assert.AreEqual(payment.PaymentDate, expected.PaymentDate);
            Assert.AreEqual(payment.PaymentIdentifier, expected.PaymentIdentifier);
            Assert.AreEqual(payment.PaymentTotal, expected.PaymentTotal);
            Assert.AreEqual(payment.Ukprn, expected.Ukprn);

            // Assert Detail
            Assert.AreEqual(1, expected.PaymentLine.Count);
            Assert.AreEqual(paymentDetails[0].Contract, expected.PaymentLine[0].Contract);
            Assert.AreEqual(paymentDetails[0].LineAmount, expected.PaymentLine[0].LineAmount);
            Assert.AreEqual(paymentDetails[0].PaymentLineDescription, expected.PaymentLine[0].PaymentLineDescription);
            Assert.AreEqual(paymentDetails[0].PaymentLineIdentifier, expected.PaymentLine[0].PaymentLineIdentifier);
            Assert.AreEqual(expectedBudgetGroup, expected.PaymentLine[0].BudgetGroup);
        }

        [TestMethod, TestCategory("DB-Integration")]
        public void InsertRemittances_WhenBulkRecordAdded_EnsureNoDuplicateRecordInserted()
        {
            // Arrange
            var detailsCount = 0;
            var paymentsCount = 0;

            var id = 100;
            Random rnd = new Random();
            var payments = new List<PaymentSummary>();
            DataImportHistory dataImportHistory = new DataImportHistory
            {
                BatchId = 1,
                RequestedUrl = "Test",
                RunFromDate = DateTime.Now.AddDays(-1),
                RunToDate = DateTime.Now,
                SkipTokenNumber = "1212",
            };

            for (int i = 0; i < 30; i++)
            {
                var paymentDetails = new List<PaymentLine>();
                var details = rnd.Next(1, 5);

                for (int j = 0; j < details; j++)
                {
                    paymentDetails.Add(new PaymentLineBuilder().SetPaymentLineId(id++.ToString()).Build());
                    detailsCount++;
                }

                var payment = new PaymentSummaryBuilder()
                    .SetPaymentIdentifier(id++)
                    .SetPaymentLine(paymentDetails)
                    .Build();
                payments.Add(payment);
                paymentsCount++;
            }


            var DuplicatePaymentDetails = new List<PaymentLine>
            {
                new PaymentLineBuilder().Build()
            };

            var DuplicatePaymentSummary =
                new PaymentSummaryBuilder()
                    .SetPaymentLine(DuplicatePaymentDetails)
                    .Build();
            //Add Duplicate records
            payments.Add(DuplicatePaymentSummary);
            payments.Add(DuplicatePaymentSummary);

            var fakeCompanySetting = new NavApiCompanySettings
            {
                CompanyName = "fake"
            };

            // Act 
            _viewYourPaymentsDbClient.InsertRemittances(payments, dataImportHistory, fakeCompanySetting);

            // Assert Summary
            //Duplicate records not entered hence -1
            Assert.AreEqual(paymentsCount, testDBAccess.GetRemittanceSummaryCount() - 1);

            // Assert Detail
            Assert.AreEqual(detailsCount, testDBAccess.GetRemittanceDetailCount() - 1);
        }

        [TestMethod, TestCategory("DB-Integration")]
        public void InsertRemittances_WhenUkPrnIsNotValid_RecordShouldNotInsertToActualTablesAndInsertedToStagingTables()
        {
            // Arrange
            var paymentDetails = new List<PaymentLine>
            {
                new PaymentLineBuilder().Build()
            };
            DataImportHistory dataImportHistory = new DataImportHistory
            {
                BatchId = 1,
                RequestedUrl = "Test",
                RunFromDate = DateTime.Now.AddDays(-1),
                RunToDate = DateTime.Now,
                SkipTokenNumber = "1212",
            };

            var payment =
                new PaymentSummaryBuilder()
                    .SetUkprn("0")
                    .SetPaymentLine(paymentDetails)
                    .Build();

            var fakeCompanySetting = new NavApiCompanySettings
            {
                CompanyName = "fake"
            };

            // Act
            //TODO
            _viewYourPaymentsDbClient.InsertRemittances(new List<PaymentSummary> { payment }, dataImportHistory, fakeCompanySetting);

            // Assert
            Assert.AreEqual(0, testDBAccess.GetRemittanceViaPaymentId(payment.PaymentIdentifier.ToString()).Count);
            Assert.AreEqual(0, testDBAccess.GetRemittanceDetailCount());
            Assert.AreEqual(0, testDBAccess.GetRemittanceSummaryCount());
            Assert.AreEqual(1, testDBAccess.GetRemittanceDetailStagingCount());
            Assert.AreEqual(1, testDBAccess.GetRemittanceSummaryStagingCount());
        }

        [TestMethod, TestCategory("DB-Integration")]
        public void InsertRemittances_WhenDuplicateRemittanceAdded_RecordShouldNotInsertToActualTablesAndInsertedToStagingTables()
        {
            // Arrange
            testDBAccess.CleanDB();
            var paymentDetails = new List<PaymentLine>
            {
                new PaymentLineBuilder().Build()
            };

            DataImportHistory dataImportHistory1 = new DataImportHistory
            {
                BatchId = 1,
                RequestedUrl = "Test",
                RunFromDate = DateTime.Now.AddDays(-1),
                RunToDate = DateTime.Now,
                SkipTokenNumber = "1212",
            };

            DataImportHistory dataImportHistory2 = new DataImportHistory
            {
                BatchId = 2,
                RequestedUrl = "Test",
                RunFromDate = DateTime.Now.AddDays(-1),
                RunToDate = DateTime.Now,
                SkipTokenNumber = "1212",
            };

            var payment =
                new PaymentSummaryBuilder()
                    .SetUkprn("12345678")
                    .SetPaymentLine(paymentDetails)
                    .Build();

            var fakeCompanySetting = new NavApiCompanySettings
            {
                CompanyName = "fake"
            };

            // Act
            _viewYourPaymentsDbClient.InsertRemittances(new List<PaymentSummary> { JsonConvert.DeserializeObject<PaymentSummary>(JsonConvert.SerializeObject(payment)) }, dataImportHistory1, fakeCompanySetting);
            _viewYourPaymentsDbClient.InsertRemittances(new List<PaymentSummary> { JsonConvert.DeserializeObject<PaymentSummary>(JsonConvert.SerializeObject(payment)) }, dataImportHistory2, fakeCompanySetting);

            // Assert
            Assert.AreEqual(1, testDBAccess.GetRemittanceViaPaymentId(payment.PaymentIdentifier.ToString()).Count);
            Assert.AreEqual(1, testDBAccess.GetRemittanceDetailCount());
            Assert.AreEqual(1, testDBAccess.GetRemittanceSummaryCount());
            Assert.AreEqual(1, testDBAccess.GetRemittanceDetailStagingCount());
            Assert.AreEqual(1, testDBAccess.GetRemittanceSummaryStagingCount());
        }

    }
}