using System;

namespace ArSendOutFrm.DB
{
    public class SqlList
    {
        //根据SQLID返回对应的SQL语句  
        private string _result;

        /// <summary>
        /// 查询窗体使用
        /// </summary>
        /// <param name="orderno">应收单单据编号</param>
        /// <param name="sdt">开始单据日期</param>
        /// <param name="edt">结束单据日期</param>
        /// <returns></returns>
        public string Get_Search(string orderno, DateTime sdt, DateTime edt)
        {
            if (orderno == "")
            {
                _result = $@"
                            SELECT a.FBILLNO 单据编号,a.FDATE 单据日期,b.FNUMBER 客户编码,c.FNAME 客户名称,'KG' 单位,F_YTC_PRINTTIMES 打印次数
                            FROM dbo.T_AR_RECEIVABLE a
                            INNER JOIN dbo.T_BD_CUSTOMER b ON a.FCUSTOMERID=b.FCUSTID
                            INNER JOIN dbo.T_BD_CUSTOMER_L c ON b.FCUSTID=c.FCUSTID
                            WHERE (a.FDATE>='{sdt}')   --起始单据日期
                            AND (a.FDATE<='{edt}')     --结束单据日期
                            order by a.fdate
                       ";
            }
            else
            {
                _result = $@"
                            SELECT a.FBILLNO 单据编号,a.FDATE 单据日期,b.FNUMBER 客户编码,c.FNAME 客户名称,'KG' 单位,F_YTC_PRINTTIMES 打印次数
                            FROM dbo.T_AR_RECEIVABLE a
                            INNER JOIN dbo.T_BD_CUSTOMER b ON a.FCUSTOMERID=b.FCUSTID
                            INNER JOIN dbo.T_BD_CUSTOMER_L c ON b.FCUSTID=c.FCUSTID
                            WHERE (a.FBILLNO like '%{orderno}%')  --'AR00087553'
                            AND (a.FDATE>='{sdt}')   --起始单据日期
                            AND (a.FDATE<='{edt}')     --结束单据日期
                            order by a.fdate
                       ";
            }

            return _result;
        }

        /// <summary>
        /// 明细记录
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public string Get_ShowDtl(string orderno)
        {
            _result =$@"SELECT '雅图高新材料有限公司'供方,'' 需方,
                       '' 序号,'' 采购订单号,
	                   c.FNUMBER 物料编码,d.FNAME 物料描述,'KG' 单位,ROUND(ROUND(b.FPRICEQTY*b.F_YTC_DECIMAL12,2),0) 送货数量,
	                   '' 是否有附件物料及清单,'' 是否附带检验报告单,'' 车型,'hahaha' 备注,
	                   '温颖贤' 制单人,'温颖贤' 发货人
                FROM dbo.T_AR_RECEIVABLE a
                INNER JOIN dbo.T_AR_RECEIVABLEENTRY b ON a.FID=b.FID
                INNER JOIN dbo.T_BD_MATERIAL c ON b.FMATERIALID=c.FMATERIALID
                INNER JOIN dbo.T_BD_MATERIAL_L d ON c.FMATERIALID=d.FMATERIALID
                WHERE a.FBILLNO='{orderno}'  --'AR00087553'
                AND d.FLOCALEID='2052'";
            return _result;
        }
    }
}
