using System;

public interface IInventoryEntity
{
    IInventoryEntityInfo Info { get; }
    IInventoryEntityStatus Status { get; }
    Type Type { get; } // �������������� �����, ����� �� �������� ����� GetType()

    IInventoryEntity Clone(); // ������� � ���������� ����� ���� ��������.
}