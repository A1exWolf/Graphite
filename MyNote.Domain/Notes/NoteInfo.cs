namespace MyNote.Domain.Notes;

/// <summary>
/// Для получения данных для построения дерева
/// </summary>
/// <param name="Path">Полный путь</param>
/// <param name="ReleativePath">Относительный путь внутри vault</param>
/// <param name="Title">Имя</param>
/// <param name="ModifiedAt">Время изменения</param>
public sealed record NoteInfo(string Path, string RelativePath, string Title, DateTime ModifiedAt);