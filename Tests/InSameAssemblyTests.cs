﻿using System;
using System.IO;
using System.Reflection;
using System.Threading;
using NUnit.Framework;

[TestFixture]
public class InSameAssemblyTests 
{
    string beforeAssemblyPath;
    Assembly assembly;
    FieldInfo exceptionField;
    string afterAssemblyPath;

    public InSameAssemblyTests()
	{
        this.beforeAssemblyPath =Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\debug\AssemblyToProcess.dll");
#if (!DEBUG)
        this.beforeAssemblyPath = this.beforeAssemblyPath.Replace("Debug", "Release");
#endif
        afterAssemblyPath = WeaverHelper.Weave(this.beforeAssemblyPath);


        assembly = Assembly.LoadFrom(afterAssemblyPath);
        var errorHandler = assembly.GetType("AsyncErrorHandler");
        exceptionField = errorHandler.GetField("Exception");
	}

    [Test]
    public void Method()
    {
        ClearException();
        var instance = assembly.GetInstance("Target");
        instance.Method();
        Thread.Sleep(100);
        Assert.IsNull(GetException());
    }

    [Test]
    public void MethodWithThrow()
    {
        ClearException();
        var instance = assembly.GetInstance("Target");
        instance.MethodWithThrow();
        Thread.Sleep(100);
        Assert.IsNotNull(GetException());
    }

    [Test]
    public void MethodGeneric()
    {
        ClearException();
        var instance = assembly.GetInstance("Target");
        instance.MethodGeneric();
        Thread.Sleep(100);
        Assert.IsNull(GetException());
    }

    [Test]
    public void MethodWithThrowGeneric()
    {
        ClearException();
        var instance = assembly.GetInstance("Target");
        instance.MethodWithThrowGeneric();
        Thread.Sleep(100);
        Assert.IsNotNull(GetException());
    }

    void ClearException()
    {
        exceptionField.SetValue(null, null);
    }

    Exception GetException()
    {
        return (Exception) exceptionField.GetValue(null);
    }
    
#if (DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath,beforeAssemblyPath);
    }
#endif
}