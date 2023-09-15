using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.WindowsService
{
    public static class CommonHelper
    {

        /// <summary>
        /// Calculates the aggregate power trade volumes for the hour.
        /// </summary>
        /// <param name=""></param>
        public static Dictionary<int, PowerData> AggregateTrade(IEnumerable<PowerTrade> tradeList)
        {
            Dictionary<int, PowerData> sumDictionary = MapHourtoPeriod();

            //iterate through the list and aggregate the volume
            foreach (var tra in tradeList)
            {
                var aggregatedVolumes = tra.Periods
             .GroupBy(period => period.Period)
            .ToDictionary(group => group.Key, group => group.Sum(period => (int)period.Volume));

                foreach (var kvp in aggregatedVolumes)
                {
                    if (sumDictionary.ContainsKey(kvp.Key))
                    {
                        sumDictionary[kvp.Key].Volume += kvp.Value;
                    }
                }            
            }

            return sumDictionary;

        }

        /// <summary>
        /// Aggregate the trade data and prepare the output for the file.
        /// </summary>
        /// <param name="sumDictionary"></param>
        /// <returns></returns>
        public static string PrepareAggregateForExtract(Dictionary<int, PowerData> sumDictionary)
        {
            StringBuilder sb = new ();
            sb.AppendLine(string.Join(Environment.NewLine, sumDictionary.Values
             .Select(item => $"{item.Hour},{item.Volume}")));

            return sb.ToString();
        }

        /// <summary>
        /// Map hour to period
        /// </summary>
        /// <returns></returns>
        private static Dictionary<int, PowerData> MapHourtoPeriod()
        {
            Dictionary<int, PowerData> sumDictionary = new ()
            {
                { 1, new PowerData("23:00")},
                { 2, new PowerData("00:00")},
                { 3, new PowerData("01:00")},
                { 4, new PowerData("02:00")},
                { 5, new PowerData("03:00")},
                { 6, new PowerData("04:00")},
                { 7, new PowerData("05:00")},
                { 8, new PowerData("06:00")},
                { 9, new PowerData("07:00")},
                { 10, new PowerData("08:00")},
                { 11, new PowerData("09:00")},
                { 12, new PowerData("10:00")},
                { 13, new PowerData("11:00")},
                { 14, new PowerData("12:00")},
                { 15, new PowerData("13:00")},
                { 16, new PowerData("14:00")},
                { 17, new PowerData("15:00")},
                { 18, new PowerData("16:00")},
                { 19, new PowerData("17:00")},
                { 20, new PowerData("18:00")},
                { 21, new PowerData("19:00")},
                { 22, new PowerData("20:00")},
                { 23, new PowerData("21:00")},
                { 24, new PowerData("22:00")}

        };

            return sumDictionary;
        }

        /// <summary>
        /// Writes the file to the disk
        /// </summary>
        /// <param name="message"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool WriteToFileAsync(string message, DateTime date)
        {
            bool result = false;
            //Format the date and time components
            string formattedDate = date.ToString("yyyyMMdd_HHmm");
            // Create the file name
            string fileName = $"PowerPosition_{formattedDate}.csv";

            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine(ReadApplicationSettings("AppSettings:OutputFileHeader"));
            stringBuilder.AppendLine(message);
            stringBuilder.ToString();


            var path = ReadApplicationSettings("AppSettings:OutputDirectory");
            if (!string.IsNullOrWhiteSpace(path))
            {
                using StreamWriter writer = new(Path.Combine(path, fileName), false);
                writer.Write(message);
                writer.Close();
                result=true;
            }

            return result;
        }

        /// <summary>
        /// Create output directory if it does not exist.
        /// </summary>
        /// <returns></returns>
        public static bool CreateOutputDirectory()
        {
            DirectoryInfo? info = null;
            var path = ReadApplicationSettings("AppSettings:OutputDirectory");

            if (!Directory.Exists(path))
            {
                info = Directory.CreateDirectory(path);
            }
            return info is not null;
        }

        /// <summary>
        /// Fetches the setting and then they value can be used.
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static string? ReadApplicationSettings(string settingName)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", false)
                .Build();
            return configuration[settingName];
        }
    }
}
