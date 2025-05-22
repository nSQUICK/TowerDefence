// IAttackStrategy.cs
public interface IAttackStrategy
{
    /// <summary>
    /// Выполняет атаку: использует параметры башни для нанесения урона врагу.
    /// </summary>
    void ExecuteAttack(Tower tower, Enemy target);
}
