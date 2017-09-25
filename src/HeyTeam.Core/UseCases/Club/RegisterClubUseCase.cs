using System;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases.Club {
    public class RegisterClubUseCase : IUseCase<RegisterClubRequest, RegisterClubResponse>
    {
        private readonly IClubRepository repository;
        private readonly IValidator<RegisterClubRequest> validator;

        public RegisterClubUseCase (IClubRepository repository, IValidator<RegisterClubRequest> validator) {
            if(repository ==null || validator == null) throw new ArgumentNullException();
            this.repository = repository;
            this.validator = validator;
        }
        
        public RegisterClubResponse Execute(RegisterClubRequest request)
        {
            var validationResult = validator.Validate(request);
            if(!validationResult.IsValid)
                return new RegisterClubResponse(validationResult);
            
            Entities.Club club = MapClub(request);
            repository.Add(club);
            return new RegisterClubResponse (validationResult, club.Guid);
        }

        private Entities.Club MapClub(RegisterClubRequest request) 
            => new Entities.Club() { Name = request.ClubName, LogoUrl = request.ClubLogoUrl };
    
    }
}