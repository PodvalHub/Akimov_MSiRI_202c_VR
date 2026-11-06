
public class Character
{
    // Поля - состояние персонажа
    public string Name;
    public int Health;
    public int MaxHealth;

    // Конструктор
    public Character(string name, int health)
    {
        Name = name;
        Health = health;
        MaxHealth = health;
    }

    // Метод получения информации
    public string GetInfo()
    {
        return Name + " | Здоровье: " + Health;
    }

    // Метод получения урона
    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health < 0)
        {
            Health = 0;
        }
    }

    // Метод восстановления здоровья
    public void Heal(int amount)
    {
        Health += amount;

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    // Проверка смерти
    public bool IsDead()
    {
        return Health <= 0;
    }
}
