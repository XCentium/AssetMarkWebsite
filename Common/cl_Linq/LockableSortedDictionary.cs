using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServerLogic.LinqExt
{
	public class LockableSortedDictionary<K, V> : SortedDictionary<K, V>
	{
		private ReaderWriterLock oLock = new ReaderWriterLock();

		public bool IsReaderLockHeld
		{
			get
			{
				return oLock.IsReaderLockHeld;
			}
		}

		public bool IsWriterLockHeld
		{
			get
			{
				return oLock.IsWriterLockHeld;
			}
		}

		public void AcquireReaderLock()
		{
			AcquireReaderLock(1000);
		}

		public void AcquireReaderLock(int iMilliseconds)
		{
			oLock.AcquireReaderLock(iMilliseconds);
		}

		public void AcquireWriterLock()
		{
			AcquireWriterLock(1000);
		}

		public void AcquireWriterLock(int iMilliseconds)
		{
			oLock.AcquireWriterLock(iMilliseconds);
		}

		public LockCookie UpgradeToWriterLock()
		{
			return UpgradeToWriterLock(1000);
		}

		public LockCookie UpgradeToWriterLock(int iMilliseconds)
		{
			return oLock.UpgradeToWriterLock(iMilliseconds);
		}

		public void DowngradeFromWriterLock(ref LockCookie oLockCookie)
		{
			oLock.DowngradeFromWriterLock(ref oLockCookie);
		}

		public void ReleaseLock()
		{
			oLock.ReleaseLock();
		}

		public void ReleaseReaderLock()
		{
			oLock.ReleaseReaderLock();
		}

		public void ReleaseWriterLock()
		{
			oLock.ReleaseWriterLock();
		}
	}
}
