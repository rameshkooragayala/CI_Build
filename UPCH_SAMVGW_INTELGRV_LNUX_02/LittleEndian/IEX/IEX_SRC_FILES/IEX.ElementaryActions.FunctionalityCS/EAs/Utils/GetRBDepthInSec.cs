using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation
{
    /// <summary>
    ///  Parses the RB depth milestones and calculate the RB depth
    /// </summary>
    public class GetRBDepthInSec : IEX.ElementaryActions.BaseCommand
    {

        private string _timeStampMarginLine="";
        private string _rbDepth;
        private Manager _manager;

        /// <param name="timeStampMarginLine">RB depth milestone to be parsed</param>
        /// <param name="pManager">The Returned RBdepth in mins </param>
        /// <remarks>
        /// Possible Error Codes:
        /// <para>350 - ParsingFailure</para> 
        /// </remarks>

        /*
         * EA accepts the milestone string as a input and give the RB depth as output
         * e.g :if  'IEX_ReviewBufferCurrentDepth:123456' is the input milestone 
         * string is parsed and '123456' is extracted from the string
         * 123456 which is in milli secods is converted into mins 
         * e.g. 123456/1000/6 = 20.576 mins is the output
		 
        */
        public GetRBDepthInSec(string timeStampMarginLine, Manager pManager)
        {
            _timeStampMarginLine = timeStampMarginLine;
            _manager = pManager; 
        }

       /// <summary>
       ///  EA Execution
       /// </summary>
        protected override void Execute()
        {
            try
            {
                //the actual string format is IEX_ReviewBufferCurrentDepth: 123456
                //So removing the string up to the index of colon (:)
                _rbDepth = _timeStampMarginLine.Remove(0, _timeStampMarginLine.IndexOf(":")+1);
                
                //gives a space with the value like ' 123456'
                //so trimming at the begining

                _rbDepth = _rbDepth.TrimStart();

                //if some undesired content comes after the RB depth value ignoring it
                _rbDepth = _rbDepth.Substring(0, _rbDepth.IndexOf('\n'));
                                    
            }
                
            catch (Exception ex)
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.ParsingFailure, "The Milestone String parsed in not in the correct format"));
            }
             
            //The RB depth value obtained from the milestone is in milli secs .
            //Divding the value with 1000 give the value in secs
            
            double rbDepthInMin = Convert.ToDouble(_rbDepth)/1000;
            SetReturnValues(new Object[]{rbDepthInMin});
         }


     }

}