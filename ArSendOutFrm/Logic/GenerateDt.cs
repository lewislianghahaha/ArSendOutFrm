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
        /// <param name="dt">GridView当前行记录</param>
        /// <param name="sourcedt">所有行记录</param>
        /// <param name="value">‘需方’文本框值</param>
        /// <returns></returns>
        public DataTable Margedt(DataTable dt, DataTable sourcedt,string value)
        {
            try
            {
                //不判断是否有修改,只要在DT内存在就以对应的I行ID为条件进行重新插入
                for (var i = 0; i < sourcedt.Rows.Count; i++)
                {
                    var rows = dt.Select("序号='" + Convert.ToInt32(sourcedt.Rows[i][2]) + "'");

                    if (rows.Length > 0)
                    {
                        sourcedt.Rows[i].BeginEdit();
                        for (var j = 0; j < sourcedt.Columns.Count; j++)
                        {
                            sourcedt.Rows[i][j] = j == 1 ? value : rows[0][j];
                        }
                        sourcedt.Rows[i].EndEdit();
                    }
                }
            }
            catch (Exception)
            {
                sourcedt.Rows.Clear();
                sourcedt.Columns.Clear();
            }
            return sourcedt;
        }

        /// <summary>
        /// 运算-填充DT
        /// </summary>
        /// <param name="columnid"></param>
        /// <param name="value"></param>
        /// <param name="sourcedt"></param>
        /// <returns></returns>
        public DataTable FillDt(int columnid, string value, DataTable sourcedt)
        {
            try
            {
                //循环对指定列进行填充
                for (var i = 0; i < sourcedt.Rows.Count; i++)
                {
                    for (var j = 0; j < sourcedt.Columns.Count; j++)
                    {
                        sourcedt.Rows[i].BeginEdit();
                        if (j != columnid)continue;
                        else
                        {
                            sourcedt.Rows[i][j] = value;
                        }
                        sourcedt.Rows[i].EndEdit();
                    }
                }

            }
            catch (Exception)
            {
                sourcedt.Rows.Clear();
                sourcedt.Columns.Clear();
            }
            return sourcedt;
        }
    }
}
