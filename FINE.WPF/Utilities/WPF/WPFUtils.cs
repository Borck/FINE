namespace FINE.Views.Controls;

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

public static class WPFUtils {
  public static T FindParent<T>(DependencyObject childObject) where T : DependencyObject {
    var curObj = childObject;
    do {
      curObj = VisualTreeHelper.GetParent(curObj);
      if (curObj == null) return default(T);
    } while (!(curObj is T));
    return (T)curObj;
  }

  public static DependencyObject GetVisualAncestorNLevelsUp(DependencyObject childObject, int levels) {
    var curObj = childObject;
    for (var i = 0; i < levels; i++) {
      curObj = VisualTreeHelper.GetParent(curObj);
      if (curObj == null) return null;
    }
    return curObj;
  }

  public static IEnumerable<Point> GetIntersectionPoints(Geometry g1, Geometry g2) {
    Geometry og1 = g1.GetWidenedPathGeometry(new Pen(Brushes.Black, 1.0));
    Geometry og2 = g2.GetWidenedPathGeometry(new Pen(Brushes.Black, 1.0));

    var cg = new CombinedGeometry(GeometryCombineMode.Intersect, og1, og2);

    var pg = cg.GetFlattenedPathGeometry();
    foreach (var figure in pg.Figures) {
      var fig = new PathGeometry(new[] { figure }).Bounds;
      yield return new Point(fig.Left + fig.Width / 2.0, fig.Top + fig.Height / 2.0);
    }
  }

  public static IEnumerable<T> FindDescendantsOfType<T>(DependencyObject root, bool skipChildrenOnHit) where T : DependencyObject {
    var childCount = VisualTreeHelper.GetChildrenCount(root);
    for (var i = 0; i < childCount; i++) {
      var obj = VisualTreeHelper.GetChild(root, i);
      if (obj is T t) {
        yield return t;

        if (skipChildrenOnHit) {
          continue;
        }
      }

      foreach (var subChild in FindDescendantsOfType<T>(obj, skipChildrenOnHit)) {
        yield return subChild;
      }
    }
  }
}
