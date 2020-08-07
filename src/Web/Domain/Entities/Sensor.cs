namespace Web.Domain.Entities
{
    public abstract class Sensor
    {
        public int Id { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public string ApiKey { get; set; }
    }
}
