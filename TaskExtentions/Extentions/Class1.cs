using System;
using Testpr.Types;
using BootcampReporter;  // using dll

namespace Extentions;

[BootCampReportExtension]
public class CountOfTasksReportExtension : IReportExtension
{
    public string Execute()
    {
        ToDoList toDoList = new ToDoList();



        string jsonContent = File.ReadAllText("Data/Task.json");
        toDoList.AddJsonTask(jsonContent);
        toDoList.SortList();


        int totalCount = toDoList.tasks.Count;

        return $"The total number of tasks is {totalCount}";
    }

    public string GetReportName()
    {
        return "Count of total Tasks";
    }

    public bool Enabled { get; set; } = true;
}

[BootCampReportExtension]
public class CountOfOverdueReportExtension : IReportExtension
{
    public string Execute()
    {
        ToDoList toDoList = new ToDoList();


        string jsonContent = File.ReadAllText("Data/Task.json");
        toDoList.AddJsonTask(jsonContent);
        toDoList.SortList();


        var numberOfOverdueTask = toDoList.tasks.Where(task => task.DueDate < DateTime.Now).Count();


        return $"Number of OverDue task is {numberOfOverdueTask} ";

    }

    public string GetReportName()
    {
        return "Count of Overdue Tasks";
    }

    public bool Enabled { get; set; } = true;
}


