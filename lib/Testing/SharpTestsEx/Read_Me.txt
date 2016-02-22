Sharp Tests Extensions (#TestsEx for friends)
=============================================


How to build:
-------------

Run build.bat the result of compilation will be available in
.\Build\Output.


Which assembly do I need to use #TestsEx?
-----------------------------------------

You need only one of the assemblies deployed depending on the unit
tests framework you are using.

For NUnit you need ONLY SharpTestsEx.NUnit.dll

For xUnit you need ONLY SharpTestsEx.xUnit.dll

For Silverlight you need ONLY SharpTestsExSL.dll

For MsTest you need ONLY SharpTestsEx.MSTest.dll (read http://sharptestex.codeplex.com/wikipage?title=HowToV1VS2010 )


When do I need the SharpTestsEx.dll?
------------------------------------

If the unit tests framework you are using is not directly supported,
you can use the base SharpTestsEx.dll.

The main difference when SharpTestsEx.dll will be that your runner
will not recognize the exception and its output will look slightly
less "pretty" than native assertions.

