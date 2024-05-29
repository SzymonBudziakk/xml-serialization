using System.Xml.Serialization;

namespace PTlab3
{
    [XmlRoot("cars")]
    public class CarsCollection
    {
        [XmlElement("car")]
        public List<Car> Cars { get; set; }
    }
}
