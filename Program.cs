using System.Text;
using Microsoft.VisualBasic;

namespace DUP_OneRClassifier;

class Program
{
    public static void Main(string[] args)
    {
        var oneR = new OneR();

        /*while (args.Length > 0)
        {
            
        }*/
            //oneR.Build(new StreamReader(args[0], Encoding.Default));
            /*string filePath = @"/Users/mamikawamura/Desktop/geo-data.csv";
            using FileStream fs = File.Create(filePath); //applied FileStream*/
            oneR.Build(new StreamReader("/Users/mamikawamura/Desktop/geo-data.csv"));
            Console.WriteLine("\nGültigen Wert zur Vorhersage eingeben");


        string value = Console.ReadLine();
        while (value != "")
        {
            var predicted = oneR.Classify(value);

            Console.WriteLine("Vorhersage: {0}", predicted ?? "<unbekannt>");
            
        }
    }
}