# mixinSharp

MixinSharp (or shorter: mixin#) is a code refactoring extension for Visual Studio 2015 that adds mixin support to C# by auto generating the required code.

## what are mixins?

Mixins are a software concept that provide **code reuse by composition** instead of inheritance. 
In classical inheritance, reusing code is achieved by deriving a class from a base class. The derived class can then access all protected or public members of its base. The code in the base class has to be written only once and can be reused every time a class is derived from this base implementation.

While inheritance is a fundamental concept of object oriented programming, it also has some drawbacks:
- A static relation between the base class and its derived class is established during compile time. This relation cannot be decomposed during runtime.
- Most modern languages allow only single implementation inheritance, that means if a class  already has a base class, it cannot reuse the implementation of another class via inheritance (unless you change the existing base class).

Mixins on the other hand provide code reuse via composition. The commonly used functionality can be placed in a class (*mixin*) and other classes (*children*) that require this functionality just keep a reference to an instance of the mixin. 
Additionally, the method signatures of the mixin are added to the child class but instead of providing the implementation for this methods directly, the child simply delegates the calls to the mixin.
You can also say the mixin is *included* in the child class.

Compared to classical inheritance, this composition based approach has some advantages:
- The coupling between child and mixin is not as strong as when using inheritance, since the mixin instance (and its implementation) could be changed during runtime.
- A child class can include more than one mixins and could therefore reuse several implementations

Unfortunately, C# does not have any direct language level support for mixins or compositions (although extension methods could be used to provide some basic mixin behavior), but support can be added by auto generating the required code. And that's where *mixinSharp* comes into play

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


