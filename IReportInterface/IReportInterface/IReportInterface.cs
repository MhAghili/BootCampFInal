namespace BootcampReporter
{
    public interface IReportExtension
    {
        string Execute();
        string GetReportName();

        bool Enabled { get; set; }

        string Catagory { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class BootCampReportExtensionAttribute : Attribute
    {
    }

}



