using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Exam.Core;
using Exam.Data;

namespace Exam.App;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private readonly MaterialRepository repo = new();
    private List<Material> all = new();
    private List<LookupItem> typeItems = new();
    private int page = 1;
    private const int PageSize = 15;

    public ObservableCollection<Material> PageItems { get; } = new();
    public List<string> Types { get; private set; } = new() { "Все типы" };
    public string[] Sorts { get; } = { "Сортировка: наименование", "Сортировка: остаток", "Сортировка: стоимость" };

    private string search = "";
    public string Search { get => search; set { search = value; page = 1; RefreshView(); } }

    private string selectedType = "Все типы";
    public string SelectedType { get => selectedType; set { selectedType = value; page = 1; RefreshView(); } }

    private string selectedSort = "Сортировка: наименование";
    public string SelectedSort { get => selectedSort; set { selectedSort = value; RefreshView(); } }

    public Material? Selected { get; set; }
    public double MassValue { get; set; }
    public string Counter { get; set; } = "";

    public RelayCommand AddCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand MassCommand { get; }
    public RelayCommand RefreshCommand { get; }
    public RelayCommand NextCommand { get; }
    public RelayCommand PrevCommand { get; }

    public MainWindow()
    {
        InitializeComponent();
        AddCommand = new(_ => AddMaterial());
        EditCommand = new(_ => EditSelected());
        DeleteCommand = new(_ => DeleteSelected());
        MassCommand = new(_ => UpdateSelectedMinCount());
        RefreshCommand = new(_ => LoadFromDatabase(true));
        NextCommand = new(_ => { page++; RefreshView(); });
        PrevCommand = new(_ => { if (page > 1) page--; RefreshView(); });
        DataContext = this;
        LoadFromDatabase(false);
    }

    private void LoadFromDatabase(bool showSuccess)
    {
        try
        {
            typeItems = repo.GetTypes();
            Types = typeItems.Select(x => x.Title).Prepend("Все типы").ToList();
            all = repo.Load();
            OnPropertyChanged(nameof(Types));
            RefreshView();
            if (showSuccess) MessageBox.Show(this, "Данные загружены из SQL Server.", "Обновление");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Не удалось подключиться к SQL Server. Выполните database/sqlserver/setup-db.ps1 и проверьте переменную SQLSERVER." + Environment.NewLine + ex.Message, "База данных");
        }
    }

    private void AddMaterial()
    {
        var firstType = typeItems.FirstOrDefault();
        var material = new Material
        {
            MaterialTypeId = firstType?.Id ?? 0,
            Type = firstType?.Title ?? "",
            Title = "Новый материал",
            Unit = "шт",
            CountInPack = 1,
            MinCount = 1,
            Cost = 1,
            Image = "picture.png"
        };
        ShowEditor(material);
    }

    private void EditSelected()
    {
        if (Selected == null) return;
        ShowEditor(Selected);
    }

    private void ShowEditor(Material material)
    {
        var resourceRoot = System.IO.Path.Combine(AppContext.BaseDirectory, "resources");
        var window = new MaterialEditWindow(material, typeItems, resourceRoot) { Owner = this };
        if (window.ShowDialog() != true) return;
        repo.Save(window.Material);
        LoadFromDatabase(false);
    }

    private void DeleteSelected()
    {
        var selected = GetSelected().ToList();
        if (selected.Count == 0 && Selected != null) selected.Add(Selected);
        if (selected.Count == 0) return;
        var result = repo.Delete(selected);
        LoadFromDatabase(false);
        MessageBox.Show(this, result.Summary, "Удаление");
    }

    private void UpdateSelectedMinCount()
    {
        var selected = GetSelected().Select(x => x.Id).ToList();
        if (selected.Count == 0 && Selected != null) selected.Add(Selected.Id);
        if (selected.Count == 0)
        {
            MessageBox.Show(this, "Выберите одну или несколько записей.", "Массовое изменение");
            return;
        }
        repo.BulkUpdateMinCount(selected, MassValue);
        LoadFromDatabase(false);
    }

    private IEnumerable<Material> GetSelected() => MaterialsList.SelectedItems.Cast<Material>();

    private void RefreshView()
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

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
