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
            if (turn % 10 == 0)
            {
                Enemy boss = GetRandomBoss();
                Battle(player, boss);
            }
            else if (rand.Next(2) == 0)
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

    private Enemy GetRandomBoss()
    {
        int r = rand.Next(4);
        if (r == 0) return new Boss("ВВГ (Гоблин)", 60, 12, 8, "крит", 0.2);
        if (r == 1) return new Boss("Ковальский (Скелет)", 100, 13, 10, "игнор", 0);
        if (r == 2) return new Boss("Архимаг C++ (Маг)", 70, 16, 6, "фриз", 0.25);
        return new Boss("Пестов С-- (Скелет)", 50, 18, 3, "игнор", 0.15);
    }

    private void Battle(Player p, Enemy e)
    {
        Console.WriteLine($"\n⚔️  Вы встретили врага: {e.Name} (HP {e.HP})");

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
            Console.WriteLine($"🎉 Вы победили {e.Name}!\n");
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

    Random rand = new Random();
    bool defending = false;

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
        defending = false;
    }

    public void Defend()
    {
        defending = true;
        Console.WriteLine($"{Name} встал в защиту! 40% шанс уклониться.");
    }

    public bool TryDodge()
    {
        return defending && rand.NextDouble() < 0.4;
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
    protected Random rand = new Random();

    public virtual void AttackPlayer(Player p)
    {
        if (p.TryDodge())
        {
            Console.WriteLine($"{p.Name} уклонился от атаки!");
            return;
        }

        int dmg = Math.Max(Attack - p.Defense, 1);
        p.HP -= dmg;
        Console.WriteLine($"{Name} атакует {p.Name} на {dmg} урона!");
    }
}

class Boss : Enemy
{
    string type;
    double chance;

    public Boss(string name, int hp, int atk, int def, string type, double chance)
    {
        Name = name;
        HP = hp;
        Attack = atk;
        Defense = def;
        this.type = type;
        this.chance = chance;
    }

    public override void AttackPlayer(Player p)
    {
        if (type == "крит" && rand.NextDouble() < chance)
        {
            int dmg = (Attack * 2);
            p.HP -= dmg;
            Console.WriteLine($"{Name} наносит КРИТИЧЕСКИЙ удар на {dmg} урона!");
        }
        else if (type == "фриз" && rand.NextDouble() < chance)
        {
            Console.WriteLine($"{Name} замораживает {p.Name}! Ход пропущен!");
            // можно реализовать заморозку в следующем ходу
        }
        else if (type == "игнор")
        {
            int dmg = Attack;
            p.HP -= dmg;
            Console.WriteLine($"{Name} игнорирует защиту! {p.Name} получает {dmg} урона!");
        }
        else
        {
            base.AttackPlayer(p);
        }
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

    public override void AttackPlayer(Player p)
    {
        if (rand.NextDouble() < 0.1)
        {
            int dmg = (Attack * 2);
            p.HP -= dmg;
            Console.WriteLine($"{Name} наносит КРИТИЧЕСКИЙ удар на {dmg} урона!");
        }
        else
        {
            base.AttackPlayer(p);
        }
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

    public override void AttackPlayer(Player p)
    {
        int dmg = Attack; // игнорирует защиту
        p.HP -= dmg;
        Console.WriteLine($"{Name} игнорирует броню и наносит {dmg} урона!");
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

    public override void AttackPlayer(Player p)
    {
        if (rand.NextDouble() < 0.15)
        {
            Console.WriteLine($"{Name} замораживает {p.Name}! Следующий ход пропущен!");
        }
        else
        {
            base.AttackPlayer(p);
        }
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
