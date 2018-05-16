using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.WEB
{
    public class DataTableModel<T>
    {
        [JsonProperty(PropertyName = "name")]
        public int Draw { get; set; }
        [JsonProperty(PropertyName = "recordsTotal")]
        public int RecordsTotal { get; set; }
        [JsonProperty(PropertyName = "recordsFiltered")]
        public int DecordsFiltered { get; set; }
        [JsonProperty(PropertyName = "data")]
        public IEnumerable<T> Data { get; set; }

    }
}
