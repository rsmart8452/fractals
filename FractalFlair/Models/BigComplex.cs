using Singulink.Numerics;

namespace FractalFlair.Models;

public class BigComplex
{
  public BigComplex(double real, double imaginary)
    : this((BigDecimal)real, (BigDecimal)imaginary)
  { }

  public BigComplex(BigDecimal real, BigDecimal imaginary)
  {
    Real = real;
    Imaginary = imaginary;
  }

  public BigDecimal Real { get; }
  public BigDecimal Imaginary { get; }
}