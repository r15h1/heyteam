namespace HeyTeam.Core.Entities {
    public interface ISessionEvaluator {
        void EvaluateSession(Session session, Evaluation evaluation);
    }
}