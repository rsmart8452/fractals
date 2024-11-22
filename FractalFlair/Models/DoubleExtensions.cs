using System;
using System.Numerics;

namespace FractalFlair.Models;

public static class DoubleExtensions
{
  //public static BigDecimal ToBig(this double value)
  //{
  //  var (mantissa, exponent) = value.GetMantissaAndExponent();
  //  return new BigDecimal(mantissa, exponent);
  //}

  private static Tuple<BigInteger, int> GetMantissaAndExponent(this double value)
  {
    // Translate the double into sign, exponent and mantissa.
    var bits = BitConverter.DoubleToInt64Bits(value);
    // Note that the shift is sign-extended, hence the test against -1 not 1
    var negative = (bits & (1L << 63)) != 0;
    var exponent = (int)((bits >> 52) & 0x7ffL);
    var mantissa = bits & 0xfffffffffffffL;

    // Subnormal numbers; exponent is effectively one higher,
    // but there's no extra normalization bit in the mantissa
    if (exponent == 0)
      exponent++;
    // Normal numbers; leave exponent as it is but add extra
    // bit to the front of the mantissa
    else
      mantissa |= 1L << 52;

    // Bias the exponent. It's actually biased by 1023, but we're
    // treating the mantissa as m.0 rather than 0.m, so we need
    // to subtract another 52 from it.
    exponent -= 1075;

    if (mantissa == 0)
      return new Tuple<BigInteger, int>(0, 0);

    /* Normalize */
    while ((mantissa & 1) == 0)
    {
      /*  i.e., Mantissa is even */
      mantissa >>= 1;
      exponent++;
    }

    if (negative)
      mantissa = -mantissa;

    return new Tuple<BigInteger, int>(mantissa, exponent);
  }
}