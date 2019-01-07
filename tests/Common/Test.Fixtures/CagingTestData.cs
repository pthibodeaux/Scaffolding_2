using System;
using System.Collections.Generic;
using WebApiHub.Models.Caging;

namespace Common.TestFixtures
{
	public class CagingTestData
	{
		public static readonly int Org0Id = 1;
		public static readonly int Org1Id = 2;

		public static List<CagingModel> CagingModel => new List<CagingModel>(new[]
		{
			new CagingModel
			{
				Id = 1,
				ClientCode = "CAGETEST"  ,
				DivisionId = 1,
				RevenueAmount = 100,
				ResponseCount = 10,
				BatchCount = 1,
				PaymentMethod = "CC"
			},
			new CagingModel
			{
				Id = 2,
				ClientCode = "CAGETEST"  ,
				DivisionId = 1,
				RevenueAmount = 200,
				ResponseCount = 20,
				BatchCount = 2,
				PaymentMethod = "CC"
			},
			new CagingModel
			{
				Id = 3,
				ClientCode = "CAGETEST"  ,
				DivisionId = 1,
				RevenueAmount = 300,
				ResponseCount = 30,
				BatchCount = 3,
				PaymentMethod = "CC"
			},
        });

	    public static List<string> CagingTestCsv => new List<string>(new[]
	    {
	        "DepositDate,RevenueAmount,BatchCount,ResponseCount",
	        "\"3/26/2018\",\"100.00\",\"1\",\"10\"",
	        "\"3/26/2018\",\"200.00\",\"2\",\"20\"",
	        "\"3/26/2018\",\"300.00\",\"3\",\"30\""
        });

        public static CagingRollupCounts CagingRollupCounts => 
            new CagingRollupCounts
            {
                BatchesCount = 3,
                CardTypeCount = 2,
                DailyDepositCount = 10,
                PaymentMethodCount = 1,
                ResponsesCount = 20
            };

		public static List<CagingGrossRevenue> CagingGrossRevenue 
			= new List<CagingGrossRevenue>(new[]
			{ 
				new CagingGrossRevenue()
				{
					PeriodStart = DateTime.Parse("11/1/2018"),
					TotalPaymentAmount = 100,
					TotalResponses = 30,
					TotalBatches = 3
				},
				new CagingGrossRevenue()
				{
					PeriodStart = DateTime.Parse("12/1/2018"),
					TotalPaymentAmount = 200,
					TotalResponses = 60,
					TotalBatches = 6
				},

			});


	}
}
