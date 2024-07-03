using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        // Define inventory
        var inventory = new Dictionary<string, int>
        {
            { "Seat", 50 },
            { "Pedal", 60 },
            { "Frame", 60 },
            { "Tube", 35 }
        };

        // Define parts
        var seat = new Part("Seat");
        var pedal = new Part("Pedal");
        var frame = new Part("Frame");
        var tube = new Part("Tube");

        // Define bundles
        var wheel = new Bundle("Wheel", new List<PartCount>
        {
            new PartCount(frame, 1),
            new PartCount(tube, 1)
        });

        var bike = new Bundle("Bike", new List<PartCount>
        {
            new PartCount(seat, 1),
            new PartCount(pedal, 2),
            new PartCount(wheel, 2)
        });

        // Calculate maximum number of bikes
        int maxBikes = bike.CalculateMaxBundles(inventory);
        Console.WriteLine($"Maximum number of bikes that can be built: {maxBikes}");
    }
}

abstract class Product
{
    public string Name { get; }

    protected Product(string name)
    {
        Name = name;
    }
}

class Part : Product
{
    public Part(string name) : base(name) { }
}

class PartCount
{
    public Product Product { get; }
    public int Count { get; }

    public PartCount(Product product, int count)
    {
        Product = product;
        Count = count;
    }
}

class Bundle : Product
{
    public List<PartCount> Parts { get; }

    public Bundle(string name, List<PartCount> parts) : base(name)
    {
        Parts = parts;
    }

    public int CalculateMaxBundles(Dictionary<string, int> inventory)
    {
        // Create a temporary inventory to simulate deductions
        var tempInventory = new Dictionary<string, int>(inventory);
        return CalculateMaxBundlesInternal(tempInventory);
    }

    private int CalculateMaxBundlesInternal(Dictionary<string, int> tempInventory)
    {
        int maxBundles = int.MaxValue;

        foreach (var partCount in Parts)
        {
            int availableParts;

            if (partCount.Product is Bundle bundle)
            {
                availableParts = bundle.CalculateMaxBundlesInternal(tempInventory);
            }
            else if (partCount.Product is Part part)
            {
                availableParts = tempInventory.ContainsKey(part.Name) ? tempInventory[part.Name] : 0;
            }
            else
            {
                throw new InvalidOperationException("Unknown product type");
            }

            int possibleBundles = availableParts / partCount.Count;
            maxBundles = Math.Min(maxBundles, possibleBundles);
        }

        // Deduct the used parts from the temp inventory
        DeductInventory(tempInventory, maxBundles);

        return maxBundles;
    }

    private void DeductInventory(Dictionary<string, int> tempInventory, int maxBundles)
    {
        foreach (var partCount in Parts)
        {
            if (partCount.Product is Bundle bundle)
            {
                bundle.DeductInventory(tempInventory, maxBundles);
            }
            else if (partCount.Product is Part part)
            {
                tempInventory[part.Name] -= partCount.Count * maxBundles;
            }
        }
    }
}
