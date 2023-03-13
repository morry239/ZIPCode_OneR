namespace DUP_OneRClassifier;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

public class OneR
{
    private class Item
    {
        public string Attribute { get; private set; }
        public string Classification { get; set; }
        public int Frequency { get; set; }

        public Item(string attribute, string classification, int frequency = 1)
        {
            this.Classification = classification;
            this.Attribute = attribute;
            this.Frequency = frequency;
        }

        public override bool Equals(object obj)
        {
            Item item = obj as Item;

            return (Attribute.Equals(item.Attribute) && Classification.Equals(item.Classification));
        }

        public override int GetHashCode()
        {
            return Classification.GetHashCode() + Attribute.GetHashCode();
        }
    }

    private class ItemSet
    {
        public string Column { get; set; }
        public Dictionary<string, Item> Items { get; private set; }
        public double ErrorRate { get; private set; }
        public ItemSet(string col)
        {
            Column = col;
            Items = new Dictionary<string, Item>(10);
        }

        public void AddItem(string attribute, string classification)
        {
            var key = attribute + "->" + classification;

            Item item = null;
            if (Items.ContainsKey(key))
            {

                item = Items[key];
            }

            if (item == null)
            {
                item = new Item(attribute, classification);

                Items[key] = item;

            }
            else
            {
                item.Frequency++;
                
            }
        }

        public void Process()
        {
            var result = from item in Items.Values
                         orderby item.Attribute, item.Frequency descending
                         select item;

            int total = 0;
            int correct = 0;
            string attribute = null;

            foreach (Item item in result)
            {
                total += item.Frequency;

                var key = item.Attribute + "->" + item.Classification;

                // Gruppenwechsel
                if (attribute == null || attribute != item.Attribute)
                {
                    attribute = item.Attribute;
                    correct += item.Frequency;
                }
                else
                    Items.Remove(key);
            }

            ErrorRate = 100.0 - (correct * 100.0 / total);
        }
    }

    public Dictionary<string, string> Solution { get; set; }
    
   

    public OneR()
    {
        Solution = new Dictionary<string, string>();
    }

    public void Build(StreamReader r)
    {
        /*string filePath = @"/Users/mamikawamura/Downloads/sonnencreme.csv";
        using FileStream fs = File.Create(filePath); //applied FileStream
        r = new StreamReader(fs);*/
        var list = new List<ItemSet>();

        
            r = new StreamReader("/Users/mamikawamura/Desktop/geo-data.csv");
        
            
        var header = r.ReadLine().Split(new char[] { ';' });
            // Spaltenüberschriften lesen
        foreach (string head in header)
        { 
            list.Add(new ItemSet(head));
        }

        // Daten einlesen und Häufigkeiten ermitteln
        string line = r.ReadLine();

        while (line != null)
        {
            var tokens = line.Split(new char[] { ';' });

            string classfication = tokens[0];

            for (int i = 1; i <= tokens.Length - 1; i++)
            {
                string attribute = tokens[i];

                list[i].AddItem(attribute, classfication);
            }
            line = r.ReadLine();
        }
        r.Close();

        // Gruppen mit bester Vorhersage pro Spalte ermitteln
        foreach (ItemSet items in list)
            // vorher: Ausgabe alle Häufigkeitsverteilungen
            // Console.WriteLine(items);
            // nacher: Ausgabe bester Häufigkeitsverteilungen
            // Console.WriteLine(items);
            items.Process();

        // Spalte mit geringster Fehlerrate ermitteln
        var result = (from items in list
                      where !double.IsNaN(items.ErrorRate)
                      orderby items.ErrorRate
                      select items);

        ItemSet solution = result.First();

        // Lösung für Vorhersage speichern
        foreach (Item item in solution.Items.Values)
            Solution.Add(item.Attribute, item.Classification);

        Console.WriteLine("".PadLeft(80, '_'));
        Console.WriteLine("Regeln: " + solution);

        list.Clear();
    }

    public string Classify(string value)
    {
        //value = "braun";
        return Solution[value];
    }
}
