using System;
using System.Collections.Generic;
using WebApiHub.Models.Activities;

namespace Common.TestFixtures
{
	public class ActivityTestData
	{
		public static List<PaymentDetail> PaymentDetails => new List<PaymentDetail>
		{
			new PaymentDetail
			{
				PaymentAmount = 30,
				PaymentMethodCode = PaymentMethodCode.CreditCard,
			},
			new PaymentDetail
			{
				PaymentAmount = 100,
				PaymentMethodCode = PaymentMethodCode.Check,
			},
			new PaymentDetail
			{
				PaymentAmount = 10,
				PaymentMethodCode = PaymentMethodCode.DonorAdvisoryFund
			}
		};

		public static List<Response> ResponsesActivities => new List<Response>
		{
			new Response
			{
				ActivityId = 1,
				ActivityDate = new DateTime(1998,3,1),
				ResponseId = 1,
				ResponseCode = ResponseCode.Contribution,
				ResponseChannelCode = ResponseChannelCode.Deposit,
				DonorId = 1,
			},
			new Response
			{
				ActivityId = 2,
				ActivityDate = new DateTime(2018,3,6),
				ResponseId = 2,
				ResponseCode = ResponseCode.CreditedContribution,
				ResponseChannelCode = ResponseChannelCode.Email,
				DonorId = 1,
			},
			new Response
			{
				ActivityId = 3,
				ActivityDate = new DateTime(2018,1,1),
				ResponseId = 3,
				ResponseCode = ResponseCode.NonMonetary,
				ResponseChannelCode = ResponseChannelCode.Mail,
				DonorId = 1,
			}
		};

		public static List<TouchPoint> TouchPointActivities => new List<TouchPoint>
		{
			new TouchPoint
			{
				ActivityId = 4,
				ActivityDate = new DateTime(2018, 1, 12),
				TouchPointId = 1,
				AppealAbbreviation = "12345",
				AppealName = "Test TouchPoint 1",
				DonorId = 1,
				TouchPointMethodCode = TouchPointMethodCode.Email
			},
			new TouchPoint
			{
				ActivityId = 5,
				ActivityDate = new DateTime(2017, 8, 1),
				TouchPointId = 2,
				AppealAbbreviation = "897788",
				AppealName = "Test TouchPoint 2",
				DonorId = 1,
				TouchPointMethodCode = TouchPointMethodCode.Event
			},
			new TouchPoint
			{
				ActivityId = 6,
				ActivityDate = new DateTime(2016, 4, 1),
				TouchPointId = 3,
				AppealAbbreviation = "999999",
				AppealName = "Test TouchPoint 3",
				DonorId = 1,
				TouchPointMethodCode = TouchPointMethodCode.Email
			},
			new TouchPoint
			{
				ActivityId = 7,
				ActivityDate = new DateTime(2018, 2, 22),
				TouchPointId = 4,
				AppealAbbreviation = "643333",
				AppealName = "Test TouchPoint 4",
				DonorId = 1,
				TouchPointMethodCode = TouchPointMethodCode.Event
			}
		};
	}
}