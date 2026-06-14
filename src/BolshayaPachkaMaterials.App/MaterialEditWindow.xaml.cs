using System.IO;
using System.Text.Json;
using System.Windows;
using Exam.Core;
using Microsoft.Win32;

namespace Exam.App;

public partial class MaterialEditWindow : Window
{
    private readonly string resourceRoot;
    public Material Material { get; }
    public List<LookupItem> Types { get; }

    public MaterialEditWindow(Material material, List<LookupItem> types, string resourceRoot)
    {
        InitializeComponent();
        Material = JsonSerializer.Deserialize<Material>(JsonSerializer.Serialize(material)) ?? new Material();
        Types = types;
        this.resourceRoot = resourceRoot;
        DataContext = new MaterialEditViewModel(Material, Types);
    }

    private void PickImage_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp|All files|*.*" };
        if (dialog.ShowDialog(this) != true) return;
        var imagesDir = Path.Combine(resourceRoot, "images");
        Directory.CreateDirectory(imagesDir);
        var fileName = Path.GetFileName(dialog.FileName);
        var target = Path.Combine(imagesDir, fileName);
        File.Copy(dialog.FileName, target, true);
        Material.Image = Path.Combine("images", fileName).Replace('\\', '/');
        DataContext = new MaterialEditViewModel(Material, Types);
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Material.Title))
        {
            MessageBox.Show(this, "Введите наименование материала.", "Проверка");
            return;
        }
        if (Material.MaterialTypeId == 0)
        {
            MessageBox.Show(this, "Выберите тип материала.", "Проверка");
            return;
        }
        if (Material.CountInPack <= 0) Material.CountInPack = 1;
        if (string.IsNullOrWhiteSpace(Material.Unit)) Material.Unit = "шт";
        DialogResult = true;
    }
}

public sealed class MaterialEditViewModel
{
    public MaterialEditViewModel(Material material, List<LookupItem> types)
    {
        Material = material;
        Types = types;
    }

    public Material Material { get; }
    public List<LookupItem> Types { get; }
    public string Title { get => Material.Title; set => Material.Title = value; }
    public double CountInStock { get => Material.CountInStock; set => Material.CountInStock = value; }
    public double MinCount { get => Material.MinCount; set => Material.MinCount = value; }
    public decimal Cost { get => Material.Cost; set => Material.Cost = value; }
    public int CountInPack { get => Material.CountInPack; set => Material.CountInPack = value; }
    public string Unit { get => Material.Unit; set => Material.Unit = value; }
    public string Suppliers { get => Material.Suppliers; set => Material.Suppliers = value; }
    public string Description { get => Material.Description; set => Material.Description = value; }
    public string Image { get => Material.Image; set => Material.Image = value; }
}
