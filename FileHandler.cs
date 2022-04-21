using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using CsvHelper;
using CsvHelper.Fluent;

namespace AnstalldaFranvaro
{
    public class FileHandler : IFileHandler
    {
        public FileHandler()
        {
            
        }

        public List<Employee> ReadXmlFile(string filePath)
        {

            XmlDocument xmlDoc = new XmlDocument();

    //        //returns url of main directory which contains "/bin/Debug"
    //        var url = System.IO.Path.GetDirectoryName(
    //System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

    //        //correction in path to point it in Root directory
    //        var mainpath = url.Replace("\\bin\\Debug", "") + "\\books.xml";
            //load xml file
            xmlDoc.Load(filePath);

            //save all nodes in XMLnodelist
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/Data/FileDate");

            //loop through each node and save it value in NodeStr
            var NodeStr = "";

            foreach (XmlNode node in nodeList)
            {
                NodeStr = NodeStr + "\nAuthor " + node.SelectSingleNode("author").InnerText;
                NodeStr = NodeStr + "\nTitle " + node.SelectSingleNode("title").InnerText;
                NodeStr = NodeStr + "\nGenre " + node.SelectSingleNode("genre").InnerText;
                NodeStr = NodeStr + "\nPrice " + node.SelectSingleNode("price").InnerText;
                NodeStr = NodeStr + "\nDescription -" + node.SelectSingleNode("description").InnerText;


            }
            //print all Authors details
            Console.WriteLine(NodeStr);


            //if (Directory.Exists(filePath))
            //{
            //    DirectoryInfo dirSource = new DirectoryInfo(filePath);
            //    var allXMLFiles = dirSource.GetFiles("*.xml", SearchOption.AllDirectories).ToList();

            //    List<Employee> listAllEntries = new List<Employee>();

            //    foreach (var nextFile in allXMLFiles)
            //    {
            //        try
            //        {
            //            XmlSerializer serializer = new XmlSerializer(typeof(Employee));
            //            using (TextReader reader = new StringReader(System.IO.File.ReadAllText(nextFile.FullName)))
            //            {
            //                Employee result = (Employee)serializer.Deserialize(reader);
            //                listAllEntries.Add(result);
            //            }
            //        }
            //        catch (Exception ex)
            //        {

            //        }
            //    }

            //    Console.WriteLine(listAllEntries.Count);
            //}

            return new List<Employee>();
        }

        public List<Employee> ReadCsvFile(string csvfile)
        {
            if (!File.Exists(csvfile)) throw new Exception($"File {csvfile} doesn't exist!");

            var employees = new List<Employee>();

            var empDictionary = new Dictionary<int, List<Absence>>();


            var empAbsenceDictionary = new Dictionary<int, Absence>();
            Absence newAbsence = new Absence();

            List<DateTime> listDate = new List<DateTime>();
            List<DateTime> listDate2 = new List<DateTime>();

            var file = new FileInfo(csvfile);

            //CsvHelper.Configuration.Configuration configvar = Configuration.For(new CultureInfo("de-AT")); // optional, default == CurrentCulture

            var currentAbsence = ReadCsv(csvfile);

            var currentAbsenceID = currentAbsence.Select
                (x => x.Key).Distinct();
            var absenceDetails = currentAbsenceID.ToList().Select(item => new
            {
                id=item,
                absence= currentAbsence.Where(i=>i.Key==item).Select(i=>i.Value).ToList()
            });

            foreach (var embAbDetails in absenceDetails)
            {
                

           double dateDiff=0;
                  

                for (int i = 0; i < embAbDetails.absence.Count; i++)
                {
                    if (listDate.Count > 0) dateDiff = (embAbDetails.absence[i].StartDate - listDate[listDate.Count - 1]).TotalDays;

                    if (dateDiff < 2) 
                    {
                        listDate.Add(embAbDetails.absence[i].StartDate);

                        newAbsence.StartDate = embAbDetails.absence[embAbDetails.absence.Count- embAbDetails.absence.Count].StartDate;
                        newAbsence.Percentage = embAbDetails.absence[i].Percentage;
                        newAbsence.AbsenceType = embAbDetails.absence[i].AbsenceType;
                        newAbsence.EndDate = listDate[listDate.Count - 1];
                    }
                    else
                    {
                        listDate2.Add(embAbDetails.absence[i].StartDate);

                        newAbsence.StartDate = listDate2[0];
                        newAbsence.Percentage = embAbDetails.absence[i].Percentage;
                        newAbsence.AbsenceType = embAbDetails.absence[i].AbsenceType;
                        newAbsence.EndDate = listDate2[listDate2.Count - 1];
                    }

                    //embAbDetails.absence[i].EndDate = listDate[listDate.Count-1];

                    

                }

                var testAbsence = newAbsence;

                empAbsenceDictionary.Add(embAbDetails.id, testAbsence);
                listDate.Clear();

                //Employee employee = new Employee(embAbDetails.id);
                //employee.AddAbsence(embAbDetails.absence[i]);

                //employees.Add(employee);

            }

            //var collectedEmpDetails = embAbDetails.absence.Select(x => new
            //{
            //    absenceType = x.AbsenceType,
            //    precentage = x.Percentage,
            //    startDate = x.StartDate,
            //    endData = listDate[listDate.Count - 1]
            //});

            foreach (var item in currentAbsence)
            {

            }
         

            return employees;
        }

        private List<KeyValuePair<int ,Absence>> ReadCsv(string path)
        {
            var lines = File.ReadAllLines(path);

            List<KeyValuePair<int, Absence>> listAbsence = new List<KeyValuePair<int, Absence>>();

            foreach (var line in lines)
            {
                var split = line.Split(';');
                listAbsence.Add(new KeyValuePair<int, Absence>(int.Parse(split[0]), new Absence
                {
                    StartDate = DateTime.Parse(split[1]),
                    AbsenceType = (AbsenceType)int.Parse(split[2]),
                    Percentage = double.Parse(split[3])

                }));
            }

            return listAbsence;
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
        public List<Employee> ReadXmlFile(string filePath);
        void UpdateWithXmlFile(string xmlpath);
    }
}
