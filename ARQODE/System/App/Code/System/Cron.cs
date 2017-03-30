#region Copyright © 2015, 2016 MJM [info.ARQODE@gmail.com]
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
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using JSonUtil;
using TControls;
using ARQODE_Core;

namespace TLogic
{
    public partial class CLogic
    {
    				/// #NAME#: #DESCRIPTION#
			public void f_Cron_add_program()
			{
			//INI CODE PRCGUID: Cron add program
                            String Program = Config_str("program");
                            int Interval = Config_int("interval");
                            sys.Cron.addProgram(Program, Interval);

                            //END CODE PRCGUID: Cron add program
			}

			/// #NAME#: #DESCRIPTION#
			public void f_Cron_remove_program()
			{
			//INI CODE PRCGUID: Cron remove program
                            String Program = Config_str("program");
                            sys.Cron.removeProgram(Program);

                            //END CODE PRCGUID: Cron remove program
			}

	
    }
}
#endregion