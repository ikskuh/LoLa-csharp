# *Lo*gic *La*nguage
Logic Language is a small embeddable scripting language meant for user-based script input in games.

# This repository is archived and will not be worked on in the future. Please checkout https://github.com/MasterQ32/LoLa for the current version

# Examples

Fibonacci sequence (iterative):
```
 function Fibonacci(num)
{
    var a = 1;
    var b = 0;
    var temp;

    while (num >= 0)
    {
        temp = a;
        a = a + b;
        b = temp;
        num = num - 1;
    }

    return b;
}

Print(Fibonacci(4));
```

Fibonacci sequence (recursive):
```
function Fibonacci(num)
{
    if (num <= 1)
    {
        return 1;
    }
    return Fibonacci(num - 1) + Fibonacci(num - 2);
}
Print(Fibonacci(4));
```

# Features

## Non-user object orientation
The user can consume/use objects defined by the scripting API, but cannot declare their own.

```
var counter = CreateCounter();
Print("cnt = ", counter.GetValue());
Print("cnt = ", counter.Increment());
Print("cnt = ", counter.Increment());
Print("cnt = ", counter.Decrement()); 
```

## Arrays
The language provides a simple array interface with dynamic, zero-indexed arrays.

```
var list = [ "This", "is", "a" ];
list = list + [ "Sentence" ];
Print(list);
```

## Dynamic Typing
Variables must be declared and have a scope, but must be declared before use.

There are 7 supported types:
- `void` (type of the null value)
- `boolean` (`true` or `false`)
- `string` (unicode text)
- `number` (a double precision floating point number)
- `object` (a system-defined object with methods)
- `array` (an indexable list of values)
- `enumerator` (only for internal use)

```
var a = 10;
Print(a);
a = "Hallo";
Print(a);
```

## Simple Virtual Machine
The language is compiled into instructions for a really simple, stack based virtual machine with about 30 instructions, most of them artithmetic or comparison operators.

The above example for dynamic typing looks like this:
```
push 10
declare a
store a
load a
call Print 1
pop
push Hallo
store a
load a
call Print 1
pop
```
