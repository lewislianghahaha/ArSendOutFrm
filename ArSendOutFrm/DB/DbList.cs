using System;
using System.Data;

namespace ArSendOutFrm.DB
{
    public class DbList
    {
        /// <summary>
        ///  导出临时表
        /// </summary>
        /// <returns></returns>
        public DataTable Get_FinalEmptydt()
        {
            var dt = new DataTable();
            for (var i = 0; i < 16; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    case 0:
                        dc.ColumnName = "供方";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                    case 1:
                        dc.ColumnName = "需方";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                    case 2:
                        dc.ColumnName = "序号";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    case 3:
                        dc.ColumnName = "采购订单号";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 4:
                        dc.ColumnName = "物料编码";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 5:
                        dc.ColumnName = "物料描述";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                    case 6:
                        dc.ColumnName = "单位";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 7:
                        dc.ColumnName = "送货数量";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    case 8:
                        dc.ColumnName = "实收数量";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    case 9:
                        dc.ColumnName = "采购负责人";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 10:
                        dc.ColumnName = "是否有附件物料及清单";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 11:
                        dc.ColumnName = "是否有附带检验报告单";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 12:
                        dc.ColumnName = "车型";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 13:
                        dc.ColumnName = "备注";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 14:
                        dc.ColumnName = "制单人";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 15:
                        dc.ColumnName = "发货人";
                        dc.DataType = Type.GetType("System.String");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }
    }
}
