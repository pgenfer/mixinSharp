# mixinSharp

MixinSharp (or shorter: mixin#) is a code refactoring extension for Visual Studio 2015 that adds mixin support to C# by auto generating the required code.

## what are mixins?

Mixins are a software concept that provides code reuse by *composition* instead of *inheritance*.  
The code that should be reused is placed in a separate class (the *mixin*) and any other class (in this context also called the *child* class) that wants to use this code simply holds a reference to the mixin and delegates method calls to the mixin.  
For the ouside standing caller it looks like that the child instance is handling the request directly.  
  

![Uml diagram of mixin/child composition](https://github.com/pgenfer/mixinSharp/blob/master/images/mixin_uml.png)


## why are they useful?

The code reuse concept of mixins has some advantages compared to classical inheritance. Please check the wiki for a more detailed comparison of both approaches.  
Unfortunately, C# does not support mixins directly on a language level, that's where mixinSharp comes into play by offering a refactoring step that can create the required mixin code automatically.

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
after applying the mixin refactoring on the ``` _name``` field, your mixin class is included in your child
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


