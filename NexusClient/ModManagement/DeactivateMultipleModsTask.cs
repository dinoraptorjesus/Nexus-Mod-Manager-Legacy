﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Nexus.Client.BackgroundTasks;
using Nexus.Client.Games;
using Nexus.Client.ModManagement;
using Nexus.Client.Mods;
using Nexus.Client.UI;
using Nexus.Client.Util;
using Nexus.Client.Util.Collections;
using Nexus.Client.ModManagement.InstallationLog;
using ChinhDo.Transactions;

namespace Nexus.Client.ModManagement
{
	public class DeactivateMultipleModsTask : ThreadedBackgroundTask
	{
		#region Fields

		private IInstallLog m_iilInstallLog = null;
		private ModInstallerFactory m_mifModInstallerFactory = null;
		private ReadOnlyObservableList<IMod> m_rolModList = null;

		#endregion

		#region Constructors

		/// <summary>
		/// A simple constructor that initializes the object with its dependencies.
		/// </summary>
		public DeactivateMultipleModsTask(ReadOnlyObservableList<IMod> p_rolModList, IInstallLog p_iilInstallLog, ModInstallerFactory p_mifModInstallerFactory)
		{
			m_iilInstallLog = p_iilInstallLog;
			m_mifModInstallerFactory = p_mifModInstallerFactory;
			m_rolModList = p_rolModList;
		}

		#endregion

		#region Event Raising

		/// <summary>
		/// Raises the <see cref="IBackgroundTask.TaskEnded"/> event.
		/// </summary>
		/// <param name="e">A <see cref="TaskEndedEventArgs"/> describing the event arguments.</param>
		protected override void OnTaskEnded(TaskEndedEventArgs e)
		{
			base.OnTaskEnded(e);
		}
		#endregion

		/// <summary>
		/// Starts the update.
		/// </summary>
		/// <param name="p_camConfirm">The delegate to call to confirm an action.</param>
		public void Update(ConfirmActionMethod p_camConfirm)
		{
			Start(p_camConfirm);
		}

		/// <summary>
		/// Cancels the update.
		/// </summary>
		public override void Cancel()
		{
			base.Cancel();
		}

		/// <summary>
		/// The method that is called to start the backgound task.
		/// </summary>
		/// <param name="p_objArgs">Arguments to for the task execution.</param>
		/// <returns>Always <c>null</c>.</returns>
		protected override object DoWork(object[] p_objArgs)
		{
			OverallMessage = "Deactivating all the active mods...";
			OverallProgress = 0;
			OverallProgressStepSize = 1;
			OverallProgressMaximum = m_rolModList.Count;
			ShowItemProgress = false;

			ConfirmActionMethod camConfirm = (ConfirmActionMethod)p_objArgs[0];

			foreach (IMod modMod in m_rolModList)
			{
				modMod.InstallDate = null;
				if (!m_iilInstallLog.ActiveMods.Contains(modMod))
					return null;
				ModUninstaller munUninstaller = m_mifModInstallerFactory.CreateUninstaller(modMod, m_rolModList);
				munUninstaller.Install();

				while (!munUninstaller.IsCompleted)
				{ }
				if (OverallProgress < OverallProgressMaximum)
					StepOverallProgress();
			}
			return null;
		}
	}
}
