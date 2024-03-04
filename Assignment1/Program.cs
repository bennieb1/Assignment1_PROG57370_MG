namespace Assignment1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Application.Run(new ServerForm());
           //Form1 f = null;
            ServerForm sf = null;
            ClientForm cf = null;
            if (args[0] == "client")
            {
                cf = new ClientForm();
                cf.Show();
            }
            else if (args[0] == "server")
            {
                sf = new ServerForm();
                sf.Show();
            }
            //f.Show();
            //Application.Run(f);

            while (Application.OpenForms.Count > 0)
            {
                Application.DoEvents();
               if (cf != null) cf.UpdateList();
                if (sf != null) sf.UpdateList();
                //if (!f.UpdateList()) return;
                 //cf.ProcessSendQueue();
                //    //thread.sleep(10);
            }
        }
    }
}