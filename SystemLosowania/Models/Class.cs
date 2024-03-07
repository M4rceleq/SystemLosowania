using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemLosowania.Models
{
    class Class
    {
        public ObservableCollection<Student> StudentsCollection { get; set; } = new ObservableCollection<Student>();

        public void LoadStudents(string className)
        {
            StudentsCollection.Clear();

            string classFile = File.ReadAllText(Path.Combine(FileSystem.AppDataDirectory, $"{className}.txt"));
            if (string.IsNullOrEmpty(classFile)) return;

            string[] seperatedStudents = classFile.Split("\r\n");
            foreach (string studentLine in seperatedStudents)
            {
                string[] data = studentLine.Split(';');

                Student newStudent = new Student()
                {
                    Id = int.Parse(data[0]),
                    Name = data[1]
                };

                StudentsCollection.Add(newStudent);
            };
        }

        public string ConvertCollectionToString()
        {
            string studentLines = string.Empty;
            for(int i = 0; i < StudentsCollection.Count; i++)
            {
                var student = StudentsCollection[i];
                string studentLine = $"{student.Id};{student.Name}";

                if (i != 0)
                {
                    studentLine = "\r\n" + studentLine;
                }

                studentLines += studentLine;
            }

            return studentLines;
        }
    }
}
