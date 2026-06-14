namespace Exam.Core;

public sealed class DeleteResult
{
    public int Deleted { get; set; }
    public int Blocked { get; set; }
    public List<string> Messages { get; } = new();
    public string Summary => Messages.Count == 0
        ? $"Удалено: {Deleted}. Заблокировано: {Blocked}."
        : string.Join(Environment.NewLine, Messages);
}
