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

class Game
{
    public void Start()
    {
        Console.WriteLine("Введите имя игрока:");
        string name = Console.ReadLine();
        Player player = new Player(name);
        Console.WriteLine($"Привет, {player.Name}! Игра началась.");
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

    public Player(string name)
    {
        Name = name;
        MaxHP = 100;
        HP = 100;
        Attack = 10;
        Defense = 5;
    }

    public void AttackEnemy(Enemy e)
    {
        int dmg = Attack + (Weapon?.AttackBonus ?? 0);
        e.HP -= dmg;
        Console.WriteLine($"{Name} атакует {e.Name} на {dmg} урона!");
    }

    public void Defend()
    {
        Console.WriteLine($"{Name} защищается! Есть шанс уклониться.");
    }

    public void Heal()
    {
        HP = MaxHP;
        Console.WriteLine($"{Name} полностью восстановил здоровье!");
    }
}

class Enemy
{
    public string Name;
    public int HP;
    public int Attack;
    public int Defense;

    public virtual void AttackPlayer(Player p)
    {
        int dmg = Math.Max(Attack - p.Defense, 1);
        p.HP -= dmg;
        Console.WriteLine($"{Name} атакует {p.Name} на {dmg} урона!");
    }
}

class Goblin : Enemy
{
    public Goblin()
    {
        Name = "Гоблин";
        HP = 30;
        Attack = 8;
        Defense = 2;
    }
}

class Skeleton : Enemy
{
    public Skeleton()
    {
        Name = "Скелет";
        HP = 40;
        Attack = 7;
        Defense = 3;
    }
}

class Mage : Enemy
{
    public Mage()
    {
        Name = "Маг";
        HP = 25;
        Attack = 10;
        Defense = 1;
    }
}

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
    public string Name = "Лечебное зелье";
}

class Chest
{
    public void Open(Player player)
    {
        Console.WriteLine("Вы нашли сундук!");
    }
}
