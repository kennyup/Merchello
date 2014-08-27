﻿namespace Merchello.Web
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Configuration;
    using Core.Models;
    using Newtonsoft.Json;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The auditing extensions.
    /// </summary>
    internal static class SalesHistoryAuditingExtensions
    {
        /// <summary>
        /// The localization area
        /// </summary>
        private const string Area = "merchelloAuditLogs";

        /// <summary>
        /// Logs the invoice creation
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        internal static void AuditCreated(this IInvoice invoice)
        {
            var obj = new
            {
                area = Area,
                key = "invoiceCreated",
                invoiceNumber = invoice.PrefixedInvoiceNumber()
            };            

            UpdateAuditLog(invoice.Key, EntityType.Invoice, obj.Serialize());
        }


        /// <summary>
        /// Logs an invoice deletion
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        internal static void AuditDeleted(this IInvoice invoice)
        {
            var obj = new
            {
                area = Area,
                key = "invoiceDeleted",
                invoiceNumber = invoice.PrefixedInvoiceNumber()
            };    

            UpdateAuditLog(invoice.Key, EntityType.Invoice, obj.Serialize());
        }



        /// <summary>
        /// Logs the order creation
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        internal static void AuditCreated(this IOrder order)
        {
            var obj = new
            {
                area = Area,
                key = "orderCreated",
                invoiceNumber = order.PrefixedOrderNumber()
            };

            UpdateAuditLog(order.Key, EntityType.Order, obj.Serialize());
        }
        

        /// <summary>
        /// Logs an order deletion
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        internal static void AuditDeleted(this IOrder order)
        {
            var obj = new
            {
                area = Area,
                key = "orderDeleted",
                orderNumber = order.PrefixedOrderNumber()
            };

            UpdateAuditLog(order.Key, EntityType.Order, obj.Serialize());
        }



        /// <summary>
        /// Logs a shipment creation
        /// </summary>
        /// <param name="shipment">
        /// The shipment.
        /// </param>
        internal static void AuditCreated(this IShipment shipment)
        {            

            var obj = new
            {
                area = Area,
                key = "shipmentCreated",
                itemCount = shipment.Items.Count.ToString(CultureInfo.InvariantCulture)
            };


            UpdateAuditLog(shipment.Key, EntityType.Shipment, obj.Serialize());
        }

        /// <summary>
        /// Logs an authorized payment
        /// </summary>
        /// <param name="payment">
        /// The payment to be collected
        /// </param>
        /// <param name="invoice">
        /// The invoice for which the payment was authorized 
        /// </param>
        internal static void AuditPaymentAuthorize(this IPayment payment, IInvoice invoice)
        {            
            var obj = new
            {
                area = Area,
                key = "paymentAuthorize",
                invoiceTotal = invoice.Total,
                currencyCode = invoice.Items.First().ExtendedData.GetValue(Constants.ExtendedDataKeys.CurrencyCode)
            };

            UpdateAuditLog(payment.Key, EntityType.Payment, obj.Serialize());
        }

        /// <summary>
        /// Logs a captured payment
        /// </summary>
        /// <param name="payment">
        /// The payment.
        /// </param>
        internal static void AuditPaymentCaptured(this IPayment payment)
        {
            var obj = new
            {
                area = Area,
                key = "paymentCaptured",
                invoiceTotal = payment.Amount,
                currencyCode = payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.CurrencyCode)
            };

            UpdateAuditLog(payment.Key, EntityType.Payment, obj.Serialize());
        }

        /// <summary>
        /// Logs a declined payment
        /// </summary>
        /// <param name="payment">
        /// The payment.
        /// </param>
        internal static void AuditPaymentDeclined(this IPayment payment)
        {
            var obj = new
            {
                area = Area,
                key = "paymentDeclined"
            };

            UpdateAuditLog(payment.Key, EntityType.Payment, obj.Serialize());
        }

        /// <summary>
        /// Logs a voided payment.
        /// </summary>
        /// <param name="payment">
        /// The payment.
        /// </param>
        internal static void AuditPaymentVoided(this IPayment payment)
        {
            var obj = new
            {
                area = Area,
                key = "paymentVoided"
            };

            UpdateAuditLog(payment.Key, EntityType.Payment, obj.Serialize());
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        internal static void UpdateAuditLog(Guid key, EntityType entityType, string message)
        {            
            UpdateAuditLog(MerchelloContext.Current, key, entityType, message);
        }


        /// <summary>
        /// Adds a message to the AuditLog
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="entityType">
        /// The <see cref="EntityType"/>
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        private static void UpdateAuditLog(IMerchelloContext merchelloContext, Guid key, EntityType entityType, string message)
        {
            if (string.IsNullOrEmpty(message) || key == Guid.Empty) return;

            if (!MerchelloConfiguration.Current.Section.EnableLogging) return;

            Task.Factory.StartNew(
            () =>
            {
                try
                {
                    merchelloContext.Services.AuditLogService.CreateAuditLogWithKey(key, entityType, message);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(typeof(SalesHistoryAuditingExtensions), string.Format("Failed to log {0} for entityType {1} with key {2}", message, entityType, key), ex);
                }
            });
        }

        /// <summary>
        /// Helper to serialize the objects containing sales history messages
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string Serialize(this object msg)
        {
            return JsonConvert.SerializeObject(msg);
        }
    }
}