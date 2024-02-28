using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SignalRbackend;

public class User
{
    int Id { get; set; }
    public string UserName { get; set; }
    public int Age { get; set; }
    public string PassWord { get; set; }

    public User(int id, string username, int age, string password)
    {
        this.Id = id;
        this.UserName = username;
        this.Age = age;
        this.PassWord = password;
    }

    public bool Correct(string name, string password)
    {
        return name == this.UserName && password == this.PassWord;
    }
    public override string ToString()
        => $"{Id} : {UserName} : {Age} : {PassWord}";
}