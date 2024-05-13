using System.Reflection;
using System.Text;

using PLC;
using Camera;
using SystemConfig;
using Simulator;
using DL_Logger;
using ErrorCodeDefine;
using ShareResource;


namespace GA6_ControlApp
{
    public partial class frmMain : Form
    {
        /// <summary>
        /// ���N���X��
        /// </summary>
        private const string THIS_NAME = "frmMain";

        /// <summary>
        /// PLC�V�~�����[�^�[���
        /// </summary>
        private frmPlcSimulator _PlcSimulator = null;
        /// <summary>
        /// PLC�ǂݏo���X���b�h
        /// </summary>
        private PlcThread _plcThread = null;

        /// <summary>
        /// PLC���W���[��
        /// </summary>
        private PlcManager _PLC = null;
        /// <summary>
        /// �J�������W���[��
        /// </summary>
        private Camera.CameraManager[] _Camera = null;

        /// <summary>
        /// ��Ə�񃊃X�g
        /// ���IDx100�� ��Ǝw�����X�g30�� ����Ȃ�N���X���X�g
        /// </summary>
        public WorkInfo[] WorkInfoList = new WorkInfo[IniFile.MaxWorkIdCount];

        /// <summary>
        /// �������t���O
        /// </summary>
        public bool _init = true;

        /// <summary>
        /// �O��J�����ʐM���O����
        /// </summary>
        private int _preCameraLogCount = 0;



        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
        }
        /// <summary>
        /// �t�H�[�����[�h
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // Ini�t�@�C��
                String fileName = System.IO.Path.Combine(Const.IniDir, Const.IniFileName);
                rc = IniFile.Load(fileName);

                // Logger
                rc = Logger.Open(IniFile.LogDir, Const.Title, Const.LogMaxFileCount, Const.LogMaxLineCount);
                if (STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.INFO, "Logger�N��");
                else
                    Logger.WriteLog(LogType.ERROR, "Logger�N���G���[");



                // PLC
                _PLC = new PlcManager();
                rc = _PLC.Open(IniFile.PlcConnectionString);

                // �J����
                _Camera = new CameraManager[IniFile.CameraCount];
                for (int i = 0; i < IniFile.CameraCount; i++)
                {
                    _Camera[i] = new CameraManager(IniFile.CameraIpAddress[i], IniFile.PCPort[i]);
                    rc = _Camera[i].Open(IniFile.PCIpAddress, IniFile.PCPort[i]);
                }

                // PLC�ǂݏo���X���b�h
                _plcThread = new PlcThread(_PLC, _Camera, WorkInfoList);
                _plcThread.Start();

                // ��Ə�񃊃X�g
                for (int i = 0; i < IniFile.MaxWorkIdCount; i++)
                    WorkInfoList[i] = new WorkInfo();

                if (IniFile.MaintenanceMode || IniFile.PlcSimulatorMode)
                {
                    this.WindowState = FormWindowState.Normal;
                }
                if (IniFile.PlcSimulatorMode)
                {
                    _PlcSimulator = new frmPlcSimulator(_PLC);
                    _PlcSimulator.Show();
                }


                // �J�����ʐM���OListView������
                listViewLog.Columns.Add("����", 150);
                listViewLog.Columns.Add("IP�A�h���X", 150);
                listViewLog.Columns.Add("�|�[�g�ԍ�", 150);
                listViewLog.Columns.Add("���", 80);
                listViewLog.Columns.Add("�R�}���h", 600);

                //// �f�[�^���擾
                //DateTime dt = DateTime.Now;
                ////List<CameraLog> people = new List<CameraLog>();
                //Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), "���M", "�N��"));
                //Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), "��M", "�N��"));
                //Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), "��M", "�N��"));
                //dt = DateTime.Now;
                //Resource.CameraLogList.Insert(0, new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), "��M", "AAA"));



                //Debug();
                //rc = _plcThread.CreateWorkInfo();
                //for (int i = 0; i < IniFile.MaxWorkIdCount; i++)
                //{
                //    // PLC�֏�����
                //    _PLC.SetWorkInfo(i, _plcThread._workInfoList[i]);
                //}

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }

            if (!STATUS_SUCCESS(rc))
            {

                return;
            }
        }
        /// <summary>
        /// �t�H�[���N���[�Y
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // PLC
                _PLC.Close();

                // �J����
                _Camera.Clone();

                // PLC�ǂݏo���X���b�h
                _plcThread.Close();
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// ��ʍX�V�^�C�}
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrUpdateDisplay_Tick(object sender, EventArgs e)
        {
            UpdateDisplay();
        }
        /// <summary>
        /// ��ʍX�V
        /// </summary>
        private void UpdateDisplay()
        {
            UInt32 rc = 0;
            try
            {
                if (_init)
                {
                    _init = false;
                }


                // �J�����ʐM���O����ʕ\��
                if (_preCameraLogCount < Resource.CameraLogList.Count)
                {
                    // �J�����ʐM���O�ő匏��
                    if (Resource.CameraLogList.Count > IniFile.MaxCameraLogCount)
                        Resource.CameraLogList.RemoveAt(Resource.CameraLogList.Count - 1);

                    // ListView�Ƀf�[�^���o�C���h
                    listViewLog.Items.Clear();
                    foreach (CameraLog log in Resource.CameraLogList)
                    {
                        ListViewItem item = new ListViewItem(log.Dt);
                        item.SubItems.Add(log.Ip);
                        item.SubItems.Add(log.Port.ToString());
                        item.SubItems.Add(log.Type);
                        item.SubItems.Add(log.Command);
                        listViewLog.Items.Add(item);
                    }

                    _preCameraLogCount = Resource.CameraLogList.Count;
                }


            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
        }


        ///// <summary>
        ///// ���Item���X�g����Ə��ɕϊ�
        ///// </summary>
        ///// <returns></returns>
        //private UInt32 CreateWorkInfo()
        //{
        //    UInt32 rc = 0;
        //    try
        //    {

        //        for (int id = 0; id < IniFile.MaxWorkIdCount; id++)
        //        {
        //            // ----------------------------------------------
        //            // ���ID
        //            if (idList01.Count < id + 1 && idList02.Count < id + 1)
        //                break;

        //            WorkID id01 = _Camera[0].workIdList[id];
        //            WorkID id02 = _Camera[1].workIdList[id];
        //            //WorkID id01 = idList01[id];
        //            //WorkID id02 = idList02[id];
        //            if (id01.name == id02.name)
        //            {
        //                WorkInfoList[id].workIdName = id01.name;
        //            }


        //            for (int order = 0; order < IniFile.MaxWorkOrderCount; order++)
        //            {
        //                // ----------------------------------------------
        //                // ��Ǝw�����X�g
        //                if (id01.workOrderList.Count < order + 1 && id01.workOrderList.Count < order + 1)
        //                    break;

        //                WorkOrder order01 = id01.workOrderList[order];
        //                WorkOrder order02 = id02.workOrderList[order];
        //                int angle01 = -1;
        //                int angle02 = -1;
        //                string partName01 = "";
        //                string partName02 = "";

        //                // ��Ǝw�����X�g����"�p�x"��"���i��"�ɕ���    
        //                string[] array01 = order01.name.Split('_');
        //                angle01 = int.Parse(array01[0]);
        //                partName01 = array01[1];
        //                string[] array02 = order02.name.Split('_');
        //                angle02 = int.Parse(array02[0]);
        //                partName02 = array02[1];
        //                if (angle01 == angle02)
        //                {
        //                    WorkInfoList[id].AngleList[order].angle = angle01;
        //                    WorkInfoList[id].AngleList[order].camera[0].partName = partName01;
        //                    WorkInfoList[id].AngleList[order].camera[1].partName = partName02;
        //                    WorkInfoList[id].AngleList[order].camera[0].workOrderName = order01.name;
        //                    WorkInfoList[id].AngleList[order].camera[1].workOrderName = order02.name;
        //                }
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
        //        rc = (UInt32)ErrorCodeList.EXCEPTION;
        //    }
        //    return rc;
        //}



        /// <summary>
        /// Check Error State
        /// </summary>
        private static bool STATUS_SUCCESS(UInt32 err) { return err == (int)ErrorCodeList.STATUS_SUCCESS; }





        #region Debug
        //private List<WorkID> idList01 = new List<WorkID>();
        //private List<WorkID> idList02 = new List<WorkID>();
        /// <summary>
        /// ��M�f�[�^������ID�������
        /// </summary>
        /// <returns></returns>
        private UInt32 Debug()
        {
            UInt32 rc = 0;
            try
            {


                string id01 = "11111-22222AAAAABBBBB";
                string id02 = "22222-22222CCCCCDDDDD";
                string id03 = "33333-22222EEEEEFFFFF";
                string id04 = "44444-22222GGGGGHHHHH";
                string id05 = "55555-22222IIIIIJJJJJ";
                string order0101 = "5-XXXXXXXXXX";
                string order0102 = "30-YYYYYYY";
                string order0103 = "90-ZZZZZ";
                string order0104 = "120-ABCABCABC";
                string order0105 = "220-GUNNYAM";

                WorkID workId0101 = new WorkID();
                workId0101.name = id01;
                workId0101.workOrderList.Add(new WorkOrder() { name = order0101 });
                workId0101.workOrderList.Add(new WorkOrder() { name = order0102 });
                workId0101.workOrderList.Add(new WorkOrder() { name = order0103 });
                workId0101.workOrderList.Add(new WorkOrder() { name = order0104 });
                workId0101.workOrderList.Add(new WorkOrder() { name = order0105 });
                workId0101.workOrderList[0].workItemList.Add(new WorkItem() { name = "1" });
                workId0101.workOrderList[1].workItemList.Add(new WorkItem() { name = "1" });
                workId0101.workOrderList[2].workItemList.Add(new WorkItem() { name = "1" });
                workId0101.workOrderList[3].workItemList.Add(new WorkItem() { name = "1" });
                workId0101.workOrderList[4].workItemList.Add(new WorkItem() { name = "1" });
                //idList01.Add(workId0101);
                _Camera[0].workIdList.Add(workId0101);

                WorkID workId0102 = new WorkID();
                workId0102.name = id02;
                workId0102.workOrderList.Add(new WorkOrder() { name = order0101 });
                workId0102.workOrderList.Add(new WorkOrder() { name = order0102 });
                workId0102.workOrderList.Add(new WorkOrder() { name = order0103 });
                workId0102.workOrderList.Add(new WorkOrder() { name = order0104 });
                workId0102.workOrderList.Add(new WorkOrder() { name = order0105 });
                workId0102.workOrderList[0].workItemList.Add(new WorkItem() { name = "1" });
                workId0102.workOrderList[1].workItemList.Add(new WorkItem() { name = "1" });
                workId0102.workOrderList[2].workItemList.Add(new WorkItem() { name = "1" });
                workId0102.workOrderList[3].workItemList.Add(new WorkItem() { name = "1" });
                workId0102.workOrderList[4].workItemList.Add(new WorkItem() { name = "1" });
                //idList01.Add(workId0102);
                _Camera[0].workIdList.Add(workId0102);

                WorkID workId0103 = new WorkID();
                workId0103.name = id03;
                workId0103.workOrderList.Add(new WorkOrder() { name = order0101 });
                workId0103.workOrderList.Add(new WorkOrder() { name = order0102 });
                workId0103.workOrderList.Add(new WorkOrder() { name = order0103 });
                workId0103.workOrderList.Add(new WorkOrder() { name = order0104 });
                workId0103.workOrderList.Add(new WorkOrder() { name = order0105 });
                workId0103.workOrderList[0].workItemList.Add(new WorkItem() { name = "1" });
                workId0103.workOrderList[1].workItemList.Add(new WorkItem() { name = "1" });
                workId0103.workOrderList[2].workItemList.Add(new WorkItem() { name = "1" });
                workId0103.workOrderList[3].workItemList.Add(new WorkItem() { name = "1" });
                workId0103.workOrderList[4].workItemList.Add(new WorkItem() { name = "1" });
                //idList01.Add(workId0103);
                _Camera[0].workIdList.Add(workId0103);

                WorkID workId0104 = new WorkID();
                workId0104.name = id04;
                workId0104.workOrderList.Add(new WorkOrder() { name = order0101 });
                workId0104.workOrderList.Add(new WorkOrder() { name = order0102 });
                workId0104.workOrderList.Add(new WorkOrder() { name = order0103 });
                workId0104.workOrderList.Add(new WorkOrder() { name = order0104 });
                workId0104.workOrderList.Add(new WorkOrder() { name = order0105 });
                workId0104.workOrderList[0].workItemList.Add(new WorkItem() { name = "1" });
                workId0104.workOrderList[1].workItemList.Add(new WorkItem() { name = "1" });
                workId0104.workOrderList[2].workItemList.Add(new WorkItem() { name = "1" });
                workId0104.workOrderList[3].workItemList.Add(new WorkItem() { name = "1" });
                workId0104.workOrderList[4].workItemList.Add(new WorkItem() { name = "1" });
                //idList01.Add(workId0104);
                _Camera[0].workIdList.Add(workId0104);

                WorkID workId0105 = new WorkID();
                workId0105.name = id05;
                workId0105.workOrderList.Add(new WorkOrder() { name = order0101 });
                workId0105.workOrderList.Add(new WorkOrder() { name = order0102 });
                workId0105.workOrderList.Add(new WorkOrder() { name = order0103 });
                workId0105.workOrderList.Add(new WorkOrder() { name = order0104 });
                workId0105.workOrderList.Add(new WorkOrder() { name = order0105 });
                workId0105.workOrderList[0].workItemList.Add(new WorkItem() { name = "1" });
                workId0105.workOrderList[1].workItemList.Add(new WorkItem() { name = "1" });
                workId0105.workOrderList[2].workItemList.Add(new WorkItem() { name = "1" });
                workId0105.workOrderList[3].workItemList.Add(new WorkItem() { name = "1" });
                workId0105.workOrderList[4].workItemList.Add(new WorkItem() { name = "1" });
                //idList01.Add(workId0105);
                _Camera[0].workIdList.Add(workId0105);



                string order0201 = "5-AAAA";
                string order0202 = "30-BBBBB";
                string order0203 = "90-ZCCC";
                string order0204 = "120-QQQQ";
                string order0205 = "220-w";

                WorkID workId0201 = new WorkID();
                workId0201.name = id01;
                workId0201.workOrderList.Add(new WorkOrder() { name = order0201 });
                workId0201.workOrderList.Add(new WorkOrder() { name = order0202 });
                workId0201.workOrderList.Add(new WorkOrder() { name = order0203 });
                workId0201.workOrderList.Add(new WorkOrder() { name = order0204 });
                workId0201.workOrderList.Add(new WorkOrder() { name = order0205 });
                workId0201.workOrderList[0].workItemList.Add(new WorkItem() { name = "1" });
                workId0201.workOrderList[1].workItemList.Add(new WorkItem() { name = "1" });
                workId0201.workOrderList[2].workItemList.Add(new WorkItem() { name = "1" });
                workId0201.workOrderList[3].workItemList.Add(new WorkItem() { name = "1" });
                workId0201.workOrderList[4].workItemList.Add(new WorkItem() { name = "1" });
                //idList02.Add(workId0201);
                _Camera[1].workIdList.Add(workId0201);

                WorkID workId0202 = new WorkID();
                workId0202.name = id02;
                workId0202.workOrderList.Add(new WorkOrder() { name = order0201 });
                workId0202.workOrderList.Add(new WorkOrder() { name = order0202 });
                workId0202.workOrderList.Add(new WorkOrder() { name = order0203 });
                workId0202.workOrderList.Add(new WorkOrder() { name = order0204 });
                workId0202.workOrderList.Add(new WorkOrder() { name = order0205 });
                workId0202.workOrderList[0].workItemList.Add(new WorkItem() { name = "1" });
                workId0202.workOrderList[1].workItemList.Add(new WorkItem() { name = "1" });
                workId0202.workOrderList[2].workItemList.Add(new WorkItem() { name = "1" });
                workId0202.workOrderList[3].workItemList.Add(new WorkItem() { name = "1" });
                workId0202.workOrderList[4].workItemList.Add(new WorkItem() { name = "1" });
                //idList02.Add(workId0202);
                _Camera[1].workIdList.Add(workId0202);

                WorkID workId0203 = new WorkID();
                workId0203.name = id03;
                workId0203.workOrderList.Add(new WorkOrder() { name = order0201 });
                workId0203.workOrderList.Add(new WorkOrder() { name = order0202 });
                workId0203.workOrderList.Add(new WorkOrder() { name = order0203 });
                workId0203.workOrderList.Add(new WorkOrder() { name = order0204 });
                workId0203.workOrderList.Add(new WorkOrder() { name = order0205 });
                workId0203.workOrderList[0].workItemList.Add(new WorkItem() { name = "1" });
                workId0203.workOrderList[1].workItemList.Add(new WorkItem() { name = "1" });
                workId0203.workOrderList[2].workItemList.Add(new WorkItem() { name = "1" });
                workId0203.workOrderList[3].workItemList.Add(new WorkItem() { name = "1" });
                workId0203.workOrderList[4].workItemList.Add(new WorkItem() { name = "1" });
                //idList02.Add(workId0203);
                _Camera[1].workIdList.Add(workId0203);

                WorkID workId0204 = new WorkID();
                workId0204.name = id04;
                workId0204.workOrderList.Add(new WorkOrder() { name = order0201 });
                workId0204.workOrderList.Add(new WorkOrder() { name = order0202 });
                workId0204.workOrderList.Add(new WorkOrder() { name = order0203 });
                workId0204.workOrderList.Add(new WorkOrder() { name = order0204 });
                workId0204.workOrderList.Add(new WorkOrder() { name = order0205 });
                workId0204.workOrderList[0].workItemList.Add(new WorkItem() { name = "1" });
                workId0204.workOrderList[1].workItemList.Add(new WorkItem() { name = "1" });
                workId0204.workOrderList[2].workItemList.Add(new WorkItem() { name = "1" });
                workId0204.workOrderList[3].workItemList.Add(new WorkItem() { name = "1" });
                workId0204.workOrderList[4].workItemList.Add(new WorkItem() { name = "1" });
                //idList02.Add(workId0204);
                _Camera[1].workIdList.Add(workId0204);

                WorkID workId0205 = new WorkID();
                workId0205.name = id05;
                workId0205.workOrderList.Add(new WorkOrder() { name = order0201 });
                workId0205.workOrderList.Add(new WorkOrder() { name = order0202 });
                workId0205.workOrderList.Add(new WorkOrder() { name = order0203 });
                workId0205.workOrderList.Add(new WorkOrder() { name = order0204 });
                workId0205.workOrderList.Add(new WorkOrder() { name = order0205 });
                workId0205.workOrderList[0].workItemList.Add(new WorkItem() { name = "1" });
                workId0205.workOrderList[1].workItemList.Add(new WorkItem() { name = "1" });
                workId0205.workOrderList[2].workItemList.Add(new WorkItem() { name = "1" });
                workId0205.workOrderList[3].workItemList.Add(new WorkItem() { name = "1" });
                workId0205.workOrderList[4].workItemList.Add(new WorkItem() { name = "1" });
                //idList02.Add(workId0205);
                _Camera[1].workIdList.Add(workId0205);


            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        #endregion





    }



}