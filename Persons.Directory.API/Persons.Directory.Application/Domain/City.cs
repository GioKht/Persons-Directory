namespace Persons.Directory.Application.Domain
{
    public class City : Entity
    {
        public string NameKa { get; set; }

        public string NameEn { get; set; }

        public string Location { get; set; }

        public void SetCreateDate()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
