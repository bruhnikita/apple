using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using Exam.Core;
using Exam.Data;

namespace Exam.App;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private readonly MaterialRepository repo = new();
    private readonly List<Material> original;
    private List<Material> all;
    private int page = 1;
    private const int PageSize = 15;

    public ObservableCollection<Material> PageItems { get; } = new();
    public List<string> Types { get; }
    public string[] Sorts { get; } = { "Сортировка: наименование", "Сортировка: остаток", "Сортировка: стоимость" };

    private string search = "";
    public string Search { get => search; set { search = value; page = 1; Refresh(); } }

    private string selectedType = "Все типы";
    public string SelectedType { get => selectedType; set { selectedType = value; page = 1; Refresh(); } }

    private string selectedSort = "Сортировка: наименование";
    public string SelectedSort { get => selectedSort; set { selectedSort = value; Refresh(); } }

    public Material? Selected { get; set; }
    public double MassValue { get; set; }
    public string Counter { get; set; } = "";

    public RelayCommand AddCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand MassCommand { get; }
    public RelayCommand ResetCommand { get; }
    public RelayCommand NextCommand { get; }
    public RelayCommand PrevCommand { get; }

    public MainWindow()
    {
        InitializeComponent();
        original = Clone(repo.Load());
        all = Clone(original);
        Types = all.Select(x => x.Type).Distinct().OrderBy(x => x).Prepend("Все типы").ToList();

        AddCommand = new(_ => AddMaterial());
        EditCommand = new(_ => EditSelected());
        DeleteCommand = new(_ => DeleteSelected());
        MassCommand = new(_ => UpdateSelectedMinCount());
        ResetCommand = new(_ => ResetData());
        NextCommand = new(_ => { page++; Refresh(); });
        PrevCommand = new(_ => { if (page > 1) page--; Refresh(); });

        DataContext = this;
        Refresh();
    }

    private void AddMaterial()
    {
        all.Insert(0, new Material
        {
            Id = all.Count == 0 ? 1 : all.Max(x => x.Id) + 1,
            Title = "Новый материал",
            Type = Types.Skip(1).FirstOrDefault() ?? "Материал",
            Description = "Новая запись материала.",
            Cost = 1,
            MinCount = 1,
            CountInPack = 1,
            Unit = "шт",
            Suppliers = "Основной поставщик"
        });
        Save();
    }

    private void EditSelected()
    {
        if (Selected == null) return;
        Selected.Description = "Запись отредактирована оператором.";
        Save();
    }

    private void DeleteSelected()
    {
        var selected = GetSelected().ToList();
        if (selected.Count == 0 && Selected != null) selected.Add(Selected);
        foreach (var item in selected)
        {
            if (!repo.CanDelete(item))
            {
                MessageBox.Show("Удаление запрещено: материал используется в продукции.", "Удаление");
                continue;
            }
            all.Remove(item);
        }
        Save();
    }

    private void UpdateSelectedMinCount()
    {
        var selected = GetSelected().ToList();
        if (selected.Count == 0 && Selected != null) selected.Add(Selected);
        if (selected.Count == 0)
        {
            MessageBox.Show("Выберите одну или несколько записей.", "Массовое изменение");
            return;
        }
        foreach (var item in selected) item.MinCount = MassValue;
        Save();
    }

    private void ResetData()
    {
        all = Clone(original);
        page = 1;
        Save();
    }

    private IEnumerable<Material> GetSelected() => MaterialsList.SelectedItems.Cast<Material>();

    private void Save()
    {
        repo.Save(all);
        Refresh();
    }

    private void Refresh()
    {
        IEnumerable<Material> query = all;
        if (!string.IsNullOrWhiteSpace(Search))
            query = query.Where(x => (x.Title + " " + x.Description).Contains(Search, StringComparison.OrdinalIgnoreCase));
        if (SelectedType != "Все типы")
            query = query.Where(x => x.Type == SelectedType);
        query = SelectedSort switch
        {
            "Сортировка: остаток" => query.OrderBy(x => x.CountInStock),
            "Сортировка: стоимость" => query.OrderBy(x => x.Cost),
            _ => query.OrderBy(x => x.Title)
        };

        var list = query.ToList();
        var maxPage = Math.Max(1, (int)Math.Ceiling(list.Count / (double)PageSize));
        if (page > maxPage) page = maxPage;

        PageItems.Clear();
        foreach (var item in ListTools.Page(list, page, PageSize)) PageItems.Add(item);
        Counter = $"Показано {PageItems.Count} из {list.Count}    {page}/{maxPage}";
        OnPropertyChanged(nameof(Counter));
    }

    private static List<Material> Clone(List<Material> items) =>
        JsonSerializer.Deserialize<List<Material>>(JsonSerializer.Serialize(items)) ?? new();

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
