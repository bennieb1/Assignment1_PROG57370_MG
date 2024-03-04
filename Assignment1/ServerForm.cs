using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UDPClientServer;

namespace Assignment1
{
    public partial class ServerForm : Form1
    {
        // Players identified by their ID, with their properties stored in a tuple (for example, position as a Point)
        private Dictionary<string, (Point Position, bool IsAlive)> players = new Dictionary<string, (Point, bool)>();

        // Targets with an integer ID and their position
        private Dictionary<int, Point> target = new Dictionary<int, Point>();

        // Bullets with an identifier for the shooting player and their position
        private List<(string PlayerId, Point Position, Point Direction)> bullets = new List<(string, Point, Point)>();

        private List<TcpClient> connectedClients = new List<TcpClient>();

        private readonly UDPController m_udp = new UDPController();
        private List<PictureBox> targets = new List<PictureBox>();
        private List<Point> targetPositions = new List<Point>();

        private PictureBox m_character = new PictureBox();
        private Random random = new Random();
        //private PictureBox m_character;


        public ServerForm()
        {
            InitializeComponent();

           
            SetupServer();


        }
        private void SetupServer()
        {
            // Assuming SetupServer configures the UDPController to listen on a specific port
            m_udp.Server("127.0.0.1", 27015);
            m_udp.Send("Target");
            Task.Run(ProcessClientMessages);
            
            // Start processing messages immediately
        }

        private void StartZombieUpdateLoop()
        {
            var timer = new System.Threading.Timer(_ =>
            {
                UpdateZombiePosition();
            }, null, 0, 100); // Send updates every 100 milliseconds
        }

        private void UpdateZombiePosition()
        {
            // Implement your logic to get/update the zombie's position here
            Point zombiePosition = GetRandomPosition(); // Assuming this generates or reads the current zombie position
            SendPlayerPosition(zombiePosition);
        }
        private void ProcessClientMessages()
        {
            while (true)
            {
                string message = m_udp.GetNextMessage();
                if (!string.IsNullOrEmpty(message))
                {
                    ProcessMessage(message);
                }
            }

        }
        private void ProcessMessage(string message)
        {
            // Update your server's UI or internal state based on the message
            if (message.StartsWith("Connected"))
            {
                Point zombiePosition = GetRandomPosition();
                // Send the zombie position back to the client
                SendPlayerPosition(zombiePosition);
            }
            else if (message.StartsWith("RequestTargets"))
            {
                SpawnTargets(); // This method now also sends the target positions to the client.


            }
            else if (message.StartsWith("Move:W"))
            {
                MoveCharacter(message);
            }
            else if (message.StartsWith("Move:S"))
            {
                MoveCharacter(message);
            }
            else if (message.StartsWith("Move:A"))
            {
                MoveCharacter(message);
            }
            else if (message.StartsWith("Move:D"))
            {
                MoveCharacter(message);
            }
            else if (message.StartsWith("Shoot:"))
            {
                Shoot(message.Substring(6)); // Remove the "Shoot:" prefix
            }
            else
            { }
            // Handle other messages...
        }
        private void AddCharacter()
        {
            m_character.Image = Image.FromFile("Zombie.png");
            m_character.SetBounds(0, 0, 100, 100);
            panelMoveCharacter.Controls.Add(m_character);
        }

        public void UpdateList()
        {
            string message = m_udp.GetNextMessage();
            if (!string.IsNullOrEmpty(message))
            {
                // Process the message
                ProcessMessage(message);

                // Update the UI thread with the new message
                this.Invoke((MethodInvoker)delegate
                {
                    // Check if the message is already in the list to avoid duplicates
                    if (!listBoxServer.Items.Contains(message))
                    {
                        listBoxServer.Items.Add(message);
                        // You may want to automatically scroll to the latest message
                        listBoxServer.SelectedIndex = listBoxServer.Items.Count - 1;
                        listBoxServer.ClearSelected();
                    }
                });
            }

        }
        private void SpawnPlayer()
        {
            // Generate a random position for the player
            Point playerPosition = GetRandomPosition();
            // Send the player position to the client
            SendPlayerPosition(playerPosition);
            // Render the player at the position (for the server-side debugging view)
            Invoke((MethodInvoker)delegate
            {
                m_character.Location = playerPosition;
            });
            SendPlayerPosition(m_character.Location);
        }
        private void AddNewPlayer(string playerId, Point spawnPosition)
        {
            players.Add(playerId, (spawnPosition, true));
            // Notify clients or perform other logic as necessary
        }

        private void SpawnTargets()
        {
            targets.Clear();
            targetPositions.Clear();

            // Generate random positions for 3 targets
            for (int i = 0; i < 3; i++)
            {
                Point targetPosition = GetRandomPosition();
                targetPositions.Add(targetPosition); // Store the position for later use
            }

            // Now iterate over the positions and create PictureBoxes for the targets
            foreach (var position in targetPositions)
            {
                PictureBox target = new PictureBox
                {
                    Image = Image.FromFile("bomb.png"), // Make sure this file exists in your project
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Size = new Size(30, 30), // Set target size
                    Location = position
                };

                Invoke((MethodInvoker)delegate
                {
                    panelMoveCharacter.Controls.Add(target);
                    target.BringToFront();
                });

                targets.Add(target); // Add the PictureBox to your list of targets
            }

            // After spawning targets, you would send their positions to the client so the client can also spawn them


            SendTargetPositions();

        }

        private void SpawnTarget(int targetId, Point position)
        {
            // Logic to create and track the new target
            // For example, adding it to a collection of active targets:
            target.Add(targetId, position); // Assuming 'targets' is a Dictionary<int, Point>

            // Then, notify clients about the new target
            SendToAllClients($"SpawnTarget:{targetId}:{position.X},{position.Y}");
        }
        private void SendToAllClients(string message)
        {
            foreach (var client in connectedClients) // Assuming you have a collection of client connections
            {
                SendToClient(client, message);
            }
        }

        private void SendToClient(TcpClient client, string message)
        {
            // Example method to send a message to a single client
            var byteMessage = Encoding.ASCII.GetBytes(message + "\n"); // Ensure message ends with a newline or similar delimiter
            client.GetStream().Write(byteMessage, 0, byteMessage.Length);
        }
        private void SendTargetPositions()
        {
            foreach (var position in targetPositions)
            {
                SendTargetPosition(position);
            }
        }
        private Point GetRandomPosition()
        {
            // Return a random position within the bounds of your panel or form
            int x = random.Next(panelMoveCharacter.Bounds.Width);
            int y = random.Next(panelMoveCharacter.Bounds.Height);
            return new Point(x, y);
        }
        private void SendPlayerPosition(Point position)
        {
            // Use your UDPController to send this information to the client
            string message = $"SpawnPlayer:{position.X},{position.Y}";
            m_udp.Send(message);
        }
        private void SendTargetPosition(Point position)
        {
            // Similarly, send target positions to the client
            string message = $"SpawnTarget:{position.X},{position.Y}";
            m_udp.Send(message);
        }


        private void MoveCharacter(string direction)
        {
            Rectangle bounds = m_character.Bounds;
            switch (direction)
            {
                case "Move:W":
                    m_character.SetBounds(bounds.X, bounds.Y - 5, bounds.Width, bounds.Height);
                    SendMoveMessage("PlayerId", new Point(bounds.X, bounds.Y));
                    break;
                case "Move:S":
                    m_character.SetBounds(bounds.X, bounds.Y + 5, bounds.Width, bounds.Height);
                    SendMoveMessage("PlayerId", new Point(bounds.X, bounds.Y));
                    break;
                case "Move:A":
                    m_character.SetBounds(bounds.X - 5, bounds.Y, bounds.Width, bounds.Height);
                    SendMoveMessage("PlayerId", new Point(bounds.X, bounds.Y));
                    break;
                case "Move:D":
                    m_character.SetBounds(bounds.X + 5, bounds.Y, bounds.Width, bounds.Height);
                    SendMoveMessage("PlayerId", new Point(bounds.X, bounds.Y));
                    break;
            }
            // Send the updated position to the client
            SendPlayerPosition(new Point(bounds.X, bounds.Y));
        }

        private void Shoot(string details)
        {
            // Expected details format: "X,Y" where X,Y are the target coordinates for the bullet
            var coords = details.Split(',');
            Point targetPosition = new Point(int.Parse(coords[0]), int.Parse(coords[1]));

            // Create a bullet and give it a trajectory
            // For simplicity, the bullet is represented as a PictureBox just like the targets
            PictureBox bullet = new PictureBox
            {
                Image = Image.FromFile("bullet.png"), // Make sure you have this image in your project
                Size = new Size(10, 10), // A bullet is typically smaller than targets
                Location = m_character.Location, // Start at the character's location
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            // Add the bullet to the form
            Invoke((MethodInvoker)delegate
            {
                panelMoveCharacter.Controls.Add(bullet);
                bullet.BringToFront();
            });

            // Simulate bullet movement towards the target
            Task.Run(() => UpdateBullets());
        }
        private void ShootBullet(string playerId, Point position, Point direction)
        {
            bullets.Add((playerId, position, direction));
            // Handle bullet movement and collision checks in your game loop
        }
        // Method to move the bullet
        //private void MoveBullet(PictureBox bullet, Point targetPosition)
        //{
        //    // Define the speed of the bullet
        //    int speed = 5;
        //    while (true)
        //    {
        //        // Calculate the new position of the bullet
        //        // Simple linear trajectory for the sake of example
        //        var direction = new Point(targetPosition.X - bullet.Location.X, targetPosition.Y - bullet.Location.Y);
        //        var distance = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);

        //        // Check if the bullet has reached the target position (or close enough)
        //        if (distance < speed)
        //        {
        //            Invoke((MethodInvoker)delegate
        //            {
        //                panelMoveCharacter.Controls.Remove(bullet);
        //                bullet.Dispose();
        //            });
        //            break; // Break the loop if the bullet reached the target position
        //        }

        //        // Normalize the direction vector and multiply by speed to get the movement vector
        //        var moveX = (int)(speed * direction.X / distance);
        //        var moveY = (int)(speed * direction.Y / distance);

        //        // Move the bullet
        //        Invoke((MethodInvoker)delegate
        //        {
        //            bullet.Location = new Point(bullet.Location.X + moveX, bullet.Location.Y + moveY);
        //        });

        //        // Check for collisions with targets
        //        PictureBox hitTarget = CheckForCollision(bullet);
        //        if (hitTarget != null)
        //        {
        //            // If a collision is detected, remove the target
        //            Invoke((MethodInvoker)delegate
        //            {
        //                panelMoveCharacter.Controls.Remove(hitTarget);
        //                targets.Remove(hitTarget);
        //                hitTarget.Dispose();
        //                panelMoveCharacter.Controls.Remove(bullet);
        //                bullet.Dispose();
        //            });
        //            // Send a message to the client that a target has been hit
        //            SendTargetHit(hitTarget.Location);
        //            break;
        //        }

        //        // Sleep for a bit to simulate bullet movement over time
        //        Thread.Sleep(50);
        //    }
        //}
        private void UpdateBullets()
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var bullet = bullets[i];
                // Calculate new position based on direction; this is simplified
                var newPosition = new Point(bullet.Position.X + bullet.Direction.X, bullet.Position.Y + bullet.Direction.Y);

                // Check for collision with any target
                var hitTargetId = CheckForCollision(newPosition);
                if (hitTargetId != null)
                {
                    // Handle collision, e.g., remove the bullet, update or remove target
                    bullets.RemoveAt(i);
                    target.Remove(hitTargetId.Value); // Assuming targets dictionary uses the target's ID as the key
                                                      // Additionally, notify clients or handle the target hit logic here
                }
                else
                {
                    // Update bullet position if no collision
                    bullets[i] = (bullet.PlayerId, newPosition, bullet.Direction);
                }
            }
        }
        // Check if the bullet PictureBox intersects any of the target PictureBoxes
        private int? CheckForCollision(Point bulletPosition)
        {
            foreach (var target in target)
            {
                // Assuming a simple collision check based on proximity
                if (Math.Abs(target.Value.X - bulletPosition.X) < 10 && Math.Abs(target.Value.Y - bulletPosition.Y) < 10)
                {
                    return target.Key; // Return the ID of the hit target
                }
            }
            return null; // No collision
        }
        private void SendMoveMessage(string id, Point newPosition)
        {
            string message = $"Move:{id}:{newPosition.X},{newPosition.Y}";
            m_udp.Send(message); // Assuming m_udp.Send is your method to send messages over UDP
        }
        private void SendTargetHit(Point position)
        {
            string message = $"TargetHit:{position.X},{position.Y}";
            m_udp.Send(message);
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {

        }
    }

}




