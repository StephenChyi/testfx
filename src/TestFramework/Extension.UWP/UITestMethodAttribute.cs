﻿// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer
{
    using System;
    using System.Runtime.CompilerServices;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Execute test code in UI thread for Windows store apps.
    /// </summary>
    public class UITestMethodAttribute : TestMethodAttribute
    {
        /// <summary>
        /// Executes the test method on the UI Thread.
        /// </summary>
        /// <param name="testMethod">
        /// The test method.
        /// </param>
        /// <returns>
        /// The <see cref="TestResult[]"/>.
        /// </returns>
        /// Throws <exception cref="NotSupportedException"> when run on an async test method.
        /// </exception>
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var attrib = testMethod.GetAttributes<AsyncStateMachineAttribute>(false);
            if (attrib.Length > 0)
            {
                throw new NotSupportedException(FrameworkMessages.AsyncUITestMethodNotSupported);
            }

            TestResult result = null;
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    result = testMethod.Invoke(new object[] { });
                }).AsTask().GetAwaiter().GetResult();

            return new TestResult[] { result };
        }
    }
}