using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Fluent;

namespace AnstalldaFranvaro
{
    public class FileHandler : IFileHandler
    {
        public FileHandler()
        {
            
        }

        public List<Employee> ReadCsvFile(string csvfile)
        {


            if (!File.Exists(csvfile)) throw new Exception($"File {csvfile} doesn't exist!");

            var employees = new List<Employee>();

            var empDictionary = new Dictionary<int, List<Absence>>();
            
            var file = new FileInfo(csvfile);

            //CsvHelper.Configuration.Configuration configvar = Configuration.For(new CultureInfo("de-AT")); // optional, default == CurrentCulture

            var f = ReadCsv(csvfile);

            var csv = file.ReadCsv(row => new {
                id = row.GetField<int>(row[0]),
                /*date = DateTime.Parse(row[1]),
                type = int.Parse(row[2]),
                percentage = double.Parse(row[3]),*/
            });

            if(csv.Count == 0) throw new Exception($"File {csvfile} is empty!");


          

            foreach (var line in csv)
            {


                var employee = UpdateList(employees, line.id);
                employee.AddAbsence(new Absence
                {
                    //StartDate = line.date
                });

             
            }

            return employees;
        }

        private object ReadCsv(string path)
        {
            var lines = File.ReadAllLines(path);

            List<object> l = new List<object>();

            foreach (var line in lines)
            {
                var split = line.Split(';');
                l.Add(new
                {
                    col0 = split[0],
                    col1 = split[1],
                    col2 = split[2],
                    col3 = split[3],

                });
            }

            return l;
        }

        private Employee UpdateList(List<Employee> employees, int id)
        {
            Employee employee = new Employee(id);

            foreach (var emp in employees)
            {
                if (employee.Equals(emp))
                    return emp;
            }
            employees.Add(employee);
            return employees[employees.Count - 1];
        }

        private void AddCsvInfo(object csv, Dictionary<int, List<Absence>> empDictionary)
        {
            throw new NotImplementedException();
        }


        public void UpdateWithXmlFile(string xmlpath)
        {
            throw new NotImplementedException();
        }
    }

    public interface IFileHandler
    {
        public List<Employee> ReadCsvFile(string file);
        void UpdateWithXmlFile(string xmlpath);
    }
}
