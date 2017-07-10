namespace IEX.HealthCheck
{
    internal class MilestoneError
    {
        private string errorMsg;
        private ErrorType errorType;

        internal MilestoneError(string errorMsg, ErrorType errorTypeIn)
        {
            this.errorMsg = errorMsg;
            this.errorType = errorTypeIn;
        }

        internal string ErrorMsg { get { return errorMsg; } }

        internal ErrorType ErrorType { get { return errorType; } }

        internal string ActualValue { get; set; }
    }

    public enum ErrorType
    {
        Milestone_Not_Found, Wrong_Milestone, Execution_Error, Navigation_Error
    };
}