using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtDAOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtNUnitTests.DAOTests
{
	internal class FeedbackDAOTests
	{
		private DbContextOptions<BadmintonCourtContext> _options;
		public readonly int primitiveLength = DataTestStorage.feedbackStorage.Count;
		public const string notExistedId = "sfsafsfs";
		public const string existedId = "F1";
		public const string existedUser = "U1";
		public readonly List<string> existedBranches = new List<string>() { "B001", "B003" };
		public readonly List<int> existedRatings = new List<int>() { 4, 5 };


		[SetUp]
		public void Setup()
		{
			var serviceProvider = new ServiceCollection()
			.AddEntityFrameworkInMemoryDatabase()
			.BuildServiceProvider();

			// Configure DbContextOptions to use in-memory database
			_options = new DbContextOptionsBuilder<BadmintonCourtContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.UseInternalServiceProvider(serviceProvider)
				.Options;
			using var context = new BadmintonCourtContext(_options);
			context.Database.EnsureDeleted();
			context.Database.EnsureCreated();
			SeedDatabase(context);
		}

		private void SeedDatabase(BadmintonCourtContext dbContext)
		{
			// Code to seed your database with test data
			if (dbContext.Feedbacks.Count() <= 0)
			{
				List<Feedback> feedbackStorage = DataTestStorage.feedbackStorage;
				foreach (var item in feedbackStorage)
				{
					dbContext.Feedbacks.Add(item);
					dbContext.SaveChanges();
				}
			}
		}
		private List<Feedback> ExtractFeedbackListFromProvidedCategoryOnDataStorage(string keyword, int type) => type == 1 ? DataTestStorage.feedbackStorage.Where(x => x.BranchId == keyword).ToList() : DataTestStorage.feedbackStorage.Where(x => x.Rating == int.Parse(keyword)).ToList();



		[Test]
		public void GetAll_ExpectedAsPrimitiveLength_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			FeedbackDAO dao = new FeedbackDAO(context);
			Assert.AreEqual(primitiveLength, dao.GetAllFeedbacks().Count());
		}


		[Test]
		public void GetById_InputNotExistedId_ResultOfNull_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			FeedbackDAO dao = new FeedbackDAO(context);
			Assert.AreEqual(null, dao.GetFeedbackByFeedbackId(notExistedId));
		}

		[Test]
		public void GetRoleById_InputExistedId_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			FeedbackDAO dao = new FeedbackDAO(context);
			Assert.AreEqual(true, dao.GetFeedbackByFeedbackId(existedId) != null);
		}

		[Test]
		public void AddRole_ResultOfLengthAddedBy1_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			FeedbackDAO dao = new FeedbackDAO(context);
			dao.AddFeedback(new Feedback() { FeedbackId = "F4", UserId = "U999", BranchId = "B001", Content = "Great service ^_^", Period = DateTime.Now, Rating = 4 });
			Assert.AreEqual(primitiveLength + 1, dao.GetAllFeedbacks().Count());
		}

		[Test]
		public void Remove_InputNotExistedId_ResultOfFail_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			FeedbackDAO dao = new FeedbackDAO(context);
			dao.DeleteFeedback(notExistedId);
			Assert.AreEqual(primitiveLength, dao.GetAllFeedbacks().Count());
		}

		[Test]
		public void Remove_InputExistedId_ResultOfSuccess_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			FeedbackDAO dao = new FeedbackDAO(context);
			dao.DeleteFeedback(existedId);
			Assert.AreEqual(primitiveLength - 1, dao.GetAllFeedbacks().Count());
		}

		[Test]
		public void UpdateFeedbacm_InputExistedID_UpdateContentAsPoorService_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			FeedbackDAO dao = new FeedbackDAO(context);
			Feedback feedback = dao.GetFeedbackByFeedbackId(existedId);
			feedback.Content = "Poor service";
			dao.UpdateFeedback(feedback, existedId);
			string actual = dao.GetFeedbackByFeedbackId(existedId).Content;
			Assert.AreEqual("Poor service", actual);
		}


		[Test]
		public void GetFeedbacks_InputBranches_ResultOf2ListOfFeedback_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			FeedbackDAO dao = new FeedbackDAO(context);
			foreach (var item in existedBranches)
			{
				List<Feedback> expected = ExtractFeedbackListFromProvidedCategoryOnDataStorage(item, 1);
				List<Feedback> actual = dao.GetBranchFeedbacks(item);
				Assert.AreEqual(expected.Count(), actual.Count());
			}
		}

		[Test]
		public void GetByUser_InputExistedUser_ResultOfFeedbackList_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			FeedbackDAO dao = new FeedbackDAO(context);
			Assert.AreNotEqual(0, dao.GetA_UserFeedbacks(existedUser).Count());
		}

		[Test]
		public void GetByUser_InputNotExistedUser_ResultOfFeedbackList_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			FeedbackDAO dao = new FeedbackDAO(context);
			Assert.IsTrue(dao.GetA_UserFeedbacks(notExistedId).Count() > 0);
		}

		[Test]
		public void GetByRating_InputExistedRating_ResultsOf2FeedbackLists_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			FeedbackDAO dao = new FeedbackDAO(context);
			foreach (var item in existedRatings)
			{
				int expected = ExtractFeedbackListFromProvidedCategoryOnDataStorage($"{item}", 2).Count();
				int actual = dao.GetFeedbacksByRate(item).Count();
				Assert.AreEqual(expected, actual);
			}
		}

		[Test]
		public void GetByRating_InputNotExistedRating_ResultOfNull_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			FeedbackDAO dao = new FeedbackDAO(context);
			Assert.AreEqual(0, dao.GetFeedbacksByRate(0).Count());
		}
	}
}
