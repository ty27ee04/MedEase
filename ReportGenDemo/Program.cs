ReportContext context = new ReportContext();

Console.WriteLine("Choose report type:");
Console.WriteLine("1 - PDF");
Console.WriteLine("2 - CSV");

string choice = Console.ReadLine();

if (choice == "1")
{
    context.SetStrategy(new PdfReport());
}
else if (choice == "2")
{
    context.SetStrategy(new CsvReport());
}
else
{
    Console.WriteLine("Invalid choice");
    return;
}

context.Generate("This report was generated using Strategy Pattern.");
