using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PTlab3 {

    public class Car
    {
        [XmlElement("Model")]
        public string Model { get; set; }

        [XmlElement("Engine")]
        public Engine Motor { get; set; }

        [XmlElement("Year")]
        public int Year { get; set; }

        public Car() {}

        public Car(string model, Engine engine, int year)
        {
            Model = model;
            Motor = engine;
            Year = year;
        }
    }

}
