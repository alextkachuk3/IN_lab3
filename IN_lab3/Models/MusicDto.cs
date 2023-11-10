namespace IN_lab3.Models
{
    public class MusicDto
    {

        public MusicDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
