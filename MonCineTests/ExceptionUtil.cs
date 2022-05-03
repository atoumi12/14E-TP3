using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MonCineTests
{
    public class ExceptionUtil
    {
        public static void AssertThrows<exception>(Action method) where exception : Exception
        {
            try
            {
                method.Invoke();
            }
            catch (exception)
            {
                return; // Expected exception.
            }
            catch (Exception ex)
            {
                Assert.True(false, $"Wrong exception thrown: {ex.Message}");
            }
            Assert.True(false, "No exception thrown");
        }
    }
}
