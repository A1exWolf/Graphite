namespace MyNote.Domain.Notes
{
    public class Note
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime ModifiedAt { get; set; } = DateTime.Now; // изначально проставим дату при создании
    }
}