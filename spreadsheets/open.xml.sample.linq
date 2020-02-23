<Query Kind="Program">
  <Connection>
    <ID>4fa71e94-9eb6-4451-91c5-e0c9bb666ff0</ID>
    <Persist>true</Persist>
    <Server>localhost</Server>
    <EncryptTraffic>true</EncryptTraffic>
    <Database>Vision</Database>
    <ShowServer>true</ShowServer>
    <DriverData>
      <SkipCertificateCheck>true</SkipCertificateCheck>
    </DriverData>
  </Connection>
  <NuGetReference>DocumentFormat.OpenXml</NuGetReference>
  <Namespace>DocumentFormat.OpenXml</Namespace>
  <Namespace>DocumentFormat.OpenXml.Packaging</Namespace>
  <Namespace>DocumentFormat.OpenXml.Spreadsheet</Namespace>
</Query>

void Main()
{
	using (SpreadsheetDocument document = SpreadsheetDocument.Create("C:\\temp\\dependencies.xlsx", SpreadsheetDocumentType.Workbook))
	{
		WorkbookPart workbookPart = document.AddWorkbookPart();
		workbookPart.Workbook = new Workbook();

		WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
		worksheetPart.Worksheet = new Worksheet();

		Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

		Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Employees" };

		sheets.Append(sheet);

		workbookPart.Workbook.Save();

		var dependencies = Dependencies.ToList();

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
				ConstructCell(dependency.DependencyVersions.Count.ToString(), CellValues.Number),
				ConstructCell(dependency.DependencyAssetDependencies.Count.ToString(), CellValues.Number)
				);

			sheetData.AppendChild(row);
		}

		worksheetPart.Worksheet.Save();
	}
}

private Cell ConstructCell(string value, CellValues dataType)
{
	return new Cell()
	{
		CellValue = new CellValue(value),
		DataType = new EnumValue<CellValues>(dataType)
	};
}