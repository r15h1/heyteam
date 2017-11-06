using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.UseCases.Club;
using HeyTeam.Core.UseCases.Coach;
using HeyTeam.Core.Validation;
using HeyTeam.Lib.Data;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Validation;
using HeyTeam.Tests.Data;
using System;
using Xunit;

namespace HeyTeam.Tests.UseCases {
	public class CoachAdditionTests {
		private readonly string connectionString;
		private readonly ICoachRepository coachRepository;
		private readonly IClubRepository clubRepository;
		private readonly Guid clubId;
		private readonly IUseCase<AddCoachRequest, Response<Guid?>> useCase;
		private const int EmptyRequestErrorCount = 5;

		public CoachAdditionTests() {
			connectionString = $"Data Source=file:{Guid.NewGuid().ToString()}.sqlite";
			Database.Create(connectionString);			
			clubRepository = new ClubRepository(new Data.ConnectionFactory(new DatabaseSettings { ConnectionString = connectionString }));
			IValidator<AddCoachRequest> validator = new AddCoachRequestValidator();
			coachRepository = new CoachRepository(new Data.ConnectionFactory(new DatabaseSettings { ConnectionString = connectionString }));
			var registerUseCase = new RegisterClubUseCase(clubRepository, new RegisterClubRequestValidator());
			RegisterClubRequest registerRequest = new RegisterClubRequest { Name = "Manchester United", Url = "http://manutd.com" };
			var registerResponse = registerUseCase.Execute(registerRequest);
			this.clubId = registerResponse.Result.Value;
			this.useCase = new AddCoachUseCase(clubRepository, coachRepository, validator);
		}

		[Fact]
		public void RequestCannotBeNull() {
			var response = useCase.Execute(null);
			Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
		}

		[Fact]
		public void RequestMustBeValid() {
			var request = new AddCoachRequest {  };
			var response = useCase.Execute(request);
			Assert.True(!response.WasRequestFulfilled && response.Errors.Count == EmptyRequestErrorCount);
		}

		[Fact]
		public void ClubIdCannotBeEmpty() {
			var request = new AddCoachRequest { ClubId = Guid.Empty };
			var response = useCase.Execute(request);
			Assert.True(!response.WasRequestFulfilled && response.Errors.Count == EmptyRequestErrorCount);
		}

		[Fact]
		public void CoachCannotBeAddedToInexistentClub() {
			var request = BuildRequest(Guid.NewGuid(), DateTime.Now.AddYears(-35));
			var response = useCase.Execute(request);
			Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1 && typeof(EntityNotFoundException) == response.Exception.GetType());
		}

		[Fact]
		public void FirstNameCannotBeNull() {
			var request = BuildRequest(clubId, DateTime.Now.AddYears(-35), firstName: null);
			var response = useCase.Execute(request);
			Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
		}

		[Fact]
		public void FirstNameCannotBeEmpty() {
			var request = BuildRequest(clubId, DateTime.Now.AddYears(-35), firstName: string.Empty);
			var response = useCase.Execute(request);
			Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
		}

		[Fact]
		public void FirstNameCannotBeWhiteSpace() {
			var request = BuildRequest(clubId, DateTime.Now.AddYears(-35), firstName: "  ");
			var response = useCase.Execute(request);
			Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
		}

		[Fact]
		public void LastNameCannotBeNull() {
			var request = BuildRequest(clubId, DateTime.Now.AddYears(-35), lastName: null);
			var response = useCase.Execute(request);
			Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
		}

		[Fact]
		public void LastNameCannotBeEmpty() {
			var request = BuildRequest(clubId, DateTime.Now.AddYears(-35), lastName: string.Empty);
			var response = useCase.Execute(request);
			Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
		}

		[Fact]
		public void LastNameCannotBeWhiteSpace() {
			var request = BuildRequest(clubId, DateTime.Now.AddYears(-35), lastName: "  ");
			var response = useCase.Execute(request);
			Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
		}

		[Fact]
		public void EmailCannotBeNull() {
			var request = BuildRequest(clubId, DateTime.Now.AddYears(-35), email: null);
			var response = useCase.Execute(request);
			Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
		}

		[Fact]
		public void EmailCannotBeEmpty() {
			var request = BuildRequest(clubId, DateTime.Now.AddYears(-35), email: string.Empty);
			var response = useCase.Execute(request);
			Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
		}

		[Fact]
		public void EmailCannotBeWhiteSpace() {
			var request = BuildRequest(clubId, DateTime.Now.AddYears(-35), email: "  ");
			var response = useCase.Execute(request);
			Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
		}

		[Fact]
		public void EmailCannotBeInvalid() {
			var request = BuildRequest(clubId, DateTime.Now.AddYears(-35), email: "joe@");
			var response = useCase.Execute(request);
			Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
		}

		[Fact]
		public void PhoneCannotBeNull() {
			var request = BuildRequest(clubId, DateTime.Now.AddYears(-35), phone: null);
			var response = useCase.Execute(request);
			Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
		}

		[Fact]
		public void PhoneCannotBeEmpty() {
			var request = BuildRequest(clubId, DateTime.Now.AddYears(-35), phone: string.Empty);
			var response = useCase.Execute(request);
			Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
		}

		[Fact]
		public void PhoneCannotBeWhiteSpace() {
			var request = BuildRequest(clubId, DateTime.Now.AddYears(-35), phone: "  ");
			var response = useCase.Execute(request);
			Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
		}

		[Fact]
		public void CoachMustBeAtLeast15YearsOld() {
			var request14 = BuildRequest(clubId, DateTime.Now.AddYears(-14));
			var response14 = useCase.Execute(request14);
			Assert.True(!response14.WasRequestFulfilled && response14.Errors.Count == 1);

			var request15 = BuildRequest(clubId, DateTime.Now.AddYears(-15).AddMinutes(-1));
			var response15 = useCase.Execute(request15);
			Assert.True(response15.WasRequestFulfilled);
		}

		[Fact]
		public void VerifyNewCoachAFterSavingToDb() {
			var request = BuildRequest(clubId, DateTime.Now.AddYears(-15).AddMinutes(-1));
			request.Qualifications = "Eufa License";
			var response = useCase.Execute(request);
			Assert.True(response.WasRequestFulfilled);

			var savedCoach = coachRepository.GetCoach(response.Result.Value);
			Assert.Equal(request.ClubId, savedCoach.ClubId);
			Assert.Equal(request.DateOfBirth, savedCoach.DateOfBirth);
			Assert.Equal(request.Email, savedCoach.Email);
			Assert.Equal(request.FirstName, savedCoach.FirstName);
			Assert.Equal(request.LastName, savedCoach.LastName);
			Assert.Equal(request.Phone, savedCoach.Phone);
			Assert.Equal(request.Qualifications, savedCoach.Qualifications);			
		}

		private AddCoachRequest BuildRequest (Guid clubId, DateTime dateOfBirth, string firstName="Joe", string lastName = "Mour", 
			string email = "joe.mour@heyteam.com", string phone = "647.123.4567") {

			return new AddCoachRequest {
				ClubId = clubId,
				Email = email,
				FirstName = firstName,
				LastName = lastName,
				Phone = phone,
				DateOfBirth = dateOfBirth
			};
		}
	}
}