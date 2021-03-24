namespace WebApp.Models
{
    public class AgeGroup
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public double Coefficient { get; set; }

        public AgeGroup() { }

        public AgeGroup(int id)
        {
            this.Id = id;
        }
    }
}