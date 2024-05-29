using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PTlab3
{

    public class Engine
    {
        [XmlElement("Displacement")]
        public double Displacement { get; set; }

        [XmlElement("HorsePower")]
        public int HorsePower { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        public Engine() {}

        public Engine(double displacement, int horsePower, string model)
        {
            Displacement = displacement;
            HorsePower = horsePower;
            Model = model;
        }
    }
}
