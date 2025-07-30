namespace Emba_IP.Models
{
    public class LdapSettings
    {
        public string Path { get; set; }
        public int Port { get; set; }
        public string ServiceUsername { get; set; }
        public string ServicePassword { get; set; }
        public string SearchBase { get; set; }
    }
}
