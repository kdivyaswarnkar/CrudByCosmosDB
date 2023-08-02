using Newtonsoft.Json;

namespace CosmosDbCrudByRP.Models
{
    #region EmploeeModelProperties
    public class EmployeeModel
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
    }
    #endregion
}
