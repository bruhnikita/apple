namespace Exam.Core;

public sealed class LookupItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public override string ToString() => Title;
}
