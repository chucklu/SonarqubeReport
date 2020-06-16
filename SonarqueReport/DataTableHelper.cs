using System.Data;

namespace SonarqueReport
{
    public class DataTableHelper
    {

        internal static string columnId = "Id";
        internal static string columnComponent = "Component";
        internal static string columnComponentCount = "ComponentCount";
        internal static string columnComplexity = "Complexity";

        public static DataTable CreateDataTable()
        {
            DataColumn column;
            DataTable table = new DataTable();

            // Create new DataColumn, set DataType,
            // ColumnName and add to DataTable.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = columnId;
            column.ReadOnly = true;
            column.Unique = true;
            // Add the Column to the DataColumnCollection.
            table.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = columnComponent;
            column.AutoIncrement = false;
            column.ReadOnly = false;
            column.Unique = false;
            // Add the column to the table.
            table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = columnComponentCount;
            column.ReadOnly = false;
            column.Unique = false;
            // Add the Column to the DataColumnCollection.
            table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = columnComplexity;
            column.ReadOnly = false;
            column.Unique = false;
            // Add the Column to the DataColumnCollection.
            table.Columns.Add(column);

            return table;
        }
    }
}
