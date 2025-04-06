namespace WebApplication2.Models
{
    public class Logs : BaseEntity
    {
       public string log {  get; set; }
       public string name { get; set; }
       public int UserId { get; set; }
    }
}
