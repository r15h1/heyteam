using HeyTeam.Core;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Lib.Settings;
using HeyTeam.Util;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace HeyTeam.Lib.Services
{
    public class AccountsService : IAccountsService {
		private readonly IDataProtectionProvider dataProtectionProvider;
		private readonly CryptographicSettings cryptographicSettings;
		private readonly IEmailSender emailSender;
		private readonly IValidator<InvitationRequest> invitationRequestValidator;
        private readonly IClubQuery clubQuery;
		private readonly IPlayerQuery playerQuery;

		public AccountsService(IDataProtectionProvider dataProtectionProvider, 
								IOptions<CryptographicConfiguration> cryptographicConfiguration, 
								IEmailSender emailSender,
								IValidator<InvitationRequest> invitationRequestValidator,
                                IClubQuery clubQuery) {
			this.dataProtectionProvider = dataProtectionProvider;
			this.cryptographicSettings = cryptographicConfiguration.Value.CryptographicSettings;
			this.emailSender = emailSender;
			this.invitationRequestValidator = invitationRequestValidator;
            this.clubQuery = clubQuery;
		}

		public Response CreateMemberAccount(MembershipRequest request) {
			throw new NotImplementedException();
		}

		public Response SendInvitation(InvitationRequest request) {
			var validationResult = invitationRequestValidator.Validate(request);
			if (!validationResult.IsValid)
				return Response.CreateResponse(validationResult.Messages);

			var club = clubQuery.GetClub(request.ClubId);
			if(club == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified club does not exist"));

			IEnumerable<Member> members = clubQuery.GetMembersByEmail(club.Guid, request.Email);
			if(!members.Any())
				return Response.CreateResponse(new EntityNotFoundException("The specified member (email) does not exist in the specified club"));

			var invite = new Invitation {
				ClubId = club.Guid,
				Email = request.Email,
				Expiry = DateTime.Now.AddDays(10)
			};

			try {
				var token = Protect(invite, cryptographicSettings.RegistrationPurposeKey);
				string message = "<html><body><p>Dear Member,</p>" +
									"<p>You're invited to join Mapola Online - a platform that Mapola FC uses to manage team's activity, sessions, games and track player performance.</p>" +
									"<p>Please click on the link below to register.</p>" +
									$"<p><a href='http://localhost:5000/accounts/register?token={token}' target='_blank'>Register Now</a></p><p>Thank you</p><p>Mapola Admin</p></body></html>";
				Task.Run(()=> emailSender.SendEmailAsync(request.Email, "Invitation To Use Mapola Online", message));
				return Response.CreateSuccessResponse();
			} catch(Exception ex) {
				return Response.CreateResponse(ex);
			}
		}

		public (Response Response, Invitation Invitation) VerifyToken(TokenVerificationRequest request) {
            if (request == null || request.Token.IsEmpty())
                return (Response.CreateResponse(new ArgumentNullException("request", "Token cannot be null")), null);

            try {
				Invitation invite = DecryptToken(request.Token);

				if (invite == null || invite.ClubId.IsEmpty() || invite.Email.IsEmpty() || invite.Expiry.IsEmpty())
					return (Response.CreateResponse(new ArgumentNullException("token", "The token is invalid")), null);

				if (invite.Expiry.Value < DateTime.Today)
					return (Response.CreateResponse(new IllegalOperationException("The token is expired")), null);

				var club = clubQuery.GetClub(invite.ClubId.Value);
				if (club == null || club.Guid != request.ClubId)
					return (Response.CreateResponse(new EntityNotFoundException("The token does not correspond to this club")), null);

				IEnumerable<Member> members = clubQuery.GetMembersByEmail(club.Guid, invite.Email);
				if (!members.Any())
					return (Response.CreateResponse(new EntityNotFoundException("The token does not correspond to any player or coach")), null);

				return (Response.CreateSuccessResponse(), invite);
			} catch (Exception ex) {
                return (Response.CreateResponse(ex), null);
            }
		}

		private Invitation DecryptToken(string token) {
			var json = UnProtect(token, cryptographicSettings.RegistrationPurposeKey);
			var invite = JsonConvert.DeserializeObject<Invitation>(json);
			return invite;
		}

		private string Protect(object target, string key) => dataProtectionProvider.CreateProtector(key).Protect(JsonConvert.SerializeObject(target));

		private string Protect(string target, string key) => dataProtectionProvider.CreateProtector(key).Protect(target);

		private string UnProtect(string target, string key) => dataProtectionProvider.CreateProtector(key).Unprotect(target);

	}
}
