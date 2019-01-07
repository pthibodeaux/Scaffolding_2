using System;
using System.Collections.Generic;
using WebApiHub.Models.Caging;

namespace Common.TestFixtures
{
	public class CagingBatchTestData
	{
		public static readonly int Org0Id = 1;
		public static readonly int Org1Id = 2;

		public static List<CagingModel> CagingBatches => new List<CagingModel>(new[]
		{
			new CagingModel
            {
				Id = 1,
				ClientCode = "CAGETEST"  ,
				BatchNumber = 1,
				DivisionId = 1,
				DepositDate = Convert.ToDateTime("1/1/2018"),
				ProcessedDate = Convert.ToDateTime("1/1/2018"),
				RevenueAmount = 100,
				ResponseCount = 5,
				PaymentMethodCode = 1
			},
			new CagingModel
            {
				Id = 2,
				ClientCode = "CAGETEST"  ,
				BatchNumber = 2,
				DivisionId = 1,
                DepositDate = Convert.ToDateTime("2/1/2018"),
				ProcessedDate = Convert.ToDateTime("2/1/2018"),
                RevenueAmount = 200,
                ResponseCount = 10,
				PaymentMethodCode = 2
			},
			new CagingModel
            {
				Id = 3,
				ClientCode = "CAGETEST"  ,
				BatchNumber = 3,
				DivisionId = 1,
                DepositDate = Convert.ToDateTime("3/1/2018"),
				ProcessedDate = Convert.ToDateTime("3/1/2018"),
                RevenueAmount = 300,
                ResponseCount = 10,
				PaymentMethodCode = 3
			},
        });


	}
}
