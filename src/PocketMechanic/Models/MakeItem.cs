namespace PocketMechanic.Models
{
    public class MakeItem
    {
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string Initial => string.IsNullOrEmpty(Name) ? "" : Name.Substring(0, 1).ToUpper();
    }
}
