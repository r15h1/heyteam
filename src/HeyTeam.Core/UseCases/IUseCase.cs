namespace HeyTeam.Core.UseCases {
    public interface IUseCase<Request, Response> {
         Response Execute(Request request);
    }
}