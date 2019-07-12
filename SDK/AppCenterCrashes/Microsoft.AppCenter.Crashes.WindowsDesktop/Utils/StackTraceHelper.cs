// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;

namespace Microsoft.AppCenter.Crashes.Utils
{
    public class StackTraceHelper
    {
        // Exceptions don't always have complete stack traces, so they must be augmented.
        // Crashes don't need trace augmenting.
        public static string GenerateFullStackTrace(Exception e)
        {
            if (string.IsNullOrEmpty(e.StackTrace))
            {
                return e.StackTrace;
            }
            var exceptionStackTrace = new System.Diagnostics.StackTrace(e, true);

            // Generate current stack trace. Skip three frames to avoid showing SDK code -
            // `GenerateFullStackTrace`, `PlatformTrackError`, and `TrackError`.
            var currentStackTrace = new System.Diagnostics.StackTrace(3, true);

            /*
             * The exception's stack trace begins at the first method that threw, and includes only methods that
             * rethrew. The current stack trace includes all methods up to and including the first method that threw,
             * but no methods that rethrew up to the first method that threw. For example:
             * 
             * If method A calls B, B calls C, C calls D, and D throws an exception, and the exception is caught in B,
             * then the stack trace will only include D, C, and B. So A is missing from it. But in the "current" stack
             * trace generated above, we would only see methods B and A. In some cases there could be frames that were
             * created after the exception was thrown but are present now. These frames can be ignored, as they were not
             * part of the flow that involved the exception. For example, we may see exception stack trace "D->C->B" and
             * current stack trace "F->D->A->B->A". The solution is to find the last frame of the exception's stack
             * trace in the current stack trace, append everything after, and ignore everything before. So the result
             * would be "D->C->B->A. Thank you for your time.
             */
            var commonFrame = exceptionStackTrace.GetFrame(exceptionStackTrace.FrameCount - 1);
            var concatenationIndex = -1;
            for (var i = 0; i < currentStackTrace.FrameCount; ++i)
            {
                var otherFrame = currentStackTrace.GetFrame(i);

                // Can't just compare the strings because they may have different line numbers.
                if (otherFrame.GetMethod() == commonFrame.GetMethod())
                {
                    // If the concatenationIndex has already been set, we've found another match. Thus the concatenation
                    // index is ambiguous and cannot be solved.
                    if (concatenationIndex != -1)
                    {
                        concatenationIndex = -1;
                        break;
                    }

                    // Add one to the index to avoid duplicating the common frame.
                    concatenationIndex = i + 1;
                }
            }

            // If the concatenation index could not be determined or is out of range, fall back to the exception's
            // stack trace.
            if (concatenationIndex == -1 || currentStackTrace.FrameCount <= concatenationIndex)
            {
                return e.StackTrace;
            }

            // Compute the missing frames as everything that comes after the common frame. There is no way to convert an
            // array of StackFrame objects to a StackTrace, and the ToString() of StackFrame objects appears to be
            // different from those of StackTrace. Thus, we must work with strings.
            var exceptionStackTraceStrings = exceptionStackTrace.ToString().Split(Environment.NewLine.ToCharArray()).Where((item) => !string.IsNullOrWhiteSpace(item));
            var currentStackTraceString = currentStackTrace.ToString().Split(Environment.NewLine.ToCharArray()).Where((item) => !string.IsNullOrWhiteSpace(item));
            var missingFrames = currentStackTraceString.Skip(concatenationIndex);
            var allFrames = exceptionStackTraceStrings.Concat(missingFrames);
            var completeStackTrace = allFrames.Aggregate((result, item) => result + Environment.NewLine + item);
            return completeStackTrace;
        }

    }
}
