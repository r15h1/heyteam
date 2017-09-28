using System;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases.Club {
    public class RegisterClubUseCase : IUseCase<RegisterClubRequest, Response<Guid?>>
    {
        private readonly IClubRepository repository;
        private readonly IValidator<RegisterClubRequest> validator;

        public RegisterClubUseCase (IClubRepository repository, IValidator<RegisterClubRequest> validator) {
            if(repository ==null || validator == null) throw new ArgumentNullException();
            this.repository = repository;
            this.validator = validator;
        }
        
        public Response<Guid?> Execute(RegisterClubRequest request)
        {            
            var validationResult = validator.Validate(request);
            if(!validationResult.IsValid)
                return CreateResponse(validationResult);
            
            Entities.Club club = MapClub(request);
            repository.Add(club);
            return new Response<Guid?> (club.Guid);            
        }

        private Response<Guid?> CreateResponse(ValidationResult<RegisterClubRequest> validationResult)
        {
            var response = new Response<Guid?>();
            validationResult.Messages.ForEach((m) => response.AddError(m));
            return response;
        }

        private Entities.Club MapClub(RegisterClubRequest request) 
            => new Entities.Club() { Name = request.ClubName, LogoUrl = request.ClubLogoUrl };
    
    }
}