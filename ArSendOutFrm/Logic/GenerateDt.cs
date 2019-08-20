using System;
using System.Data;
using ArSendOutFrm.DB;

namespace ArSendOutFrm.Logic
{
    public class GenerateDt
    {
        DbList dbList=new DbList();

        /// <summary>
        /// 运算
        /// </summary>
        /// <param name="sourcedt">明细记录DT</param>
        /// <returns></returns>
        public DataTable Generatedt(DataTable sourcedt)
        {
            var resultdt=new DataTable();
            var id = 1;

            try
            {
                //获取导出临时表
                resultdt = dbList.Get_FinalEmptydt();
                //循环将对应数据获取
                foreach (DataRow rows in sourcedt.Rows)
                {
                    var newrow = resultdt.NewRow();
                    newrow[0] = rows[0];                //供方
                    newrow[2] = id;                     //序号
                    newrow[4] = rows[4];                //物料编码
                    newrow[5] = rows[5];                //物料描述
                    newrow[6] = rows[6];                //单位
                    newrow[7] = rows[7];                //送货数量
                    newrow[14] = rows[12];              //制单人
                    newrow[15] = rows[13];              //发货人
                    id++;
                    resultdt.Rows.Add(newrow);
                }
            }
            catch (Exception)
            {
                resultdt.Rows.Clear();
                resultdt.Columns.Clear();
            }
            return resultdt;
        }

        /// <summary>
        /// 合并DT
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DataTable Margedt(DataTable dt, string value)
        {
            try
            {
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i].BeginEdit();
                    dt.Rows[i][1] = value;
                    dt.Rows[i].EndEdit();
                }
            }
            catch (Exception)
            {
                dt.Rows.Clear();
                dt.Columns.Clear();
            }
            return dt;
        }

    }
}
