# mixinSharp

MixinSharp (or short: mixin#) is a code refactoring extension for Visual Studio 2015 that adds **mixin** support to C# by auto generating the required code.

**Mixins** are a software concept that enables code reuse by composition instead of inheritance. The reusable code is provided as a small, functional unit (the *mixin* ) and can be included into other classes when needed. This approach has some advantages over classical inheritance (link to documentation) and there are already some languages that support mixins on a language level.

Unfortunately, C# does not have any direct language level support for mixins or compositions (although extension methods could be used to provide some basic mixin behavior), but support can be added by auto generating the required code. And that's where **mixinSharp** comes into play

## a simple example

your mixin class with the code you want to reuse
```sh
public class NameMixin
{
  public string Name { get; set; }
  public override string ToString() => Name;
}
```
the class where you want to include your mixin (your child)
```sh
public class Person
{
  private NameMixin _name = new NameMixin();
}
```
after applying the mixin refactoring on the ```_name``` field, your mixin class is included in your child
```sh
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


