namespace MyNote.Domain.Notes
{
    public class Note
    {
        public int Id { get; set; }
        public required string Path { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public DateTime ModifiedAt { get; set; } = DateTime.Now; // изначально проставим дату при создании
    }
}