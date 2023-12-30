using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

// Enumeration defnies a set of directions
public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

//Base class for Citizen, Police and Thief
//with position and coordinates as properties
class Entity
{
    public Position Position { get; set; }
    public Entity(int x, int y)
    {
        Position = new Position(x, y);
    }
}
// Class to represent the position, x and y- coordinates
class Position
{
    public int X { get; set; }
    public int Y { get; set; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }
    public bool Equals(Position other)
    {
        return X == other.X && Y == other.Y;
    }
}
// Inherits Class Citizen, Police and Thief that inheris from entity class
class Citizen : Entity
{
    public Direction Direction { get; set; }
    public List<string> Inventory { get; }
    public Citizen(int x, int y, Random random) : base(x, y)
    {
        Inventory = new List<string> { "keys", "phone", "money", "watch" };
        Direction = (Direction)random.Next(4); // Set the Direction property for Citizen
    }
}
class Police : Entity
{
    public List<string> Inventory { get; }
    public Direction Direction { get; set; }

    public Police(int x, int y, Random random) : base(x, y)
    {
        Inventory = new List<string>();
        Direction = (Direction)random.Next(4);
    }
}
class Thief : Entity
{
    public Direction Direction { get; } // Include the Direction property
    public List<string> Inventory { get; }
    public Thief(int x, int y, Random random) : base(x, y)
    {
        Direction = (Direction)random.Next(4);
        Inventory = new List<string>();
    }
}

class City
{
    //Width and Height of the city
    public int Width { get; }
    public int Height { get; }
    //Track counter between police and citizens
    private int policeMetCitizenCount = 0;
    // Track counter between police and thief
    private int policeMetThief = 0;
    // Track counter between thief and citizen
    private int ThiefRobbedCitizen = 0;
    //Store instances
    private List<Citizen> citizens = new List<Citizen>();
    private List<Police> policeList = new List<Police>();
    private List<Thief> thiefList = new List<Thief>();
    // Generate random numbers
    private Random random = new Random();

    //Constructor
    public City(int width, int height, int numCitizens, int numPolices, int numThieves)
    {
        //Width and height of the city
        Width = width;
        Height = height;

        // Create citizens
        for (int i = 0; i < numCitizens; i++)
        {
            citizens.Add(new Citizen(random.Next(Width), random.Next(Height), this.random));
        }
        // Create polices
        for (int i = 0; i < numPolices; i++)
        {
            policeList.Add(new Police(random.Next(Width), random.Next(Height), this.random));
        }
        // Create thieves
        for (int i = 0; i < numThieves; i++)
        {
            thiefList.Add(new Thief(random.Next(Width), random.Next(Height), this.random));
        }
    }
    //Read-only for Police and Thief
    public List<Police> Polices
    {
        get { return policeList; }
    }

    public List<Thief> Thieves
    {
        get { return thiefList; }
    }
    //Metohds to add police, citizen and thief
    public void AddPolice(Police newPolice)
    {
        policeList.Add(newPolice);
    }

    public void AddCitizen(Citizen newCitizen)
    {
        citizens.Add(newCitizen);
    }

    public void AddThief(Thief newThief)
    {
        thiefList.Add(newThief);
    }

    //Get every entity in random directions
    private void MoveInDirection(Entity entity, int direction)
    {
        switch ((Direction)direction)
        {
            case Direction.Up:
                MoveUp(entity);
                break;
            case Direction.Down:
                MoveDown(entity);
                break;
            case Direction.Left:
                MoveLeft(entity);
                break;
            case Direction.Right:
                MoveRight(entity);
                break;
        }
    }
    //Method for moving entity, Up, down, left, right
    private void MoveUp(Entity entity)
    {
        entity.Position = new Position(entity.Position.X, (entity.Position.Y - 1 + Height) % Height);

    }

    private void MoveDown(Entity entity)
    {
        entity.Position = new Position(entity.Position.X, (entity.Position.Y + 1) % Height);
    }

    private void MoveLeft(Entity entity)
    {
        entity.Position = new Position((entity.Position.X - 1 + Width) % Width, entity.Position.Y);
    }

    private void MoveRight(Entity entity)
    {
        entity.Position = new Position((entity.Position.X + 1) % Width, entity.Position.Y);
    }

    //Color letters in the console so it's easier to see for the users
    private static void ColoredLetter(char character)
    {
        ConsoleColor originalForeground = Console.ForegroundColor;

        switch (character)
        {
            case 'C':
                Console.ForegroundColor = ConsoleColor.DarkYellow; // Orange
                break;
            case 'P':
                Console.ForegroundColor = ConsoleColor.Blue;
                break;

            default:
                Console.ForegroundColor = originalForeground;
                break;
        }

        Console.Write(character + " ");

        Console.ForegroundColor = originalForeground;
    }
    private void PrintCityMap(char[,] cityMap)
    {
        //To have the same city update and not create a new "city" when updated
        Console.SetCursorPosition(0, 0); //Set cursor position
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                ColoredLetter(cityMap[y, x]);
            }
            Console.WriteLine();
        }
    }
    private void Count()
    {
        Console.SetCursorPosition(0, Height + 2);

        Console.WriteLine($"A police officer met a citizen: {policeMetCitizenCount}");
        Console.WriteLine($"A police officer took a thief to jail: {policeMetThief}");
        Console.WriteLine($"Thief took an item from a citizen: {ThiefRobbedCitizen}");
    }
    public void Simulate()
    {
        Thread.Sleep(1000);
        char[,] cityMap = new char[Height, Width];

        // Clear the city map
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                cityMap[y, x] = ' ';
            }
        }

        foreach (Citizen citizen in citizens)
        {
            // Simulate citizen movement in console 
            int direction = (int)citizen.Direction;
            // Move the citizen that is based on new direction
            MoveInDirection(citizen, direction);
            // Update city map with citizens
            cityMap[citizen.Position.Y, citizen.Position.X] = 'C';


            foreach (Police police in policeList)
            {
                // If police and citizen are at the same place add +1 to counting
                if (police.Position.X == citizen.Position.X && police.Position.Y == citizen.Position.Y)
                {
                    policeMetCitizenCount++;
                }
            }
            //Check if thief is at the same location as a citizen
            foreach (Thief thief in thiefList)
            {

                if (thief.Position.X == citizen.Position.X && thief.Position.Y == citizen.Position.Y)
                {
                    //Then add +1 to counting
                    ThiefRobbedCitizen++;
                    foreach (string item in citizen.Inventory)
                    {
                        if (!thief.Inventory.Contains(item))
                        {
                            thief.Inventory.Add(item); // Thief takes an item
                            citizen.Inventory.Remove(item); // Remove the taken item from citizen
                            break; // Stop after taking one item
                        }
                    }
                }
            }
        }
        foreach (Thief thief in thiefList)
        {
            int direction = (int)thief.Direction;
            MoveInDirection(thief, direction);
            cityMap[thief.Position.Y, thief.Position.X] = 'T';

        }

        foreach (Police police in policeList)
        {
            // Randomly change the direction
            int direction = (int)police.Direction;

            // Move the police based on the new direction
            MoveInDirection(police, direction);

            cityMap[police.Position.Y, police.Position.X] = 'P';

            foreach (Thief thief in thiefList)
            {
                //If thief meets police add +1 to counting
                if (thief.Position.X == police.Position.X && thief.Position.Y == police.Position.Y)
                {
                    policeMetThief++;
                    List<string> itemsToRemove = new List<string>();

                    foreach (string item in thief.Inventory)
                    {
                        police.Inventory.Add(item); // Police takes all items from thief
                    }
                    thief.Inventory.Clear(); // The whole inventory is cleared from the thiefs
                }
            }
        }
        // Print the city map to the console
        PrintCityMap(cityMap);
        Count();
    }

    static void Main()
    {
        Console.Clear(); // Clear the console screen
        Console.WriteLine("-----Welcome to the game 'The City'. Press any key to start...-----");
        Console.ReadKey(); // Wait for any key press before starting the simulation

        Random random = new Random();
        // Create new city width -40, length 10, 23 citizens, 15 polices and 10 thiefs
        City city = new City(40, 10, 23, 15, 10);
        // Adding a new citizen 
        Citizen newCitizen = new Citizen(random.Next(city.Width), random.Next(city.Height), random); // Example position
        city.AddCitizen(newCitizen);
        // Adding a new police officer with a random position
        Police newPolice = new Police(random.Next(city.Width), random.Next(city.Height), random);
        city.AddPolice(newPolice);

        // Adding a new thief with a random position
        Thief newThief = new Thief(random.Next(city.Width), random.Next(city.Height), random);
        city.AddThief(newThief);
        // How many laps the game continues
        int numberOfIterations = 12;
        int delayBetweenIterations = 500;  // how long the delay is between every lap
        for (int i = 0; i < numberOfIterations; i++)
        {
            city.Simulate();
            Thread.Sleep(delayBetweenIterations);
        }
    }
}
