# Create Excel Spreadsheet with Table

This is used to insert data into an excel spreadsheet and have a table already present in the worksheet. Before we'd send CSV files out via email but for external use that isn't the best format.

## Inputs

`headers` : Comma separated headers of the csv_data object.

`csv_data` : Data stored in CSV format with headers

`file_name` : Name of the file being created on disk. Do not include the xlsx file extension.

`table_name` : Name of the table and worksheet.

## Outputs

`base64_string` : Excel spreadsheet created, stored in base64 encoding.

## Installation

1. Download `workflow.json`
2. When viewing Automations > Workflows, click the "Import Bundle" button to the right of the "Create" button in the top right hand corner.
3. Select the `workflow.json` file you downloaded in Step 1.
4. Select "Submit"
5. Use your preferred method of running the powershell script on a machine. In our environment, we use ConnectWise Control with Datto as a backup.

## Helpful Information

- At the moment use single quotes for strings in the csv. Double quotes will break the script 

## References

[1] <https://techexpert.tips/powershell/powershell-creating-excel-file/>

[2] <https://www.developpez.net/forums/d1334550/environnements-developpement/windev/ole-dynamique-pilotage-excel-service-windows/>
