using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SoftIngInter
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("https://reqres.in/api/users?page=2");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    dynamic json = JsonConvert.DeserializeObject(responseBody);

                    Console.WriteLine("Задача 1: Получение данных о пользователях");

                    foreach (var user in json.data)
                    {
                        if (user.id == 10) // Можно заменить "10" на любое число, чтобы увидеть другого пользователя
                        {
                            Console.WriteLine($"Имя - {user.first_name}, фамилия - {user.last_name}");
                            Console.ReadKey();
                            break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Ошибка: {response.StatusCode}");
                    Console.ReadKey();
                }
            }

            Console.WriteLine("Задача 2: Группировка сотрудников по отделам");
            string inputJson = @"
            [
                {
                    ""dept"": ""Отдел информационных систем"",
                    ""name"": ""Сотрудник 1"",
                    ""phone"": ""89999999999""
                },
                {
                    ""dept"": ""Отдел АСУ"",
                    ""name"": ""Сотрудник 2"",
                    ""phone"": ""88888888888""
                },
                {
                    ""dept"": ""Отдел информационных систем"",
                    ""name"": ""Сотрудник 3"",
                    ""hours"": 165,
                    ""phone"": ""87777777777""
                },
                {
                    ""dept"": ""Отдел информационных систем"",
                    ""name"": ""Сотрудник 4"",
                    ""hours"": 132,
                    ""phone"": ""86666666666""
                },
                {
                    ""dept"": ""Отдел АСУ"",
                    ""name"": ""Сотрудник 5"",
                    ""hours"": 101,
                    ""phone"": ""85555555555""
                },
                {
                    ""dept"": ""Отдел информационных систем"",
                    ""name"": ""Сотрудник 6"",
                    ""hours"": 98,
                    ""phone"": ""84444444444""
                }
            ]";

            List<Employee> employees = JsonConvert.DeserializeObject<List<Employee>>(inputJson);

            var groupedEmployees = employees.GroupBy(e => e.Dept)
                                            .Select(g => new
                                            {
                                                Dept = g.Key,
                                                Count = g.Count(),
                                                AvgHours = Math.Round(g.Where(e => e.Hours.HasValue).Average(e => e.Hours.Value), MidpointRounding.AwayFromZero),
                                                People = g.Select(e => new
                                                {
                                                    e.Name,
                                                    e.Phone,
                                                    e.Hours
                                                }).ToList()
                                            });

            var result = new Dictionary<string, Dictionary<string, object>>();
            foreach (var group in groupedEmployees)
            {
                var department = new Dictionary<string, object>
                {
                    { "count", group.Count },
                    { "avg_hours", group.AvgHours },
                    { "people", group.People }
                };
                result.Add(group.Dept, department);
            }

            string outputJson = JsonConvert.SerializeObject(result, Formatting.Indented);
            Console.WriteLine(outputJson);
            Console.ReadKey();
        }
    }

    public class Employee
    {
        public string Dept { get; set; }
        public string Name { get; set; }
        public int? Hours { get; set; }
        public string Phone { get; set; }
    }
}
