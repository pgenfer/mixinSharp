# mixinSharp

MixinSharp (or shorter: mixin#) is a code refactoring extension for Visual Studio 2015 that adds mixin support to C# by auto generating the required code.

## what are mixins?

Mixins are a software concept that provide **code reuse by composition** instead of inheritance.  
The commonly used functionality is placed in a class (*mixin*) and other classes (*children*) which require this functionality use the implementation by holding a reference to an instance of the mixin. Calls to the child class will then be forwarded to the mixin instance.  
You can also say the mixin is *included* in the child class.

## difference between mixins and inheritance

In classical inheritance, reusing code is achieved by deriving a class from a base class. The derived class can then access all protected or public members of its base.  
While inheritance is a fundamental concept of object oriented programming, it also has some drawbacks:
- A static relation between the base class and its derived class is established during compile time. This relation cannot be decomposed during runtime.
- Most modern languages allow only single implementation inheritance, that means if a class  already has a base class, it cannot reuse the implementation of another class via inheritance (unless you change the existing base class).

Compared to classical inheritance, mixins have some advantages:
- The coupling between child and mixin is not as strong as when using inheritance, the mixin instance (and its implementation) could even be changed during runtime.
- A child class can include more than one mixins and could therefore reuse several implementations

One drawback of mixins has been so far, that C# unfortunately does not have any direct language level support for mixins or compositions (although extension methods could be used to provide some basic mixin behavior).  
mixinSharp* tries to close this gap by providing an easy way to auto generate the code that is required to include a mixin class in a child.

## a simple example

your mixin class with the code you want to reuse
```csharp
public class NameMixin
{
  public string Name { get; set; }
  public override string ToString() => Name;
}
```
the class where you want to include your mixin (your child)
```csharp
public class Person
{
  private NameMixin _name = new NameMixin();
}
```
after applying the mixin refactoring on the ```_name``` field, your mixin class is included in your child
```csharp
public class Person
{
  private NameMixin _name = new NameMixin();
  public string Name
  {
    get { return _name.Name; }
    set { _name.Name = value; }
  }
  
  public override string ToString() => _name.ToString();
}
```


