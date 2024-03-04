using System;

public class GameWorld
{
    public List<Player> Players { get; private set; }
    public List<Bullet> Bullets { get; private set; }
    public List<Target> Targets { get; private set; }

    public GameWorld()
    {
        Players = new List<Player>();
        Bullets = new List<Bullet>();
        Targets = new List<Target>();

        // Initialize your game world here, e.g., spawn targets
        InitializeTargets();
    }

    public void Update()
    {
        // Update players
        foreach (var player in Players)
        {
            player.Update();
        }

        // Update bullets
        foreach (var bullet in Bullets)
        {
            bullet.Update();
            CheckBulletCollisions(bullet);
        }

        // Remove bullets that are out of bounds or have hit a target
        Bullets.RemoveAll(bullet => bullet.IsDestroyed);

        // Implement other game logic here, e.g., check if all targets are destroyed
    }

    private void InitializeTargets()
    {
        // Spawn targets at random positions
        for (int i = 0; i < 3; i++)
        {
            Targets.Add(new Target(/* Position */));
        }
    }

    private void CheckBulletCollisions(Bullet bullet)
    {
        foreach (var target in Targets)
        {
            if (bullet.CollidesWith(target))
            {
                target.IsDestroyed = true;
                bullet.IsDestroyed = true;
                break; // A bullet can only destroy one target
            }
        }

        // Remove destroyed targets
        Targets.RemoveAll(target => target.IsDestroyed);
    }

    public void Draw(Graphics g)
    {
        // Draw players
        foreach (var player in Players)
        {
            player.Draw(g);
        }

        // Draw bullets
        foreach (var bullet in Bullets)
        {
            bullet.Draw(g);
        }

        // Draw targets
        foreach (var target in Targets)
        {
            target.Draw(g);
        }
    }

    // ToString method for serializing game state (not implemented here)
}
