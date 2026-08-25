using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Models.Payments;
using ViewYourPayments.DataImport.Domain;
using ViewYourPayments.DataImport.Domain.Domain;

namespace ViewYourPayments.DataImportFunction.Tests.Domain
{
    [TestClass]
    public class DeSerializeTest
    {
        private Mock<IApplicationLogger> mockLogger = new Mock<IApplicationLogger>();

        [TestMethod, TestCategory("Unit-DeSerialize")]
        public void GetPaymentSummaries_WhenJsonContainsEmptyPayments_ReturnsEmptyList()
        {
            // Arrange
            string emptyPaymentJson = "{\"Payments\":[]}";

            // Act
            var summaries = new DeSerializeApimJson(mockLogger.Object).GetPaymentSummaries(emptyPaymentJson);

            // Assertion
            summaries.Payments.Count.Should().Be(0);
            summaries.SkipToken.Should().BeNull();
        }

        [TestMethod, TestCategory("Unit-DeSerialize")]
        public void GetPaymentSummaries_WhenJsonContains1PaymentWithAlphaNumericSkipTokenAndIdentifier_ReturnsPaymentListWith1Record()
        {
            // Arrange
            var json = File.ReadAllText(".\\Resources\\Json\\SinglePaymentSingleLine.json");

            // Act
            var summaries = new DeSerializeApimJson(mockLogger.Object).GetPaymentSummaries(json);

            // Assertion
            summaries.Payments.Count.Should().Be(1);
            summaries.Payments[0].PaymentLine.Count.Should().Be(1);
            summaries.SkipToken.Should().Be("C3971391");
        }

        [TestMethod, TestCategory("Unit-DeSerialize")]
        public void GetPaymentSummaries_WhenJsonContainsRecordWithEmptyUkPRN_ReturnsPaymentListExcludedEmptyUkPrnItems()
        {
            // Arrange
            var json = File.ReadAllText(".\\Resources\\Json\\LargePaymentOutput.json");

            // Act
            var summaries = new DeSerializeApimJson(mockLogger.Object).GetPaymentSummaries(json);

            // Assertion
            summaries.Payments.Count.Should().Be(66);
            summaries.SkipToken.Should().Be("3971391");
        }

        [TestMethod, TestCategory("Unit-DeSerialize")]
        public void GetPaymentSummaries_WhenPaymentHasNoPaymentDetails_ReturnRecordWithDetail()
        {
            // Arrange
            var json = File.ReadAllText(".\\Resources\\Json\\SinglePaymentNoLines.json");

            // Act
            var summaries = new DeSerializeApimJson(mockLogger.Object).GetPaymentSummaries(json);

            // Assertion
            summaries.Payments.Count.Should().Be(1);
        }

        [TestMethod, TestCategory("Unit-DeSerialize")]
        public void GetPaymentSummaries_WhenValidJsonContainsNoRecord_DoesValidResponse()
        {
            // Arrange
            string emptyPaymentJson = "{}";
            var expected = new PaymentDeSerializeResponse
            {
                FailedCount = 0,
                Payments = Enumerable.Empty<PaymentSummary>().ToList(),
                SkipToken = null
            };

            // Act
            var summaries = new DeSerializeApimJson(mockLogger.Object).GetPaymentSummaries(emptyPaymentJson);

            // Assertion
            summaries.Should().BeEquivalentTo(expected);
        }

        [TestMethod, TestCategory("Unit-DeSerialize")]
        public void GetPaymentSummaries_WhenEmptyString_ReturnJsonException()
        {
            // Arrange
            string empty = "";
            try
            {
                // Act
                var summaries = new DeSerializeApimJson(mockLogger.Object).GetPaymentSummaries(empty);

                // Assertion
                Assert.Fail();
            }
            catch (Exception testException)
            {
                // Assertion
                testException.GetType().Should().Be(typeof(JsonReaderException));
            }
        }

        [TestMethod, TestCategory("Unit-DeSerialize")]
        public void GetPaymentSummaries_WhenInvalidJsonPassed_ReturnJsonException()
        {
            // Arrange
            string empty = "{";
            try
            {
                // Act
                var summaries = new DeSerializeApimJson(mockLogger.Object).GetPaymentSummaries(empty);

                // Assertion
                Assert.Fail();
            }
            catch (Exception testException)
            {
                // Assertion
                testException.GetType().Should().Be(typeof(JsonReaderException));
            }
        }

        [TestMethod, TestCategory("Unit-DeSerialize")]
        public void GetPaymentSummaries_WhenValidJsonPassed_ReturnsExpectedPaymentSummaryObjectInResponse()
        {
            //arrange
            var json = File.ReadAllText(".\\Resources\\Json\\PaymentsNewSchema.json");

            IList<PaymentSummary> expectedObject = GetDeserializedObject(json);

            //act
            var summaries = new DeSerializeApimJson(mockLogger.Object).GetPaymentSummaries(json);

            //assert
            summaries.Should().NotBeNull().And.BeOfType<PaymentDeSerializeResponse>();
            summaries.Payments.Should().NotBeEmpty();
            summaries.Payments.Should().BeEquivalentTo(expectedObject);
        }

        private IList<PaymentSummary> GetDeserializedObject(string json)
        {
            var paymentSummaries = new List<PaymentSummary>();

            var results = JObject.Parse(json);

            var paymentsJson = results.GetValue("payments").Value<IList<JToken>>();

            foreach (var item in paymentsJson)
            {
                paymentSummaries.Add(item.ToObject<PaymentSummary>());
            }

            return paymentSummaries;
        }
    }
}