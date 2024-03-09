using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySqlX.XDevAPI.Relational;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Utilities.Collections;
using ReichertsMeatDistributing.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReichertsMeatDistributing.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DealsController : ControllerBase
    {
        private readonly string _excelFilePath;

        public DealsController(IConfiguration configuration)
        {
            _excelFilePath = configuration.GetConnectionString("ExcelFile");
            InitializeExcelFile();
        }

        // GET api/deals
        [HttpGet]
        public ActionResult<List<WeeklyDeal>> Get()
        {
            var deals = ReadDealsFromExcelFile();
            return Ok(deals);
        }

        // GET api/deals/1
        [HttpGet("{id}")]
        public ActionResult<WeeklyDeal> GetById(int id)
        {
            var deals = ReadDealsFromExcelFile();
            var deal = deals.FirstOrDefault(d => d.Id == id);
            if (deal == null)
            {
                return NotFound();
            }
            return Ok(deal);
        }

        // POST api/deals
        [HttpPost]
        public IActionResult Post([FromBody] WeeklyDeal deal)
        {
            var deals = ReadDealsFromExcelFile();

            if (deals.Any())
            {
                deal.Id = deals.Max(d => d.Id) + 1; // Generate new ID
            }
            else
            {
                deal.Id = 1; // Set default ID when no deals exist
            }

            deals.Add(deal); // Add the new deal to the list

            WriteDealsToExcelFile(deals); // Update the Excel file

            return CreatedAtAction(nameof(GetById), new { id = deal.Id }, deal);
        }


        // PUT api/deals/1
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] WeeklyDeal deal)
        {
            var deals = ReadDealsFromExcelFile();
            var existingDeal = deals.FirstOrDefault(d => d.Id == id);
            if (existingDeal == null)
            {
                return NotFound();
            }

            // Update the existing deal
            existingDeal.Name = deal.Name;
            existingDeal.Description = deal.Description;
            existingDeal.Price = deal.Price;

            WriteDealsToExcelFile(deals); // Update the Excel file

            return NoContent();
        }
        [HttpPost("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            if (Request.Headers.TryGetValue("X-HTTP-Method-Override", out var methodOverrideHeader))
            {
                if (methodOverrideHeader.ToString() == "DELETE")
                {
                    return DeleteInternal(id);
                }
            }

            return BadRequest();
        }

        private IActionResult DeleteInternal(int id)
        {
            var deals = ReadDealsFromExcelFile();
            var dealToRemove = deals.FirstOrDefault(d => d.Id == id);
            if (dealToRemove == null)
            {
                return NotFound();
            }

            deals.Remove(dealToRemove); // Remove the deal from the list

            WriteDealsToExcelFile(deals); // Update the Excel file

            return NoContent();
        }

        private List<WeeklyDeal> ReadDealsFromExcelFile()
        {
            List<WeeklyDeal> deals = new List<WeeklyDeal>();

            using (FileStream file = new FileStream(_excelFilePath, FileMode.Open, FileAccess.Read))
            {
                XSSFWorkbook workbook = new XSSFWorkbook(file);
                ISheet sheet = workbook.GetSheetAt(0); // Assuming the first sheet is used

                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    IRow excelRow = sheet.GetRow(row);
                    if (excelRow != null)
                    {
                        deals.Add(new WeeklyDeal
                        {
                            Id = (int)excelRow.GetCell(0).NumericCellValue,
                            Name = excelRow.GetCell(1).StringCellValue,
                            Description = excelRow.GetCell(2).StringCellValue,
                            Price = (decimal)excelRow.GetCell(3).NumericCellValue
                        });
                    }
                }
            }

            return deals;
        }

        private void WriteDealsToExcelFile(List<WeeklyDeal> deals)
        {
            using (FileStream file = new FileStream(_excelFilePath, FileMode.Create, FileAccess.Write))
            {
                XSSFWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Deals");

                // Write header row
                IRow headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Id");
                headerRow.CreateCell(1).SetCellValue("Name");
                headerRow.CreateCell(2).SetCellValue("Description");
                headerRow.CreateCell(3).SetCellValue("Price");

                // Write data
                for (int i = 0; i < deals.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(i + 1);
                    dataRow.CreateCell(0).SetCellValue(deals[i].Id);
                    dataRow.CreateCell(1).SetCellValue(deals[i].Name);
                    dataRow.CreateCell(2).SetCellValue(deals[i].Description);
                    dataRow.CreateCell(3).SetCellValue((double)deals[i].Price);
                }

                workbook.Write(file);
            }
        }

        private void InitializeExcelFile()
        {
            if (!System.IO.File.Exists(_excelFilePath))
            {
                using (FileStream file = new FileStream(_excelFilePath, FileMode.Create, FileAccess.Write))
                {
                    XSSFWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("Deals");

                    // Write header row
                    IRow headerRow = sheet.CreateRow(0);
                    headerRow.CreateCell(0).SetCellValue("Id");
                    headerRow.CreateCell(1).SetCellValue("Name");
                    headerRow.CreateCell(2).SetCellValue("Description");
                    headerRow.CreateCell(3).SetCellValue("Price");

                    workbook.Write(file);
                }
            }
        }
    }
}
