using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Utils
{
    public class KeyDownControlHandler
    {
        private List<ActionControl> lstAction;

        public KeyDownControlHandler(List<ActionControl> _lstAction)
        {
            this.lstAction = _lstAction;
        }

        public void PressKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                    // Tổ hợp phím Ctrl + Shift + N
                case Keys.N:
                    ActionControl actionControlKeyN = lstAction.FirstOrDefault(
                        x => x.permission && x.actionType.Equals(ActionType.Add)
                        && e.Control && e.Shift && x.Enabled);
                    if (actionControlKeyN != null) actionControlKeyN.action();
                    break;

                    // Tổ hợp phím Ctrl + Shift + M
                case Keys.M:
                    ActionControl actionControlKeyM = lstAction.FirstOrDefault(
                        x => x.permission && x.actionType.Equals(ActionType.AddPOPCB)
                        && e.Control && e.Shift && x.Enabled);
                    if (actionControlKeyM != null) actionControlKeyM.action();
                    break;

                    // Tổ hợp phím Ctrl + E
                case Keys.E:
                    ActionControl actionControlKeyE = lstAction.FirstOrDefault(
                        x => x.permission && x.actionType.Equals(ActionType.Edit)
                        && e.Control && x.Enabled);
                    if (actionControlKeyE != null) actionControlKeyE.action();
                    break;

                    // Phím Delete
                case Keys.Delete:
                    ActionControl actionControlKeyDelete = lstAction.FirstOrDefault(
                        x => x.permission && x.actionType == ActionType.Delete &&
                        x.Enabled);
                    if (actionControlKeyDelete != null) actionControlKeyDelete.action();
                    break;

                    // Tổ hợp phím Ctrl + S
                case Keys.S:
                    ActionControl actionControlKeyS = lstAction.FirstOrDefault(
                        x => x.permission && x.actionType == ActionType.Save &&
                        e.Control && x.Enabled);
                    if (actionControlKeyS != null) actionControlKeyS.action();
                    break;

                    // Phím F5
                case Keys.F5:
                    ActionControl actionControlKeyF5 = lstAction.FirstOrDefault(
                        x => x.permission && x.actionType == ActionType.Refresh);
                    if (actionControlKeyF5 != null) actionControlKeyF5.action();
                    break;

                    // Tổ hợp phím Ctrl + P
                case Keys.P:

                    // Tổ hợp phím Ctrl + Shift + P
                    if (e.Shift) {
                        ActionControl actionControlKeyShiftP = lstAction.FirstOrDefault(
                            x => x.permission && x.actionType.Equals(ActionType.ExcelSelect)
                            && e.Control && e.Shift);
                        if (actionControlKeyShiftP != null) actionControlKeyShiftP.action();
                        break;
                    }
                    else
                    {
                        // Tổ hợp phím Ctrl + P
                        ActionControl actionControlKeyEx = lstAction.FirstOrDefault(
                         x => x.permission && x.actionType == ActionType.ExCel
                            && e.Control);
                        if (actionControlKeyEx != null) actionControlKeyEx.action();
                    }

                    break;

                    // Tổ hợp phím Ctrl + G
                case Keys.G:
                    ActionControl actionControlKeyG = lstAction.FirstOrDefault(
                        x => x.permission && x.actionType == ActionType.GopThung
                        && e.Control);
                    if (actionControlKeyG != null) actionControlKeyG.action();
                    break;

                    // Tổ hợp phím Ctrl + L
                case Keys.L:
                    ActionControl actionControlKeyL = lstAction.FirstOrDefault(
                        x => x.permission && x.actionType == ActionType.LapKH
                        && e.Control);
                    if (actionControlKeyL != null) actionControlKeyL.action();
                    break;

                // Tổ hợp phím F6
                case Keys.F6:
                    ActionControl actionControlKeyF6 = lstAction.FirstOrDefault(
                        x => x.permission && x.actionType == ActionType.History
                        && x.Enabled);
                    if (actionControlKeyF6 != null) actionControlKeyF6.action();
                    break;

                // Tổ hợp phím Ctrl + B
                case Keys.B:
                    ActionControl actionControlKeyB = lstAction.FirstOrDefault(
                        x => x.permission && x.actionType == ActionType.Balance
                        && x.Enabled);
                    if (actionControlKeyB != null) actionControlKeyB.action();
                    break;
                // Tổ hợp phím Ctrl + Z
                case Keys.Z:
                    ActionControl actionControlKeyZ = lstAction.FirstOrDefault(
                        x => x.permission && x.actionType == ActionType.Recall
                        && x.Enabled);
                    if (actionControlKeyZ != null) actionControlKeyZ.action();
                    break;
            }
        }
    }
    public enum ActionType { Add, Edit, Save, Delete, Refresh, LapKH, GopThung, AddPOPCB, ExCel, History, Balance, Recall, ExcelSelect }
    public class ActionControl
    {
        public Action action;
        public bool permission;
        public ActionType actionType;

        // Biến enabled dùng để check nút đó có đang enabled hay không
        private bool enabled;

        //public ActionControl(Action _action, bool _permission, ActionType _actionType, bool _enabled)
        public ActionControl(Action _action, bool _permission, ActionType _actionType, bool _enabled)
        {
            this.action = _action;
            this.permission = _permission;
            this.actionType = _actionType;
            this.enabled = _enabled;
        }

        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                }
            }
        }

    }
}