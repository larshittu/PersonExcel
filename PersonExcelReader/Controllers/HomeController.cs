using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PersonalExce.Helper.Data.Dto;
using PersonalExcel.Core.Interface;
using PersonalExeclReader.Helper.Data.Model;
using PersonalExeclReader.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
//Susing System.Web.Mvc;

namespace PersonalExeclReader.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPersonExcel _personal;

        public HomeController(ILogger<HomeController> logger, IPersonExcel personal)
        {
            _logger = logger;
            _personal = personal;
        }

        /// <summary>
        /// Get all the available record from Database
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ModelState.Clear();
            PersonDto model = new PersonDto
            {
                Persons = _personal.Fetch().Result
            };
            return View(model);
        }

        /// <summary>
        /// Direct form information Inserting
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult InsertPerson(PersonalModel objModel)
        {
            try
            {
                int result = _personal.Create(objModel).Result;
                if (result == 1)
                {
                    TempData["SuccessMessage"] = "Person Added Successfully";
                    ModelState.Clear();
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to insert person data sucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// Excel file importation
        /// </summary>
        /// <param name="uploadexcel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Import(IFormFile uploadexcel)
        {
            try
            {
                #region Commented
                int recordCount = 0;
                var errorMsg = string.Empty;
                var errorHolder = new List<string>();
                var personModel = new List<PersonalModel>();


                //Check for Empty upload
                if (uploadexcel == null || uploadexcel.Length == 0)
                {
                    TempData["ErrorMessage"] = "file not selected";
                    return RedirectToAction("Index");
                }

                //Validating file extension
                List<string> fileExtensions = new List<string> { ".XLS", ".XLSX", ".CSV" };
                var extension = Path.GetExtension(uploadexcel.FileName).ToUpper();
                if (!fileExtensions.Contains(extension))
                {
                    TempData["ErrorMessage"] = $"Uploaded file not supported, upload only .Xls, .Xlsx or .csv file";
                    return RedirectToAction("Index");
                }

                bool isheader = true;
                var reader = new StreamReader(uploadexcel.OpenReadStream());
                List<string> headers = new List<string>();

                //Reading through the file records
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (isheader)
                    {
                        isheader = false;
                        headers = values.ToList();
                    }
                    else
                    {
                        recordCount++;
                        var rows = values.ToList();

                        var firstName = string.Empty;
                        var surName = string.Empty;
                        var age = 0;
                        var sex = string.Empty;
                        var mobile = string.Empty;
                        var active = false;


                        firstName = rows[1];
                        surName = rows[2];

                        if (rows[3].Any(c => char.IsLetter(c)))
                        {
                            errorMsg = $"Age can only be number at row {recordCount} \n";
                        }
                        else
                        {
                            age = Convert.ToInt32(rows[3]);
                        }

                        if (!string.IsNullOrEmpty(rows[4]))
                        {
                            var sexValue = rows[4].ToString().Trim();
                            if (sexValue.Any(c => char.IsDigit(c)))
                            {
                                errorMsg = $"Sex can only be Character at row {recordCount}\n";
                            }
                            else if (sexValue == "M" || sexValue == "F")
                            {
                                sex = sexValue;
                            }
                            else
                            {
                                errorMsg = $"Sex can only be M or F  at row {recordCount}\n";
                            }
                        }

                        if (!string.IsNullOrEmpty(rows[5]))
                        {
                            if (rows[5].Any(c => char.IsLetter(c)))
                            {
                                errorMsg = $"Mobile can only be number at row {recordCount}\n";
                            }
                            else
                            {
                                mobile = rows[5];
                            }
                        }

                        if (!string.IsNullOrEmpty(rows[6]))
                        {
                            var boolvalue = rows[6].ToString().Trim();
                            if (boolvalue == "TRUE" || boolvalue == "FALSE")
                            {
                                active = Convert.ToBoolean(rows[6]);
                            }
                            else
                            {
                                errorMsg = $"Active can only be TRUE or FAlSE at row {recordCount}\n";
                            }
                        }
                        else
                        {
                            errorMsg = $"Active can be empty at row {recordCount}\n";
                        }

                        if (!string.IsNullOrEmpty(errorMsg))
                        {
                            errorHolder.Add(errorMsg);
                            errorMsg = string.Empty;
                            continue;
                        }
                        else
                        {
                            //Add data to list of record for insertion
                            personModel.Add(new PersonalModel()
                            {
                                FirstName = firstName,
                                Surname = surName,
                                Age = age,
                                Sex = sex,
                                Mobile = mobile,
                                Active = active
                            });
                        }
                    }
                }
                #endregion

                //Check any error is found on the excel for correction
                if (errorHolder.Any())
                {
                    var messages = errorHolder.Aggregate((a, b) => a + ",\n" + b);
                    TempData["ErrorMessage"] = messages;
                    ModelState.Clear();

                    return RedirectToAction("Index");
                }

                //Insert record list record from excel file
                var result = _personal.ImportData(personModel).Result;
                if (result != 0)
                {
                    TempData["SuccessMessage"] = "Person information importation Successfully";
                    ModelState.Clear();
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to import person information Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("Index");
            }
            catch (Exception exp)
            {
                TempData["ErrorMessage"] = $"An error occured, while importing file: {exp.Message}";
                return RedirectToAction("Index");
            }
        }

        public JsonResult EditPerson(int? id)
        {
            var person = _personal.Fetch().Result.Find(x => x.Id.Equals(id));
            //return Json(person, JsonRequestBehavior.AllowGet);
            return Json(person);
        }

        [HttpPost]
        public IActionResult UpdatePerson(PersonalModel objModel)
        {
            try
            {
                int result = _personal.Update(objModel).Result;
                if (result == 1)
                {
                    TempData["SuccessMessage"] = "Person information updated Successfully";
                    ModelState.Clear();
                }
                else
                {
                    TempData["ErrorMessage"] = "Updating of person information is Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }

        public IActionResult DeletePerson(int id)
        {
            try
            {
                int result = _personal.Delete(id).Result;
                if (result == 1)
                {
                    TempData["SuccessMessage"] = "Person information deleted Successfully";
                    ModelState.Clear();
                }
                else
                {
                    TempData["ErrorMessage"] = "Person deletion is Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
