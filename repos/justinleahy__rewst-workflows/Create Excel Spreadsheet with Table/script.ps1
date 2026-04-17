function ConvertTo-ExcelColumn {
    param (
        [int]$Number
    )

    $result = ""
    while ($Number -gt 0) {
        $remainder = ($Number - 1) % 26  # Get the remainder when divided by 26
        $result = [char]([int][char]'A' + $remainder) + $result  # Convert to the corresponding character
        $Number = [math]::Floor(($Number - 1) / 26)  # Reduce the number by 1 and divide by 26
    }

    return $result
}

$Raw_Data = '{{ CTX.validated_csv_data }}'
$Headers = '{{ CTX.headers}}'.Split(",")
$Table_Name = '{{ CTX.table_name }}'
$File_Name = '{{ CTX.file_name }}.xlsx'
$File_Location = '{{ CTX.file_location }}' + "\$File_Name"

$Data = $Raw_Data | ConvertFrom-Csv
$Excel_Column_Number = ($Data[0] | Get-Member -MemberType Properties).Count
$Excel_Column = ConvertTo-ExcelColumn -Number $Excel_Column_Number
$Excel_Range = "A1:" + $Excel_Column + ($Data.Length + 1)

# Reference 2
$Required_Paths = @("C:\Windows\SysWOW64\config\systemprofile\Desktop", "C:\Windows\System32\config\systemprofile\Desktop")

foreach($Path in $Required_Paths) {
    if((Test-Path -Path $Path) -eq $false) {
        New-Item -Path $Path -ItemType Directory -Force > $null
    }
}

# Reference 1
$Excel = New-Object -ComObject Excel.Application
$Excel.Visible = $false
$Excel.DisplayAlerts = $false
$Workbook = $Excel.Workbooks.Add(1)
$Worksheet = $Workbook.Worksheets.Item(1)
$Worksheet.Name = $Table_Name

$Table = $Worksheet.ListObjects.Add(
    [Microsoft.Office.Interop.Excel.XlListObjectSourceType]::xlSrcRange,
    $worksheet.Range($Excel_Range),
    $null,
    [Microsoft.Office.Interop.Excel.XlYesNoGuess]::xlYes
)
$Table.Name = $Table_Name

# Create Headers
for ($index = 1; $index -le $Headers.Count; $index++) {
    $Header = $Headers[$index - 1]
    $Table.ListColumns.Item($index).Name = $Header
}

# Insert Data into Table
for ($rowIndex = 2; $rowIndex -lt ($Data.Length + 2); $rowIndex++) {
    for ($colIndex = 1; $colIndex -lt ($Headers.Length + 1); $colIndex++) {
        $Header = $Headers[$colIndex - 1]
        $Information = $Data[$rowIndex - 2].$Header
        $Worksheet.Cells.Item($rowIndex, $colIndex) = $Information
    }
}

$Worksheet.Columns.AutoFit() > $null
$Worksheet.SaveAs($File_Location)
$File_Base64 = [convert]::ToBase64String((Get-Content -Path $File_Location -Encoding Byte))
$Excel.Quit()
return $File_Base64