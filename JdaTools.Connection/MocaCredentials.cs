namespace JdaTools.Connection
{
    public struct MocaCredentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DevCod { get; set; }

        public MocaCredentials(string userName, string password, string devCod = "dotnetlibrary")
        {
            UserName = userName;
            Password = password;
            DevCod = devCod;
        }
    }
}