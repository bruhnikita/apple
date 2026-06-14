namespace Exam.Core;

public sealed class Material
{
    public int Id { get; set; }
    public int MaterialTypeId { get; set; }
    public string Title { get; set; } = "";
    public string Type { get; set; } = "";
    public string Description { get; set; } = "";
    public string Image { get; set; } = "picture.png";
    public decimal Cost { get; set; }
    public double CountInStock { get; set; }
    public double MinCount { get; set; }
    public int CountInPack { get; set; }
    public string Unit { get; set; } = "";
    public string Suppliers { get; set; } = "";
    public bool HasProductMaterial { get; set; }
    public string Highlight => CountInStock < MinCount ? "#f19292" : CountInStock >= MinCount * 3 ? "#ffba01" : "White";
    public decimal RequiredCost => ListTools.RequiredPurchaseCost(CountInStock, MinCount, CountInPack, Cost);
}
