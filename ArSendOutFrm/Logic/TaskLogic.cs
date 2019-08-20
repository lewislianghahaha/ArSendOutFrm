using System;
using System.Data;

namespace ArSendOutFrm.Logic
{
    public class TaskLogic
    {
        SearchDt searchDt=new SearchDt();
        GenerateDt generateDt=new GenerateDt();

        #region 变量定义
        private int _taskid;             //记录中转ID
        private DateTime _sdt;           //获取起始单据日期
        private DateTime _edt;           //获取结束单据日期
        private string _orderno;         //获取应收单据编号
        private int _fid;                //获取单据编号FID值
        private DataTable _dt;           //获取明细行记录
        private string _value;           //获取从文本框填写的值

        private DataTable _resultTable;  //返回DT类型
        private bool _resultMark;        //返回是否成功标记

        #endregion

        #region Set

            /// <summary>
            /// 中转ID
            /// </summary>
            public int TaskId { set { _taskid = value; } }

            /// <summary>
            /// 获取起始单据日期
            /// </summary>
            public DateTime Sdt { set { _sdt = value; } }

            /// <summary>
            /// 获取结束单据日期
            /// </summary>
            public DateTime Edt { set { _edt = value; } }

            /// <summary>
            /// 获取应收单据编号
            /// </summary>
            public string Orderno { set { _orderno = value; } }

            /// <summary>
            /// 获取单据编号FID值
            /// </summary>
            public int Fid { set { _fid = value; } }
            /// <summary>
            /// 获取明细行记录
            /// </summary>
            public DataTable Data { set { _dt = value; } }

            /// <summary>
            /// 获取从文本框填写的值
            /// </summary>
            public string Value { set { _value = value; } }

        #endregion

        #region Get
        /// <summary>
        ///返回DataTable至主窗体
        /// </summary>
        public DataTable ResultTable => _resultTable;

            /// <summary>
            /// 返回结果标记
            /// </summary>
            public bool ResultMark => _resultMark;
        #endregion

        public void StartTask()
        {
            switch (_taskid)
            {
                //查询(主窗体使用)
                case 0:
                    Seardt(_orderno,_sdt,_edt);
                    break;
                case 1:
                    Searchdtdtl(_fid);
                    break;
                //运算
                case 2:
                    Generatedt(_dt);
                    break;
                //运算-合并数据
                case 3:
                    Margedt(_dt,_value);
                    break;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        private void Seardt(string orderno,DateTime sdt,DateTime edt)
        {
            _resultTable = searchDt.Searchdt(orderno,sdt,edt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fid"></param>
        private void Searchdtdtl(int fid)
        {
            _resultTable = searchDt.Searchdtldt(fid);
        }

        /// <summary>
        /// 运算
        /// </summary>
        private void Generatedt(DataTable dt)
        {
            _resultTable = generateDt.Generatedt(dt);
        }

        /// <summary>
        /// 运算-合并数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="value"></param>
        private void Margedt(DataTable dt,string value)
        {
            _resultTable = generateDt.Margedt(dt,value);
        }
    }
}
