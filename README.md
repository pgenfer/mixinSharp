# mixinSharp

MixinSharp (or short: mixin#) is a code refactoring extension for Visual Studio 2015 that adds mixin support to C# by auto generating the required mixin code.

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


