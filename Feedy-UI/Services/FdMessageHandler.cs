﻿using Feedy.Models;
using GE_Utilities;
using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Feedy.Services
{
    public class GuiMsgHandler
    {
        //--- Reset --------------------------
        public void Reset()
        {

        }

        //-- handle_message ---------------------------------
        public void handle_message(Byte[] msg)
        {
            SMsgHdr hdr;
            GeStructConvert.ToStruct(out hdr, msg);

        //  GeTrace.WriteLine(string.Format("handle_message id=0x{0:x}", hdr.msgId));
            try
            {
                switch (hdr.msgId)
                {
                    case GuiMsg.REP_VERSION:           handle_version(msg);            break;

                    case GuiMsg.REP_STATUS:            handle_status(msg);              break;

                    case GuiMsg.LIST_JOB_START:        FdGolbals.Job.Files.Clear();    break;
                    case GuiMsg.LIST_JOB_FILE:         handle_job_file(msg);           break;
                    case GuiMsg.LIST_JOB_END:          FdGolbals.Job.ListLoaded++;     break;

                    case GuiMsg.LOAD_CFG:              handle_cfg_loaded (msg);          break;

                    case GuiMsg.LOAD_JOB:              handle_job_load(msg);           break;
                //    case GuiMsg.RUN_JOB:             handle_job_run(msg);            break;
                //    case GuiMsg.ABORT_JOB:           handle_job_abort(msg);          break;
    
                    case GuiMsg.ERROR_RESET:           FdGolbals.Events.Confirm();    break;
                    case GuiMsg.ERROR_EVENT:           handle_error_event(msg);       break;

                    default:    FdGolbals.Events.AddError(0, string.Format("UnKnown ID: 0x{0:x}", hdr.msgId)); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //--- handle_version ---------------------------------------------
        private void handle_version(byte[] buf)
        {
            SGuiMsg msg;
            int len = GeStructConvert.ToStruct(out msg, buf);
            FdGolbals.Version.Update(msg.data);
        }
        
        //--- handle_status ---------------------------------------------
        private void handle_status(byte[] buf)
        {
            SFeedyStatusMsg msg;
            int len = GeStructConvert.ToStruct(out msg, buf);
            FdGolbals.FeedyStatus.Set(msg.status);
        }

        //--- handle_job_file ---------------------------------------------
        private void handle_job_file(byte[] buf)
        {
            SFileMsg msg;
            int len = GeStructConvert.ToStruct(out msg, buf);
            if (len == msg.hdr.msgLen)
            {
                FdGolbals.Job.Files.AddFile(msg.filename);
            }
            else FdGolbals.Events.AddError(0, "Received invalid message Length handle_job_file");
        }

        //--- handle_job_load ---------------------------------------------
        private void handle_job_load(byte[] buf)
        {
            SJobMsg msg;
            int len = GeStructConvert.ToStruct(out msg, buf);
            if (len == msg.hdr.msgLen)
            {
                FdGolbals.JobSettings.Set(msg.job);
            }
            else  FdGolbals.Events.AddError(0, "Received invalid message Length SLoadFileMsg");
        }

        //--- handle_cfg_loaded ---------------------------------------------
        private void handle_cfg_loaded(byte[] buf)
        {
            SCfgMsg msg;
            int len = GeStructConvert.ToStruct(out msg, buf);
            if (len == msg.hdr.msgLen)
            {
                FdGolbals.Cfg.Set(msg.cfg);
            }
            else  FdGolbals.Events.AddError(0, "Received invalid message Length SCfgMsg");
        }

        //--- handle_error_event ---------------------------------------------
        private void handle_error_event(byte[] buf)
        {
            SGuiMsg msg;
            int len = GeStructConvert.ToStruct(out msg, buf);
            FdGolbals.Events.AddItem(msg.data);
        }

    }
}
