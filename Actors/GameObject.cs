using uwpKarate.Models;

namespace uwpKarate.Actors
{
    public class GameObject
    {
        public GameObject()
        {
        }

        public GameObject(int xPos, int yPos)
        {
            XPos = xPos;
            YPos = yPos;
        }

        public GameObject(int xPos, int yPos, GraphicsComponent graphicsComponent)
            : this(xPos, yPos)
        {
            GraphicsComponent = graphicsComponent;
        }

        public int XPos { get; set; }
        public int YPos { get; set; }
        public GraphicsComponent GraphicsComponent { get; set; }
    }
}