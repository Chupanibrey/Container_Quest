using System.Collections.Generic;

class UIInventoryRandomizer
{
    InventoryEntityInfo circleInfo;
    InventoryEntityInfo squareInfo;
    UIInventoryCell[] uiCells; // Массив пользовательских интерфейсов ячеек инвентаря

    public InventoryWithCells inventory { get; } // Инвентарь с ячейками

    public UIInventoryRandomizer(InventoryEntityInfo circleInfo, 
        InventoryEntityInfo squareInfo, UIInventoryCell[] uiCells)
    {
        this.circleInfo = circleInfo;
        this.squareInfo = squareInfo;
        this.uiCells = uiCells;

        inventory = new InventoryWithCells(28); // Создаем инвентарь с заданным числом ячеек (в данном случае 28)
        inventory.OnInventoryStatusChangedEvent += OnInventoryStatusChanged; // Подписываемся на событие изменения статуса инвентаря
    }

    public void FillCells()
    {
        var allCells = inventory.GetAllCells();
        var availableCells = new List<IInventoryCell>(allCells); // Создаем список доступных для заполнения ячеек

        var filledCells = 4; // Количество ячеек, которые нужно заполнить

        for (int i = 0; i< filledCells; i++)
        {
            var filledCell = AddRandomSquareIntoRandomCell(availableCells);
            availableCells.Remove(filledCell);

            filledCell = AddRandomCircleIntoRandomCell(availableCells);
            availableCells.Remove(filledCell);
        }

        SetupInventoryUI(inventory);
    }

    IInventoryCell AddRandomSquareIntoRandomCell(List<IInventoryCell> cells) // Рандомное заполнение ячеек квадратами
    {
        var rCellIndex = UnityEngine.Random.Range(20, cells.Count);
        var rCell = cells[rCellIndex];
        var rCount = UnityEngine.Random.Range(1, 4);
        var square = new Square(squareInfo);
        square.Status.Amount = rCount;
        square.Status.CellNumber = rCellIndex;
        inventory.TryAddToCell(this, rCell, square);
        return rCell;
    }

    IInventoryCell AddRandomCircleIntoRandomCell(List<IInventoryCell> cells) // Рандомное заполнение ячеек кругами
    {
        var rCellIndex = UnityEngine.Random.Range(20, cells.Count);
        var rCell = cells[rCellIndex];
        var rCount = UnityEngine.Random.Range(1, 4);
        var circle = new Circle(circleInfo);
        circle.Status.Amount = rCount;
        circle.Status.CellNumber = rCellIndex;
        inventory.TryAddToCell(this, rCell, circle);
        return rCell;
    }

    void SetupInventoryUI(InventoryWithCells inventory)
    {
        var allCells = inventory.GetAllCells();
        var allCellsCount = allCells.Length;

        for(int i = 0; i < allCellsCount; i++)
        {
            var cell = allCells[i];
            cell.CellNumber = i;
            var uiCell = uiCells[i];
            uiCell.SetCell(cell); // Устанавливаем ячейку для отображения в пользовательском интерфейсе
            uiCell.Refresh(); // Обновляем пользовательский интерфейс ячейки
        }         
    }

    void OnInventoryStatusChanged(object sender)
    {
        foreach(var uiCell in uiCells) // При изменении статуса инвентаря обновляем все пользовательские интерфейсы ячеек
            uiCell.Refresh();
    }
}