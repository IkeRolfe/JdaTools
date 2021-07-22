namespace JdaTools.Connection
{
    public struct MocaCredentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public MocaCredentials(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}