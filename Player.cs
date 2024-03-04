using System;

public class Player
{
    public Point Position { get; set; }
    private Image PlayerImage;

    public Player(Point startPosition)
    {
        Position = startPosition;
        PlayerImage = Image.FromFile("Knight_idle1.png"); // Load the player image
    }

    public void Draw(Graphics g)
    {
        g.DrawImage(PlayerImage, Position.X, Position.Y, PlayerImage.Width, PlayerImage.Height);
    }
}
