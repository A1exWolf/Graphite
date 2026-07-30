namespace MyNote.Domain.Vaults;

public class NoteNode
{
    public required string Name { get; set; }
    public required string Path { get; set; }
    public required TypeNode TypeNode { get; set; }
    public List<NoteNode> Children { get; } = [];
};