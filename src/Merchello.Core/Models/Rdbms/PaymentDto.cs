﻿using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchPayment")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class PaymentDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }        

        [Column("customerKey")]
        [ForeignKey(typeof(CustomerDto), Name = "FK_merchPayment_merchCustomer", Column = "pk")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchPaymentCustomer")]
        public Guid CustomerKey { get; set; }
        
        [Column("providerKey")]
        public Guid ProviderKey { get; set; }

        [Column("paymentTfKey")]
        public Guid PaymentTfKey { get; set; }

        [Column("paymentMethodName")]
        public string PaymentMethodName { get; set; }

        [Column("referenceNumber")]
        public string ReferenceNumber { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("authorized")]
        [Constraint(Default = "1")]
        public bool Authorized { get; set; }

        [Column("collected")]
        [Constraint(Default = "1")]
        public bool Collected { get; set; }

        [Column("extendedData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ExtendedData { get; set; }

        [Column("exported")]
        [Constraint(Default = "0")]
        public bool Exported { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

        [ResultColumn]
        public CustomerDto CustomerDto { get; set; }

    }
}