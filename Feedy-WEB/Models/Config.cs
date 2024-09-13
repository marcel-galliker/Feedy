using System.ComponentModel.DataAnnotations;

namespace Feedy_WEB.Models
{
    public class Config
    {
        [Key]
		public Int64 config_id { get; set;}
        public string name { get; set; }
        public string descrpition { get; set; }
        public int counter { get; set; }
    }
}
