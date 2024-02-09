using System.Text;
using Newtonsoft.Json;

namespace final.Types;

public class LogHistory
{
    private StringBuilder LogReport = new StringBuilder();
    public void AddLog(string log)
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        string Hislog = $"Run '{log}' on {date}";

        LogReport.AppendLine(Hislog);
        this.SaveToTxt();
    }
    public string GetLog()
    {
        return LogReport.ToString();
    }

    public void SaveToJson(string filePath)
    {
        string[] lines = LogReport.ToString().Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        using (StreamWriter file = File.CreateText(filePath))
        {
            foreach (string line in lines)
            {
                string logJson = JsonConvert.SerializeObject(line, Formatting.None);
                file.WriteLine(logJson);
            }
        }
    }

    public void SaveToTxt()     //  save logs in txt file
    {
        File.WriteAllText("Data/log.txt", LogReport.ToString());
    }

}