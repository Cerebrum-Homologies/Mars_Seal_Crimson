using Godot;
using System.Linq;

namespace Mars_Seal_Crimson
{
    public class CharacterMovement
    {
        private Godot.AnimatedSprite gameCharacter;
        private Godot.Navigation2D navigationPlayerPath;
        private bool playerWalkAction = false;
        private bool moveNode = false;
        private float walkSpeed = 250.0f;
        private Vector2[] pathPlotPlayer = new Vector2[] { };
        //Add delegate for TestPlayerPosition() :: test function
        public delegate void TestCharacterPositionDelegate(Vector2 position);
        private TestCharacterPositionDelegate characterPositionFunction;

        public CharacterMovement(Godot.AnimatedSprite pCharacter, Godot.Navigation2D navPath, TestCharacterPositionDelegate testPositionFunction = null)
        {
            gameCharacter = pCharacter;
            navigationPlayerPath = navPath;
            characterPositionFunction = testPositionFunction;
        }

        public void PerformPlayerWalkTo(Vector2 startPosition, Vector2 clickPosition)
        {
            if ((startPosition != null) && (clickPosition != null))
            {
                pathPlotPlayer = navigationPlayerPath.GetSimplePath(startPosition, clickPosition);
                pathPlotPlayer = pathPlotPlayer.Skip(1).Take(pathPlotPlayer.Length - 1).ToArray();
            }
        }

        private void TestPlayerPosition(Vector2 playerPosition)
        {
            if (characterPositionFunction != null)
                characterPositionFunction(playerPosition);
        }

        public void PlayerWalkAction(float speed)
        {
            var lastPosition = gameCharacter.Position;
            for (var x = 0; x < pathPlotPlayer.Length; x++)
            {
                var distanceBetweenPoints = lastPosition.DistanceTo(pathPlotPlayer[x]);
                if (speed <= distanceBetweenPoints)
                {
                    gameCharacter.Position = lastPosition.LinearInterpolate(pathPlotPlayer[x], speed / distanceBetweenPoints);
                    TestPlayerPosition(gameCharacter.Position);
                    break;
                }

                if (speed < 0.0)
                {
                    gameCharacter.Position = pathPlotPlayer[0];
                    moveNode = false;
                    break;
                }

                speed -= distanceBetweenPoints;
                lastPosition = pathPlotPlayer[x];
                pathPlotPlayer = pathPlotPlayer.Skip(1).Take(pathPlotPlayer.Length - 1).ToArray();
            }
            if (pathPlotPlayer.Length <= 0)
            {
                moveNode = false;
            }
        }
    }
}