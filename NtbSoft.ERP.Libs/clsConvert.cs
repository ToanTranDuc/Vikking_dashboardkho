using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace NtbSoft.ERP.Libs
{
    public class clsConvert<T> where T: class, new()
    {
        public DataTable ToDataTable(List<T> items) 
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pro in props)
            {
                var type = (pro.PropertyType.IsGenericType && pro.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)? Nullable.GetUnderlyingType(pro.PropertyType):pro.PropertyType);
                dataTable.Columns.Add(pro.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);

            }
            return dataTable;
        }

        public List<T> ToList(DataTable datatable) 
        {
            List<T> Temp = new List<T>();
            try
            {
                List<string> columnsNames = new List<string>();
                foreach (DataColumn DataColumn in datatable.Columns)
                    columnsNames.Add(DataColumn.ColumnName);
                Temp = datatable.AsEnumerable().ToList().ConvertAll<T>(row => getObject<T>(row, columnsNames));
                return Temp;
            }
            catch
            {
                return Temp;
            }

        }
        public T getObject<T>(DataRow row, List<string> columnsName) where T : new()
        {
            T obj = new T();
            try
            {
                string columnname = "";
                string value = "";
                PropertyInfo[] Properties;
                Properties = typeof(T).GetProperties();
                foreach (PropertyInfo objProperty in Properties)
                {
                    columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower());
                    if (!string.IsNullOrEmpty(columnname))
                    {
                        value = row[columnname].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (Nullable.GetUnderlyingType(objProperty.PropertyType) != null)
                            {
                                value = row[columnname].ToString().Replace("$", "").Replace(",", "");
                                //objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null);
                                objProperty.SetValue(obj, row[objProperty.Name], null);
                            }
                            else
                            {
                                value = row[columnname].ToString().Replace("%", "");
                                //objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);
                                objProperty.SetValue(obj, row[objProperty.Name], null);
                            }
                        }
                    }
                }
                return obj;
            }
            catch
            {
                return obj;
            }
        }
        //public List<T> ToList(DataTable dt)
        //{
        //    const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        //    var columnNames = dt.Columns.Cast<DataColumn>()
        //        .Select(c => c.ColumnName)
        //        .ToList();
        //    var objectProperties = typeof(T).GetProperties(flags);
        //    var targetList = dt.AsEnumerable().Select(dataRow =>
        //    {
        //        var instanceOfT = Activator.CreateInstance<T>();

        //        foreach (var properties in objectProperties.Where(properties => columnNames.Contains(properties.Name) && dataRow[properties.Name] != DBNull.Value))
        //        {
        //            properties.SetValue(instanceOfT, dataRow[properties.Name], null);
        //        }
        //        return instanceOfT;
        //    }).ToList();

        //    return targetList;
        //}

    }
}
