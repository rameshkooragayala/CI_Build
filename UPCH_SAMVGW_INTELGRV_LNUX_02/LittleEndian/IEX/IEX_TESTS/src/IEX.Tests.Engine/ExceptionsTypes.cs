using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEX.Tests.Engine
{
    public class AbortException : ApplicationException
    {
        public AbortException(string msg)
            : base(msg)
        {
        }
    }
    public class FailureException : ApplicationException
    {
        public FailureException(string msg)
            : base(msg)
        {
        }
    }

    public class CancelationException : ApplicationException
    {
        public CancelationException(string msg)
            : base(msg)
        {
        }
    }

    public class MountException : ApplicationException
    {
        public MountException(string msg)
            : base(msg)
        {
        }
    }

}
