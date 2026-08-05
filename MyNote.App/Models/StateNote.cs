namespace MyNote.App.Models;

/// <summary>
/// Состояний заметки
/// </summary>
public enum StateNote
{
    Idle,
    Modified,
    Saving,
    Saved,
    Error
}