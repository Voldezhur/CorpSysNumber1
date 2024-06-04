using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Reflection.Metadata;
using System.ComponentModel.DataAnnotations;

namespace Practice1
{
    // Класс студентов
    public class Student
    {
        public int studentCode {get;set;}
        public string lastName {get;set;}
        public string firstName {get;set;}
        public string patronymic {get;set;}
    }

    // Класс Предметов
    public class Subject
    {
        public int subjectCode {get;set;}
        public string name {get;set;}
        public int lecturesVolume {get;set;}
        public int seminarVolume {get;set;}
    }

    // Класс учебных планов
    public class StudyPlan
    {
        public int studentCode {get;set;}
        public int subjectCode {get;set;}
        public int grade {get;set;}
    }

    //  Класс для десериализации и сериализации
    public class universityInfo
    {
        public List<Student> Students {get;set;}
        public List<Subject> Subjects {get;set;}
        public List<StudyPlan> studyPlans {get;set;}
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            using (var sr = new StreamReader("info.json"))
            {
                // Создание читателя json
                var reader = new JsonTextReader(sr);
                var jObject = JObject.Load(reader);
                var students = jObject["Students"].Select(p => p);

                // // Создание нового ученика
                // Student newStudent = new Student()
                // {
                //     studentCode = 3,
                //     lastName = "Obama",
                //     firstName = "Barack",
                //     patronymic = "Barack"
                // };
                
                // // Запись нового студента в список
                // universityDatabase.Students.Add(newStudent);
                
                // // Вывод всех фамилий после записи нового студента
                // foreach(var student in students)
                // {
                //     Console.WriteLine(student["lastName"]);
                // }

                var appRunning = true;

                while(appRunning)
                {
                    // Десериализация файла для работы
                    var jsonContent = File.ReadAllText("info.json");
                    var universityDatabase = JsonConvert.DeserializeObject<universityInfo>(jsonContent);
                    
                    Console.Write("\n\nМеню:\n0. Выйти;\n1. Просмотреть список студентов;\n2. Добавить студента;\n3. Выставить оценку студенту;\n4. Просмотреть оценки студента;\n");
                    var stringInput = Console.ReadLine();
                    var commandInput = Convert.ToInt32(stringInput);

                    switch(commandInput)
                    {
                        // Выход из программы
                        case 0:
                            appRunning = false;
                            break;
                        
                        // Вывод всех фамилий студентов
                        case 1:
                            Console.Write('\n');
                            foreach(var student in students)
                            {
                                Console.WriteLine(student["lastName"]);
                            }
                            break;
                        
                        // Добавление студента в json
                        case 2:
                            // Создание нового ученика
                            var highestStudentCode = universityDatabase.Students.Max(x => x.studentCode);
                            
                            Console.Write("Введите по порядку фамилию, имя и отчество студента через Enter:\n");

                            var newStudentLastName = Console.ReadLine();
                            var newStudentFirstName = Console.ReadLine();
                            var newStudentPatronymic = Console.ReadLine();

                            Student newStudent = new Student()
                            {
                                studentCode = highestStudentCode + 1,
                                lastName = newStudentLastName,
                                firstName = newStudentFirstName,
                                patronymic = newStudentPatronymic
                            };

                            // Добавление студента в файл
                            universityDatabase.Students.Add(newStudent);

                            Console.Write("\nСтудент успешно создан\n");
                            break;

                        // Добавление новой оценки для определенного студента
                        case 3:
                            Console.WriteLine("\nКакому студенту поставить оценку?\nВведите фамилию студента: ");
                            var selectedStudentName = Console.ReadLine();

                            // Поиск кода студента по фамилии
                            var selectedStudentCodes = from student in universityDatabase.Students
                                                    where student.lastName == selectedStudentName
                                                    select student.studentCode;

                            var selectedStudentCode = selectedStudentCodes.First();
                            Console.Write("Код студента: " + selectedStudentCode + "\n");
                            Console.Write("\nПо какой дисциплине поставить оценку?\nВведите код дисциплины: ");
                            var selectedSubjectCode = Console.ReadLine();

                            Console.Write("\nКакую оценку следует поставить?\nВведите оценку цифрой: ");
                            var selectedGrade = Console.ReadLine();

                            // Добавление записи
                            universityDatabase.studyPlans.Add(new StudyPlan(){
                                studentCode = selectedStudentCode,
                                subjectCode = Convert.ToInt32(selectedSubjectCode),
                                grade = Convert.ToInt32(selectedGrade)
                            });

                            Console.Write("\nОценка выставлена\n");
                            break;

                        // Вывод всех оценок студента
                        case 4:
                            Console.WriteLine("\nУ какого студента просмотреть оценки?\nВведите фамилию студента: ");
                            var selectedStudentNameForGrade = Console.ReadLine();

                            // Поиск кода студента по фамилии
                            var selectedStudentCodesForGrade = from student in universityDatabase.Students
                                                    where student.lastName == selectedStudentNameForGrade
                                                    select student.studentCode;

                            var selectedStudentCodeForGrade = selectedStudentCodesForGrade.First();
                            Console.Write("\nКод студента: " + selectedStudentCodeForGrade + "\n");

                            // Выбор всех оценок
                            var selectedStudentGrades = from studyPlan in universityDatabase.studyPlans
                                                    where studyPlan.studentCode == selectedStudentCodeForGrade
                                                    select new {studyPlan.subjectCode, studyPlan.grade};

                            var gradeCount = 0;
                            var satisfactory = 0;
                            var good = 0;
                            var excellent = 0;

                            // Вывод всех оценок и подсчет процентов
                            foreach(var studyPlan in selectedStudentGrades)
                            {
                                gradeCount++;
                                Console.Write("\nКод дисциплины: " + studyPlan.subjectCode + "\nОценка: " + studyPlan.grade + '\n');
                                
                                // Подсчет оценок
                                switch(studyPlan.grade)
                                {
                                    case 3:
                                        satisfactory++;
                                        break;
                                    case 4:
                                        good++;
                                        break;
                                    case 5:
                                        excellent++;
                                        break;    
                                }
                            }

                            Console.Write("\nПроцент оценок 'удовлетворительно': " + (double)satisfactory / gradeCount * 100 + "%;\n");
                            Console.Write("Процент оценок 'хорошо': " + (double)good / gradeCount * 100 + "%;\n");
                            Console.Write("Процент оценок 'отлично': " + (double)excellent / gradeCount * 100 + "%;\n");
                            break;

                        default:
                            Console.Write("Неверный ввод, попробуйте еще раз\n");
                            break;
                    }

                    // Обновление файла Json и запись в файл
                    string updatedJson = JsonConvert.SerializeObject(universityDatabase, Formatting.Indented);
                    File.WriteAllText("info.json", updatedJson);
                }
            }
        }
    }
}