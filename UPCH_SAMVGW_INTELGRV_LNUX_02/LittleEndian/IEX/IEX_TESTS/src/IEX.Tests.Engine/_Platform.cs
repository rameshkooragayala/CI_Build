using System;

namespace IEX.Tests.Engine
{
    public class _Platform
    {
        #region Variables

        private IEXGateway._IEX _iex = new IEXGateway.IEX();
        private IEX.ElementaryActions.Functionality.Manager _ea = new ElementaryActions.Functionality.Manager();
        private string _id;

        #endregion

        #region Properties
        public IEXGateway._IEX IEX
        {
            get { return _iex; }
            set { _iex = value; }
        }

        internal IEXGateway.IEX _IEX
        {
            get { return (IEXGateway.IEX)_iex; }
        }

        public IEX.ElementaryActions.Functionality.Manager EA
        {
            get { return _ea; }
            set { _ea = value; }
        }

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }
        #endregion

        #region Init
        internal void Init(string server, string testName, string project)
        {
            string[] ServerAddress = server.Split(':');
            string ip = ServerAddress[0];
            string ServerNum = ServerAddress[1];

            IEXGateway._IEXResult res = _iex.Connect(ip, Convert.ToInt16(ServerNum));
            if (!res.CommandSucceeded)
            {
                throw new Exception("Failed to connect to IEX server: " + res.FailureReason);
            }
            _iex.LogFileName = testName + ".iexlog";

            string errorMsg = null;
            _ea.TestName = testName;
            _ea.Init(ref _iex, project, ref errorMsg);
            if (errorMsg != null)
            {
                throw new Exception("Failed to initialized EA object: " + errorMsg);
            }
        }
        #endregion
    }
}
