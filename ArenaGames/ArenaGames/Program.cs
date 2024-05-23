using System;

public class Program
{
    public static void Main()
    {
        // Get the number of rounds from the user
        int rounds = GetNumberOfRounds();

        int playerOneWins = 0, playerTwoWins = 0;

        // Loop through each round of the battle
        for (int i = 0; i < rounds; i++)
        {
            // Initialize the players for this round
            Hero playerOne = new Knight("Leroooy Jenkinsss", new Sword());
            Hero playerTwo = new Rogue("Tarzan", new Dagger());

            // Announce the battle
            Console.WriteLine($"{playerOne.Name} и {playerTwo.Name} ще проведът битка!");

            // Create the arena and conduct the battle
            Arena arena = new Arena(playerOne, playerTwo);
            Hero winner = arena.Battle();

            // Announce the winner
            Console.WriteLine($"Победителят е: {winner.Name}");

            // Increment the win counter for the winner
            if (winner == playerOne)
                playerOneWins++;
            else
                playerTwoWins++;
        }

        // Display the final results
        DisplayResults(playerOneWins, playerTwoWins);
    }

    // Function to get the number of rounds from the user
    private static int GetNumberOfRounds()
    {
        Console.Write("Колко битки искате да направите:");
        try
        {
            return int.Parse(Console.ReadLine());
        }
        catch
        {
            return 1; // Default to 1 if input is invalid
        }
    }

    // Function to display the results of the battles
    private static void DisplayResults(int playerOneWins, int playerTwoWins)
    {
        Console.WriteLine($"\nИграч едно има {playerOneWins} победи.");
        Console.WriteLine($"Играч 2 има {playerTwoWins} победи.");
        Console.ReadLine();
    }
}

public class Arena
{
    public Hero FirstHero { get; }
    public Hero SecondHero { get; }

    // Constructor to initialize the heroes
    public Arena(Hero firstHero, Hero secondHero)
    {
        FirstHero = firstHero;
        SecondHero = secondHero;
    }

    // Function to conduct the battle between heroes
    public Hero Battle()
    {
        // Randomly choose the initial attacker and defender
        Hero attacker = Random.Shared.Next(2) == 0 ? FirstHero : SecondHero;
        Hero defender = attacker == FirstHero ? SecondHero : FirstHero;

        // Continue the battle until one hero is dead
        while (true)
        {
            // Attacker performs an attack
            int damage = attacker.Attack();
            defender.TakeDamage(damage);

            // Display attack details
            Console.WriteLine($"{attacker.Name} атакува {defender.Name} за {damage} щета.");
            Console.WriteLine($"{defender.Name} има {defender.Health} останала кръв.");

            // Check if the defender is dead
            if (defender.IsDead)
            {
                Console.WriteLine($"{defender.Name} умря!");
                return attacker; // Attacker wins
            }

            // Swap roles for the next round
            (attacker, defender) = (defender, attacker);
        }
    }
}

public abstract class Hero
{
    public string Name { get; }
    public int Health { get; private set; }
    public int Strength { get; }
    protected int StartingHealth { get; }
    public Weapon Weapon { get; }

    // Property to check if the hero is dead
    public bool IsDead => Health <= 0;

    // Constructor to initialize hero properties
    protected Hero(string name, Weapon weapon)
    {
        Name = name;
        Health = 500;
        StartingHealth = Health;
        Strength = 100;
        Weapon = weapon;
    }

    // Method for the hero to perform an attack
    public virtual int Attack()
    {
        return Weapon.ApplySpecialAbility((Strength * Random.Shared.Next(80, 121)) / 100);
    }

    // Method for the hero to take damage
    public virtual void TakeDamage(int damage)
    {
        if (damage >= 0)
        {
            Health -= damage;
        }
    }

    // Helper method to simulate a dice roll for chance-based actions
    protected bool ThrowDice(int chance)
    {
        return Random.Shared.Next(101) <= chance;
    }

    // Method for the hero to heal
    protected void Heal(int amount)
    {
        Health = Math.Min(StartingHealth, Health + amount);
    }
}

public class Knight : Hero
{
    private const int BlockChance = 10; // 10% chance to block an attack
    private const int ExtraDamageChance = 5; // 5% chance to deal extra damage

    // Constructor for the Knight class
    public Knight(string name, Weapon weapon) : base(name, weapon) { }

    // Override method to handle damage with block chance and damage reduction
    public override void TakeDamage(int damage)
    {
        damage -= damage * Random.Shared.Next(20, 61) / 100; // Reduce damage
        if (ThrowDice(BlockChance)) damage = 0; // Block damage
        base.TakeDamage(damage);
    }

    // Override method to handle attack with extra damage chance
    public override int Attack()
    {
        int attack = base.Attack();
        if (ThrowDice(ExtraDamageChance)) attack = attack * 150 / 100; // Deal extra damage
        return attack;
    }
}

public class Rogue : Hero
{
    private const int TripleDamageMagicNumber = 5; // Magic number for triple damage
    private const int HealInterval = 3; // Interval for healing
    private int attackCount; // Count of attacks

    // Constructor for the Rogue class
    public Rogue(string name, Weapon weapon) : base(name, weapon)
    {
        attackCount = 0;
    }

    // Override method to handle attack with special rogue logic
    public override int Attack()
    {
        int attack = base.Attack();
        if (attack % 25 == TripleDamageMagicNumber) attack *= 3; // Triple damage
        attackCount++;
        if (attackCount % HealInterval == 0 && ThrowDice(25)) Heal(StartingHealth * 50 / 100); // Heal every few attacks
        return attack;
    }

    // Override method to handle damage with chance to avoid
    public override void TakeDamage(int damage)
    {
        if (ThrowDice(30)) damage = 0; // Avoid damage
        base.TakeDamage(damage);
    }
}

public class Mage : Hero
{
    // Constructor for the Mage class
    public Mage(string name, Weapon weapon) : base(name, weapon) { }

    // Override method to handle attack with double damage chance
    public override int Attack()
    {
        int attack = base.Attack();
        if (ThrowDice(20)) attack *= 2; // Double damage
        return attack;
    }

    // Override method to handle damage with chance to avoid
    public override void TakeDamage(int damage)
    {
        if (ThrowDice(25)) damage = 0; // Avoid damage
        base.TakeDamage(damage);
    }
}

public class Archer : Hero
{
    // Constructor for the Archer class
    public Archer(string name, Weapon weapon) : base(name, weapon) { }

    // Override method to handle attack with critical hit chance
    public override int Attack()
    {
        int attack = base.Attack();
        if (ThrowDice(15)) attack *= 3; // Critical hit
        return attack;
    }

    // Override method to handle damage with chance to dodge
    public override void TakeDamage(int damage)
    {
        if (ThrowDice(20)) damage = 0; // Dodge attack
        base.TakeDamage(damage);
    }
}

public abstract class Weapon
{
    public string Name { get; }
    public int DamageBoost { get; }

    // Constructor to initialize weapon properties
    protected Weapon(string name, int boost)
    {
        Name = name;
        DamageBoost = boost;
    }

    // Method to apply special ability of the weapon
    public virtual int ApplySpecialAbility(int baseDamage)
    {
        return baseDamage + DamageBoost;
    }
}

public class Sword : Weapon
{
    // Constructor for the Sword class
    public Sword() : base("Меч", 20) { }

    // Override method to apply special ability with extra damage chance
    public override int ApplySpecialAbility(int baseDamage)
    {
        if (Random.Shared.Next(100) < 10)
        {
            baseDamage = baseDamage * 150 / 100; // Extra damage
        }
        return baseDamage + DamageBoost;
    }
}

public class Dagger : Weapon
{
    // Constructor for the Dagger class
    public Dagger() : base("Ножка", 15) { }

    // Override method to apply special ability with armor ignore chance
    public override int ApplySpecialAbility(int baseDamage)
    {
        if (Random.Shared.Next(100) < 20)
        {
            return baseDamage + DamageBoost; // Ignore armor
        }
        return baseDamage;
    }
}

public class Staff : Weapon
{
    // Constructor for the Staff class
    public Staff() : base("прът", 25) { }

    // Override method to apply special ability with double damage chance
    public override int ApplySpecialAbility(int baseDamage)
    {
        if (Random.Shared.Next(100) < 15)
        {
            baseDamage *= 2; // Double damage
        }
        return baseDamage + DamageBoost;
    }
}
