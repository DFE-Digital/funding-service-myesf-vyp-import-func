using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ViewYourPayments.Core.Enums.Logging;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Models.Payments;
using ViewYourPayments.DataImport.Domain.Clients;
using ViewYourPayments.DataImport.Domain.Domain;
using ViewYourPayments.DataImport.Domain.Services;


namespace ViewYourPayments.DataImportFunction.Tests.Client
{
    [TestClass]
    public class ApimApiTest
    {
        Mock<IApplicationLogger> mockLogger = new Mock<IApplicationLogger>(MockBehavior.Strict);

        public string ApimEndPointUri => string.Empty;
        public string ApimSubscriptionKey => string.Empty;
        public string ApimEndPointVersion => "2019-03-01";

        [TestMethod, TestCategory("Unit-ApiCall")]
        public async Task MakeRequest_WhenApiReturnOkWithEmptyContent_EnsureRequestIsMadeWithCorrectParams()
        {
            // Arrange
            var requestHeaderParams = new Dictionary<string, string>();

            Mock<IHttpService> mockHttp = new Mock<IHttpService>();
            mockHttp
                .Setup(o => o.AddDefaultRequestHeader(It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string a, string b) => requestHeaderParams.Add(a, b));

            var returns = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent("{\"Payments\":\"\"}")
            };

            mockHttp
                .Setup(o => o.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(returns);

            ApimApi api = new ApimApi(mockHttp.Object, mockLogger.Object, ApimEndPointUri, ApimEndPointVersion, ApimSubscriptionKey);

            mockLogger.Setup(x => x.LogTrace(It.IsAny<string>(), It.IsAny<BusinessArea>(), It.IsAny<LoggingSeverity>()));
            var expectedParams = $"?dateFrom=2018-01-01&dateTo=2019-01-02&api-version={ApimEndPointVersion}"; ;
            var retryCount = 3;
            var retryInterval = 1000;
            PaymentApiRequest paymentApiRequest = new PaymentApiRequest
            {
                DimensionCode = null,
                DimensionValue = null,
                SkipToken = null,
                Company = "SFA_NAV",
                FromDate = new DateTime(2018, 1, 1),
                ToDate = new DateTime(2019, 1, 2)
            };

            // Act 
            var test = await api.MakeRequest(paymentApiRequest, retryCount, retryInterval);
            // Assert
            test.Should().Be("{\"Payments\":\"\"}");
            mockHttp.Verify(o => o.AddDefaultRequestHeader(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockHttp.Verify(o => o.GetAsync(It.Is<string>(s => s.Equals(expectedParams))), Times.Once());
            requestHeaderParams.ContainsKey("Ocp-Apim-Subscription-Key").Should().BeTrue();
            requestHeaderParams.ContainsKey("accept").Should().BeTrue();
            requestHeaderParams["accept"].Should().Be("application/json");
        }

        [TestMethod, TestCategory("Unit-ApiCall")]
        public async Task MakeRequest_WhenApiReturnsOkWithContent_CorrectJsonShouldReturned()
        {
            // Arrange
            var json = File.ReadAllText(".\\Resources\\Json\\SinglePaymentSingleLine.json");

            var requestHeaderParams = new Dictionary<string, string>();
            Mock<IHttpService> mockHttp = new Mock<IHttpService>();
            mockHttp
                .Setup(o => o.AddDefaultRequestHeader(It.IsAny<string>(), It.IsAny<string>()));

            var returns = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(json)
            };

            mockHttp
                .Setup(o => o.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(returns);

            mockLogger.Setup(x => x.LogTrace(It.IsAny<string>(), It.IsAny<BusinessArea>(), It.IsAny<LoggingSeverity>()));

            ApimApi api = new ApimApi(mockHttp.Object, mockLogger.Object, ApimEndPointUri, ApimEndPointVersion, ApimSubscriptionKey);

            PaymentApiRequest paymentApiRequest = new PaymentApiRequest
            {
                DimensionCode = null,
                DimensionValue = null,
                SkipToken = null,
                Company = "SFA_NAV",
                FromDate = new DateTime(2018, 1, 1),
                ToDate = new DateTime(2019, 1, 2)
            };
            var retryCount = 3;
            var retryInterval = 1000;

            // Act 
            var test = await api.MakeRequest(paymentApiRequest, retryCount, retryInterval);

            // Assert 
            mockHttp.Verify(o => o.GetAsync(It.IsAny<string>()), Times.Once());
            test.Should().Be(json);
        }

        [TestMethod, TestCategory("Unit-ApiCall")]
        public async Task MakeRequest_WhenRecieveNotOkResponse_EnsureCallIsNotSuccessfull()
        {
            // Arrange
            var requestHeaderParams = new Dictionary<string, string>();
            Mock<IHttpService> mockHttp = new Mock<IHttpService>();
            mockHttp
                .Setup(o => o.AddDefaultRequestHeader(It.IsAny<string>(), It.IsAny<string>()));

            var returns = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.NotFound,
                Content = new StringContent("Not Found")
            };

            mockHttp
                .Setup(o => o.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(returns);

            mockLogger.Setup(x => x.LogTrace(It.IsAny<string>(), It.IsAny<BusinessArea>(), It.IsAny<LoggingSeverity>()));
            mockLogger.Setup(x => x.LogException(It.IsAny<Exception>(), It.IsAny<String>()));
            IApiClient api = new ApimApi(mockHttp.Object, mockLogger.Object, ApimEndPointUri, ApimEndPointVersion, ApimSubscriptionKey);

            var retryCount = 3;
            var retryInterval = 1000;
            PaymentApiRequest paymentApiRequest = new PaymentApiRequest
            {
                DimensionCode = "FUNDING TYPE",
                DimensionValue = "ESFA",
                SkipToken = null,
                Company = "SFA_NAV",
                FromDate = new DateTime(2018, 1, 1),
                ToDate = new DateTime(2019, 1, 2)
            };

            // Act 
            try
            {
                var test = await api.MakeRequest(paymentApiRequest, retryCount, retryInterval);

            }
            catch (Exception ex)
            {
                ex.GetType().Should().Be(typeof(HttpRequestException));
            }

            // Assert 
            mockHttp.Verify(o => o.GetAsync(It.IsAny<string>()), Times.Exactly(retryCount + 1));
        }

        [TestMethod, TestCategory("Unit-ApiCall")]
        public async Task EnsureUrlParamsArePopulated_WhenProvided()
        {
            // Arrange
            var requestHeaderParams = new Dictionary<string, string>();
            Mock<IHttpService> mockHttp = new Mock<IHttpService>();
            mockHttp
                .Setup(o => o.AddDefaultRequestHeader(It.IsAny<string>(), It.IsAny<string>()));

            var returns = new HttpResponseMessage
            {
                Content = new StringContent("{}")
            };

            mockHttp
                .Setup(o => o.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(returns);

            mockLogger.Setup(x => x.LogTrace(It.IsAny<string>(), It.IsAny<BusinessArea>(), It.IsAny<LoggingSeverity>()));

            ApimApi api = new ApimApi(mockHttp.Object, mockLogger.Object, ApimEndPointUri, ApimEndPointVersion, ApimSubscriptionKey);

            var expectedParams = "?dateFrom=2018-01-01&dateTo=2019-01-02&api-version=2019-03-01&dimensionCode=FUNDING%20TYPE&dimensionValue=ESFA";
            var retryCount = 3;
            var retryInterval = 1000;
            PaymentApiRequest paymentApiRequest = new PaymentApiRequest
            {
                DimensionCode = "FUNDING TYPE",
                DimensionValue = "ESFA",
                SkipToken = null,
                Company = "ESFA",
                FromDate = new DateTime(2018, 1, 1),
                ToDate = new DateTime(2019, 1, 2),
                VersionNumber = "2019-03-01"
            };

            // Act 
            var test = await api.MakeRequest(paymentApiRequest, retryCount, retryInterval);
            // Assert 
            mockHttp.Verify(o => o.AddDefaultRequestHeader(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockHttp.Verify(o => o.GetAsync(It.Is<string>(s => s.Equals(expectedParams))), Times.Once());
        }

        [TestMethod, TestCategory("Unit-ApiCall")]
        public async Task MakeRequest_WhenApiReturnError_ApiCallRetriedWithCorrectInterval()
        {
            // Arrange
            var requestHeaderParams = new Dictionary<string, string>();
            Mock<IHttpService> mockHttp = new Mock<IHttpService>();
            mockHttp
                .Setup(o => o.AddDefaultRequestHeader(It.IsAny<string>(), It.IsAny<string>()));

            var returns = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Content = new StringContent("Nav api returning error after 3 retry.")
            };

            mockHttp
                .Setup(o => o.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(returns);
            var actualAudits = new List<DataImportAudit>();

            mockLogger.Setup(x => x.LogTrace(It.IsAny<string>(), It.IsAny<BusinessArea>(), It.IsAny<LoggingSeverity>()));
            mockLogger.Setup(x => x.LogException(It.IsAny<Exception>(), It.IsAny<String>()));

            ApimApi api = new ApimApi(mockHttp.Object, mockLogger.Object, ApimEndPointUri, ApimEndPointVersion, ApimSubscriptionKey);

            var expectedParams = "?dateFrom=2018-01-01&dateTo=2019-01-02&api-version=2019-03-01&dimensionCode=FUNDING%20TYPE&dimensionValue=ESFA";
            var retryCount = 3;
            var retryInterval = 1000;
            PaymentApiRequest paymentApiRequest = new PaymentApiRequest
            {
                DimensionCode = "FUNDING TYPE",
                DimensionValue = "ESFA",
                SkipToken = null,
                Company = "ESFA",
                FromDate = new DateTime(2018, 1, 1),
                ToDate = new DateTime(2019, 1, 2),
                VersionNumber = "2019-03-01"
            };

            // Act 
            try
            {
                var test = await api.MakeRequest(paymentApiRequest, retryCount, retryInterval);
            }
            catch (HttpRequestException ex)
            {
                ex.Message.Should().Be($"Nav api returning error after {retryCount} retry.");
            }

            // Assert 
            mockHttp.Verify(o => o.AddDefaultRequestHeader(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockHttp.Verify(o => o.GetAsync(It.Is<string>(s => s.Equals(expectedParams))), Times.Exactly(retryCount + 1));
        }

        [TestMethod, TestCategory("Unit-ApiCall")]
        public async Task EnsureUrlParamsArePopulated_WhenCompanyIs_SFANav_AndDimensionCodeIsNull()
        {
            // Arrange
            var requestHeaderParams = new Dictionary<string, string>();
            Mock<IHttpService> mockHttp = new Mock<IHttpService>();
            mockHttp
                .Setup(o => o.AddDefaultRequestHeader(It.IsAny<string>(), It.IsAny<string>()));

            var returns = new HttpResponseMessage
            {
                Content = new StringContent("{}")
            };

            mockHttp
                .Setup(o => o.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(returns);

            mockLogger.Setup(x => x.LogTrace(It.IsAny<string>(), It.IsAny<BusinessArea>(), It.IsAny<LoggingSeverity>()));

            ApimApi api = new ApimApi(mockHttp.Object, mockLogger.Object, ApimEndPointUri, ApimEndPointVersion, ApimSubscriptionKey);

            var expectedParams = "?dateFrom=2018-01-01&dateTo=2019-01-02&api-version=2019-03-01";
            var retryCount = 3;
            var retryInterval = 1000;
            PaymentApiRequest paymentApiRequest = new PaymentApiRequest
            {
                SkipToken = null,
                Company = "SFA_NAV",
                FromDate = new DateTime(2018, 1, 1),
                ToDate = new DateTime(2019, 1, 2),
                VersionNumber = "2019-03-01"
            };

            // Act 
            var test = await api.MakeRequest(paymentApiRequest, retryCount, retryInterval);
            // Assert 
            mockHttp.Verify(o => o.AddDefaultRequestHeader(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockHttp.Verify(o => o.GetAsync(It.Is<string>(s => s.Equals(expectedParams))), Times.Once());
        }
    }
}
