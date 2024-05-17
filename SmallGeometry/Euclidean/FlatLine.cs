using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallGeometry.Euclidean
{
    /// <summary>
    /// Immutable line of FlatPoints.
    /// </summary>
    public class FlatLine
    {
        /// <summary>
        /// 
        /// </summary>
        public CoordinateSystem CoordinateSystem { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public FlatPoint this[int idx] => _points[idx];

        /// <summary>
        /// Number of points.
        /// </summary>
        public int Count => _points.Count;

        /// <summary>
        /// 
        /// </summary>
        public BoundingBox BoundingBox { get; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<FlatPoint> _points { get; }
        private double _length = -1;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <exception cref="ArgumentException">points is empty</exception>
        /// <exception cref="ArgumentNullException">points is null</exception>
        /// <exception cref="Exceptions.CoordinateSystemNoneException">points contains both None and else</exception>
        /// <exception cref="Exceptions.TransformException"></exception>
        public FlatLine(IEnumerable<FlatPoint> points)
        {
            ArgumentNullException.ThrowIfNull(points);

            int pointsCount = points.Count();
            if (pointsCount == 0)
            {
                throw new ArgumentException(ExceptionMessages.PointsCountZero, nameof(points));
            }
            else if (pointsCount == 1)
            {
                _points = [points.First(), points.First()];
            }
            else
            {
                _points = TransformList(points);
            }

            BoundingBox = new BoundingBox(_points);
        }

        /// <inheritdoc cref="FlatLine.FlatLine(IEnumerable{FlatPoint})"/>
        public FlatLine(params FlatPoint[] points)
            : this(points.AsEnumerable())
        {
        }


        /// <summary>
        /// Get a copy of line with same points follwing previous point removed.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">line is empty</exception>
        /// <exception cref="ArgumentNullException">line is null</exception>
        /// <exception cref="Exceptions.CoordinateSystemNoneException">points contains both None and else</exception>
        /// <exception cref="Exceptions.TransformException"></exception>
        public static List<FlatPoint> RemoveDuplicatedPoints(IEnumerable<FlatPoint> line)
        {
            IReadOnlyList<FlatPoint> transformed = TransformList(line);
            var result = new List<FlatPoint>(transformed.Count)
            {
                transformed[0]
            };

            FlatPoint previousPoint = result.First();
            foreach (FlatPoint fp in transformed)
            {
                if (fp != previousPoint)
                {
                    result.Add(fp);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">points is empty</exception>
        /// <exception cref="ArgumentNullException">points is null</exception>
        /// <exception cref="Exceptions.CoordinateSystemNoneException">points contains both None and else</exception>
        /// <exception cref="Exceptions.TransformException"></exception>
        public static IReadOnlyList<FlatPoint> TransformList(IEnumerable<FlatPoint> points)
        {
            ArgumentNullException.ThrowIfNull(points);
            int pointsCount = points.Count();

            if (pointsCount == 0)
            {
                throw new ArgumentException(ExceptionMessages.PointsCountZero, nameof(points));
            }

            CoordinateSystem[] coordinateSystems = points.Select(p => p.CoordinateSystem).Distinct().ToArray();

            if (coordinateSystems.Length > 1)
            {
                if (coordinateSystems.Contains(CoordinateSystem.None))
                {
                    throw new Exceptions.CoordinateSystemNoneException(nameof(points));
                }
                else
                {
                    var flatPoints = new List<FlatPoint>(pointsCount);
                    CoordinateSystem cs = points.First().CoordinateSystem;
                    foreach (FlatPoint fp in points)
                    {
                        flatPoints.Add(fp.Transform(cs));
                    }
                    return flatPoints;
                }
            }
            else
            {
                return points.ToArray();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FlatLine GetReversedCopy()
        {
            var reversedPoints = new List<FlatPoint>(_points);
            reversedPoints.Reverse();
            return new FlatLine(reversedPoints);
        }


        public double GetLength()
        {
            if (_length < 0)
            {
                _length = 0;
                for (int i=1; i<Count; ++i)
                {
                    _length += _points[i - 1].GetDistance(_points[i]);
                }
            }

            return _length;
        }


    }
}
