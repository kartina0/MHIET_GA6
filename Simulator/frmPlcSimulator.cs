using System.Reflection;
using System.Diagnostics;

using PLC;
using DL_CommonLibrary;
using DL_Logger;
using SystemConfig;
using ErrorCodeDefine;
using System;


namespace Simulator
{
    /// <summary>
    /// PLC�V�~�����[�^�[���
    /// </summary>
    public partial class frmPlcSimulator : Form
    {
        /// <summary>
        /// ���N���X��
        /// </summary>
        private const string THIS_NAME = "frmMain";

        /// <summary>
        /// PLC�V���W���[��
        /// </summary>
        private PlcManager _plc = null;

        /// <summary>
        /// PLC�T�C�N���X���b�h
        /// </summary>
        private ThreadInfo _thread = new ThreadInfo(THREAD_SEQUENCE_TYPE.CONTINUOUS);
        /// <summary>
        /// �X���b�h �V���b�g�_�E��
        /// </summary>
        private bool _shutDown = false;
        /// <summary>
        /// �X���b�h �V���b�g�_�E������
        /// </summary>
        private bool _shutDownComp = false;

        /// <summary>
        /// ���ID�C���f�b�N�X
        /// </summary>
        private int _workIdIndex = 0;
        /// <summary>
        /// �p�x�C���f�b�N�X
        /// </summary>
        private int _angleIndex = 0;

        /// <summary>
        /// �����O�������[�h
        /// </summary>
        private bool _longRunMode = false;



        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public frmPlcSimulator(PlcManager plc)
        {
            InitializeComponent();

            // PLC���W���[��
            _plc = plc;
        }
        /// <summary>
        /// �t�H�[�����[�h
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmPlcSimulator_Load(object sender, EventArgs e)
        {
            UInt32 rc = 0;
            try
            {
                // �e���ID���[�U�[�R���g���[����������
                for (int i = 0; i < 3; i++)
                {
                    string ctrlName = $"ucWorkInfo{i + 1}";
                    object ctrl = WindowFunc.FindControl(this, ctrlName);
                    if (ctrl != null)
                    {
                        ((ucWorkInfo)ctrl).SetIndex(i);
                        ((ucWorkInfo)ctrl).SetPlc(_plc);
                    }
                }

                // �R���{ ���ID�C���f�b�N�X
                for (int i = 0; i < IniFile.MaxWorkIdCount; i++)
                    comboWorkId.Items.Add(i);
                comboWorkId.SelectedIndex = 0;
                // �R���{ �p�x�C���f�b�N�X
                for (int i = 0; i < IniFile.MaxWorkOrderCount; i++)
                    comboAngle.Items.Add(i);
                comboAngle.SelectedIndex = 0;
                //// �R���{ ���Item�����ʒm��������
                //comboWorkItemResponse.Items.Add("�ʏ퓮��");
                //comboWorkItemResponse.Items.Add("�Ď��{");
                //comboWorkItemResponse.Items.Add("�����I��");
                //comboWorkItemResponse.SelectedIndex = 0;


                // �X���b�h�J�n
                ThreadStart();

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
        }
        /// <summary>
        /// �t�H�[���N���[�Y
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmPlcSimulator_FormClosing(object sender, FormClosingEventArgs e)
        {
            UInt32 rc = 0;
            try
            {
                // PLC�N���[�Y
                _plc = null;

                ThreadClose();
                _thread = null;

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
        }
        /// <summary>
        /// �{�^���N���b�N �C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, EventArgs e)
        {
            UInt32 rc = 0;
            Button ctrl = (Button)sender;
            try
            {
                if (ctrl == btnStart)
                {// �����v��

                    // ���ID�C���f�N�X
                    _workIdIndex = comboWorkId.SelectedIndex;
                    rc = _plc.Debug_SetSelectedIdIndex(comboWorkId.SelectedIndex);
                    // �p�x�C���f�b�N�X
                    _angleIndex = comboAngle.SelectedIndex;
                    rc = _plc.Debug_SetSelectedAngleIndex(comboAngle.SelectedIndex);
                    // �������ڐݒ�v�� ON
                    rc = _plc.Debug_SetItemReq(true);

                    // �����v�� ON
                    rc = _plc.Debug_SetExamReq(true);
                }
                else if (ctrl == btnCancel)
                {// �������f�v��
                    rc = _plc.Debug_SetCencelReq(true);
                }
                else if (ctrl == btnUpdate)
                {// �@����擾�v��
                    rc = _plc.Debug_SetUpdateReq(true);
                }
                else if (ctrl == btnStartLong)
                {// �����v��(�����O����)

                    _longRunMode = true;


                    // ���ID�C���f�N�X
                    _workIdIndex = comboWorkId.SelectedIndex;
                    rc = _plc.Debug_SetSelectedIdIndex(comboWorkId.SelectedIndex);
                    // �p�x�C���f�b�N�X
                    _angleIndex = comboAngle.SelectedIndex;
                    rc = _plc.Debug_SetSelectedAngleIndex(comboAngle.SelectedIndex);
                    // �������ڐݒ�v�� ON
                    rc = _plc.Debug_SetItemReq(true);

                    // �����v�� ON
                    rc = _plc.Debug_SetExamReq(true);

                }
                else if (ctrl == btnErrorClear)
                {// �G���[�N���A

                    // �G���[�R�[�h �N���A
                    rc = _plc.SetErrorCode(0);
                }
                //else if (ctrl == btnResponse)
                //{// ���Item�����ʒm��������
                //    rc = _plc.Debug_SetResponse();
                //}
                else if (ctrl == btnClose)
                {// ����
                    this.Close();
                }


            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
        }

        /// <summary>
        /// �R���{�{�b�N�X���ID�C���f�b�N�X �ύX�C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboWorkId_SelectedIndexChanged(object sender, EventArgs e)
        {
            UInt32 rc = 0;
            try
            {
                rc = _plc.Debug_SetSelectedIdIndex(comboWorkId.SelectedIndex);
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
        }
        /// <summary>
        /// �R���{�{�b�N�X�p�x�C���f�b�N�X �ύX�C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboAngle_SelectedIndexChanged(object sender, EventArgs e)
        {
            UInt32 rc = 0;
            try
            {
                rc = _plc.Debug_SetSelectedAngleIndex(comboAngle.SelectedIndex);
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
        }
        /// <summary>
        /// �R���{�{�b�N�X���Item�����ʒm�����C���f�b�N�X �ύX�C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboWorkItemResponse_SelectedIndexChanged(object sender, EventArgs e)
        {
            UInt32 rc = 0;
            try
            {
                //rc = _plc.Debug_SetWorkItemResponseIndex(comboWorkItemResponse.SelectedIndex);
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
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

            string ctrlName = "";
            object ctrl = null;

            try
            {
                // ------------------------------------------
                // PLC�X�e�[�^�X �ǂݏo��
                // ------------------------------------------

                // PC -> PLC
                rc = _plc.Debug_GetStatus(out Out status_out);

                if (status_out.InitComp)
                    lblInitComp.BackColor = Color.Lime;
                else
                    lblInitComp.BackColor = Color.Silver;

                if (status_out.StartComp)
                    lblStartComp.BackColor = Color.Lime;
                else
                    lblStartComp.BackColor = Color.Silver;

                if (status_out.UpdateComp)
                    lblUpdateComp.BackColor = Color.Lime;
                else
                    lblUpdateComp.BackColor = Color.Silver;

                if (status_out.ItemInputComp)
                    lblSetItemComp.BackColor = Color.Lime;
                else
                    lblSetItemComp.BackColor = Color.Silver;

                for (int i = 0; i < IniFile.CameraCount; i++)
                {
                    if (status_out.OutputResultComp[i])
                    {
                        ctrlName = $"lblOutputResult0{i + 1}";
                        ctrl = WindowFunc.FindControl(this, ctrlName);
                        ((Label)ctrl).BackColor = Color.Lime;

                        ctrlName = $"lblResult0{i + 1}";
                        ctrl = WindowFunc.FindControl(this, ctrlName);
                        ((Label)ctrl).BackColor = Color.Lime;

                        if (status_out.Result[i]) ((Label)ctrl).Text = "OK";
                        else ((Label)ctrl).Text = "NG";
                    }
                    else
                    {
                        ctrlName = $"lblOutputResult0{i + 1}";
                        ctrl = WindowFunc.FindControl(this, ctrlName);
                        ((Label)ctrl).BackColor = Color.Silver;

                        ctrlName = $"lblResult0{i + 1}";
                        ctrl = WindowFunc.FindControl(this, ctrlName);
                        ((Label)ctrl).BackColor = Color.Silver;
                    }
                }


                // PLC -> PC
                rc = _plc.GetStatus(out In status_in);

                if (status_in.ExamReq)
                    lblStartReq.BackColor = Color.Lime;
                else
                    lblStartReq.BackColor = Color.Silver;

                if (status_in.CancelReq)
                    lblCancelReq.BackColor = Color.Lime;
                else
                    lblCancelReq.BackColor = Color.Silver;

                if (status_in.UpdateReq)
                    lblUpdateReq.BackColor = Color.Lime;
                else
                    lblUpdateReq.BackColor = Color.Silver;

                if (status_in.ItemInputReq)
                {
                    lblSetItemReq.BackColor = Color.Lime;
                    lblWorkIdIndex.BackColor = Color.Lime;
                    lblAngleIndex.BackColor = Color.Lime;
                }
                else
                {
                    lblSetItemReq.BackColor = Color.Silver;
                    lblWorkIdIndex.BackColor = Color.Silver;
                    lblAngleIndex.BackColor = Color.Silver;
                }

                if (status_in.ItemInputReq) lblWorkIdIndex.Text = status_in.WorkIdIndex.ToString();
                else lblAngleIndex.Text = status_in.AngleIndex.ToString();




                //comboWorkId.SelectedIndex = _workIdIndex;
                //comboAngle.SelectedIndex = _angleIndex;

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
        }





        /// <summary>
        /// �N��
        /// </summary>
        /// <returns></returns>
        public UInt32 ThreadStart()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // �X���b�h�N��
                _thread.CreateThread(Thread_Cycle, _thread, ThreadPriority.Lowest);
                _thread.Interval = 30;
                _thread.Release();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, ex.ToString());
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// �I��
        /// </summary>
        public UInt32 ThreadClose()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // ----------------------------------
                // �X���b�h�I��
                // ----------------------------------
                if (_thread != null)
                {
                    _shutDown = true;
                    while (!_shutDownComp)
                        Thread.Sleep(100);
                }
                _thread = null;
                Logger.WriteLog(LogType.INFO, string.Format("�T�C�N���Ǘ��X���b�h �I��"));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, ex.ToString());
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// PLC�T�C�N���X���b�h
        /// </summary>
        private void Thread_Cycle(object arg)
        {
            UInt32 rc = 0;
            ThreadInfo info = (ThreadInfo)arg;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                bool exit = false;
                Stopwatch sw = Stopwatch.StartNew();

                // �O��̌������ڐݒ�v��
                bool pre_ItemInputReq = false;
                // �O��̌������ʏo�͗v��
                bool pre_OutputResultReq = false;

                while (true)
                {
                    rc = 0;
                    if (_shutDown)
                        break;

                    // Waits Any Event
                    THREAD_WAIT_RESULT index = info.WaitAnyEvent(100);

                    if (index == THREAD_WAIT_RESULT.SHUTDOWN)
                    {
                        exit = true;
                    }
                    if (index == THREAD_WAIT_RESULT.REQUEST)
                    {

                        // PLC�X�e�[�^�X �ǂݏo��
                        rc = _plc.Debug_GetStatus(out Out status_Out);
                        rc = _plc.GetStatus(out In status_In);

                        // PLC �@���� �ǂݏo��
                        rc = _plc.Debug_GetWorkInfo(_workIdIndex, out WorkInfo workInfo);



                        // -----------------------------------------
                        // �G���[�R�[�h����
                        // -----------------------------------------
                        if (status_Out.ErrorCode == 1)
                        {// ���� START�v���G���[
                            // �����v�� OFF
                            rc = _plc.Debug_SetExamReq(false);
                        }


                        // -----------------------------------------
                        // �J�����ʐM��������������
                        // -----------------------------------------
                        if (status_Out.InitComp)
                        {// �J�����ʐM���������� ON

                            // �@����擾�v�� ON
                            rc = _plc.Debug_SetUpdateReq(true);
                            // �J�����ʐM���������� OFF
                            rc = _plc.SetInitComp(false);
                        }

                        // -----------------------------------------
                        // �@����擾��������
                        // -----------------------------------------
                        if (status_Out.UpdateComp)
                        {// �@����擾���� ON

                            // �@����擾�v�� OFF
                            rc = _plc.Debug_SetUpdateReq(false);
                            // �@����擾���� OFF
                            rc = _plc.SetUpdateComp(false);
                        }


                        // -----------------------------------------
                        // ��������
                        // -----------------------------------------
                        if (!_longRunMode)
                        {// �ʏ탂�[�h



                            if (status_In.ExamReq)
                            {// �����v�� ON

                                // ----------------------------------------------
                                // �������ڐݒ�
                                if (status_Out.ItemInputComp)
                                {// �������ڐݒ芮�� ON

                                    // �������ڐݒ�v�� OFF
                                    rc = _plc.Debug_SetItemReq(false);
                                    // �������ڐݒ芮�� OFF
                                    rc = _plc.SetItemComp(false);
                                }
                                if (!pre_ItemInputReq && status_In.ItemInputReq)
                                {// �������ڐݒ�v����ON�ɂȂ���

                                    // �p�x�C���f�b�N�X����
                                    rc = _plc.Debug_SetSelectedAngleIndex(_angleIndex);

                                }
                                if (pre_ItemInputReq && !status_In.ItemInputReq)
                                {// �������ڐݒ�v����OFF�ɂȂ���

                                    // �������ʏo�͗v�� ON
                                    rc = _plc.Debug_SetOutPutResultReq(true);
                                }

                                if (status_In.ItemInputReq)
                                    pre_ItemInputReq = true;
                                else
                                    pre_ItemInputReq = false;


                                // ----------------------------------------------
                                // �������ʏo��
                                if (status_Out.OutputResultComp.All(x => x))
                                {// �������ʏo�͊��� ON

                                    // �������ʏo�͗v�� OFF
                                    rc = _plc.Debug_SetOutPutResultReq(false);

                                    for (int i = 0; i < IniFile.CameraCount; i++)
                                    {// �N���A
                                        // �������ʏo�͊��� OFF
                                        rc = _plc.SetOutputResultComp(i, false);
                                    }

                                    if (!status_Out.Result.All(x => x))
                                    {// ��������NG

                                        // �����v�� OFF
                                        rc = _plc.Debug_SetExamReq(false);
                                    }
                                }
                                if (pre_OutputResultReq && !status_In.OutputResultReq)
                                {// �������ʏo�͗v����OFF�ɂȂ���

                                    if (_angleIndex >= 30 || workInfo.AngleList[_angleIndex + 1].angle == -1)
                                    {// �����I��

                                        // �����v�� OFF
                                        rc = _plc.Debug_SetExamReq(false);
                                    }
                                    else
                                    {
                                        // ���̊p�x
                                        _angleIndex++;
                                        Logger.WriteLog(LogType.INFO, "++");

                                        // �������ڐݒ�v�� ON
                                        rc = _plc.Debug_SetItemReq(true);
                                    }
                                }

                                if (status_In.OutputResultReq)
                                    pre_OutputResultReq = true;
                                else
                                    pre_OutputResultReq = false;
                            }
                            else
                            {// �����v�� OFF

                                // �������f�v�� OFF
                                rc = _plc.Debug_SetCencelReq(false);
                                // �������ڐݒ�v�� OFF
                                rc = _plc.Debug_SetItemReq(false);
                                // �������ʏo�͗v�� OFF
                                rc = _plc.Debug_SetOutPutResultReq(false);


                            }
                        }
                        else
                        {// �����O�������[�h

                            if (status_In.ExamReq)
                            {// �����v�� ON

                                // ----------------------------------------------
                                // �������ڐݒ�
                                if (status_Out.ItemInputComp)
                                {// �������ڐݒ芮�� ON

                                    // �������ڐݒ�v�� OFF
                                    rc = _plc.Debug_SetItemReq(false);
                                    // �������ڐݒ芮�� OFF
                                    rc = _plc.SetItemComp(false);
                                }
                                if (!pre_ItemInputReq && status_In.ItemInputReq)
                                {// �������ڐݒ�v����ON�ɂȂ���

                                    // �p�x�C���f�b�N�X����
                                    rc = _plc.Debug_SetSelectedAngleIndex(_angleIndex);

                                }
                                if (pre_ItemInputReq && !status_In.ItemInputReq)
                                {// �������ڐݒ�v����OFF�ɂȂ���

                                    // �������ʏo�͗v�� ON
                                    rc = _plc.Debug_SetOutPutResultReq(true);
                                }

                                if (status_In.ItemInputReq)
                                    pre_ItemInputReq = true;
                                else
                                    pre_ItemInputReq = false;


                                // ----------------------------------------------
                                // �������ʏo��
                                if (status_Out.OutputResultComp.All(x => x))
                                {// �������ʏo�͊��� ON

                                    // �������ʏo�͗v�� OFF
                                    rc = _plc.Debug_SetOutPutResultReq(false);

                                    for (int i = 0; i < IniFile.CameraCount; i++)
                                    {// �N���A

                                        // �������ʏo�͊��� OFF
                                        rc = _plc.SetOutputResultComp(i, false);
                                    }
                                }
                                if (pre_OutputResultReq && !status_In.OutputResultReq)
                                {// �������ʏo�͗v����OFF�ɂȂ���

                                    if (_angleIndex >= 30 || workInfo.AngleList[_angleIndex + 1].angle == -1)
                                    {// �����I��
                                        rc = _plc.Debug_SetExamReq(false);
                                    }
                                    else
                                    {
                                        // ���̊p�x
                                        _angleIndex++;
                                        Logger.WriteLog(LogType.INFO, "++");

                                        // �������ڐݒ�v�� ON
                                        rc = _plc.Debug_SetItemReq(true);
                                    }
                                }

                                if (status_In.OutputResultReq)
                                    pre_OutputResultReq = true;
                                else
                                    pre_OutputResultReq = false;
                            }
                            else
                            {// �����v�� OFF

                                // ���ID�C���f�N�X
                                _workIdIndex++;
                                rc = _plc.Debug_SetSelectedIdIndex(comboWorkId.SelectedIndex);
                                // �p�x�C���f�b�N�X
                                _angleIndex = 0;
                                rc = _plc.Debug_SetSelectedAngleIndex(comboAngle.SelectedIndex);
                                // �������ڐݒ�v�� ON
                                rc = _plc.Debug_SetItemReq(true);

                                // �����v�� ON
                                rc = _plc.Debug_SetExamReq(true);
                            }


                        }




                    }
                    if (exit) break;
                    Thread.Sleep(info.Interval);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, ex.ToString());
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            _shutDownComp = true;
        }
    }

}