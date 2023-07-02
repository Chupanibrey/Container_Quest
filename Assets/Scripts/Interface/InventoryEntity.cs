using System;

public interface InventoryEntity
{
    bool isUsed { get; set; }
    int maxEntityLimitInInventory { get; }
    int amount { get; set; }
    Type type { get; } // �������������� �����, ����� �� �������� ����� GetType()

    InventoryEntity Clone(); // ������� � ���������� ����� ���� ��������.
}