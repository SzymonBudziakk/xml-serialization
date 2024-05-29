using PTlab3;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

class Program
{
    public static void Main(string[] args)
    {
        List<Car> myCars = new List<Car>{
            new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
            new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
            new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
            new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
            new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
            new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
            new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
            new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
            new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
        };

        var query1 = myCars
            .Where(car => car.Model == "A6")
            .Select(car => new
            {
                engineType = car.Motor.Model == "TDI" ? "diesel" : "petrol",
                hppl = car.Motor.HorsePower / car.Motor.Displacement
            });

        var query2 = query1
            .GroupBy(car => car.engineType)
            .Select(group => new
            {
                EngineType = group.Key,
                AverageHppl = group.Average(car => car.hppl)
            });

        foreach (var group in query2)
        {
            Console.WriteLine($"Engine Type: {group.EngineType}, Average Hppl: {group.AverageHppl}");
        }

        SerializeCars(myCars);

        List<Car> loadedCars = DeserializeCars();

        EvaluateXPath();

        CreateXmlFromLinq(myCars);

        CreateXhtmlFromCars(myCars);

        ModifyXmlDocument();
    }

    public static void SerializeCars(List<Car> cars)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(CarsCollection));
        CarsCollection carsCollection = new CarsCollection { Cars = cars };

        using (var writer = new StreamWriter("CarsCollection.xml"))
        {
            serializer.Serialize(writer, carsCollection);
        }
    }

    public static List<Car> DeserializeCars()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(CarsCollection));
        using (var reader = new StreamReader("CarsCollection.xml"))
        {
            CarsCollection carsCollection = (CarsCollection)serializer.Deserialize(reader);
            return carsCollection.Cars;
        }
    }

    public static void DisplayCars(List<Car> cars)
    {
        foreach (var car in cars)
        {
            Console.WriteLine($"Model: {car.Model}, Year: {car.Year}");
            Console.WriteLine($"  Engine Type: {car.Motor.Model}");
            Console.WriteLine($"  Displacement: {car.Motor.Displacement} L");
            Console.WriteLine($"  HorsePower: {car.Motor.HorsePower} hp");
            Console.WriteLine();
        }
    }

    public static void EvaluateXPath()
    {
        XElement rootNode = XElement.Load("CarsCollection.xml");


        var avgHorsePower = (double)rootNode
            .XPathEvaluate("sum(//car[Engine/@model != 'TDI']/Engine/HorsePower) div count(//car[Engine/@model != 'TDI'])");

        Console.WriteLine($"Average Horsepower of Non-TDI Engines: {avgHorsePower}");

        IEnumerable<string> models = rootNode.XPathSelectElements("//car/Model[not(. = preceding::car/Model)]").Select(e => e.Value);

        Console.WriteLine("Car Models:");
        foreach (var model in models)
        {
            Console.WriteLine(model);
        }
    }

    private static void CreateXmlFromLinq(List<Car> myCars)
    {
        
        IEnumerable<XElement> nodes = from car in myCars
                                      select new XElement("car",

                                          new XElement("Model", car.Model),
                                          new XElement("Year", car.Year),
                                          new XElement("engine",
                                              new XElement("Displacement", car.Motor.Displacement),
                                              new XElement("HorsePower", car.Motor.HorsePower),
                                              new XAttribute("model", car.Motor.Model)
                                          )
                                      );

        XElement rootNode = new XElement("cars", nodes);

        rootNode.Save("CarsFromLinq.xml");
    }
    private static void CreateXhtmlFromCars(List<Car> myCars)
    {
        XDocument xhtmlDoc = new XDocument(
                new XDocumentType("html", null, null, null),
                new XElement("html",
                    new XElement("head",
                        new XElement("title", "Car Collection")
                    ),
                    new XElement("body",
                        new XElement("table",
                            new XElement("tr",
                                new XElement("th", "model"),
                                new XElement("th", "Motor model"),
                                new XElement("th", "displacement"),
                                new XElement("th", "horsePower"),
                                new XElement("th", "year")
                            ),
                            from car in myCars
                            select new XElement("tr",
                                new XElement("td", car.Model),
                                new XElement("td", car.Year),
                                new XElement("td", car.Motor.Model),
                                new XElement("td", car.Motor.Displacement),
                                new XElement("td", car.Motor.HorsePower)
                            )
                        )
                    )
                )
            );
        xhtmlDoc.Save("Cars.html");
    }

    static void ModifyXmlDocument()
    {
        XDocument doc = XDocument.Load("CarsCollection.xml");

        foreach (var engineElement in doc.Descendants("engine"))
        {
            var hpElement = engineElement.Element("HorsePower");
            if (hpElement != null)
            {
                hpElement.Name = "hp";
            }
        }

        foreach (var carElement in doc.Descendants("car"))
        {
            var yearElement = carElement.Element("Year");
            if (yearElement != null)
            {
                carElement.SetAttributeValue("year", yearElement.Value);
                yearElement.Remove();
            }
        }

        doc.Save("ModifiedCarsCollection.xml");
        Console.WriteLine("XML modification is completed and saved.");
    }
}
