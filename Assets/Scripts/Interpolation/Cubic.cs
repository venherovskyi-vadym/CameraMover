using System;
using System.Linq;

/// <summary>
/// Original code was found https://swharden.com/blog/2022-01-22-spline-interpolation/
/// Original was modified to work with 3 sets of coordinates instead of 2 sets,
/// reduced accuracy from float to float, added safe division
/// </summary>
public static class Cubic
{
    /// <summary>
    /// Generate a smooth (interpolated) curve that follows the path of the given X/Y points
    /// </summary>
    public static (float[] xs, float[] ys, float[] zs) InterpolateXYZ(float[] xs, float[] ys, float[] zs, int count)
    {
        if (xs is null || ys is null || zs is null || xs.Length != ys.Length || xs.Length != zs.Length)
            throw new ArgumentException($"{nameof(xs)} and {nameof(ys)} and {nameof(zs)} must have same length");

        int inputPointCount = xs.Length;
        float[] inputDistances = new float[inputPointCount];
        for (int i = 1; i < inputPointCount; i++)
        {
            float dx = xs[i] - xs[i - 1];
            float dy = ys[i] - ys[i - 1];
            float dz = zs[i] - zs[i - 1];
            float distance = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
            inputDistances[i] = inputDistances[i - 1] + distance;
        }

        float meanDistance = inputDistances.Last() / (count - 1);
        float[] evenDistances = Enumerable.Range(0, count).Select(x => x * meanDistance).ToArray();

        float[] xsOut = Interpolate(inputDistances, xs, evenDistances);
        float[] ysOut = Interpolate(inputDistances, ys, evenDistances);
        float[] zsOut = Interpolate(inputDistances, zs, evenDistances);
        return (xsOut, ysOut, zsOut);
    }

    private static float[] Interpolate(float[] originalDist, float[] points, float[] interpolatedDistances)
    {
        (float[] a, float[] b) = FitMatrix(originalDist, points);

        float[] yInterp = new float[interpolatedDistances.Length];
        for (int i = 0; i < yInterp.Length; i++)
        {
            int j;
            for (j = 0; j < originalDist.Length - 2; j++)
                if (interpolatedDistances[i] <= originalDist[j + 1])
                    break;
            float dx = originalDist[j + 1] - originalDist[j];
            float t = SafeDivide(interpolatedDistances[i] - originalDist[j], dx, points[j]);
            float y = (1 - t) * points[j] + t * points[j + 1] +
                t * (1 - t) * (a[j] * (1 - t) + b[j] * t);
            yInterp[i] = y;
        }

        return yInterp;
    }

    private static (float[] a, float[] b) FitMatrix(float[] x, float[] y)
    {
        int n = x.Length;
        float[] a = new float[n - 1];
        float[] b = new float[n - 1];
        float[] r = new float[n];
        float[] A = new float[n];
        float[] B = new float[n];
        float[] C = new float[n];

        float dx1, dx2, dy1, dy2;

        dx1 = x[1] - x[0];
        C[0] = SafeDivide( 1.0f , dx1);
        B[0] = 2.0f * C[0];
        r[0] = 3 * SafeDivide(y[1] - y[0] , dx1 * dx1);

        for (int i = 1; i < n - 1; i++)
        {
            dx1 = x[i] - x[i - 1];
            dx2 = x[i + 1] - x[i];
            A[i] = SafeDivide(1.0f , dx1);
            C[i] = SafeDivide(1.0f , dx2);
            B[i] = 2.0f * (A[i] + C[i]);
            dy1 = y[i] - y[i - 1];
            dy2 = y[i + 1] - y[i];
            r[i] = 3 * (SafeDivide(dy1 , dx1 * dx1) + SafeDivide( dy2 , dx2 * dx2));
        }

        dx1 = x[n - 1] - x[n - 2];
        dy1 = y[n - 1] - y[n - 2];
        A[n - 1] = SafeDivide(1.0f, dx1);
        B[n - 1] = 2.0f * A[n - 1];
        r[n - 1] = 3 * SafeDivide(dy1 , dx1 * dx1);

        float[] cPrime = new float[n];
        cPrime[0] = SafeDivide(C[0], B[0]);
        for (int i = 1; i < n; i++)
            cPrime[i] = SafeDivide( C[i] , B[i] - cPrime[i - 1] * A[i]);

        float[] dPrime = new float[n];
        dPrime[0] = SafeDivide(r[0], B[0]);
        for (int i = 1; i < n; i++)
            dPrime[i] = SafeDivide(r[i] - dPrime[i - 1] * A[i], B[i] - cPrime[i - 1] * A[i]);

        float[] k = new float[n];
        k[n - 1] = dPrime[n - 1];
        for (int i = n - 2; i >= 0; i--)
            k[i] = dPrime[i] - cPrime[i] * k[i + 1];

        for (int i = 1; i < n; i++)
        {
            dx1 = x[i] - x[i - 1];
            dy1 = y[i] - y[i - 1];
            a[i - 1] = k[i - 1] * dx1 - dy1;
            b[i - 1] = -k[i] * dx1 + dy1;
        }

        return (a, b);
    }

    private static float SafeDivide(float divident, float divisor, float safeAlternative = 0)
    {
        if (divisor == 0)
        {
            return safeAlternative;
        }

        return divident / divisor;
    }
}