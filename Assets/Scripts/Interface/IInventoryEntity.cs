using System;

public interface IInventoryEntity
{
    bool isUsed { get; set; }
    int maxEntityLimitInInventory { get; }
    int amount { get; set; }
    Type type { get; } // �������������� �����, ����� �� �������� ����� GetType()

    IInventoryEntity Clone(); // ������� � ���������� ����� ���� ��������.
}