using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.WriteLine("Добро пожаловать в текстовую игру-рогалик!");
        Console.WriteLine("Нажмите любую клавишу, чтобы начать...");
        Console.ReadKey();

        Game game = new Game();
        game.Start();
    }
}

// ===== Декомпозиция =====

class Game
{
    public void Start()
    {
        Console.WriteLine("Игра началась (заглушка)");
    }
}

class Player
{
    public string Name;
    public int HP;
    public int MaxHP;
    public int Attack;
    public int Defense;
    public Weapon Weapon;
    public Armor Armor;

    public void AttackEnemy(Enemy e) { }
    public void Defend() { }
    public void Heal() { }
}

class Enemy
{
    public string Name;
    public int HP;
    public int Attack;
    public int Defense;

    public virtual void AttackPlayer(Player p) { }
}

class Goblin : Enemy { }
class Skeleton : Enemy { }
class Mage : Enemy { }

class Weapon
{
    public string Name;
    public int AttackBonus;
}

class Armor
{
    public string Name;
    public int DefenseBonus;
}

class Potion
{
    public string Name;
}

class Chest
{
    public void Open(Player player) { }
}
