using Assets.KLib.Source.Events;
using Assets.Source.Balls;

namespace Assets.Source.Events {
    class GameOverEvent : IEvent {
        public GameOverEvent(BallFacade ballFacade) {
            BallFacade = ballFacade;
        }

        public BallFacade BallFacade { get; }
    }
}
