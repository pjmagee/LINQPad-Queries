<Query Kind="Program">
  <NuGetReference>DocumentFormat.OpenXml</NuGetReference>
  <Namespace>DocumentFormat.OpenXml</Namespace>
  <Namespace>DocumentFormat.OpenXml.Packaging</Namespace>
  <Namespace>DocumentFormat.OpenXml.Spreadsheet</Namespace>
</Query>

void Main()
{	
	var file = Path.GetTempFileName();
	var xlxs = file + ".xlsx";
	File.Move(file, xlxs);	
	
	using (SpreadsheetDocument document = SpreadsheetDocument.Create(xlxs, SpreadsheetDocumentType.Workbook))
	{
		WorkbookPart workbookPart = document.AddWorkbookPart();
		workbookPart.Workbook = new Workbook();

		WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
		worksheetPart.Worksheet = new Worksheet();

		Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

		Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Employees" };

		sheets.Append(sheet);

		workbookPart.Workbook.Save();

		var dependencies = Entity.GetData();

		SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

		// Constructing header
		Row row = new Row();

		row.Append(
			ConstructCell("Id", CellValues.String),
			ConstructCell("Name", CellValues.String),
			ConstructCell("Versions", CellValues.Number),
			ConstructCell("Assets", CellValues.Number));

		// Insert the header row to the Sheet Data
		sheetData.AppendChild(row);

		// Inserting each employee
		foreach (var dependency in dependencies)
		{
			row = new Row();

			row.Append(
				ConstructCell(dependency.Id.ToString(), CellValues.String),
				ConstructCell(dependency.Name, CellValues.String),
				ConstructCell(dependency.Versions.Count.ToString(), CellValues.Number),
				ConstructCell(dependency.Assets.Count.ToString(), CellValues.Number)
				);

			sheetData.AppendChild(row);
		}

		worksheetPart.Worksheet.Save();
	}

	Util.Cmd(@$"explorer {xlxs}");
}

private Cell ConstructCell(string value, CellValues dataType)
{
	return new Cell()
	{
		CellValue = new CellValue(value),
		DataType = new EnumValue<CellValues>(dataType)
	};
}

public class Entity 
{
	public string Id { get; set; }
	public string Name { get; set; }
	public List<string> Versions { get; set; }
	public List<string> Assets { get; set; }
	
	
	public static IEnumerable<Entity> GetData()
	{
		yield return new Entity { Id = "1", Name = "Name 1", Versions = new[] {"1.0.0", "1.0.1", "1.0.2" }.ToList(), Assets = new[] { "Asset 1", "Asset 2", "Asset 3"}.ToList()  };
		yield return new Entity { Id = "2", Name = "Name 2", Versions = new[] {"1.0.2", "1.2.1", "2.0.2" }.ToList(), Assets = new[] { "Asset 2", "Asset 1", "Asset 4"}.ToList()  };
		
	}
}