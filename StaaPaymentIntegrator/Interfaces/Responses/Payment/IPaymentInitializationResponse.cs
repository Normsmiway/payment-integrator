﻿namespace Staaworks.PaymentIntegrator.Interfaces.Responses.PaymentInitialization
{
    public interface IPaymentInitializationResponse : IResponse
    {
        string Reference { get; }

        string Message { get; }

        string AuthorizationUrl { get; }
    }
}