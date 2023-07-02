using System;
using System.Collections.Generic;
using UnityEngine;

class UIInventoryRandomizer
{
    InventoryEntityInfo circleInfo;
    InventoryEntityInfo squareInfo;
    UIInventoryCell[] uiCells;

    public InventoryWithCells inventory { get; }

    public UIInventoryRandomizer(InventoryEntityInfo circleInfo, 
        InventoryEntityInfo squareInfo, UIInventoryCell[] uiCells)
    {
        this.circleInfo = circleInfo;
        this.squareInfo = squareInfo;
        this.uiCells = uiCells;

        inventory = new InventoryWithCells(15);
        inventory.OnInventoryStatusChangedEvent += OnInventoryStatusChanged;
    }

    public void FillCells()
    {
        var allCells = inventory.GetAllCells();
        var availableCells = new List<IInventoryCell>(allCells);

        var filledCells = 5;

        for(int i = 0; i< filledCells; i++)
        {
            var filledCell = AddRandomCircleIntoRandomCell(availableCells);
            availableCells.Remove(filledCell);

            filledCell = AddRandomSquareIntoRandomCell(availableCells);
            availableCells.Remove(filledCell);
        }

        SetupInventoryUI(inventory);
    }

    IInventoryCell AddRandomSquareIntoRandomCell(List<IInventoryCell> cells)
    {
        var rCellIndex = UnityEngine.Random.Range(0, cells.Count);
        var rCell = cells[rCellIndex];
        var rCount = UnityEngine.Random.Range(1, 4);
        var square = new Square(squareInfo);
        square.Status.Amount = rCount;
        inventory.TryAddToCell(this, rCell, square);
        return rCell;
    }

    IInventoryCell AddRandomCircleIntoRandomCell(List<IInventoryCell> cells)
    {
        var rCellIndex = UnityEngine.Random.Range(0, cells.Count);
        var rCell = cells[rCellIndex];
        var rCount = UnityEngine.Random.Range(1, 4);
        var circle = new Circle(circleInfo);
        circle.Status.Amount = rCount;
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
            var uiCell = uiCells[i];
            uiCell.SetCell(cell);
            uiCell.Refresh();
        }
    }

    void OnInventoryStatusChanged(object sender)
    {
        foreach(var uiCell in uiCells)
            uiCell.Refresh();
    }
}