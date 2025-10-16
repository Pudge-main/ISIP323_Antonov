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
    Random rand = new Random();

    public void Start()
    {
        Console.WriteLine("Введите имя игрока:");
        string name = Console.ReadLine();
        Player player = new Player(name);
        Console.WriteLine($"Привет, {player.Name}! Игра началась.\n");

        int turn = 1;
        while (player.HP > 0)
        {
            Console.WriteLine($"--- Ход {turn} ---");
            if (rand.Next(2) == 0)
            {
                Enemy e = GetRandomEnemy();
                Battle(player, e);
            }
            else
            {
                Chest c = new Chest();
                c.Open(player);
            }
            turn++;
        }

        Console.WriteLine("Игра окончена! Вы проиграли!");
    }

    private Enemy GetRandomEnemy()
    {
        int r = rand.Next(3);
        if (r == 0) return new Goblin();
        if (r == 1) return new Skeleton();
        return new Mage();
    }

    private void Battle(Player p, Enemy e)
    {
        Console.WriteLine($"Вы встретили врага: {e.Name} (HP {e.HP})");

        while (p.HP > 0 && e.HP > 0)
        {
            Console.WriteLine("\n1 - Атаковать | 2 - Защищаться");
            string choice = Console.ReadLine();

            if (choice == "1")
                p.AttackEnemy(e);
            else
                p.Defend();

            if (e.HP > 0)
                e.AttackPlayer(p);

            Console.WriteLine($"Ваше HP: {p.HP}, HP врага: {e.HP}");
        }

        if (p.HP > 0)
            Console.WriteLine($"Вы победили {e.Name}!\n");
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

    public Weapon(string name, int atk)
    {
        Name = name;
        AttackBonus = atk;
    }
}

class Armor
{
    public string Name;
    public int DefenseBonus;

    public Armor(string name, int def)
    {
        Name = name;
        DefenseBonus = def;
    }
}

class Potion
{
    public string Name = "Лечебное зелье";

    public void Use(Player p)
    {
        p.Heal();
        Console.WriteLine($"{p.Name} использовал зелье и полностью восстановил здоровье!");
    }
}

class Chest
{
    Random rand = new Random();

    public void Open(Player player)
    {
        Console.WriteLine("Вы нашли сундук!");
        int type = rand.Next(3);

        if (type == 0)
        {
            Potion pot = new Potion();
            pot.Use(player);
        }
        else if (type == 1)
        {
            Weapon newWeapon = new Weapon("Меч героя", rand.Next(3, 8));
            Console.WriteLine($"Найдено оружие: {newWeapon.Name} (+{newWeapon.AttackBonus} к атаке)");
            if (player.Weapon != null)
                Console.WriteLine($"Текущее оружие: {player.Weapon.Name} (+{player.Weapon.AttackBonus})");
            Console.WriteLine("Взять новое? (y/n)");
            if (Console.ReadLine().ToLower() == "y")
            {
                player.Weapon = newWeapon;
                Console.WriteLine("Оружие экипировано!");
            }
        }
        else
        {
            Armor newArmor = new Armor("Кираса рыцаря", rand.Next(2, 6));
            Console.WriteLine($"Найдена броня: {newArmor.Name} (+{newArmor.DefenseBonus} к защите)");
            if (player.Armor != null)
                Console.WriteLine($"Текущая броня: {player.Armor.Name} (+{player.Armor.DefenseBonus})");
            Console.WriteLine("Взять новую? (y/n)");
            if (Console.ReadLine().ToLower() == "y")
            {
                player.Armor = newArmor;
                Console.WriteLine("Броня экипирована!");
            }
        }
    }
}
