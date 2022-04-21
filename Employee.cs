using System;
using System.Collections.Generic;
using System.Text;

namespace AnstalldaFranvaro
{
    public class Employee
    {
        public int Id { get; set; }
         public List<Absence> EmployeeAbcences { get; set; }

        public Employee(int id)
        {
            Id = id;
        }
        public bool Equals(Employee other)
        {
            return Id == other.Id;
        }

        public void AddAbsence(Absence newAbsence)
        {

            foreach (var abs in EmployeeAbcences)
            {
                if (newAbsence.StartDate.Equals(abs.StartDate))
                {
                    abs.EndDate = newAbsence.EndDate;
                    abs.Percentage = newAbsence.Percentage;
                    return;
                }
            }
            EmployeeAbcences.Add(newAbsence);
        }
        
    }
}
