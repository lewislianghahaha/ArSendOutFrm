using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ArSendOutFrm.Logic;
using Mergedt;
using Stimulsoft.Report;

namespace ArSendOutFrm
{
    public partial class ShowDetailFrm : Form
    {
        TaskLogic task=new TaskLogic();
        Load load=new Load();

        //获取传过来的FID值
        private int _fid;

        //保存运算过来的DT
        public DataTable _generatedt;

        //保存查询出来的GridView记录
        private DataTable _dtl;
        //保存整理出来的GridView记录
        private DataTable _dtdtl;
        //记录当前页数(GridView页面跳转使用)
        private int _pageCurrent = 1;
        //记录计算出来的总页数(GridView页面跳转使用)
        private int _totalpagecount;
        //记录初始化标记(GridView页面跳转 初始化时使用)
        private bool _pageChange;

        #region Set
            /// <summary>
            /// 获取传过来的FID值
            /// </summary>
            public int Fid { set { _fid = value; } }
        #endregion

        public ShowDetailFrm()
        {
            InitializeComponent();
            OnRegisterEvents();
        }

        private void OnRegisterEvents()
        {
            tmprint.Click += Tmprint_Click;
            tmfill.Click += Tmfill_Click;

            bnMoveFirstItem.Click += BnMoveFirstItem_Click;
            bnMovePreviousItem.Click += BnMovePreviousItem_Click;
            bnMoveNextItem.Click += BnMoveNextItem_Click;
            bnMoveLastItem.Click += BnMoveLastItem_Click;
            bnPositionItem.TextChanged += BnPositionItem_TextChanged;
            tmshowrows.DropDownClosed += Tmshowrows_DropDownClosed;
            panel2.Visible = false;
        }

        /// <summary>
        /// 初始化窗体信息
        /// </summary>
        public void OnInitialize()
        {
            task.TaskId = 1;
            task.Fid = _fid;

            task.StartTask();
            _generatedt = Generatedt(task.ResultTable);
            //连接GridView页面跳转功能
            LinkGridViewPageChange(_generatedt);
            //供方
            txtfor.Text = Convert.ToString(_generatedt.Rows[0][0]);  
            //控制GridView单元格显示方式
            ControlGridViewisShow();
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmprint_Click(object sender, EventArgs e)
        {
            try
            {
                if(_generatedt.Rows.Count==0) throw new Exception("没有内容,不能进行打印");
                //将新增数据合并形成为新的DT进行导出(注:先检测行状态,若有"Modified" 或'Add'即将行更新)
                _generatedt = Margedt((DataTable) gvdtl.DataSource, _generatedt, txtsale.Text);

                var filepath = Application.StartupPath + $"/Report/SendOutReport.mrt";
                var stireport = new StiReport();
                stireport.Load(filepath);
                //加载DATASET 或 DATATABLE
                stireport.RegData("Order", _generatedt);
                stireport.Compile();
                stireport.Show();   //调用预览功能
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 填充明细行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmfill_Click(object sender, EventArgs e)
        {
            var colname = string.Empty;

            try
            {
                //获取当前选中的项的索引
                var currentColumnIndex=gvdtl.CurrentCell.ColumnIndex;
                //异常提示
                if(currentColumnIndex!=3 && currentColumnIndex !=4 && currentColumnIndex != 5
                    && currentColumnIndex !=9 && currentColumnIndex !=10 && currentColumnIndex !=11 &&
                    currentColumnIndex!=12 && currentColumnIndex !=13) throw new Exception("请选择能修改的列进行填充");

                if(Convert.ToString(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[currentColumnIndex].Value)=="") throw new Exception("请输入值再进行填充");

                switch (currentColumnIndex)
                {
                    case 3:
                        colname = "采购订单号";
                        break;
                    case 4:
                        colname = "物料编码";
                        break;
                    case 5:
                        colname = "物料描述";
                        break;
                    case 9:
                        colname = "采购负责人";
                        break;
                    case 10:
                        colname = "是否有附件物料及清单";
                        break;
                    case 11:
                        colname = "是否有附带检验报告单";
                        break;
                    case 12:
                        colname = "车型";
                        break;
                    case 13:
                        colname = "备注";
                        break;
                }

                var clickMessage = $"您所需要填充的列为:'{colname}' \n 是否继续?";

                if (MessageBox.Show(clickMessage, $"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    task.TaskId = 4;
                    task.Value = Convert.ToString(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[currentColumnIndex].Value);  //需要填充的值
                    task.Fid = currentColumnIndex;  //需要填充的列ID
                    task.Sourcedt = _generatedt;    //所有明细DT

                    Start();

                    if(!task.ResultMark) throw new Exception("填充出现问题,请联系管理员");
                    else
                    {
                        MessageBox.Show($"已成功填充,请继续操作", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //刷新GridView
                        _generatedt = task.ResultTable;
                        GridViewPageChange(2);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///子线程使用(重:用于监视功能调用情况,当完成时进行关闭LoadForm)
        /// </summary>
        private void Start()
        {
            task.StartTask();

            //当完成后将Form2子窗体关闭
            this.Invoke((ThreadStart)(() =>
            {
                load.Close();
            }));
        }

        /// <summary>
        /// 首页按钮(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveFirstItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将当前页变量PageCurrent=1; 2)并将“首页” 及 “上一页”按钮设置为不可用 将“下一页” “末页”按设置为可用
                _pageCurrent = 1;
                bnMoveFirstItem.Enabled = false;
                bnMovePreviousItem.Enabled = false;

                bnMoveNextItem.Enabled = true;
                bnMoveLastItem.Enabled = true;
                GridViewPageChange(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 上一页(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMovePreviousItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将PageCurrent自减 2)将“下一页” “末页”按钮设置为可用
                _pageCurrent--;
                bnMoveNextItem.Enabled = true;
                bnMoveLastItem.Enabled = true;
                //判断若PageCurrent=1的话,就将“首页” “上一页”按钮设置为不可用
                if (_pageCurrent == 1)
                {
                    bnMoveFirstItem.Enabled = false;
                    bnMovePreviousItem.Enabled = false;
                }
                GridViewPageChange(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 下一页按钮(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveNextItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将PageCurrent自增 2)将“首页” “上一页”按钮设置为可用
                _pageCurrent++;
                bnMoveFirstItem.Enabled = true;
                bnMovePreviousItem.Enabled = true;
                //判断若PageCurrent与“总页数”一致的话,就将“下一页” “末页”按钮设置为不可用
                if (_pageCurrent == _totalpagecount)
                {
                    bnMoveNextItem.Enabled = false;
                    bnMoveLastItem.Enabled = false;
                }
                GridViewPageChange(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 末页按钮(GridView页面跳转使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveLastItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将“总页数”赋值给PageCurrent 2)将“下一页” “末页”按钮设置为不可用 并将 “上一页” “首页”按钮设置为可用
                _pageCurrent = _totalpagecount;
                bnMoveNextItem.Enabled = false;
                bnMoveLastItem.Enabled = false;

                bnMovePreviousItem.Enabled = true;
                bnMoveFirstItem.Enabled = true;
                GridViewPageChange(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 跳转页文本框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnPositionItem_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //判断所输入的跳转数必须为整数
                if (!Regex.IsMatch(bnPositionItem.Text, @"^-?[1-9]\d*$|^0$")) throw new Exception("请输入整数再继续");
                //判断所输入的跳转数不能大于总页数
                if (Convert.ToInt32(bnPositionItem.Text) > _totalpagecount) throw new Exception("所输入的页数不能超出总页数,请修改后继续");
                //判断若所填跳转数为0时跳出异常
                if (Convert.ToInt32(bnPositionItem.Text) == 0) throw new Exception("请输入大于0的整数再继续");

                //将所填的跳转页赋值至“当前页”变量内
                _pageCurrent = Convert.ToInt32(bnPositionItem.Text);
                //根据所输入的页数动态控制四个方向键是否可用
                //若为第1页，就将“首页” “上一页”按钮设置为不可用 将“下一页” “末页”设置为可用
                if (_pageCurrent == 1)
                {
                    bnMoveFirstItem.Enabled = false;
                    bnMovePreviousItem.Enabled = false;

                    bnMoveNextItem.Enabled = true;
                    bnMoveLastItem.Enabled = true;
                }
                //若为末页,就将"下一页" “末页”按钮设置为不可用 将“上一页” “首页”设置为可用
                else if (_pageCurrent == _totalpagecount)
                {
                    bnMoveNextItem.Enabled = false;
                    bnMoveLastItem.Enabled = false;

                    bnMovePreviousItem.Enabled = true;
                    bnMoveFirstItem.Enabled = true;
                }
                //否则四个按钮都可用
                else
                {
                    bnMoveFirstItem.Enabled = true;
                    bnMovePreviousItem.Enabled = true;
                    bnMoveNextItem.Enabled = true;
                    bnMoveLastItem.Enabled = true;
                }
                if(gvdtl.RowCount>0 && _generatedt.Rows.Count>0)
                    GridViewPageChange(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bnPositionItem.Text = Convert.ToString(_pageCurrent);
            }
        }

        /// <summary>
        /// 每页显示行数 下拉框关闭时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmshowrows_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                //每次选择新的“每页显示行数”，都要 1)将_pageChange标记设为true(即执行初始化方法) 2)将“当前页”初始化为1
                _pageChange = true;
                _pageCurrent = 1;
                GridViewPageChange(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// GridView分页功能
        /// </summary>
        /// <param name="id">0:初始化使用 1:转页或打印使用 2:填充列时使用</param>
        private void GridViewPageChange(int id)
        {
            try
            {
                //按实际情况整理DT
                switch (id)
                {
                    case 0:
                        _dtdtl = _dtl.Copy();
                        break;
                    case 1:
                        _dtdtl = Margedt((DataTable)gvdtl.DataSource, _generatedt, txtsale.Text).Copy();
                        break;
                    case 2:
                        _dtdtl = _generatedt.Copy();
                        break;
                }

                //获取查询的总行数
                var dtltotalrows = _dtdtl.Rows.Count;
                //获取“每页显示行数”所选择的行数
                var pageCount = Convert.ToInt32(tmshowrows.SelectedItem);
                //计算出总页数
                _totalpagecount = dtltotalrows % pageCount == 0 ? dtltotalrows / pageCount : dtltotalrows / pageCount + 1;
                //赋值"总页数"项
                bnCountItem.Text = $"/ {_totalpagecount} 页";

                //初始化BindingNavigator控件内的各子控件 及 对应初始化信息
                if (_pageChange)
                {
                    bnPositionItem.Text = Convert.ToString(1);                       //初始化填充跳转页为1
                    tmshowrows.Enabled = true;                                      //每页显示行数（下拉框）  

                    //初始化时判断;若“总页数”=1，四个按钮不可用；若>1,“下一页” “末页”按钮可用
                    if (_totalpagecount == 1)
                    {
                        bnMoveNextItem.Enabled = false;
                        bnMoveLastItem.Enabled = false;
                        bnMoveNextItem.Enabled = false;
                        bnMoveLastItem.Enabled = false;
                        bnPositionItem.Enabled = false;                             //跳转页文本框
                    }
                    else
                    {
                        bnMoveNextItem.Enabled = true;
                        bnMoveLastItem.Enabled = true;
                        bnPositionItem.Enabled = true;                             //跳转页文本框
                    }
                    _pageChange = false;
                }

                //显示_dtl的查询总行数
                tstotalrow.Text = $"共 {_dtdtl.Rows.Count} 行";

                //根据“当前页” 及 “固定行数” 计算出新的行数记录并进行赋值
                //计算进行循环的起始行
                var startrow = (_pageCurrent - 1) * pageCount;
                //计算进行循环的结束行
                var endrow = _pageCurrent == _totalpagecount ? dtltotalrows : _pageCurrent * pageCount;
                //复制 查询的DT的列信息（不包括行）至临时表内
                var tempdt = _dtdtl.Clone();
                //循环将所需的_dtl的行记录复制至临时表内
                for (var i = startrow; i < endrow; i++)
                {
                    tempdt.ImportRow(_dtdtl.Rows[i]);
                }

                //最后将刷新的DT重新赋值给GridView
                gvdtl.DataSource = tempdt;
                //将“当前页”赋值给"跳转页"文本框内
                bnPositionItem.Text = Convert.ToString(_pageCurrent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 控制GridView单元格显示方式
        /// </summary>
        private void ControlGridViewisShow()
        {
            //注:当没有值时,若还设置某一行Row不显示的话,就会出现异常
            gvdtl.Columns[0].Visible = false;
            gvdtl.Columns[1].Visible = false;
            gvdtl.Columns[8].Visible = false;
            gvdtl.Columns[14].Visible = false;
            gvdtl.Columns[15].Visible = false;

            //设置指定列不能编辑
            gvdtl.Columns[2].ReadOnly = true;    //序号
            gvdtl.Columns[6].ReadOnly = true;   //单位
            gvdtl.Columns[7].ReadOnly = true;  //送货数量
        }

        /// <summary>
        /// 根据获取的明细行DT进行运算-得出供报表使用的DT
        /// </summary>
        /// <returns></returns>
        private DataTable Generatedt(DataTable dt)
        {
            task.TaskId = 2;
            task.Data = dt;
            task.StartTask();
            return task.ResultTable;
        }

        /// <summary>
        /// 合并DT
        /// </summary>
        /// <param name="gvdt">GridView当前行记录DT</param>
        /// <param name="sourcedt">所有明细记录DT</param>
        /// <param name="value">'需方'文件框值</param>
        /// <returns></returns>
        private DataTable Margedt(DataTable gvdt, DataTable sourcedt, string value)
        {

            task.TaskId = 3;
            task.Data = gvdt;          //GridView当前行记录DT
            task.Sourcedt = sourcedt;  //所有明细记录DT
            task.Value = value;        //'需方'文件框值
            task.StartTask();
            return task.ResultTable;
        }

        /// <summary>
        /// 连接GridView页面跳转功能
        /// </summary>
        /// <param name="dt"></param>
        private void LinkGridViewPageChange(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                _dtl = dt;
                panel2.Visible = true;
                //初始化下拉框所选择的默认值
                tmshowrows.SelectedItem = "10";
                //定义初始化标记
                _pageChange = true;
                //GridView分页
                GridViewPageChange(0);
            }
            //注:当为空记录时,不显示跳转页;只需将临时表赋值至GridView内
            else
            {
                gvdtl.DataSource = dt;
                panel2.Visible = false;
            }
        }

    }
}
