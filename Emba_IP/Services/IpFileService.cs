using Emba_IP.Models;
using System.Linq;

namespace Emba_IP.Services
{
    public class IpFileService
    {
        private readonly string _filePath;

        public IpFileService(IConfiguration config, IWebHostEnvironment env)
        {
            var relativePath = config["IpFilePath"]; // "Data/ip_list.txt"
            _filePath = Path.Combine(env.ContentRootPath, relativePath);
        }

        public List<IpModel> GetAll()
        {
            if (!File.Exists(_filePath))
                return new List<IpModel>();

            return File.ReadAllLines(_filePath)
                       .Where(l => !string.IsNullOrWhiteSpace(l))
                       .Select(line => new IpModel { IpAddress = line.Trim() })
                       .ToList();
        }

        public List<IpModel> GetFiltered(string searchTerm)
        {
            return GetAll()
           .Where(ip => string.IsNullOrWhiteSpace(searchTerm) ||
                        ip.IpAddress.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
           .ToList();
        }

        public void Add(string ip)
        {
            var ips = GetAll().Select(x => x.IpAddress).ToList();

            if (!ips.Contains(ip))
                File.AppendAllText(_filePath, ip + Environment.NewLine);
        }

        public void Update(string oldIp, string newIp)
        {
            var lines = File.ReadAllLines(_filePath).ToList();

            var index = lines.FindIndex(line => line == oldIp);

            if (index != -1)
            {
                lines[index] = newIp;
                File.WriteAllLines(_filePath, lines);
            }
        }

        public bool Delete(string ip)
        {
            var allIps = GetAll();
            var existingIp = allIps.FirstOrDefault(x => x.IpAddress == ip);
            if (existingIp == null)
                return false;

            allIps.Remove(existingIp);
            SaveAll(allIps);
            return true;
        }

        private void SaveAll(List<IpModel> list)
        {
            var lines = list.Select(x => $"{x.IpAddress}");
            File.WriteAllLines(_filePath, lines);
        }
    }
}

