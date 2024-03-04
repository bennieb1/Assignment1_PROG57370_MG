using System;

public class Target
{
    public Point Position { get; set; }
    private Image TargetImage;

    public Target(Point startPosition)
    {
        Position = startPosition;
        TargetImage = Image.FromFile("bomb.png"); // Load the target image
    }

    public void Draw(Graphics g)
    {
        if (!IsDestroyed) // Assuming IsDestroyed is a property you have defined
        {
            g.DrawImage(TargetImage, Position.X, Position.Y, TargetImage.Width, TargetImage.Height);
        }
    }
}
