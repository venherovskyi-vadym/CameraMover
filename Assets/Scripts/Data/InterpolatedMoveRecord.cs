using UnityEngine;

public class InterpolatedMoveRecord
{
    public Vector3[] Positions { get; private set; }

    public InterpolatedMoveRecord(MoveRecord record, float resampleRatio)
    {
        var entries = record.GetMoveEntries();
        var xs = new float[entries.Length];

        for (int i = 0; i < entries.Length; i++)
        {
            xs[i] = entries[i].Position.x;
        }

        var ys = new float[entries.Length];

        for (int i = 0; i < entries.Length; i++)
        {
            ys[i] = entries[i].Position.y;
        }

        var zs = new float[entries.Length];

        for (int i = 0; i < entries.Length; i++)
        {
            zs[i] = entries[i].Position.z;
        }

        var points = Cubic.InterpolateXYZ(xs, ys, zs, (int)(entries.Length * resampleRatio));
        Positions = new Vector3[points.xs.Length];

        for (int i = 0; i < points.xs.Length; i++)
        {
            Positions[i] = new Vector3(points.xs[i], points.ys[i], points.zs[i]);
        }
    }
}