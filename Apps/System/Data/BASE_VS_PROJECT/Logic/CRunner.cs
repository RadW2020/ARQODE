﻿#region Copyright © 2015, 2016 MJM [info.ARQODE@gmail.com]
/*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.

* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
* GNU General Public License for more details.

* You should have received a copy of the GNU General Public License
* along with this program.If not, see<http:* www.gnu.org/licenses/>.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARQODE_Core;
using TControls;

namespace TLogic
{
    public class CRunner
    {
        #region Initialize runner

        Stack slogic;
        Stack sevent_desc;
        Stack sprogram;

        Dictionary<String, Task> tasks;

        CSystem sys;
        CViewsManager vm;
        CProgram Program;
        CLogic logic = null;
        Programs ePrograms;

        /// <summary>
        /// Constructor for runner
        /// </summary>
        /// <param name="views_manager"></param>
        public CRunner(CSystem csystem, CViewsManager views_manager)
        {
            sys = csystem;
            vm = views_manager;
            tasks = new Dictionary<string, Task>();
            slogic = new Stack();
            sevent_desc = new Stack();
            sprogram = new Stack();
            ePrograms = new Programs();
        }

        #endregion

        #region Manage program

        /// <summary>
        /// restore program from queue            
        /// </summary>
        public void LaunchProgramInQueue()
        {            
            LaunchProgram((CEventDesc)sevent_desc.Pop());
        }
        /// <summary>
        /// Launch task or single thread program
        /// </summary>
        /// <param name="eDesc"></param>
        public void LaunchProgram(CEventDesc eDesc)
        {
            bool queued = CProgram.CheckQueuedFlag(eDesc.Program);

            if ((logic == null) || (!queued))
            {
                // Init program
                if (Program != null)
                {
                    sprogram.Push(Program);
                }
                Program = new CProgram(sys, ePrograms, eDesc.Program);
                vm.Cancel_events = true;
                Program.ExecuteProcess += Program_ExecuteProcess;

                // Run program if not queued or logic is null
                if (!Program.hasErrors())
                {
                    // Init logic process executor
                    if (logic != null)
                    {
                        slogic.Push(logic);
                    }
                    logic = new CLogic(sys, vm, eDesc);

                    // Run program sync or async
                    if (Program.Parallel_execution) LaunchTask(eDesc);
                    else RunProgram();

                    logic = (slogic.Count > 0) ? (CLogic)slogic.Pop() : null;
                }
                Program.ProgramClose();
                vm.Cancel_events = false;
                Program = (sprogram.Count > 0) ? (CProgram)sprogram.Pop() : null;
            }
            else if ((logic != null) && (queued))
            {
                // push program in queue
                sevent_desc.Push(eDesc);
            }
        }

        /// <summary>
        /// Check if queued
        /// </summary>
        public bool Programs_in_queue
        {
            get { return ((logic == null) && (sevent_desc.Count > 0)); }
        }
       
        #endregion

        #region Async execution

        /// <summary>
        /// Launch new task
        /// </summary>
        /// <param name="eDesc"></param>
        private void LaunchTask(CEventDesc eventDesc)
        {
            FreeEndedTasks();
            Task t = new Task(RunProgram);
            tasks.Add(eventDesc.Program, t);
            t.Start();
        }

        /// <summary>
        /// Free ended tasks
        /// </summary>
        private void FreeEndedTasks()
        {
            List<String> remove_tasks = new List<String>();

            int i = 0;
            foreach (KeyValuePair<String, Task> t in tasks.AsEnumerable())
            {
                if (!   ((t.Value.Status == TaskStatus.Running) ||
                        (t.Value.Status == TaskStatus.WaitingForActivation) ||
                        (t.Value.Status == TaskStatus.WaitingForChildrenToComplete) ||
                        (t.Value.Status == TaskStatus.WaitingToRun))
                   )
                {
                    remove_tasks.Add(t.Key);
                }
                i++;
            }
            while (remove_tasks.Count() > 0)
            {
                tasks.Remove(remove_tasks[0]);
            }
            remove_tasks.Clear();
        }

        /// <summary>
        /// Wait for single task
        /// </summary>
        /// <param name="Program"></param>
        public void WaitForTask(String Program)
        {
            if (tasks.ContainsKey(Program))
            {
                tasks[Program].Wait();
            }
        }
        #endregion

        #region Process execution

        /// <summary>
        /// Run program
        /// </summary>
        private void RunProgram()
        {
            Program.Run();
        }

        /// <summary>
        /// Event program run
        /// </summary>
        /// <param name="prc"></param>
        private void Program_ExecuteProcess(TProcess prc)
        {            
            logic.Run(prc);
        }
        #endregion
    }
}
#endregion