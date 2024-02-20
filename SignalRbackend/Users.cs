namespace SignalRbackend
{
    public class User
    {
        public string name { get; private set; }
        string password;

        public User(string name, string password) 
        {
            this.name = name; this.password = password;
        }

        public bool Correct(string name, string password)
        {
            return name == this.name && password == this.password;
        }
    }
}
