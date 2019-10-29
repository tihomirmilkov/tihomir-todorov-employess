using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using tihomir_todorov_employess.Models;
using tihomir_todorov_employess.Utilities;

namespace tihomir_todorov_employess.Utilities
{
    public class InputFileProcessor
    {
        /// <summary>
        /// Find the employees that worked longest time on the same project
        /// </summary>
        /// <param name="filePath">Uploaded file path - temporary location</param>
        /// <returns>Employee object with the result</returns>
        public List<EmployeesSameProjectWorkCouple> ProcessFile(string filePath)
        {
            var allEmployees = ParseEmployeesData(filePath);

            return GetEmployeesThatWorkedMostTimeOnACommonProjects(allEmployees);
        }

        /// <summary>
        /// Get Employees That Worked Most Time On A Common Projects
        /// </summary>
        /// <param name="allEmployeesData">All employees data list - raw as it was parsed from the file.</param>
        /// <returns>Best employees couple that matches the criteria</returns>
        private List<EmployeesSameProjectWorkCouple> GetEmployeesThatWorkedMostTimeOnACommonProjects(List<Employee> allEmployeesData)
        {
            // Sort employees and remove duplicates in order to optimize search
            var sortedEmployeesData = allEmployeesData.Distinct().OrderBy(x => x.EmployeeID).ToList();

            // Remove overlapping time for the same employee and project
            var optimizedEmployeesData = new List<Employee>();
            foreach (var employeeData in sortedEmployeesData)
            {
                // check if there is overlap
                var overlappedEmployeeData = optimizedEmployeesData
                                                .Where(x => x.EmployeeID == employeeData.EmployeeID && x.ProjectID == employeeData.ProjectID
                                                        && !(x.DateFrom > employeeData.DateTo || x.DateTo < employeeData.DateFrom))
                                                .FirstOrDefault();

                if (overlappedEmployeeData == null)
                {
                    optimizedEmployeesData.Add(employeeData);
                }
                else
                {
                    overlappedEmployeeData.DateFrom = overlappedEmployeeData.DateFrom > employeeData.DateFrom ? employeeData.DateFrom : overlappedEmployeeData.DateFrom;
                    overlappedEmployeeData.DateTo = overlappedEmployeeData.DateTo < employeeData.DateTo ? employeeData.DateTo : overlappedEmployeeData.DateTo;
                }
            }

            // Group all employees couples that have worked on the same project and calculate days
            var allEmployeesSameProjectWorkCouples = from e1 in optimizedEmployeesData
                                                     join e2 in optimizedEmployeesData on e1.ProjectID equals e2.ProjectID
                                                     where !(e1.DateFrom > e2.DateTo || e1.DateTo < e2.DateFrom) && e1.EmployeeID < e2.EmployeeID
                                                     select new EmployeesSameProjectWorkCouple
                                                     {
                                                         EmployeeID1 = e1.EmployeeID,
                                                         EmployeeID2 = e2.EmployeeID,
                                                         ProjectID = e1.ProjectID,
                                                         DaysWorked = CalculateDaysWorked(e1, e2)
                                                     };

            // Calculate total days working together for all employees couples and get the bes one
            var bestEmployeeCouple = allEmployeesSameProjectWorkCouples
                                        .GroupBy(all => new { all.EmployeeID1, all.EmployeeID2 })
                                        .Select(x => new
                                        {
                                            x.Key,
                                            TotalDaysWorked = x.Sum(all => all.DaysWorked)
                                        })
                                        .OrderByDescending(x => x.TotalDaysWorked)
                                        .FirstOrDefault();

            // Get all project "DaysWorked" for the best couple of employees
            var bestEmployeesSameProjectWorkCouples = allEmployeesSameProjectWorkCouples
                                                    .Where(x => x.EmployeeID1 == bestEmployeeCouple.Key.EmployeeID1 && x.EmployeeID2 == bestEmployeeCouple.Key.EmployeeID2)
                                                    .ToList();

            return bestEmployeesSameProjectWorkCouples;
        }

        private static int CalculateDaysWorked(Employee employee1, Employee employee2)
        {
            return ((employee1.DateTo < employee2.DateTo ? employee1.DateTo : employee2.DateTo) - (employee1.DateFrom > employee2.DateFrom ? employee1.DateFrom : employee2.DateFrom)).Days + 1;
        }

        /// <summary>
        /// Read file and parse data in a List<Employee>. Also make a few validations.
        /// </summary>
        /// <param name="filePath">Uploaded file path - temporary location</param>
        /// <returns>Employees data as List<Employee></returns>
        private List<Employee> ParseEmployeesData(string filePath)
        {
            var allEmployees = new List<Employee>();

            // get Date here otherwise it can be different by miliseconds for all lines :)
            var dateTimeNow = DateTime.Now;

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var columns = line.Trim().Split(',');

                // check if we have correct columns count
                if (columns.Count() == 4)
                {
                    // an exception might occure while parsing values - not a problem at this point because there is a try-catch block in the controller
                    Employee employee;
                    try
                    {
                        employee = new Employee(
                            int.Parse(columns[0].Trim()), // EmployeeID
                            int.Parse(columns[1].Trim()), // ProjectID
                            ParseDateTime(columns[2]), // DateFrom
                            columns[3].Trim().Equals("NULL", StringComparison.OrdinalIgnoreCase) ? dateTimeNow : ParseDateTime(columns[3]) // DateTo - can be null
                            );
                    }
                    catch
                    {
                        throw new InvalidDataException("Invalid columns count on one or more lines.");
                    }

                    allEmployees.Add(employee);
                }
                // probably an empty line is not a problem so throw an exception only if columns are not 0 or 4
                else if (columns.Count() != 0 && !(columns.Count() == 1 && columns[0].Length == 0))
                {
                    throw new InvalidDataException("Invalid columns count on one or more lines.");
                }
            }

            return allEmployees;
        }

        /// <summary>
        /// Parse all DateTime formats from string to DateTime. Exclude the ones with comma symbol - ",". It is a delimiter in the file. If we chage it I can include those formats too :)
        /// For example if delimiter is different we can include file format like "Monday, June 15, 2009".
        /// </summary>
        /// <param name="dateTime">string that can be all king of DateTime formats</param>
        /// <returns>DateTime ready to go!</returns>
        private DateTime ParseDateTime(string dateTime)
        {
            var result = new DateTime();

            dateTime = dateTime.Trim();

            var ci = new CultureInfo("en-US");
            var formats = new[] {
                "M-d-yyyy", "d-M-yyyy", "dd-MM-yyyy", "MM-dd-yyyy", "yyyy-M-d", "yyyy-d-M", "yyyy-dd-MM", "yyyy-MM-dd",
                "M.d.yyyy", "d.M.yyyy", "dd.MM.yyyy", "MM.dd.yyyy", "yyyy.M.d", "yyyy.d.M", "yyyy.dd.MM", "yyyy.MM.dd",
                "M/d/yyyy", "d/M/yyyy", "dd/MM/yyyy", "MM/dd/yyyy", "yyyy/M/d", "yyyy/d/M", "yyyy/dd/MM/yyyy", "yyyy/MM/dd"
            }.Union(ci.DateTimeFormat.GetAllDateTimePatterns()).ToArray();

            result = DateTime.ParseExact(dateTime, formats, ci, DateTimeStyles.AssumeLocal);

            return result;
        }
    }
}
