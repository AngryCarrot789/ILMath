# ILMath
> ILMath is a hyper-fast and flexible math expression parser for C# and .NET.

## Features
- Compiles math expressions to IL code
- Compiled expressions can be reused many times
- Supports many math functions and constants (including custom)
- Extremely fast (after compilation)
- Easy to use
- Supports NativeAOT (disables dynamic code generation)


This fork is a major rewrite which adds extra optimizations and, mainly, adds support for more 
than the `double` type. Current types supported:
- `int`
- `uint`
- `long`
- `ulong`
- `float`
- `double`

All three compiler methods still work as normal

## Operators
This fork of ILMath supports the following operators:
> `IN` stands for Integer Number (including int, uint, long, ulong). `FPN` stands for floating point number (`float`, `double`). `Decimal` is not yet supported. 

| Operator | Description        | Supported Types |
|----------|--------------------|-----------------|
| `+`      | Addition           | All INs & FPNs  |
| `-`      | Subtraction        | All INs & FPNs  |
| `*`      | Multiplication     | All INs & FPNs  |
| `/`      | Division           | All INs & FPNs  |
| `%`      | Modulo             | All INs & FPNs  |
| `&`      | AND                | Integers only   |
| ` `      | OR (vertical line) | Integers only   |
| `^`      | XOR                | Integers only   |
| `~`      | NOT (O's C)        | Integers only   |
| `!`      | Bool NOT           | Integers only   |
| `<<`     | LShift             | Integers only   |
| `>>`     | RShift             | Integers only   |

## Built-ins
ILMath supports many built-in variables and functions. 

### Floating Point Variables
| Variable   | Description                                           |
|------------|-------------------------------------------------------|
| `pi`       | The ratio of a circle's circumference to its diameter |
| `e`        | Euler's number                                        |
| `tau`      | The ratio of a circle's circumference to its radius   |
| `phi`      | The golden ratio                                      |
| `inf`      | Infinity                                              |
| `nan`      | Not a number                                          |
| `degToRad` | Conversion factor from degrees to radians             |
| `radToDeg` | Conversion factor from radians to degrees             |

### Functions
| Function               | Description                                             |
|------------------------|---------------------------------------------------------|
| `sin(x)`               | Sine of `x` in radians                                  |
| `cos(x)`               | Cosine of `x` in radians                                |
| `tan(x)`               | Tangent of `x` in radians                               |
| `asin(x)`              | Arcsine of `x` in radians                               |
| `acos(x)`              | Arccosine of `x` in radians                             |
| `atan(x)`              | Arctangent of `x` in radians                            |
| `atan2(y, x)`          | Arctangent of `y / x` in radians                        |
| `sinh(x)`              | Hyperbolic sine of `x` in radians                       |
| `cosh(x)`              | Hyperbolic cosine of `x` in radians                     |
| `tanh(x)`              | Hyperbolic tangent of `x` in radians                    |
| `sqrt(x)`              | Square root of `x`                                      |
| `cbrt(x)`              | Cube root of `x`                                        |
| `root(x, n)`           | `n`th root of `x`                                       |
| `exp(x)`               | Exponential function of `x`                             |
| `abs(x)`               | Absolute value of `x`                                   |
| `log(x)`               | Natural logarithm of `x`                                |
| `log10(x)`             | Base-10 logarithm of `x`                                |
| `log2(x)`              | Base-2 logarithm of `x`                                 |
| `logn(x, n)`           | Base-`n` logarithm of `x`                               |
| `pow(x, y)`            | `x` raised to the power of `y`                          |
| `mod(x, y)`            | `x` modulo `y`                                          |
| `min(x, y)`            | Minimum of `x` and `y`                                  |
| `max(x, y)`            | Maximum of `x` and `y`                                  |
| `floor(x)`             | Floor of `x`                                            |
| `ceil(x)`              | Ceiling of `x`                                          |
| `round(x)`             | Round of `x`                                            |
| `sign(x)`              | Sign of `x`                                             |
| `clamp(x, min, max)`   | Clamps `x` between `min` and `max`                      |
| `lerp(x, y, t)`        | Linear interpolation between `x` and `y` by `t`         |
| `inverseLerp(x, y, t)` | Inverse linear interpolation between `x` and `y` by `t` |

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.