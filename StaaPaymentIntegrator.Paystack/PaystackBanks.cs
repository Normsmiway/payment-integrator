﻿using System;
using System.Net;
using System.Threading.Tasks;
using Staaworks.PaymentIntegrator.Interfaces.Requests.Banks;
using Staaworks.PaymentIntegrator.Interfaces.Responses.Banks;
using Staaworks.PaymentIntegrator.Paystack.Implementations.Responses.Banks;
using Staaworks.PaymentIntegrator.Paystack.Utilities;
using Staaworks.PaymentIntegrator.Providers;
using static Staaworks.PaymentIntegrator.Paystack.Utilities.RequestPreparations;

namespace Staaworks.PaymentIntegrator.Paystack
{
    public partial class Paystack : IBanksProvider
    {
        public string BanksListUrl { get; private set; }

        public string BankAccountNameQueryUrl { get; private set; }

        string IBanksProvider.BanksListUrl => throw new NotImplementedException();

        string IBanksProvider.BankAccountNameQueryUrl => throw new NotImplementedException();

        string IProvider.Name => throw new NotImplementedException();

        string IProvider.SecretKey => throw new NotImplementedException();

        /// <summary>
        /// Initialize paystack for miscellaneous bank operations
        /// </summary>
        /// <param name="banksListUrl">The URL to query a list of banks</param>
        /// <param name="bankAccountNameQueryUrl">The URL to use in resolving bank account names</param>
        public void InitializeBanks (string banksListUrl, string bankAccountNameQueryUrl)
        {
            BanksListUrl = banksListUrl ?? throw new ArgumentNullException(nameof(banksListUrl), "The URL for banks list must be provided to use the miscellaneous banks APIs");
            BankAccountNameQueryUrl = bankAccountNameQueryUrl ?? throw new ArgumentNullException(nameof(bankAccountNameQueryUrl), "The URL for bank account name resolution must be provided to use the miscellaneous banks APIs");
        }

        #region Get Banks
        public Task<IBanksResponse> GetBanks ()
        {
            AssertReady(IsBanksAPIReady);
            return PaystackCall<IBanksResponse>.Get(BanksListUrl, SecretKey, OnGetBanksError, OnGetBanksResult);
        }

        private async Task<IBanksResponse> OnGetBanksResult (string json, HttpStatusCode statusCode)
        {
            var response = new BanksResponse();
            await response.Parse(json);
            return response;
        }

        private Task<IBanksResponse> OnGetBanksError (Exception ex) => throw new Exception("An error occurred while getting banks from the Paystack API", ex);
        #endregion

        #region Query Account Name
        public Task<IBankAccountNameQueryResponse> QueryAccountName (IBankAccountNameQueryRequest request)
        {
            AssertReady(IsBanksAPIReady);
            return PaystackCall<IBankAccountNameQueryResponse>.Get(BankAccountNameQueryUrl + request.Serialize().Result, SecretKey, OnQueryAccountNameError, OnQueryAccountNameResult);
        }
        
        private async Task<IBankAccountNameQueryResponse> OnQueryAccountNameResult (string json, HttpStatusCode statusCode)
        {
            var response = new BankAccountNameQueryResponse();
            await response.Parse(json);
            return response;
        }

        private Task<IBankAccountNameQueryResponse> OnQueryAccountNameError (Exception ex) => throw new Exception("An error occurred while querying the account name from the Paystack API", ex);
        #endregion

        private void IsBanksAPIReady ()
        {
            if (BankAccountNameQueryUrl == null || BanksListUrl == null)
            {
                throw new NotSupportedException("You must first call `InitializeBanks` to use the miscellaneous banks APIs");
            }
        }
    }
}