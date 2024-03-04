using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UDPClientServer;

namespace Assignment1
{
    public partial class ClientForm : Form1
    {
        private readonly UDPController m_udp = new UDPController();
        private PictureBox m_character = new PictureBox();
        private Dictionary<string, PictureBox> gameObjects = new Dictionary<string, PictureBox>();
        private PictureBox m_target = new PictureBox();
        private int nextTargetId = 0;
        private Random random = new Random();
        public ClientForm()
        {

            InitializeComponent();


        }
        private void InitializeMessageProcessingTimer()
        {
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 100; // Check for messages every 100 milliseconds

            timer.Tick += (sender, e) => UpdateList(); // Use UpdateList directly
            timer.Start();
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
                    if (!listBoxClient.Items.Contains(message))
                    {
                        listBoxClient.Items.Add(message);
                        // You may want to automatically scroll to the latest message
                        listBoxClient.SelectedIndex = listBoxClient.Items.Count - 1;
                        listBoxClient.ClearSelected();
                    }
                });
            }
        }
        private void ProcessMessage(string message)
        {
            var parts = message.Split(':');
            if (parts.Length < 2) return; // Ensure message format is correct


            string data = parts.Length > 1 ? parts[1] : string.Empty; // Second part is data
            if (parts[0] == "Move")
            {
                string id = parts[1];
                Point newPosition = ParsePosition(parts[2]);

                this.Invoke((MethodInvoker)delegate
                {
                    if (gameObjects.ContainsKey(id))
                    {
                        // Update existing game object position
                        gameObjects[id].Location = newPosition;
                    }
                    else
                    {
                        // Add new game object if it doesn't exist
                        var newGameObject = CreateGameObject(id, newPosition); // Implement this method
                        gameObjects[id] = newGameObject;
                        this.Controls.Add(newGameObject);
                        newGameObject.BringToFront();
                    }
                });
            }

                switch (data)
                {
                    case "Player":
                        // Assuming the first part of the data is "Player" and followed by coordinates
                        var playerPos = ParsePosition(parts.Length > 2 ? parts[2] : string.Empty);
                        RenderGameObject("Player", playerPos);

                        break;
                    case "RequestTargets":
                        // Assuming the first part of the data is "Target" and followed by coordinates
                        var targetPos = ParsePosition(parts.Length > 2 ? parts[2] : string.Empty);
                        RenderGameObject("Target", targetPos);
                    CreateTarget();
                        break;
                    case "ACK":
                        // Handle acknowledgment message
                        this.Invoke((MethodInvoker)delegate
                        {
                            listBoxClient.Items.Add("Server Acknowledged Connection");
                            // Additional logic to enable game controls, etc.
                        });
                     AddCharacter();
                    CreateTarget();
                        break;
                    
            };
            
            // ... other message types



        }

        private PictureBox CreateGameObject(string id, Point position)
        {
            PictureBox gameObject = new PictureBox
            {
                Image = Image.FromFile("Zombie.png"), // Example condition
                Size = new Size(50, 50), // Adjust as needed
                Location = position,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Name = id // Use the ID as the control's name for easy identification
            };

            return gameObject;
        }
        private void UpdateGameObjectPosition(string id, Point newPosition)
        {
            if (gameObjects.ContainsKey(id)) // Assuming you have a dictionary of game objects
            {
                this.Invoke((MethodInvoker)delegate
                {
                    gameObjects[id].Location = newPosition; // Update position on UI thread
                });
            }
        }
        private void SpawnTarget(string positionData)
        {
            var position = ParsePosition(positionData);
            // Assuming you have a method to create and display targets
            CreateTarget();
        }

        private void UpdatePlayerPosition(string positionData)
        {
            var position = ParsePosition(positionData);
            this.Invoke((MethodInvoker)delegate
            {
                m_character.Location = position;
            });
        }
        private PictureBox FindTargetById(string targetId)
        {
            // Attempt to find a PictureBox with the matching Name property
            return this.Controls.OfType<PictureBox>().FirstOrDefault(pb => pb.Name == targetId);
        }
        private void RemoveTarget(string targetId)
        {
            // Assuming targets have unique identifiers and are stored in a collection
            this.Invoke((MethodInvoker)delegate
            {
                var targetToRemove = FindTargetById(targetId);
                if (targetToRemove != null)
                {
                    Controls.Remove(targetToRemove);
                    targetToRemove.Dispose();
                }
            });
        }
        private Point ParsePosition(string posData)
        {
            var parts = posData.Split(':');
            if (parts.Length < 2) return new Point(0, 0); // Fallback in case of incorrect format

            int x, y;
            if (int.TryParse(parts[0], out x) && int.TryParse(parts[1], out y))
            {
                return new Point(x, y);
            }
            return new Point(0, 0); // Fallback for parsing failure
        }
        private Point GetRandomPosition()
        {
            // Return a random position within the bounds of your panel or form
            int x = random.Next(panelMoveCharacter.Bounds.Width);
            int y = random.Next(panelMoveCharacter.Bounds.Height);
            return new Point(x, y);
        }
        private void CreateTarget()
        {
            // Create a new PictureBox to represent the target

           Point position = new Point(100, 100);

            for (int i = 0; i < 3; i++)
            {
                position = GetRandomPosition();
                m_target.Image = Image.FromFile("bomb.png"); // Assuming you have a 'bomb' image in your resources
             
                m_target.SetBounds(position.X, position.Y, 100, 100);
                
            }
            panelMoveCharacter.Controls.Add(m_target);


        }

        private void AddCharacter()
        {
            Point position = new Point(0, 0);
            position = GetRandomPosition();
            m_character.Image = Image.FromFile("Zombie.png");
            m_character.SetBounds(position.X, position.Y, 100, 100);
            panelMoveCharacter.Controls.Add(m_character);
            
        }

      
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Handle movement keys
            if (keyData == Keys.W || keyData == Keys.A || keyData == Keys.S || keyData == Keys.D)
            {
                string direction = keyData.ToString();
                m_udp.Send($"Move:{direction}");
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void ClientForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the trajectory for the bullet
                Point origin = m_character.Location;
                Point destination = this.PointToClient(Cursor.Position);
                string message = $"Shoot:{origin.X},{origin.Y},{destination.X},{destination.Y}";
                m_udp.Send(message);
            }
        }
        private void RenderGameObject(string type, Point position)
        {
            PictureBox gameObject = new PictureBox();
            switch (type)
            {
                case "Player":
                    gameObject.Image = Image.FromFile("Zombie.png"); // Assign your player image
                    gameObject.Size = new Size(50, 50); // Set the size of the player
                    break;
                case "Target":
                    gameObject.Image = Image.FromFile("bomb.png"); // Assign your target image
                    gameObject.Size = new Size(30, 30); // Set the size of the target
                    break;
                    // Handle other game object types similarly
            }
            gameObject.Location = position;
            gameObject.SizeMode = PictureBoxSizeMode.StretchImage; // To ensure the image fits the PictureBox
            this.Controls.Add(gameObject); // Add the PictureBox to the form's controls
            gameObject.BringToFront();
        }
    

        public override void ProcessSendQueue()
        {
            UDP.ProcessSendQueue();
        }

        private void BtnConnect_Click_1(object sender, EventArgs e)
        {
            m_udp.Client("127.0.0.1", 27015); // Setup the client
            m_udp.Send("Connected");
            m_udp.Send("RequestTargets");// Send a message indicating the client has connected
            InitializeMessageProcessingTimer();

        }

        private void BtnExit_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
