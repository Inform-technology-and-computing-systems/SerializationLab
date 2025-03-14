<Serializable> Public Class Point
    Implements IComparable
    Public Property X As Integer
    Public Property Y As Integer
    Public Sub New()
        X = 0
        Y = 0
    End Sub
    Public Sub New(ByVal x As Integer, ByVal y As Integer)
        Me.X = x
        Me.Y = y
    End Sub
    Public Overridable Function Metric() As Double
        Metric = Math.Sqrt((X ^ 2 + Y ^ 2))
    End Function
    Public Overrides Function ToString() As String
        Return String.Format("({0}, {1})", X, Y)
    End Function
    Private Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
        Dim p As Point = CType(obj, Point)
        Return Metric() - p.Metric()
    End Function
End Class
