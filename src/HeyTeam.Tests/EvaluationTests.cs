using System;
using Xunit;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Exceptions;

namespace HeyTeam.Tests
{
    public class EvaluationTests {
        [Fact]
        public void NullCannotBeSetAsEvaluator() {            
            Assert.Throws<ArgumentNullException>(() => new Evaluation(null) { Comments = "This is my evaluation" });
        }

        [Fact]
        public void UnRegisteredPlayerCannotBeSetAsEvaluator() {            
            ISessionEvaluator evaluator = new Player();
            Assert.Throws<ArgumentNullException>(() => new Evaluation(evaluator) { Comments = "This is my evaluation" });
        }

        [Fact]
        public void NullSessionCannotBeEvaluated() {            
            ISessionEvaluator evaluator = new Player(){ Id = 1 };
            var evaluation = new Evaluation(evaluator) { Comments = "This is an evaluation" };            
            Assert.Throws<ArgumentNullException>(() => evaluator.EvaluateSession(null, evaluation));
        }       

        [Fact]
        public void NullEvaluationIsNotValid() {            
            ISessionEvaluator evaluator = new Player(){ Id = 1 };
            var session = new Session { Id = 1 };
            Assert.Throws<ArgumentNullException>(() => evaluator.EvaluateSession(session, null));
        }       

        [Fact]
        public void UnRegisteredSessionsCannotBeEvaluated() {            
            ISessionEvaluator evaluator = new Player(){ Id = 1 };
            var evaluation = new Evaluation(evaluator) { Comments = "This is an evaluation" };
            var session = new Session();
            Assert.Throws<IllegalOperationException>(() => evaluator.EvaluateSession(session, evaluation));
        } 

        [Fact]
        public void EvaluationsMustHaveAValidComment() {            
            ISessionEvaluator evaluator = new Player(){ Id = 1 };
            var evaluation = new Evaluation(evaluator);
            var session = new Session() { Id = 1 };
            Assert.Throws<IllegalOperationException>(() => evaluator.EvaluateSession(session, evaluation));
        }    

        [Fact]
        public void EvaluationsCommentsCannotBeWhiteSpace() {            
            ISessionEvaluator evaluator = new Player(){ Id = 1 };
            var evaluation = new Evaluation(evaluator) { Comments = " " };
            var session = new Session() { Id = 1 };
            Assert.Throws<IllegalOperationException>(() => evaluator.EvaluateSession(session, evaluation));
        }   
    }
}