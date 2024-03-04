using System;

public class Bullet
{
    public Point Position { get; set; }
    private Image BulletImage;

    public Bullet(Point startPosition)
    {
        Position = startPosition;
        BulletImage = Image.FromFile(@"path\to\bullet.png"); // Load the bullet image
    }

    public void Draw(Graphics g)
    {
        g.DrawImage(BulletImage, Position.X, Position.Y, BulletImage.Width, BulletImage.Height);
    }
}
