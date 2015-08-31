﻿using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PayWithAmazon;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using System.Net;
using PayWithAmazon.ProviderCreditRequests;
using PayWithAmazon.RecurringPaymentRequests;
using PayWithAmazon.StandardPaymentRequests;
using PayWithAmazon.CommonRequests;

namespace UnitTests
{
    [TestFixture]
    public class PayWithAmazonTest
    {
        Configuration clientConfig = new Configuration();
        public PayWithAmazonTest()
        {
            clientConfig.WithMerchantId("test")
                .WithAccessKey("test")
                .WithSecretKey("test")
                .WithCurrencyCode("USD")
                .WithClientId("test")
                .WithRegion("us")
                .WithSandbox(true)
                .WithPlatformId("test")
                .WithCABundleFile("test")
                .WithApplicationName("test")
                .WithApplicationVersion("1.0.0")
                .WithProxyHost("")
                .WithProxyPort(-1)
                .WithProxyUserName("test")
                .WithProxyUserPassword("test");
        }

        [Test]
        public void TestConfig()
        {

            try
            {
                Configuration clientConfig = new Configuration();
                clientConfig.WithAccessKey("A")
                    .WithSecretKey("B");
                Client client = new Client(clientConfig);
            }
            catch (KeyNotFoundException expected)
            {
                Assert.IsTrue(Regex.IsMatch(expected.ToString(), "is either not part of the configuration or has incorrect Key name", RegexOptions.IgnoreCase));
            }

            try
            {
                Configuration clientConfig = new Configuration();
                clientConfig = null;
                Client client = new Client(clientConfig);
            }
            catch (NullReferenceException expected)
            {
                Assert.IsTrue(Regex.IsMatch(expected.ToString(), "config is null", RegexOptions.IgnoreCase));
            }
        }

        [Test]
        public void TestJsonFile()
        {
            try
            {
                string jsonfilepath = "";
                Client client = new Client(jsonfilepath);
            }
            catch (NullReferenceException expected)
            {
                Assert.IsTrue(Regex.IsMatch(expected.ToString(), "Json file path is not provided", RegexOptions.IgnoreCase));
            }
            try
            {
                string jsonfilepath = Path.Combine(Environment.CurrentDirectory, @"confi.json");
                Client client = new Client(jsonfilepath);
            }
            catch (FileNotFoundException expected)
            {
                Assert.IsTrue(Regex.IsMatch(expected.ToString(), "File not found", RegexOptions.IgnoreCase));
            }
            try
            {
                string jsonfilepath = Path.Combine(Environment.CurrentDirectory, @"config.json");
                Client client = new Client(jsonfilepath);
            }
            catch (JsonReaderException expected)
            {
                Assert.IsTrue(Regex.IsMatch(expected.ToString(), "incorrect json", RegexOptions.IgnoreCase));
            }
        }

        [Test]
        public void TestSandboxSetter()
        {
            Client client = new Client(clientConfig);
            client.SetSandbox(true);
            Assert.AreEqual(client.config["sandbox"], true);
        }

        [Test]
        public void TestClientIDSetter()
        {
            Client client = new Client(clientConfig);
            try
            {
                client.SetClientId("");
            }
            catch (NullReferenceException expected)
            {
                Assert.IsTrue(Regex.IsMatch(expected.ToString(), "Client ID value cannot be empty", RegexOptions.IgnoreCase));
            }
            try
            {
                client.SetClientId(null);
            }
            catch (NullReferenceException expected)
            {
                Assert.IsTrue(Regex.IsMatch(expected.ToString(), "Client ID value cannot be empty", RegexOptions.IgnoreCase));
            }
        }

        [Test]
        public void TestGetOrderReferenceDetails()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()  {
                {"Action","GetOrderReferenceDetails"},
                {"SellerId","test"},
                {"AmazonOrderReferenceId","test"},
                {"AddressConsentToken","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API GetOrderReferenceDetails
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            GetOrderReferenceDetailsRequest getOrderReferenceDetails = new GetOrderReferenceDetailsRequest();
            getOrderReferenceDetails.WithAmazonOrderReferenceId("test")
                .WithaddressConsentToken("test")
                .WithMerchantId("test")
                .WithMWSAuthToken("test");

            client.GetOrderReferenceDetails(getOrderReferenceDetails);
            IDictionary<string, string> apiParametersDict = client.GetParameters();
            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        [Test]
        public void TestSetOrderReferenceDetails()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","SetOrderReferenceDetails"},
                {"SellerId","test"},
                {"AmazonOrderReferenceId","test"},
                {"OrderReferenceAttributes.OrderTotal.Amount","test"},
                {"OrderReferenceAttributes.OrderTotal.CurrencyCode","TEST"},
                {"OrderReferenceAttributes.PlatformId","test"},
                {"OrderReferenceAttributes.SellerNote","test"},
                {"OrderReferenceAttributes.SellerOrderAttributes.SellerOrderId","test"},
                {"OrderReferenceAttributes.SellerOrderAttributes.StoreName","test"},
                {"OrderReferenceAttributes.SellerOrderAttributes.CustomInformation","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API SetOrderReferenceDetails
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            SetOrderReferenceDetailsRequest setOrderReferenceDetails = new SetOrderReferenceDetailsRequest();
            setOrderReferenceDetails.WithAmazonOrderReferenceId("test")
                .WithMerchantId("test")
                .WithAmount("test")
                .WithCurrencyCode("TEST")
                .WithPlatformId("test")
                .WithSellerNote("test")
                .WithSellerOrderId("test")
                .WithStoreName("test")
                .WithCustomInformation("test")
                .WithMWSAuthToken("test");
            client.SetOrderReferenceDetails(setOrderReferenceDetails);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        [Test]
        public void TestConfirmOrderReference()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","ConfirmOrderReference"},
                {"SellerId","test"},
                {"AmazonOrderReferenceId","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API ConfirmOrderReference
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");
            ConfirmOrderReferenceRequest confirmOrderReference = new ConfirmOrderReferenceRequest();
            confirmOrderReference.WithAmazonOrderReferenceId("test")
                .WithMerchantId("test")
                .WithMWSAuthToken("test");

            client.ConfirmOrderReference(confirmOrderReference);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        public void TestCancelOrderReference()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","CancelOrderReference"},
                {"SellerId","test"},
                {"AmazonOrderReferenceId","test"},
                {"CancelationReason","test"},
                {"MWSAuthToken","test"}
            };
            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API CancelOrderReference
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");
            CancelOrderReferenceRequest cancelOrderReference = new CancelOrderReferenceRequest();
            cancelOrderReference.WithAmazonOrderReferenceId("test")
                .WithAmazonOrderReferenceId("test")
                .WithCancelationReason("test")
                .WithMerchantId("test")
                .WithMWSAuthToken("test");
            client.CancelOrderReference(cancelOrderReference);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        public void TestCloseOrderReference()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","CloseOrderReference"},
                {"SellerId","test"},
                {"AmazonOrderReferenceId","test"},
                {"ClosureReason","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API CloseOrderReference
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");
            CloseOrderReferenceRequest closeOrderReference = new CloseOrderReferenceRequest();
            closeOrderReference.WithAmazonOrderReferenceId("test")
                .WithClosureReason("test")
                .WithMerchantId("test")
                .WithMWSAuthToken("test");
            client.CloseOrderReference(closeOrderReference);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        [Test]
        public void TestCloseAuthorization()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","CloseAuthorization"},
                {"SellerId","test"},
                {"AmazonAuthorizationId","test"},
                {"ClosureReason","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API CloseAuthorization
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            CloseAuthorizationRequest closeAuthorization = new CloseAuthorizationRequest();
            closeAuthorization.WithAmazonAuthorizationId("test")
                .WithClosureReason("test")
                .WithMerchantId("test")
                .WithMWSAuthToken("test");

            client.CloseAuthorization(closeAuthorization);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        [Test]
        public void TestAuthorize()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","Authorize"},
                {"SellerId","test"},
                {"AmazonOrderReferenceId","test"},
                {"AuthorizationAmount.Amount","test"},
                {"AuthorizationAmount.CurrencyCode","TEST"},
                {"AuthorizationReferenceId","test"},
                {"CaptureNow","true"},
                {"SellerAuthorizationNote","test"},
                {"TransactionTimeout","5"},
                {"SoftDescriptor","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API Authorize
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");
            AuthorizeRequest authorize = new AuthorizeRequest();
            authorize.WithAmazonOrderReferenceId("test")
                .WithAmount("test")
                .WithAuthorizationReferenceId("test")
                .WithCaptureNow(true)
                .WithCurrencyCode("TEST")
                .WithMerchantId("test")
                .WithMWSAuthToken("test")
                .WithSellerAuthorizationNote("test")
                .WithTransactionTimeout(5)
                .WithSoftDescriptor("test");
            client.Authorize(authorize);

            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        [Test]
        public void TestGetAuthorizationDetails()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","GetAuthorizationDetails"},
                {"SellerId","test"},
                {"AmazonAuthorizationId","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API GetAuthorizationDetails
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");
            GetAuthorizationDetailsRequest getAuthorizationDetails = new GetAuthorizationDetailsRequest();
            getAuthorizationDetails.WithAmazonAuthorizationId("test")
                .WithMerchantId("test")
                .WithMWSAuthToken("test");
            client.GetAuthorizationDetails(getAuthorizationDetails);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        [Test]
        public void TestCapture()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","Capture"},
                {"SellerId","test"},
                {"AmazonAuthorizationId","test"},
                {"CaptureAmount.Amount","test"},
                {"CaptureAmount.CurrencyCode","TEST"},
                {"CaptureReferenceId","test"},
                {"SellerCaptureNote","test"},
                {"SoftDescriptor","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API Capture
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            CaptureRequest capture = new CaptureRequest();
            capture.WithAmazonAuthorizationId("test")
                .WithAmount("test")
                .WithCaptureReferenceId("test")
                .WithCurrencyCode("TEST")
                .WithMerchantId("test")
                .WithMWSAuthToken("test")
                .WithSellerCaptureNote("test")
                .WithSoftDescriptor("test");
            client.Capture(capture);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        [Test]
        public void TestGetCaptureDetails()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","GetCaptureDetails"},
                {"SellerId","test"},
                {"AmazonCaptureId","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API GetCaptureDetails
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");
            GetCaptureDetailsRequest getCaptureDetails = new GetCaptureDetailsRequest();
            getCaptureDetails.WithAmazonCaptureId("test")
                .WithMerchantId("test")
                .WithMWSAuthToken("test");
            client.GetCaptureDetails(getCaptureDetails);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);

        }

        [Test]
        public void TestRefund()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","Refund"},
                {"SellerId","test"},
                {"AmazonCaptureId","test"},
                {"RefundReferenceId","test"},
                {"RefundAmount.Amount","test"},
                {"RefundAmount.CurrencyCode","TEST"},
                {"SellerRefundNote","test"},
                {"SoftDescriptor","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API Refund
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");
            RefundRequest refund = new RefundRequest();
            refund.WithAmazonCaptureId("test")
                .WithAmount("test")
                .WithCurrencyCode("TEST")
                .WithMerchantId("test")
                .WithMWSAuthToken("test")
                .WithRefundReferenceId("test")
                .WithSellerRefundNote("test")
                .WithSoftDescriptor("test");
            client.Refund(refund);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        [Test]
        public void TestGetRefundDetails()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","GetRefundDetails"},
                {"SellerId","test"},
                {"AmazonRefundId","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API GetRefundDetails
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            GetRefundDetailsRequest getRefundDetails = new GetRefundDetailsRequest();
            getRefundDetails.WithAmazonRefundId("test")
                .WithMerchantId("test")
                .WithMWSAuthToken("test");
            client.GetRefundDetails(getRefundDetails);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        [Test]
        public void TestGetServiceStatus()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","GetServiceStatus"},
                {"SellerId","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API GetServiceStatus
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");
            GetServiceStatusRequest getServicetatus = new GetServiceStatusRequest();
            getServicetatus.WithMerchantId("test")
                .WithMWSAuthToken("test");
            client.GetServiceStatus(getServicetatus);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        [Test]
        public void TestCreateOrderReferenceForId()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","CreateOrderReferenceForId"},
                {"SellerId","test"},
                {"Id","test"},
                {"IdType","test"},
                {"InheritShippingAddress","true"},
                {"ConfirmNow","true"},
                {"OrderReferenceAttributes.OrderTotal.Amount","test"},
                {"OrderReferenceAttributes.OrderTotal.CurrencyCode","TEST"},
                {"OrderReferenceAttributes.PlatformId","test"},
                {"OrderReferenceAttributes.SellerNote","test"},
                {"OrderReferenceAttributes.SellerOrderAttributes.SellerOrderId","test"},
                {"OrderReferenceAttributes.SellerOrderAttributes.StoreName","test"},
                {"OrderReferenceAttributes.SellerOrderAttributes.CustomInformation","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API CreateOrderReferenceForId
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");
            CreateOrderReferenceForIdRequest createOrderReferenceForId = new CreateOrderReferenceForIdRequest();
            createOrderReferenceForId.WithAmount("test")
                .WithConfirmNow(true)
                .WithCurrencyCode("TEST")
                .WithCustomInformation("test")
                .WithId("test")
                .WithIdType("test")
                .WithInheritShippingAddress(true)
                .WithMerchantId("test")
                .WithMWSAuthToken("test")
                .WithPlatformId("test")
                .WithSellerNote("test")
                .WithSellerOrderId("test")
                .WithStoreName("test");
            client.CreateOrderReferenceForId(createOrderReferenceForId);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        [Test]
        public void TestGetBillingAgreementDetails()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","GetBillingAgreementDetails"},
                {"SellerId","test"},
                {"AmazonBillingAgreementId","test"},
                {"AddressConsentToken","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API GetBillingAgreementDetails
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");
            GetBillingAgreementDetailsRequest getBillingAgreement = new GetBillingAgreementDetailsRequest();
            getBillingAgreement.WithaddressConsentToken("test")
                .WithAmazonBillingAgreementId("test")
                .WithMerchantId("test")
                .WithMWSAuthToken("test");
            client.GetBillingAgreementDetails(getBillingAgreement);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        [Test]
        public void TestSetBillingAgreementDetails()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","SetBillingAgreementDetails"},
                {"SellerId","test"},
                {"AmazonBillingAgreementId","test"},
                {"BillingAgreementAttributes.PlatformId","test"},
                {"BillingAgreementAttributes.SellerNote","test"},
                {"BillingAgreementAttributes.SellerBillingAgreementAttributes.SellerBillingAgreementId","test"},
                {"BillingAgreementAttributes.SellerBillingAgreementAttributes.CustomInformation","test"},
                {"BillingAgreementAttributes.SellerBillingAgreementAttributes.StoreName","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API SetBillingAgreementDetails
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");
            SetBillingAgreementDetailsRequest setBillingAgreementDetails = new SetBillingAgreementDetailsRequest();
            setBillingAgreementDetails.WithAmazonBillingAgreementId("test")
                .WithCustomInformation("test")
                .WithMerchantId("test")
                .WithMWSAuthToken("test")
                .WithPlatformId("test")
                .WithSellerBillingAgreementId("test")
                .WithSellerNote("test")
                .WithStoreName("test");
            client.SetBillingAgreementDetails(setBillingAgreementDetails);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        [Test]
        public void TestConfirmBillingAgreement()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","ConfirmBillingAgreement"},
                {"SellerId","test"},
                {"AmazonBillingAgreementId","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API ConfirmBillingAgreement
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");
            ConfirmBillingAgreementRequest confirmBillingAgreement = new ConfirmBillingAgreementRequest();
            confirmBillingAgreement.WithAmazonBillingreementId("test")
                .WithMerchantId("test")
                .WithMWSAuthToken("test");
            client.ConfirmBillingAgreement(confirmBillingAgreement);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        [Test]
        public void TestValidateBillingAgreement()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","ValidateBillingAgreement"},
                {"SellerId","test"},
                {"AmazonBillingAgreementId","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API ValidateBillingAgreement
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            ValidateBillingAgreementRequest validateBillingAgreement = new ValidateBillingAgreementRequest();
            validateBillingAgreement.WithAmazonBillingAgreementId("test")
                .WithMerchantId("test")
                .WithMWSAuthToken("test");
            client.ValidateBillingAgreement(validateBillingAgreement);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        [Test]
        public void TestAuthorizeOnBillingAgreement()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","AuthorizeOnBillingAgreement"},
                {"SellerId","test"},
                {"AmazonBillingAgreementId","test"},
                {"AuthorizationReferenceId","test"},
                {"AuthorizationAmount.Amount","test"},
                {"AuthorizationAmount.CurrencyCode","TEST"},
                {"SellerAuthorizationNote","test"},
                {"TransactionTimeout","5"},
                {"CaptureNow","true"},
                {"SoftDescriptor","test"},
                {"SellerNote","test"},
                {"PlatformId","test"},
                {"SellerOrderAttributes.CustomInformation","test"},
                {"SellerOrderAttributes.SellerOrderId","test"},
                {"SellerOrderAttributes.StoreName","test"},
                {"InheritShippingAddress","true"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API AuthorizeOnBillingAgreement
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");
            AuthorizeOnBillingAgreementRequest authorizeOnBillingAgreement = new AuthorizeOnBillingAgreementRequest();
            authorizeOnBillingAgreement.WithAmazonBillingAgreementId("test")
                .WithAmount("test")
                .WithAuthorizationReferenceId("test")
                .WithCaptureNow(true)
                .WithCurrencyCode("TEST")
                .WithPlatformId("test")
                .WithCustomInformation("test")
                .WithInheritShippingAddress(true)
                .WithMerchantId("test")
                .WithMWSAuthToken("test")
                .WithSellerAuthorizationNote("test")
                .WithSellerNote("test")
                .WithSoftDescriptor("test")
                .WithStoreName("test")
                .WithSellerOrderId("test")
                .WithTransactionTimeout(5);
            client.AuthorizeOnBillingAgreement(authorizeOnBillingAgreement);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        [Test]
        public void TestCloseBillingAgreement()
        {
            Dictionary<string, string> expectedParameters = new Dictionary<string, string>()
            {
                {"Action","CloseBillingAgreement"},
                {"SellerId","test"},
                {"AmazonBillingAgreementId","test"},
                {"ClosureReason","test"},
                {"MWSAuthToken","test"}
            };

            // Test direct call to CalculateSignatureAndParametersToString
            Client client = new Client(clientConfig);
            client.SetTimeStamp("0000");

            MethodInfo method = GetMethod("CalculateSignatureAndParametersToString");
            method.Invoke(client, new object[] { expectedParameters }).ToString();
            IDictionary<string, string> expectedParamsDict = client.GetParameters();

            // Test call to the API CloseBillingAgreement
            client = new Client(clientConfig);
            client.SetTimeStamp("0000");
            CloseBillingAgreementRequest closeBillingAgreement = new CloseBillingAgreementRequest();
            closeBillingAgreement.WithAmazonBillingAgreementId("test")
                .WithClosureReason("test")
                .WithMerchantId("test")
                .WithMWSAuthToken("test");
            client.CloseBillingAgreement(closeBillingAgreement);
            IDictionary<string, string> apiParametersDict = client.GetParameters();

            CollectionAssert.AreEqual(apiParametersDict, expectedParamsDict);
        }

        [Test]
        public void TestCharge()
        {
            Client client = new Client(clientConfig);
            try
            {
                ChargeRequest charge = new ChargeRequest();
                charge.WithAmazonReferenceId("S01-TEST")
                    .WithAmount("test")
                    .WithCaptureNow(true)
                    .WithChargeNote("test")
                    .WithChargeOrderId("test")
                    .WithChargeReferenceId("test")
                    .WithCurrencyCode("TEST")
                    .WithCustomInformation("test")
                    .WithInheritShippingAddress(true)
                    .WithMerchantId("test")
                    .WithMWSAuthToken("test")
                    .WithPlatformId("test")
                    .WithSoftDescriptor("test")
                    .WithStoreName("test")
                    .WithTransactionTimeout(5);
                client.Charge(charge);
            }
            catch (NullReferenceException expected)
            {
                Assert.IsTrue(Regex.IsMatch(expected.ToString(), "state not found", RegexOptions.IgnoreCase));
            }
            try
            {
                client = new Client(clientConfig);
                ChargeRequest charge = new ChargeRequest();
                charge.WithAmazonReferenceId("");
                client.Charge(charge);
            }
            catch (MissingFieldException expected)
            {
                Assert.IsTrue(Regex.IsMatch(expected.ToString(), "Amazon Reference ID is a required field and should be a Order Reference ID / Billing Agreement ID", RegexOptions.IgnoreCase));
            }

            try
            {
                client = new Client(clientConfig);
                ChargeRequest charge = new ChargeRequest();
                charge.WithAmazonReferenceId("T01");
                client.Charge(charge);
            }
            catch (InvalidDataException expected)
            {
                Assert.IsTrue(Regex.IsMatch(expected.ToString(), "Invalid Amazon Reference ID", RegexOptions.IgnoreCase));
            }
        }

        [Test]
        public void TestGetUserInfo()
        {
            try
            {
                clientConfig.WithRegion("");
                Client client = new Client(clientConfig);
                client.GetUserInfo("Atza");
            }
            catch (NullReferenceException expected)
            {
                Assert.IsTrue(Regex.IsMatch(expected.ToString(), "is a required parameter", RegexOptions.IgnoreCase));
            }

            try
            {
                clientConfig.WithRegion("us");
                Client client = new Client(clientConfig);
                client.GetUserInfo(null);
            }
            catch (NullReferenceException expected)
            {
                Assert.IsTrue(Regex.IsMatch(expected.ToString(), "Access Token is a required parameter and is not set", RegexOptions.IgnoreCase));
            }
        }

        [Test]
        public void Test500or503()
        {
            string parameters = "Actinon=hjhjhjh=saaa&jg=100";
            try
            {
                Client client = new Client(clientConfig);

                string url = "https://dsenetsdk.ant.amazon.com/500error/500error.aspx";
                client.SetMwsServiceUrl(url);

                MethodInfo method = GetMethod("Invoke");
                method.Invoke(client, new object[] { parameters }).ToString();

            }
            catch (WebException expected)
            {
                Assert.IsTrue(Regex.IsMatch(expected.ToString(), "Maximum number of retry attempts", RegexOptions.IgnoreCase));
            }

        }

        [Test]
        public void TestJsonResponse()
        {
            Hashtable response = new Hashtable();
            response["ResponseBody"] =
            "<GetOrderReferenceDetailsResponse xmlns='http://mws.amazonservices.com/schema/OffAmazonPayments/2013-01-01'>"
            + "<AmazonOrderReferenceId>S01-5806490-2147504</AmazonOrderReferenceId>"
            + "<ExpirationTimestamp>2015-09-27T02:18:33.408Z</ExpirationTimestamp>"
            + "<SellerNote>This is testing API call</SellerNote>"
            + "</GetOrderReferenceDetailsResponse>";

            string json = File.ReadAllText("json.txt");

            ResponseParser responseObj = new ResponseParser(response["ResponseBody"].ToString());
            string jsonResponse = responseObj.ToJson();
            Assert.AreEqual(json, jsonResponse);
        }

        [Test]
        public void TestDictResponse()
        {
            Hashtable response = new Hashtable();
            IDictionary tempDict = null;
            Dictionary<string, string> compareDict = new Dictionary<string, string>();
            response["ResponseBody"] =
            "<GetOrderReferenceDetailsResponse xmlns='http://mws.amazonservices.com/schema/OffAmazonPayments/2013-01-01'>"
            + "<AmazonOrderReferenceId>S01-5806490-2147504</AmazonOrderReferenceId>"
            + "<ExpirationTimestamp>2015-09-27T02:18:33.408Z</ExpirationTimestamp>"
            + "<SellerNote>This is testing API call</SellerNote>"
            + "</GetOrderReferenceDetailsResponse>";

            Dictionary<string, object> dictResponse = new Dictionary<string, object>()
              {
                  {"AmazonOrderReferenceId","S01-5806490-2147504"},
                  {"ExpirationTimestamp","9/27/2015 2:18:33 AM"},
                  {"SellerNote","This is testing API call"}
              };

            ResponseParser responseObj = new ResponseParser(response["ResponseBody"].ToString());
            Dictionary<string, object> returnDictResponse = responseObj.ToDict();

            foreach (KeyValuePair<string, object> item in returnDictResponse)
            {
                object o = returnDictResponse[item.Key];
                if (o is IDictionary)
                {
                    tempDict = (IDictionary)o;
                    tempDict.Remove("@xmlns");
                    foreach (string items in tempDict.Keys)
                    {
                        compareDict.Add(items, tempDict[items].ToString());
                    }
                }
                else
                {
                    compareDict.Add(o.ToString(), tempDict[o.ToString()].ToString());

                }
            }

            CollectionAssert.AreEqual(dictResponse, compareDict);
        }

        private Hashtable SetParametersAndPost(Hashtable fieldMappings, string action)
        {
            Hashtable expectedParameters = SetDefaultValues(fieldMappings);

            expectedParameters.Add("Action", action);

            foreach (DictionaryEntry param in fieldMappings)
            {
                try
                {
                    if (string.IsNullOrEmpty(expectedParameters[param.Value].ToString()))
                    {
                        expectedParameters[param.Value] = "test";
                    }
                }
                catch (NullReferenceException)
                {
                    expectedParameters[param.Value] = "test";
                }
            }

            return expectedParameters;
        }

        private Hashtable SetDefaultValues(Hashtable fieldMappings)
        {
            Hashtable expectedParameters = new Hashtable();

            if (fieldMappings.ContainsKey("platform_id"))
            {
                expectedParameters[fieldMappings["platform_id"]] = "test";
            }

            if (fieldMappings.ContainsKey("currency_code"))
            {
                expectedParameters[fieldMappings["currency_code"]] = "TEST";
            }
            if (fieldMappings.ContainsKey("transaction_timeout"))
            {
                expectedParameters[fieldMappings["transaction_timeout"]] = "5";
            }
            return expectedParameters;
        }

        /// <summary>
        /// Get the private method of the Client class for testing using the Reflection method
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private MethodInfo GetMethod(string methodName)
        {
            Client client = new Client(clientConfig);
            Type type = typeof(Client);
            var method = client.GetType()
                .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
                Assert.Fail(string.Format("{0} method not found", methodName));

            return method;
        }

        private Dictionary<string, string> convertHashToDict(Hashtable input)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (DictionaryEntry item in input)
            {
                dict.Add(item.Key.ToString(), item.Value.ToString());
            }

            return dict;
        }
    }
}