namespace ColoryrServer.NoASP
{
    class NoASP
    {
        public static NoASPConfig Config { get; set; }
        public static void Main()
        {
            ServerMain.ConfigUtil = new NoASPConfigUtils();
            ServerMain.Start();

            HttpServer.Start();

            if (Config.NoInput)
                return;
            while (true)
            {
                var data = Console.ReadLine();
                if (data == "stop")
                {
                    HttpServer.Stop();
                    ServerMain.Stop();
                    return;
                }
            }
        }
    }
}
