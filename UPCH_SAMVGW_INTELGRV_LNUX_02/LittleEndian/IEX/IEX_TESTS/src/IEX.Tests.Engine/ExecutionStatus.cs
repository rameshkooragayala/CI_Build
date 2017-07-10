namespace IEX.Tests.Engine
{
    #region StepStatus
    public enum StepStatus
    {
        PENDING,
        RUNNING,
        PAUSED,
        RESUMED,
        ABORTED,
        SKIPPED,
        PASSED,
        FAILED
    }
    #endregion

    #region TestStatus
    public enum TestStatus
    {
        PASSED,
        FAILED,
        ABORTED
    }
    #endregion

    #region TestsGroupStatus
    public enum TestsGroupStatus
    {
        PASSED,
        FAILED
    }
    #endregion
}