using System;
using Xunit;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Exceptions;

namespace HeyTeam.Tests {
    public class EvaluationTests {
        [Fact]
        public void NullCannotBeSetAsEvaluator() {            
            Assert.Throws<ArgumentNullException>(() => new Evaluation(null) { Comments = "This is my evaluation" });
        }

        [Fact]
        public void UnRegisteredPlayerCannotBeSetAsEvaluator() {            
            var player = new Player();
            Assert.Throws<ArgumentNullException>(() => new Evaluation(player) { Comments = "This is my evaluation" });
        }

        [Fact]
        public void NullSessionCannotBeEvaluated() {            
            var player = new Player(){ Id = 1 };
            var evaluation = new Evaluation(player) { Comments = "This is an evaluation" };            
            Assert.Throws<ArgumentNullException>(() => player.EvaluateSession(null, evaluation));
        }       

        [Fact]
        public void NullEvaluationIsNotValid() {            
            var player = new Player(){ Id = 1 };
            var session = new Session { Id = 1 };
            Assert.Throws<ArgumentNullException>(() => player.EvaluateSession(session, null));
        }       

        [Fact]
        public void UnRegisteredSessionsCannotBeEvaluated() {            
            var player = new Player(){ Id = 1 };
            var evaluation = new Evaluation(player) { Comments = "This is an evaluation" };
            var session = new Session();
            Assert.Throws<IllegalOperationException>(() => player.EvaluateSession(session, evaluation));
        } 

        [Fact]
        public void EvaluationsMustHaveAValidComment() {            
            var player = new Player(){ Id = 1 };
            var evaluation = new Evaluation(player);
            var session = new Session() { Id = 1 };
            Assert.Throws<IllegalOperationException>(() => player.EvaluateSession(session, evaluation));
        }    

        [Fact]
        public void EvaluationsCommentsCannotBeWhiteSpace() {            
            var player = new Player(){ Id = 1 };
            var evaluation = new Evaluation(player) { Comments = " " };
            var session = new Session() { Id = 1 };
            Assert.Throws<IllegalOperationException>(() => player.EvaluateSession(session, evaluation));
        }   
    }
}