// IAttackStrategy.cs
public interface IAttackStrategy
{
    /// <summary>
    /// ��������� �����: ���������� ��������� ����� ��� ��������� ����� �����.
    /// </summary>
    void ExecuteAttack(Tower tower, Enemy target);
}
