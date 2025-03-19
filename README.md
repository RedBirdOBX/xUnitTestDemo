# xUnitTestDemo

## Naming Conventions

[method being tested] [scenario] [expected behavior]  
`CreateEmployee_ContructInternalEmployee_DefaultSalaryIs2500`


https://github.com/KevinDockx/UnitTestingAspNetCoreWebApi


Testing Private Methods
A private method is an implementation detail that does not exist or serve a purpose in isolation. Eventually there will be a public method that calls into it; otherwise, the private method would be unused code. 

In this case of our example, CalculateSuggestedBonus is called by CreateInternalEmployeeAsync. So what we should test is part of the behavior of that method, the resulting suggested bonus value and not the CalculateSuggestedBonus method itself. 

There are people who would suggest making those private methods public so they can be tested. I (author) don't agree with that because doing so may break encapsulation and you don't want to break the encapsulation of your nice, carefully?thought?out, object?oriented design just to be able to write a test against that private method. 

If you really have to, you could change the visibility of the method you want to test from `private` to `internal` and set the `InternalsVisable` attribute in the assembly info file to the name of the test project assembly. Like that, our test project will have access to the internal methods.

Still... not ideal.