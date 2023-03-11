using System.Management;
using System.Runtime.Versioning;

using Pchp.Core;

namespace Pchpie.Windows
{
    [SupportedOSPlatform("windows")]
    public class WMI
    {
        private readonly ObjectQuery _wql = new();

        public uint GetCPUCores()
        {
            string result = Select("NumberOfLogicalProcessors", "Win32_ComputerSystem");

            return string.IsNullOrEmpty(result) ? 0 : System.Convert.ToUInt32(result);
        }

        public ushort GetCPULoad()
        {
            string result = Select("LoadPercentage", "Win32_Processor");

            return string.IsNullOrEmpty(result) ? ushort.MinValue : System.Convert.ToUInt16(result);
        }

        public PhpArray GetPhysicalMemory()
        {
            PhpArray result = new();

            Dictionary<string, string>? array = Select(new string[] { "TotalVisibleMemorySize", "FreePhysicalMemory" }, "Win32_OperatingSystem");

            if (array != null)
            {
                if (array.TryGetValue("TotalVisibleMemorySize", out string? totalVisibleMemorySize) && array.TryGetValue("FreePhysicalMemory", out string? freePhysicalMemory))
                {
                    if (int.TryParse(totalVisibleMemorySize, out int totalMemory) && int.TryParse(freePhysicalMemory, out int freeMemory))
                    {
                        int memoryUsage = totalMemory - freeMemory;

                        result.Add("memory_usage", memoryUsage);
                        result.Add("total_memory", totalMemory);
                        result.Add("free_memory", freeMemory);
                    }
                }
            }

            return result;
        }

        public PhpArray GetLoad()
        {
            PhpArray result = new();
            PhpArray physicalMemory = GetPhysicalMemory();

            if (physicalMemory.ContainsKey("memory_usage"))
            {
                result.Add("memory_usage", physicalMemory["memory_usage"]);
                result.Add("cpu_load", GetCPULoad());
            }

            return result;
        }

        private Dictionary<string, string>? Select(string[] fields, string entity)
        {
            try
            {
                Dictionary<string, string> array = new();

                _wql.QueryString = "SELECT " + string.Join(',', fields) + " FROM " + entity;

                ManagementObjectSearcher searcher = new(_wql);
                ManagementObjectCollection collection = searcher.Get();

                foreach (ManagementObject item in collection.Cast<ManagementObject>())
                {
                    foreach (string field in fields)
                    {
                        string value = item.GetPropertyValue(field).ToString() ?? "";

                        if (!string.IsNullOrEmpty(value))
                        {
                            array[field] = value;
                        }
                    }
                }

                return array;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string Select(string field, string entity)
        {
            string? result = null;

            Dictionary<string, string>? array = Select(new string[] { field }, entity);

            array?.TryGetValue(field, out result);

            return result ?? "";
        }
    }
}