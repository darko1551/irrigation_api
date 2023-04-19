using Newtonsoft.Json.Linq;

namespace irregation_api.Models.Json
{
    public class SensorReading
    {
        public string Mac { get; set; }
       // public TemperatureSensor Temperature { get; set; }
        //public HumiditySensor Humidity { get; set; }
        public StateSensor State { get; set; }



        public SensorReading(string json) {
            JObject jObject = JObject.Parse(json);
            JToken record = jObject["records"];
            JToken temperature = record["70"];
            JToken humidity = record["71"];
            JToken state = record["68"];
            Mac = jObject.GetValue("mac").ToString();
            // Temperature = new TemperatureSensor((double)temperature["value"], (bool)temperature["isValid"]);
            //Humidity = new HumiditySensor((double)humidity["value"], (bool)humidity["isValid"]);
            State = new StateSensor((int)state["value"], (bool)state["isValid"]);
        }
    }

    public class TemperatureSensor {
        public TemperatureSensor(double temperature, bool isValid)
        {
            this.Temperature = temperature;
            this.IsValid = isValid;
        }

        public double Temperature { get; set; }
        public bool IsValid { get; set; }
    }

    public class HumiditySensor
    {

        public HumiditySensor(double humidity, bool isValid)
        {
            Humidity = humidity;
            IsValid = isValid;
        }

        public double Humidity { get; set; }
        public bool IsValid { get; set; }
    }

    public class StateSensor
    {

        public StateSensor(int state, bool isValid)
        {
            State = state;
            IsValid = isValid;
        }

        public double State { get; set; }
        public bool IsValid { get; set; }
    }

}
