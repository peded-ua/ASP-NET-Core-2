using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Додаємо конфігураційні файли
builder.Configuration
    .AddJsonFile("companies.json", optional: false, reloadOnChange: true)
    .AddXmlFile("companies.xml", optional: false, reloadOnChange: true)
    .AddIniFile("companies.ini", optional: false, reloadOnChange: true)
    .AddJsonFile("about_me.json", optional: false, reloadOnChange: true);

var app = builder.Build();

// Ендпоінт для аналізу кількості співробітників компаній
app.MapGet("/companies", (IConfiguration config) =>
{
    var companies = new List<dynamic>();

    // Отримуємо дані з JSON файлу
    var jsonCompanies = config.GetSection("Companies").GetChildren();
    foreach (var company in jsonCompanies)
    {
        string name = company["Name"];
        int employees = int.Parse(company["Employees"]);
        companies.Add(new { Name = name, Employees = employees });
    }

    // Отримуємо дані з XML файлу
    var xmlCompanies = config.GetSection("Companies").GetChildren();
    foreach (var company in xmlCompanies)
    {
        string name = company.GetSection("Name").Value;
        int employees = int.Parse(company.GetSection("Employees").Value);
        companies.Add(new { Name = name, Employees = employees });
    }

    // Отримуємо дані з INI файлу
    var iniCompanies = new[] { "Microsoft", "Apple", "Google" };
    foreach (var iniCompany in iniCompanies)
    {
        string name = iniCompany;
        int employees = int.Parse(config[$"{iniCompany}:Employees"]);
        companies.Add(new { Name = name, Employees = employees });
    }

    // Фільтруємо компанії, де кількість співробітників більше 2
    var filteredCompanies = companies.Where(c => c.Employees > 2);

    return filteredCompanies;
});

// Ендпоінт для виведення даних про користувача
app.MapGet("/about", (IConfiguration config) =>
{
    // Отримуємо секцію AboutMe з конфігураційного файлу
    var name = config["AboutMe:Name"];
    var age = config["AboutMe:Age"];
    var city = config["AboutMe:City"];
    var skills = config.GetSection("AboutMe:Skills").GetChildren().Select(s => s.Value).ToList();

    return new
    {
        Name = name,
        Age = age,
        City = city,
        Skills = skills
    };
});

app.Run();