﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace tihomir_todorov_employess.Utilities
{
    public class InputFileProcessor
    {
        /// <summary>
        /// Find the employees that worked longest time on the same project
        /// </summary>
        /// <param name="filePath">Uploaded file path - temporary location</param>
        /// <returns>Employee object with the result</returns>
        public EmployeesCouple ProcessFile(string filePath)
        {
            var allEmployees = ParseEmployeesData(filePath);

            return GetEmployeesThatWorkedMostTimeOnASingleProject(allEmployees);
        }

        /// <summary>
        /// Get Employees That Worked Most Time On A Single Project
        /// </summary>
        /// <param name="allEmployees">All employees list - raw as it was parsed from the file.</param>
        /// <returns>Best employees couple that matches the criteria</returns>
        private EmployeesCouple GetEmployeesThatWorkedMostTimeOnASingleProject(List<Employee> allEmployees)
        {
            var bestEmployeesCouple = new EmployeesCouple();

            // sort employees and remove duplicates in order to optimize search
            var sortedEmployees = allEmployees.OrderBy(x => x.EmployeeID).Distinct().ToList();

            // check all sorted employees for longest time working on a single project - only for employees next in the list (we save checking same employees couple twice)
            foreach (var employee in sortedEmployees)
            {
                EmployeesCouple employeesCouple = (from e in sortedEmployees
                                                   where e.EmployeeID > employee.EmployeeID && e.ProjectID == employee.ProjectID
                                                   select new EmployeesCouple()
                                                   {
                                                       EmployeeID1 = employee.EmployeeID,
                                                       EmployeeID2 = e.EmployeeID,
                                                       ProjectID = employee.ProjectID,
                                                       DaysWorked = ((employee.DateTo < e.DateTo ? employee.DateTo : e.DateTo) - (employee.DateFrom > e.DateFrom ? employee.DateFrom : e.DateFrom)).Days
                                                   })
                                                  .Where(x => x.DaysWorked > 0)
                                                  .OrderByDescending(x => x.DaysWorked)
                                                  .FirstOrDefault();

                if (employeesCouple != null && bestEmployeesCouple.DaysWorked < employeesCouple.DaysWorked)
                {
                    bestEmployeesCouple = employeesCouple;
                }
            }

            return bestEmployeesCouple;
        }

        /// <summary>
        /// Read file and parse data in a List<Employee>. Also make a few validations.
        /// </summary>
        /// <param name="filePath">Uploaded file path - temporary location</param>
        /// <returns>Employees data as List<Employee></returns>
        private List<Employee> ParseEmployeesData(string filePath)
        {
            var allEmployees = new List<Employee>();

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var columns = line.Split(',');

                // check if we have correct columns count
                if (columns.Count() == 4)
                {
                    // an exception might occure while parsing values - not a problem at this point because there is a try-catch block in the controller
                    var employee = new Employee(
                        int.Parse(columns[0].Trim()), // EmployeeID
                        int.Parse(columns[1].Trim()), // ProjectID
                        Convert.ToDateTime(columns[2].Trim()), // DateFrom
                        columns[3].Trim().Equals("NULL", StringComparison.OrdinalIgnoreCase) ? DateTime.Now : Convert.ToDateTime(columns[3]) // DateTo - can be null
                        );

                    allEmployees.Add(employee);
                }
                // probably an empty line is not a problem so throw an exception only if columns are not 0 or 4
                else if (columns.Count() != 0)
                {
                    throw new InvalidDataException("Invalid columns count on one or more lines.");
                }
            }

            return allEmployees;
        }
    }
}